import { STRATEGIES } from '../data/gameData'
import { formatPayoff } from '../utils/gameUtils'

export function HistoryPanel({ roundHistory, onClear }) {
  return (
    <section className="panel history">
      <div className="history-head">
        <h2>Історія раундів</h2>
        {roundHistory.length > 0 && (
          <button type="button" className="btn tiny ghost" onClick={onClear}>
            Очистити
          </button>
        )}
      </div>
      {roundHistory.length === 0 ? (
        <p className="muted">Ще немає зіграних раундів.</p>
      ) : (
        <ol className="history-list">
          {roundHistory.map((r) => (
            <li key={r.id}>
              <span className="mono history-payoffs">
                {formatPayoff(r.payoffA)} / {formatPayoff(r.payoffB)}
              </span>
              <span>
                {STRATEGIES[r.i].name} vs {STRATEGIES[r.j].name}
              </span>
              <time dateTime={r.t.toISOString()}>
                {r.t.toLocaleTimeString('uk-UA', {
                  hour: '2-digit',
                  minute: '2-digit',
                  second: '2-digit',
                })}
              </time>
            </li>
          ))}
        </ol>
      )}
    </section>
  )
}
