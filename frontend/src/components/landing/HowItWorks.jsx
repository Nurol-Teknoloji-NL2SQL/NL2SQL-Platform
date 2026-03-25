import { MessageSquare, Database, BarChart3, ArrowRight } from "lucide-react";
import SectionWrapper from "../shared/SectionWrapper";
import ScrollReveal from "../shared/ScrollReveal";

const steps = [
  {
    icon: MessageSquare,
    title: "Sorunuzu Yazın",
    description:
      "Doğal dilde, Türkçe veya İngilizce olarak veritabanınıza sormak istediğiniz soruyu yazın.",
    color: "bg-blue-100 text-blue-600",
  },
  {
    icon: Database,
    title: "SQL Otomatik Üretilsin",
    description:
      "Yapay zeka modelimiz sorunuzu analiz eder ve optimize edilmiş SQL sorgusunu otomatik olarak üretir.",
    color: "bg-violet-100 text-violet-600",
  },
  {
    icon: BarChart3,
    title: "Sonuçları Görün",
    description:
      "Üretilen SQL sorgusu veritabanınızda çalıştırılır ve sonuçlar anlaşılır bir tablo halinde sunulur.",
    color: "bg-emerald-100 text-emerald-600",
  },
];

export default function HowItWorks() {
  return (
    <div className="bg-slate-50">
      <SectionWrapper id="how-it-works">
        <ScrollReveal>
          <div className="text-center mb-16">
            <span className="inline-block px-4 py-1.5 rounded-full bg-blue-100 text-blue-700 text-sm font-medium mb-4">
              Adım Adım
            </span>
            <h2 className="text-3xl sm:text-4xl font-bold text-slate-900 mb-4">
              Nasıl Çalışır?
            </h2>
            <p className="text-lg text-slate-600 max-w-2xl mx-auto">
              Üç basit adımda veritabanınızdan istediğiniz veriye ulaşın.
            </p>
          </div>
        </ScrollReveal>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 relative">

          {steps.map((step, i) => (
            <ScrollReveal key={i} delay={(i + 1) * 100}>
              <div className="relative flex flex-col items-center text-center group">
                {/* Step number */}
                <div className="absolute -top-3 -right-1 w-7 h-7 rounded-full bg-blue-600 text-white text-xs font-bold flex items-center justify-center z-10">
                  {i + 1}
                </div>

                {/* Icon */}
                <div
                  className={`w-16 h-16 rounded-2xl ${step.color} flex items-center justify-center mb-6 group-hover:scale-110 transition-transform duration-300 relative z-10`}
                >
                  <step.icon className="w-7 h-7" />
                </div>

                {/* Arrow between steps (mobile) */}
                {i < steps.length - 1 && (
                  <div className="md:hidden my-2 text-slate-300">
                    <ArrowRight className="w-5 h-5 rotate-90" />
                  </div>
                )}

                {/* Content */}
                <h3 className="text-xl font-semibold text-slate-900 mb-3">
                  {step.title}
                </h3>
                <p className="text-slate-600 leading-relaxed max-w-xs">
                  {step.description}
                </p>
              </div>
            </ScrollReveal>
          ))}
        </div>
      </SectionWrapper>
    </div>
  );
}
