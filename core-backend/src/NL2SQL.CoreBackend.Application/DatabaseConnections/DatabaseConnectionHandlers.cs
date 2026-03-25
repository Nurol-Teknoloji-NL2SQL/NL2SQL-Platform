using MediatR;
using NL2SQL.CoreBackend.Application.Common.Extensions;
using NL2SQL.CoreBackend.Application.Common.Interfaces;
using NL2SQL.CoreBackend.Application.Common.Models;
using NL2SQL.CoreBackend.Application.DatabaseConnections.DTOs;
using NL2SQL.CoreBackend.Domain.Entities;

namespace NL2SQL.CoreBackend.Application.DatabaseConnections;

public record ListDatabaseConnectionsQuery(Guid UserId) : IRequest<ApiResponse<List<DatabaseConnectionDto>>>;

public record GetDatabaseConnectionByIdQuery(Guid UserId, Guid Id) : IRequest<ApiResponse<DatabaseConnectionDto>>;

public record CreateDatabaseConnectionCommand(Guid UserId, CreateDatabaseConnectionRequest Request)
    : IRequest<ApiResponse<DatabaseConnectionDto>>;

public record UpdateDatabaseConnectionCommand(Guid UserId, Guid Id, UpdateDatabaseConnectionRequest Request)
    : IRequest<ApiResponse<DatabaseConnectionDto>>;

public record DeleteDatabaseConnectionCommand(Guid UserId, Guid Id) : IRequest<ApiResponse<object?>>;

public record TestDatabaseConnectionCommand(Guid UserId, Guid Id) : IRequest<ApiResponse<TestConnectionResult>>;

public record TestConnectionResult(bool Success, string? Message);

public sealed class DatabaseConnectionHandlers :
    IRequestHandler<ListDatabaseConnectionsQuery, ApiResponse<List<DatabaseConnectionDto>>>,
    IRequestHandler<GetDatabaseConnectionByIdQuery, ApiResponse<DatabaseConnectionDto>>,
    IRequestHandler<CreateDatabaseConnectionCommand, ApiResponse<DatabaseConnectionDto>>,
    IRequestHandler<UpdateDatabaseConnectionCommand, ApiResponse<DatabaseConnectionDto>>,
    IRequestHandler<DeleteDatabaseConnectionCommand, ApiResponse<object?>>,
    IRequestHandler<TestDatabaseConnectionCommand, ApiResponse<TestConnectionResult>>
{
    private readonly IUnitOfWork _uow;
    private readonly ISqlExecutionService _sqlExec;

    public DatabaseConnectionHandlers(IUnitOfWork uow, ISqlExecutionService sqlExec)
    {
        _uow = uow;
        _sqlExec = sqlExec;
    }

    public async Task<ApiResponse<List<DatabaseConnectionDto>>> Handle(ListDatabaseConnectionsQuery q, CancellationToken ct)
    {
        var list = await _uow.DatabaseConnections.FindAsync(c => c.UserId == q.UserId, ct);
        var dtos = list.OrderByDescending(c => c.CreatedAt).Select(ToDto).ToList();
        return ApiResponse<List<DatabaseConnectionDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<DatabaseConnectionDto>> Handle(GetDatabaseConnectionByIdQuery q, CancellationToken ct)
    {
        var c = await _uow.DatabaseConnections.GetByIdAsync(q.Id, ct);
        if (c is null || c.UserId != q.UserId)
            return ApiResponse<DatabaseConnectionDto>.Fail("Bağlantı bulunamadı.");
        return ApiResponse<DatabaseConnectionDto>.Ok(ToDto(c));
    }

    public async Task<ApiResponse<DatabaseConnectionDto>> Handle(CreateDatabaseConnectionCommand cmd, CancellationToken ct)
    {
        if (!DatabaseProviderExtensions.TryParseProvider(cmd.Request.Provider, out var provider))
            return ApiResponse<DatabaseConnectionDto>.Fail("Geçersiz provider.");

        var dup = await _uow.DatabaseConnections.AnyAsync(
            x => x.UserId == cmd.UserId && x.DbId == cmd.Request.DbId, ct);
        if (dup)
            return ApiResponse<DatabaseConnectionDto>.Fail("Bu db_id için zaten bir kayıt var.");

        var entity = new DatabaseConnection
        {
            UserId = cmd.UserId,
            DbId = cmd.Request.DbId.Trim(),
            DisplayName = cmd.Request.DisplayName.Trim(),
            ConnectionString = cmd.Request.ConnectionString,
            Provider = provider,
            IsActive = true
        };

        await _uow.DatabaseConnections.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        return ApiResponse<DatabaseConnectionDto>.Ok(ToDto(entity));
    }

    public async Task<ApiResponse<DatabaseConnectionDto>> Handle(UpdateDatabaseConnectionCommand cmd, CancellationToken ct)
    {
        var c = await _uow.DatabaseConnections.GetByIdAsync(cmd.Id, ct);
        if (c is null || c.UserId != cmd.UserId)
            return ApiResponse<DatabaseConnectionDto>.Fail("Bağlantı bulunamadı.");

        if (cmd.Request.DisplayName is not null)
            c.DisplayName = cmd.Request.DisplayName.Trim();
        if (cmd.Request.ConnectionString is not null)
            c.ConnectionString = cmd.Request.ConnectionString;
        if (cmd.Request.Provider is not null && DatabaseProviderExtensions.TryParseProvider(cmd.Request.Provider, out var p))
            c.Provider = p;
        if (cmd.Request.IsActive is not null)
            c.IsActive = cmd.Request.IsActive.Value;

        _uow.DatabaseConnections.Update(c);
        await _uow.SaveChangesAsync(ct);

        return ApiResponse<DatabaseConnectionDto>.Ok(ToDto(c));
    }

    public async Task<ApiResponse<object?>> Handle(DeleteDatabaseConnectionCommand cmd, CancellationToken ct)
    {
        var c = await _uow.DatabaseConnections.GetByIdAsync(cmd.Id, ct);
        if (c is null || c.UserId != cmd.UserId)
            return ApiResponse<object?>.Fail("Bağlantı bulunamadı.");

        _uow.DatabaseConnections.Remove(c);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<object?>.Ok(null, "Silindi.");
    }

    public async Task<ApiResponse<TestConnectionResult>> Handle(TestDatabaseConnectionCommand cmd, CancellationToken ct)
    {
        var c = await _uow.DatabaseConnections.GetByIdAsync(cmd.Id, ct);
        if (c is null || c.UserId != cmd.UserId)
            return ApiResponse<TestConnectionResult>.Fail("Bağlantı bulunamadı.");

        try
        {
            var ok = await _sqlExec.TestConnectionAsync(c.ConnectionString, c.Provider, ct);
            return ok
                ? ApiResponse<TestConnectionResult>.Ok(new TestConnectionResult(true, "Bağlantı başarılı."))
                : ApiResponse<TestConnectionResult>.Fail("Bağlantı kurulamadı.");
        }
        catch (Exception ex)
        {
            return ApiResponse<TestConnectionResult>.Ok(new TestConnectionResult(false, ex.Message));
        }
    }

    private static DatabaseConnectionDto ToDto(DatabaseConnection c) => new(
        c.Id,
        c.DbId,
        c.DisplayName,
        c.Provider.ToString(),
        c.IsActive,
        c.CreatedAt);
}
