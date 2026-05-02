import { formatPayoff } from '../utils/gameUtils'

export function ScoreboardPanel({ totals, leaderSummary }) {
  return (
    <section className="panel scoreboard-panel" aria-live="polite">
      <h2 className="scoreboard-heading">Загальний рахунок</h2>
      <div className="scoreboard-row">
        <div className="scoreboard-cell">
          <span className="scoreboard-label">Ви (A)</span>
          <span className={`scoreboard-value mono ${totals.sumA >= 0 ? 'pos' : 'neg'}`}>
            {formatPayoff(totals.sumA)}
          </span>
        </div>
        <div className="scoreboard-sep" aria-hidden="true">
          :
        </div>
        <div className="scoreboard-cell">
          <span className="scoreboard-label">Ворог (B)</span>
          <span className={`scoreboard-value mono ${totals.sumB >= 0 ? 'pos' : 'neg'}`}>
            {formatPayoff(totals.sumB)}
          </span>
        </div>
      </div>
      <p className="scoreboard-meta muted small">
        Раундів: {totals.rounds}. {leaderSummary}
      </p>
    </section>
  )
}
