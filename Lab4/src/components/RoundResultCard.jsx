import { STRATEGIES, PLAYER_CLASSES } from '../data/gameData'
import { formatPayoff, outcomeText } from '../utils/gameUtils'

export function RoundResultCard({ lastCell }) {
  if (!lastCell) return null

  const bothClassesUsed =
    lastCell.bothClassesUsed ??
    (lastCell.classAIdx !== null && lastCell.classBIdx !== null)

  const nameA =
    lastCell.classAIdx !== null
      ? PLAYER_CLASSES[lastCell.classAIdx].name
      : 'не обрано'
  const nameB =
    lastCell.classBIdx !== null
      ? PLAYER_CLASSES[lastCell.classBIdx].name
      : 'не обрано'

  const partialClassPick =
    !bothClassesUsed &&
    (lastCell.classAIdx !== null || lastCell.classBIdx !== null)

  return (
    <div className="result-card" aria-live="polite">
      {bothClassesUsed ? (
        <div className="result-classes muted small">
          <span>
            Клас A: <strong>{nameA}</strong>
            {', '}
            рядок {STRATEGIES[lastCell.i].name}:{' '}
            <span className="mono">{formatPayoff(lastCell.bonusAAtRow)}</span>
          </span>
          <span>
            Клас B: <strong>{nameB}</strong>
            {', '}
            стовпець {STRATEGIES[lastCell.j].name}:{' '}
            <span className="mono">{formatPayoff(lastCell.bonusBAtCol)}</span> (мінус для A)
          </span>
        </div>
      ) : (
        <p className="result-classes muted small">
          {partialClassPick
            ? 'Обрано лише одного з класів — бонуси не застосовуються. Раунд за базовою матрицею (оберіть клас і для A, і для B).'
            : 'Класи не обрані — раунд за базовою матрицею без бонусів героїв.'}
        </p>
      )}
      <p className="result-breakdown muted small">
        Значення в таблиці:{' '}
        <strong className="mono">{formatPayoff(lastCell.effectiveCell)}</strong>
        {lastCell.matrixRaw !== lastCell.effectiveCell && (
          <>
            {' '}
            (= база {formatPayoff(lastCell.matrixRaw)} + A{' '}
            {formatPayoff(lastCell.bonusAAtRow)} − B {formatPayoff(lastCell.bonusBAtCol)})
          </>
        )}
        {lastCell.sameStrategyNonZero &&
          `; за правилом однакових стратегій — ${formatPayoff(lastCell.payoffA)} для обох.`}
        {!lastCell.sameStrategyNonZero && '.'}
      </p>
      <div className="result-grid">
        <div>
          <span className="lbl">A</span>
          <strong>{STRATEGIES[lastCell.i].name}</strong>
        </div>
        <div>
          <span className="lbl">B (випадково)</span>
          <strong>{STRATEGIES[lastCell.j].name}</strong>
        </div>
        <div className="result-payoff">
          <span className="lbl">Виграш A</span>
          <strong className={lastCell.payoffA >= 0 ? 'pos' : 'neg'}>
            {formatPayoff(lastCell.payoffA)}
          </strong>
        </div>
        <div className="result-payoff">
          <span className="lbl">Виграш B</span>
          <strong className={lastCell.payoffB >= 0 ? 'pos' : 'neg'}>
            {formatPayoff(lastCell.payoffB)}
          </strong>
        </div>
      </div>
      <p className="result-note">{outcomeText(lastCell)}</p>
    </div>
  )
}
