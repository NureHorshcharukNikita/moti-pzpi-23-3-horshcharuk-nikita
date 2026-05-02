import { STRATEGIES } from '../data/gameData'

export function StrategiesPanel() {
  return (
    <section className="panel">
      <h2>Стратегії</h2>
      <ul className="strategy-grid">
        {STRATEGIES.map((s) => (
          <li key={s.id} className="strategy-card">
            <span className="strategy-meta">
              {s.short} · {s.name}
            </span>
            <strong>{s.title}</strong>
            <p>{s.desc}</p>
          </li>
        ))}
      </ul>
    </section>
  )
}
