import { STRATEGIES } from '../data/gameData'
import { formatPayoff } from '../utils/gameUtils'

export function MaximinBlock({ maximin, visible, onToggle }) {
  return (
    <div className="maximin-block">
      <button type="button" className="btn secondary" onClick={onToggle}>
        {visible ? 'Приховати' : 'Показати'} оптимальну стратегію A (максимін)
      </button>
      {visible && maximin.alpha !== null && (
        <div className="maximin-out">
          <p>
            Нижня ціна гри <span className="mono">α = {formatPayoff(maximin.alpha)}</span>.
            Оптимальні чисті стратегії A (максимін):{' '}
            <strong>{maximin.rowIndices.map((i) => STRATEGIES[i].name).join(', ')}</strong>.
          </p>
          <p className="muted small">
            Для кожного рядка взято мінімум по стовпцях; серед цих мінімумів обрано
            найбільший — це гарантований результат при найгіршому B. Рахунок по
            поточній таблиці (з бонусами класів за рядками/стовпцями).
          </p>
        </div>
      )}
    </div>
  )
}
