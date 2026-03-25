import { useState } from "react";
import { ChevronDown } from "lucide-react";
import SectionWrapper from "../shared/SectionWrapper";
import ScrollReveal from "../shared/ScrollReveal";

const faqs = [
  {
    question: "NL2SQL nedir ve nasıl çalışır?",
    answer:
      "NL2SQL, doğal dilde yazdığınız soruları otomatik olarak SQL sorgularına çeviren bir yapay zeka platformudur. LLM (Large Language Model) teknolojisi kullanarak sorunuzu analiz eder, veritabanı şemanızı inceler ve optimize edilmiş bir SQL sorgusu üretir.",
  },
  {
    question: "SQL bilmem gerekiyor mu?",
    answer:
      "Hayır, hiçbir SQL bilgisine ihtiyacınız yok. Sorunuzu Türkçe veya İngilizce olarak doğal dilde yazmanız yeterli. Sistem sizin için uygun SQL sorgusunu otomatik olarak oluşturur.",
  },
  {
    question: "Hangi veritabanlarını destekliyor?",
    answer:
      "Şu anda PostgreSQL desteği mevcuttur. MySQL, SQLite ve diğer popüler veritabanları için destek eklenmeye devam edilmektedir.",
  },
  {
    question: "Verilerim güvende mi?",
    answer:
      "Evet. Platform JWT tabanlı kimlik doğrulama, sorgu validasyonu ve rate limiting kullanır. Sorgularınız şifrelenerek iletilir ve veritabanı bağlantılarınız güvenli bir şekilde yönetilir.",
  },
  {
    question: "Ücretsiz mi?",
    answer:
      "NL2SQL açık kaynaklı bir projedir. Kendi sunucunuzda barındırarak ücretsiz olarak kullanabilirsiniz. Docker Compose ile birkaç dakikada kurulum yapabilirsiniz.",
  },
];

export default function FAQ() {
  const [openIndex, setOpenIndex] = useState(null);

  const toggle = (i) => setOpenIndex(openIndex === i ? null : i);

  return (
    <div className="bg-slate-50">
      <SectionWrapper id="faq">
        <ScrollReveal>
          <div className="text-center mb-16">
            <span className="inline-block px-4 py-1.5 rounded-full bg-emerald-100 text-emerald-700 text-sm font-medium mb-4">
              SSS
            </span>
            <h2 className="text-3xl sm:text-4xl font-bold text-slate-900 mb-4">
              Sıkça Sorulan Sorular
            </h2>
            <p className="text-lg text-slate-600 max-w-2xl mx-auto">
              NL2SQL hakkında merak edilen soruların yanıtları.
            </p>
          </div>
        </ScrollReveal>

        <div className="max-w-3xl mx-auto space-y-3">
          {faqs.map((faq, i) => (
            <ScrollReveal key={i} delay={(i + 1) * 100}>
              <div className="rounded-xl border border-slate-200 bg-white overflow-hidden transition-shadow hover:shadow-md">
                <button
                  onClick={() => toggle(i)}
                  className="w-full flex items-center justify-between p-5 text-left cursor-pointer"
                >
                  <span className="font-semibold text-slate-900 pr-4">
                    {faq.question}
                  </span>
                  <ChevronDown
                    className={`w-5 h-5 text-slate-400 shrink-0 transition-transform duration-300 ${
                      openIndex === i ? "rotate-180" : ""
                    }`}
                  />
                </button>
                <div
                  className={`faq-content ${openIndex === i ? "open" : ""}`}
                >
                  <div className="px-5 pb-5 text-slate-600 leading-relaxed">
                    {faq.answer}
                  </div>
                </div>
              </div>
            </ScrollReveal>
          ))}
        </div>
      </SectionWrapper>
    </div>
  );
}
