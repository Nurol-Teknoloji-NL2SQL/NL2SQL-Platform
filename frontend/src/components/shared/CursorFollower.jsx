import { useEffect, useRef, useState } from "react";

const INTERACTIVE_SELECTORS = "a, button, [role='button'], input, textarea, select, [data-cursor-hover]";

export default function CursorFollower() {
  const dotRef = useRef(null);
  const pos = useRef({ x: -40, y: -40 });
  const target = useRef({ x: -40, y: -40 });
  const [isHovering, setIsHovering] = useState(false);
  const [isVisible, setIsVisible] = useState(false);
  const [isTouch, setIsTouch] = useState(false);

  useEffect(() => {
    // Disable on touch devices
    const touch = "ontouchstart" in window || navigator.maxTouchPoints > 0;
    if (touch) {
      setIsTouch(true);
      return;
    }

    let rafId;
    let visible = false;

    function onMouseMove(e) {
      target.current.x = e.clientX;
      target.current.y = e.clientY;
      if (!visible) {
        visible = true;
        setIsVisible(true);
      }
    }

    function onMouseLeave() {
      visible = false;
      setIsVisible(false);
    }

    function onMouseOver(e) {
      if (e.target.closest(INTERACTIVE_SELECTORS)) {
        setIsHovering(true);
      }
    }

    function onMouseOut(e) {
      if (e.target.closest(INTERACTIVE_SELECTORS)) {
        setIsHovering(false);
      }
    }

    function animate() {
      const lerp = 0.15;
      pos.current.x += (target.current.x - pos.current.x) * lerp;
      pos.current.y += (target.current.y - pos.current.y) * lerp;

      if (dotRef.current) {
        const halfW = dotRef.current.offsetWidth / 2;
        const halfH = dotRef.current.offsetHeight / 2;
        dotRef.current.style.transform = `translate3d(${pos.current.x - halfW}px, ${pos.current.y - halfH}px, 0)`;
      }

      rafId = requestAnimationFrame(animate);
    }

    document.addEventListener("mousemove", onMouseMove);
    document.documentElement.addEventListener("mouseleave", onMouseLeave);
    document.addEventListener("mouseover", onMouseOver);
    document.addEventListener("mouseout", onMouseOut);

    rafId = requestAnimationFrame(animate);

    return () => {
      cancelAnimationFrame(rafId);
      document.removeEventListener("mousemove", onMouseMove);
      document.documentElement.removeEventListener("mouseleave", onMouseLeave);
      document.removeEventListener("mouseover", onMouseOver);
      document.removeEventListener("mouseout", onMouseOut);
    };
  }, []);

  if (isTouch) return null;

  const size = isHovering ? 40 : 10;

  return (
    <div
      ref={dotRef}
      className="cursor-dot"
      style={{
        opacity: isVisible ? 1 : 0,
        width: size,
        height: size,
        backgroundColor: isHovering ? "rgba(37, 99, 235, 0.12)" : "rgba(15, 23, 42, 0.85)",
        border: isHovering ? "1.5px solid rgba(37, 99, 235, 0.4)" : "none",
      }}
    />
  );
}
