import { useEffect, useRef, useState } from "react";
import SectionWrapper from "../shared/SectionWrapper";
import ScrollReveal from "../shared/ScrollReveal";

function useCountUp(target, duration = 2000, isVisible = false) {
  const [count, setCount] = useState(0);
  const hasAnimated = useRef(false);

  useEffect(() => {
    if (!isVisible || hasAnimated.current) return;
    hasAnimated.current = true;

    const startTime = Date.now();
    const tick = () => {
      const elapsed = Date.now() - startTime;
      const progress = Math.min(elapsed / duration, 1);
      const eased = 1 - Math.pow(1 - progress, 3);
      setCount(Math.floor(eased * target));
      if (progress < 1) requestAnimationFrame(tick);
    };
    requestAnimationFrame(tick);
  }, [isVisible, target, duration]);

  return count;
}

const stats = [
  { value: 3, suffix: "+", label: "Desteklenen Veritabanı", description: "PostgreSQL, MySQL ve daha fazlası" },
  { value: 99, suffix: "%", label: "Doğruluk Oranı", description: "LLM tabanlı akıllı sorgu üretimi" },
  { value: 2, suffix: "s", prefix: "<", label: "Ortalama Yanıt Süresi", description: "Hızlı ve optimize sorgular" },
  { value: 7, suffix: "/24", label: "Sürekli Erişim", description: "Her zaman kullanıma hazır platform" },
];

export default function Stats() {
  const sectionRef = useRef(null);
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) setIsVisible(true);
      },
      { threshold: 0.3 }
    );
    if (sectionRef.current) observer.observe(sectionRef.current);
    return () => observer.disconnect();
  }, []);

  return (
    <div className="bg-gradient-to-br from-slate-900 via-blue-900 to-slate-900" ref={sectionRef}>
      <SectionWrapper id="stats">
        <ScrollReveal>
          <div className="text-center mb-16">
            <span className="inline-block px-4 py-1.5 rounded-full bg-blue-500/20 text-blue-300 text-sm font-medium mb-4">
              Rakamlarla
            </span>
            <h2 className="text-3xl sm:text-4xl font-bold text-white mb-4">
              Platform İstatistikleri
            </h2>
            <p className="text-lg text-blue-200/70 max-w-2xl mx-auto">
              NL2SQL platformunun sunduğu performans ve güvenilirlik.
            </p>
          </div>
        </ScrollReveal>

        <div className="grid grid-cols-2 lg:grid-cols-4 gap-8">
          {stats.map((stat, i) => (
            <ScrollReveal key={i} delay={(i + 1) * 100}>
              <StatCard stat={stat} isVisible={isVisible} />
            </ScrollReveal>
          ))}
        </div>
      </SectionWrapper>
    </div>
  );
}

function StatCard({ stat, isVisible }) {
  const count = useCountUp(stat.value, 1500, isVisible);

  return (
    <div className="text-center p-6 rounded-2xl bg-white/5 border border-white/10 backdrop-blur-sm hover:bg-white/10 transition-colors duration-300">
      <div className="text-3xl sm:text-4xl font-bold text-white mb-2">
        {stat.prefix || ""}
        {count}
        {stat.suffix}
      </div>
      <div className="text-base font-semibold text-blue-200 mb-1">{stat.label}</div>
      <div className="text-sm text-blue-300/60">{stat.description}</div>
    </div>
  );
}
