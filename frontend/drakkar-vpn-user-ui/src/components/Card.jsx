import "../styles/Tariffs.css";

export default function Card({ title, subtitle, children }) {
  return (
    <div className="card">
      <div>
        <h3>{title}</h3>
        <p>{subtitle}</p>
      </div>
      {children}
    </div>
  );
}
