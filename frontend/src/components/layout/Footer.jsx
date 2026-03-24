import { Database, ExternalLink } from "lucide-react";

export default function Footer() {
  return (
    <footer className="bg-slate-900 text-slate-400">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Brand */}
          <div>
            <div className="flex items-center gap-2 text-white font-bold text-lg mb-3">
              <Database className="w-5 h-5 text-blue-400" />
              NL2SQL
            </div>
            <p className="text-sm leading-relaxed max-w-xs">
              Doğal dil ile veritabanı sorgulama platformu. Yapay zeka destekli,
              güvenli ve hızlı.
            </p>
          </div>

          {/* Quick links */}
          <div>
            <h4 className="text-white font-semibold mb-4">Hızlı Bağlantılar</h4>
            <ul className="space-y-2 text-sm">
              <li>
                <a href="#how-it-works" className="hover:text-blue-400 transition-colors">
                  Nasıl Çalışır
                </a>
              </li>
              <li>
                <a href="#features" className="hover:text-blue-400 transition-colors">
                  Özellikler
                </a>
              </li>
              <li>
                <a href="#stats" className="hover:text-blue-400 transition-colors">
                  İstatistikler
                </a>
              </li>
              <li>
                <a href="#faq" className="hover:text-blue-400 transition-colors">
                  SSS
                </a>
              </li>
              <li>
                <a href="#query" className="hover:text-blue-400 transition-colors">
                  Sorgu Yap
                </a>
              </li>
            </ul>
          </div>

          {/* Tech stack */}
          <div>
            <h4 className="text-white font-semibold mb-4">Teknolojiler</h4>
            <ul className="space-y-2 text-sm">
              <li>React + Vite</li>
              <li>.NET 8 (Core Backend)</li>
              <li>FastAPI + LangGraph (AI)</li>
              <li>PostgreSQL</li>
              <li>Docker & Kubernetes</li>
            </ul>
          </div>
        </div>

        {/* Bottom bar */}
        <div className="mt-12 pt-8 border-t border-slate-800 flex flex-col sm:flex-row items-center justify-between gap-4">
          <p className="text-sm">
            &copy; {new Date().getFullYear()} NL2SQL Platform. Tüm hakları saklıdır.
          </p>
          <a
            href="https://github.com"
            target="_blank"
            rel="noopener noreferrer"
            className="flex items-center gap-2 text-sm hover:text-blue-400 transition-colors"
          >
            <ExternalLink className="w-4 h-4" />
            GitHub
          </a>
        </div>
      </div>
    </footer>
  );
}
