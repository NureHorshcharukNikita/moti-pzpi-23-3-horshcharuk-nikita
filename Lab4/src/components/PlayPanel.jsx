import { STRATEGIES } from '../data/gameData'
import { RoundResultCard } from './RoundResultCard'
import { MaximinBlock } from './MaximinBlock'

export function PlayPanel({
  selectedRow,
  onSelectRow,
  editOpen,
  onPlayRound,
  onReset,
  lastCell,
  maximin,
  maximinVisible,
  onToggleMaximin,
}) {
  const canPlay = selectedRow !== null && !editOpen

  return (
    <section className="panel play">
      <h2>Хід гри</h2>
      <p className="hint">
        Оберіть стратегію гравця A і натисніть «Зіграти раунд» — використовується
        базова матриця. За бажанням над матрицею вкажіть класи A та B: тоді в
        раунді та в таблиці враховуються бонуси по рядках і стовпцях.
      </p>

      <h3 className="subhead">Стратегія A</h3>
      <div className="pick-row" role="group" aria-label="Стратегія гравця A">
        {STRATEGIES.map((s, idx) => (
          <button
            key={s.id}
            type="button"
            className={`pick ${selectedRow === idx ? 'pick--on' : ''}`}
            onClick={() => onSelectRow(idx)}
          >
            <span className="pick-name">{s.name}</span>
            <span className="pick-sub">{s.title}</span>
          </button>
        ))}
      </div>
      <div className="actions">
        <button
          type="button"
          className="btn primary"
          disabled={!canPlay}
          onClick={onPlayRound}
        >
          Зіграти раунд
        </button>
        <button type="button" className="btn ghost" onClick={onReset}>
          Скинути матрицю
        </button>
      </div>

      <RoundResultCard lastCell={lastCell} />

      <MaximinBlock
        maximin={maximin}
        visible={maximinVisible}
        onToggle={onToggleMaximin}
      />
    </section>
  )
}
