import { Routes, Route } from "react-router-dom";
import Navbar from "./components/layout/Navbar";
import Footer from "./components/layout/Footer";
import LandingPage from "./components/landing/LandingPage";
import QueryInterface from "./components/query/QueryInterface";
import SmoothScroll from "./components/shared/SmoothScroll";
import CursorFollower from "./components/shared/CursorFollower";

export default function App() {
  return (
    <SmoothScroll>
      <div className="min-h-screen bg-white">
        <CursorFollower />
        <Navbar />
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route
            path="/query"
            element={
              <div className="pt-16">
                <QueryInterface />
              </div>
            }
          />
        </Routes>
        <Footer />
      </div>
    </SmoothScroll>
  );
}
