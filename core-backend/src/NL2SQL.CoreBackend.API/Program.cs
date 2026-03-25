using System.Security.Claims;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using NL2SQL.CoreBackend.API.Middleware;
using NL2SQL.CoreBackend.API.Swagger;
using NL2SQL.CoreBackend.Application;
using NL2SQL.CoreBackend.Application.Common.Options;
using NL2SQL.CoreBackend.Infrastructure;
using NL2SQL.CoreBackend.Infrastructure.Persistence;
using Serilog;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// ─── Serilog ───
builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration)
          .Enrich.FromLogContext()
          .WriteTo.Console());

// ─── Services ───
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => string.IsNullOrEmpty(e.ErrorMessage) ? "Geçersiz alan." : e.ErrorMessage)
            .ToList();
        return new BadRequestObjectResult(new
        {
            success = false,
            message = "Doğrulama hatası.",
            errors
        });
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NL2SQL Core Backend API",
        Version = "v1",
        Description = "Auth, veritabanı bağlantıları, sorgu geçmişi ve NL→SQL akışı."
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Önce /api/auth/login ile token alın; buraya: Bearer {accessToken}"
    });
    c.OperationFilter<AuthorizeOperationFilter>();
    var xml = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    if (File.Exists(xml))
        c.IncludeXmlComments(xml, includeControllerXmlComments: true);
});

// Application & Infrastructure katmanları
builder.Services.AddApplicationServices();
builder.Services.Configure<QueryExecutionOptions>(
    builder.Configuration.GetSection(QueryExecutionOptions.SectionName));
builder.Services.Configure<ConcurrencyOptions>(
    builder.Configuration.GetSection(ConcurrencyOptions.SectionName));
builder.Services.AddInfrastructureServices(builder.Configuration);

var concurrencyForRateLimit = builder.Configuration.GetSection(ConcurrencyOptions.SectionName)
    .Get<ConcurrencyOptions>() ?? new ConcurrencyOptions();

// ─── Health Checks ───
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// ─── Rate Limiting (.NET 8 built-in) ───
builder.Services.AddRateLimiter(options =>
{
    // Genel API limiti
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 10,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            }));

    // AI sorgu: kullanıcı başına eşzamanlı istek (ConcurrencyLimiter)
    options.AddPolicy("ai-query", httpContext =>
    {
        var uid = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var partitionKey = string.IsNullOrEmpty(uid)
            ? httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"
            : $"u:{uid}";
        return RateLimitPartition.GetConcurrencyLimiter(
            partitionKey,
            _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = Math.Max(1, concurrencyForRateLimit.PerUserMaxConcurrentAiQueries),
                QueueLimit = Math.Max(0, concurrencyForRateLimit.PerUserAiQueryQueueLimit),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    // Auth endpoint'leri – brute-force koruması
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(5);
        opt.QueueLimit = 0;
    });

    // Onboarding extract/register – DB + AI yükü
    options.AddFixedWindowLimiter("onboarding", opt =>
    {
        opt.PermitLimit = 30;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 8;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Rate limit exceeded",
            message = "İstek veya eşzamanlılık limiti aşıldı. Lütfen kısa süre sonra tekrar deneyin.",
            retryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                ? retryAfter.TotalSeconds : 60
        }, token);
    };
});

// ─── CORS ───
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:3000"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// ─── Middleware Pipeline ───
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Prometheus metrics
app.UseHttpMetrics();
app.UseMetricServer();

app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();
