import {
  MessageCircle,
  Zap,
  Shield,
  Database,
  Globe,
  Clock,
} from "lucide-react";
import SectionWrapper from "../shared/SectionWrapper";
import ScrollReveal from "../shared/ScrollReveal";

const features = [
  {
    icon: MessageCircle,
    title: "Doğal Dil Desteği",
    description: "Türkçe veya İngilizce yazarak veritabanınızla konuşun. SQL bilmenize gerek yok.",
  },
  {
    icon: Zap,
    title: "Otomatik SQL Üretimi",
    description: "LLM tabanlı yapay zeka, optimize edilmiş SQL sorgularını saniyeler içinde üretir.",
  },
  {
    icon: Shield,
    title: "Güvenli Sorgulama",
    description: "JWT kimlik doğrulama ve sorgu validasyonu ile verileriniz güvende.",
  },
  {
    icon: Database,
    title: "Çoklu Veritabanı",
    description: "PostgreSQL, MySQL ve daha fazlası. Farklı veritabanlarınızı tek bir arayüzden sorgulayın.",
  },
  {
    icon: Globe,
    title: "Türkçe & İngilizce",
    description: "İki dilde tam destek. Arayüz ve sorgular her iki dilde de çalışır.",
  },
  {
    icon: Clock,
    title: "Gerçek Zamanlı Sonuçlar",
    description: "Sorgularınızın sonuçlarını anında tablo formatında görün ve analiz edin.",
  },
];

export default function Features() {
  return (
    <div className="bg-white">
      <SectionWrapper id="features">
        <ScrollReveal>
          <div className="text-center mb-16">
            <span className="inline-block px-4 py-1.5 rounded-full bg-violet-100 text-violet-700 text-sm font-medium mb-4">
              Özellikler
            </span>
            <h2 className="text-3xl sm:text-4xl font-bold text-slate-900 mb-4">
              Neden NL2SQL?
            </h2>
            <p className="text-lg text-slate-600 max-w-2xl mx-auto">
              Modern yapay zeka teknolojisiyle veritabanı sorgulama deneyiminizi dönüştürün.
            </p>
          </div>
        </ScrollReveal>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {features.map((feature, i) => (
            <ScrollReveal key={i} delay={(i + 1) * 100}>
              <div className="group p-6 rounded-2xl border border-slate-200 bg-white hover:border-blue-200 hover:shadow-lg hover:shadow-blue-500/5 transition-all duration-300">
                <div className="w-12 h-12 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center mb-4 group-hover:bg-blue-600 group-hover:text-white transition-colors duration-300">
                  <feature.icon className="w-6 h-6" />
                </div>
                <h3 className="text-lg font-semibold text-slate-900 mb-2">
                  {feature.title}
                </h3>
                <p className="text-slate-600 text-sm leading-relaxed">
                  {feature.description}
                </p>
              </div>
            </ScrollReveal>
          ))}
        </div>
      </SectionWrapper>
    </div>
  );
}
