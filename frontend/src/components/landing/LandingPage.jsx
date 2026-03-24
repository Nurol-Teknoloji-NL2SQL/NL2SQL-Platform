import Hero from "./Hero";
import HowItWorks from "./HowItWorks";
import Features from "./Features";
import Stats from "./Stats";
import QueryInterface from "../query/QueryInterface";
import FAQ from "./FAQ";

export default function LandingPage() {
  return (
    <>
      <Hero />
      <QueryInterface />
      <HowItWorks />
      <Features />
      <Stats />
      <FAQ />
    </>
  );
}
