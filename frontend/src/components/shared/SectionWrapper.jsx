export default function SectionWrapper({ id, className = "", children }) {
  return (
    <section id={id} className={`max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-16 sm:py-24 ${className}`}>
      {children}
    </section>
  );
}
