export const STRATEGIES = [
  {
    id: 'aggro',
    short: 'A1',
    name: 'Aggro',
    title: 'Агресія',
    desc: 'Швидкий тиск і шкода супернику до розгортання його плану.',
  },
  {
    id: 'control',
    short: 'A2',
    name: 'Control',
    title: 'Контроль',
    desc: 'Очищення поля, відповіді на загрози, нейтралізація темпу.',
  },
  {
    id: 'midrange',
    short: 'A3',
    name: 'Midrange',
    title: 'Мідрейндж',
    desc: 'Баланс розвитку поля, відповіді та поступового тиску.',
  },
  {
    id: 'combo',
    short: 'A4',
    name: 'Combo',
    title: 'Комбо',
    desc: 'Накопичення карт для потужного розіграшу в один хід.',
  },
  {
    id: 'defensive',
    short: 'A5',
    name: 'Defensive',
    title: 'Захист',
    desc: 'Лікування, блокування, затягування гри.',
  },
]

export const PLAYER_CLASSES = [
  {
    id: 'knight',
    short: 'K',
    name: 'Лицар',
    perStrategy: [2, 0, 0, 0, 2],
    desc: 'Сила ближнього бою: посилює Aggro та Defensive у матриці (+2 до відповідних рядків/стовпців для власника класу).',
  },
  {
    id: 'mage',
    short: 'M',
    name: 'Маг',
    perStrategy: [0, 2, 0, 2, 0],
    desc: 'Магія та заклинання: посилює Control і Combo — «магічні» архетипи колоди.',
  },
  {
    id: 'rogue',
    short: 'R',
    name: 'Розбійник',
    perStrategy: [2, 0, 0, 2, 0],
    desc: 'Темп і вибухові комбо: Aggro та Combo отримують бонус у клітинках.',
  },
  {
    id: 'priest',
    short: 'P',
    name: 'Жрець',
    perStrategy: [0, 1, 0, 0, 2],
    desc: 'Підтримка та стійкість: Control і Defensive.',
  },
  {
    id: 'druid',
    short: 'D',
    name: 'Друїд',
    perStrategy: [0, 0, 2, 0, 1],
    desc: 'Природа та ресурс: Midrange і частково Defensive.',
  },
]

export const DEFAULT_MATRIX = [
  [-5, 5, 8, 4, -7],
  [-6, -7, 8, -3, 3],
  [-7, 8, -8, 0, 5],
  [-6, 4, -6, -5, 8],
  [-8, -2, 1, -4, 7],
]

export function cloneMatrix(m) {
  return m.map((row) => [...row])
}

export function buildDisplayMatrix(base, classAIdx, classBIdx) {
  if (classAIdx === null || classBIdx === null) return base
  const a = PLAYER_CLASSES[classAIdx].perStrategy
  const b = PLAYER_CLASSES[classBIdx].perStrategy
  return base.map((row, i) => row.map((v, j) => v + a[i] - b[j]))
}
