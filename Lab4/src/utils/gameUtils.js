import { STRATEGIES } from '../data/gameData'

export function formatPayoff(v) {
  if (v > 0) return `+${v}`
  return String(v)
}

export function classPerksLine(perStrategy) {
  const parts = STRATEGIES.map((s, i) =>
    perStrategy[i] !== 0 ? `${s.name} ${formatPayoff(perStrategy[i])}` : null,
  ).filter(Boolean)
  return parts.length ? parts.join(' · ') : 'без бонусів'
}

export function roundPayoffs(i, j, effectiveCell) {
  if (i === j && effectiveCell !== 0) {
    const p = -effectiveCell
    return { payoffA: p, payoffB: p, sameStrategyNonZero: true }
  }
  return {
    payoffA: effectiveCell,
    payoffB: -effectiveCell,
    sameStrategyNonZero: false,
  }
}

export function outcomeText(lc) {
  if (lc.sameStrategyNonZero) {
    return `Однакові стратегії й ефективне значення ≠ 0: обидва отримують ${formatPayoff(lc.payoffA)} (мінус ефективної бази ${formatPayoff(lc.effectiveCell)}).`
  }
  if (lc.payoffA > 0) return 'Перевага гравця A (виграш A, програш B).'
  if (lc.payoffA < 0) return 'Перевага гравця B (програш A, виграш B).'
  return 'Рівновага в раунді (нічия за матрицею).'
}
