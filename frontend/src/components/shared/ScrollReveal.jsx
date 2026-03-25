import { useEffect, useRef, useState } from "react";

export default function ScrollReveal({
  children,
  className = "",
  direction = "up",
  delay = 0,
  threshold = 0.1,
}) {
  const ref = useRef(null);
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    const observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          setIsVisible(true);
          observer.unobserve(entry.target);
        }
      },
      { threshold }
    );

    if (ref.current) observer.observe(ref.current);
    return () => observer.disconnect();
  }, [threshold]);

  const hiddenClass =
    direction === "left"
      ? "scroll-hidden-left"
      : direction === "right"
      ? "scroll-hidden-right"
      : "scroll-hidden";

  const visibleClass =
    direction === "left"
      ? "scroll-visible-left"
      : direction === "right"
      ? "scroll-visible-right"
      : "scroll-visible";

  const delayClass = delay > 0 ? `delay-${delay}` : "";

  return (
    <div
      ref={ref}
      className={`${hiddenClass} ${isVisible ? visibleClass : ""} ${delayClass} ${className}`}
    >
      {children}
    </div>
  );
}
