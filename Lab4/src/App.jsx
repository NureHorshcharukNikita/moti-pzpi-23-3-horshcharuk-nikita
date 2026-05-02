import { useCallback, useMemo, useState } from 'react'
import {
  PLAYER_CLASSES,
  DEFAULT_MATRIX,
  cloneMatrix,
  buildDisplayMatrix,
} from './data/gameData'
import { computeMaximin } from './utils/maximin'
import { roundPayoffs } from './utils/gameUtils'
import { AppHeader } from './components/AppHeader'
import { ClassesHeroPanel } from './components/ClassesHeroPanel'
import { StrategiesPanel } from './components/StrategiesPanel'
import { ScoreboardPanel } from './components/ScoreboardPanel'
import { PlayPanel } from './components/PlayPanel'
import { MatrixPanel } from './components/MatrixPanel'
import { HistoryPanel } from './components/HistoryPanel'
import { AppFooter } from './components/AppFooter'
import './App.css'

export default function App() {
  const [matrix, setMatrix] = useState(() => cloneMatrix(DEFAULT_MATRIX))
  const [editMatrix, setEditMatrix] = useState(() => cloneMatrix(DEFAULT_MATRIX))
  const [editOpen, setEditOpen] = useState(false)
  const [selectedClassA, setSelectedClassA] = useState(null)
  const [selectedClassB, setSelectedClassB] = useState(null)
  const [selectedRow, setSelectedRow] = useState(null)
  const [lastCell, setLastCell] = useState(null)
  const [roundHistory, setRoundHistory] = useState([])
  const [maximinVisible, setMaximinVisible] = useState(false)

  const bothClassesPicked =
    selectedClassA !== null && selectedClassB !== null

  const displayMatrix = useMemo(
    () => buildDisplayMatrix(matrix, selectedClassA, selectedClassB),
    [matrix, selectedClassA, selectedClassB],
  )

  const maximin = useMemo(
    () => computeMaximin(displayMatrix),
    [displayMatrix],
  )

  const totals = useMemo(() => {
    let sumA = 0
    let sumB = 0
    for (const r of roundHistory) {
      sumA += r.payoffA
      sumB += r.payoffB
    }
    return { sumA, sumB, rounds: roundHistory.length }
  }, [roundHistory])

  const leaderSummary = useMemo(() => {
    const { sumA, sumB, rounds } = totals
    if (rounds === 0) return 'Зіграйте перший раунд — тут з’явиться рахунок.'
    const d = sumA - sumB
    if (d > 0) return `Лідируєте ви (A): +${d} балів до ворога.`
    if (d < 0) return `Лідирує ворог (B): +${-d} балів до вас.`
    return 'Рахунок рівний: паритет.'
  }, [totals])

  const applyEdits = useCallback(() => {
    setMatrix(cloneMatrix(editMatrix))
    setEditOpen(false)
    setLastCell(null)
    setMaximinVisible(false)
  }, [editMatrix])

  const cancelEdit = useCallback(() => {
    setEditOpen(false)
    setEditMatrix(cloneMatrix(matrix))
  }, [matrix])

  const resetDefaults = useCallback(() => {
    const d = cloneMatrix(DEFAULT_MATRIX)
    setMatrix(d)
    setEditMatrix(d)
    setEditOpen(false)
    setLastCell(null)
    setRoundHistory([])
    setMaximinVisible(false)
    setSelectedClassA(null)
    setSelectedClassB(null)
  }, [])

  const updateCell = (i, j, raw) => {
    const n = raw === '' || raw === '-' ? 0 : Number(raw)
    setEditMatrix((prev) => {
      const next = prev.map((row) => [...row])
      next[i][j] = Number.isFinite(n) ? n : 0
      return next
    })
  }

  const playRound = () => {
    if (selectedRow === null) return
    const j = Math.floor(Math.random() * matrix[0].length)
    const matrixRaw = matrix[selectedRow][j]
    const bothClassesUsed =
      selectedClassA !== null && selectedClassB !== null
    const bonusAAtRow = bothClassesUsed
      ? PLAYER_CLASSES[selectedClassA].perStrategy[selectedRow]
      : 0
    const bonusBAtCol = bothClassesUsed
      ? PLAYER_CLASSES[selectedClassB].perStrategy[j]
      : 0
    const effectiveCell = bothClassesUsed
      ? displayMatrix[selectedRow][j]
      : matrixRaw
    const { payoffA, payoffB, sameStrategyNonZero } = roundPayoffs(
      selectedRow,
      j,
      effectiveCell,
    )

    setLastCell({
      i: selectedRow,
      j,
      matrixRaw,
      effectiveCell,
      bonusAAtRow,
      bonusBAtCol,
      classAIdx: selectedClassA,
      classBIdx: selectedClassB,
      bothClassesUsed,
      payoffA,
      payoffB,
      sameStrategyNonZero,
    })
    
    setRoundHistory((h) => [
      {
        id: crypto.randomUUID(),
        i: selectedRow,
        j,
        matrixRaw,
        effectiveCell,
        bonusAAtRow,
        bonusBAtCol,
        classAIdx: selectedClassA,
        classBIdx: selectedClassB,
        bothClassesUsed,
        payoffA,
        payoffB,
        sameStrategyNonZero,
        t: new Date(),
      },
      ...h,
    ])
  }

  const handleSelectClassA = useCallback((idx) => {
    setSelectedClassA((prev) => {
      const next = prev === idx ? null : idx
      return next
    })
    setLastCell(null)
  }, [])

  const handleSelectClassB = useCallback((idx) => {
    setSelectedClassB((prev) => {
      const next = prev === idx ? null : idx
      return next
    })
    setLastCell(null)
  }, [])

  const handleStartEdit = useCallback(() => {
    setEditMatrix(cloneMatrix(matrix))
    setEditOpen(true)
  }, [matrix])

  return (
    <div className="app">
      <AppHeader />
      <ClassesHeroPanel />
      <StrategiesPanel />
      <ScoreboardPanel totals={totals} leaderSummary={leaderSummary} />

      <div className="layout-split">
        <PlayPanel
          selectedRow={selectedRow}
          onSelectRow={setSelectedRow}
          editOpen={editOpen}
          onPlayRound={playRound}
          onReset={resetDefaults}
          lastCell={lastCell}
          maximin={maximin}
          maximinVisible={maximinVisible}
          onToggleMaximin={() => setMaximinVisible((v) => !v)}
        />

        <MatrixPanel
          editOpen={editOpen}
          bothClassesPicked={bothClassesPicked}
          editMatrix={editMatrix}
          displayMatrix={displayMatrix}
          lastCell={lastCell}
          selectedClassA={selectedClassA}
          selectedClassB={selectedClassB}
          onSelectClassA={handleSelectClassA}
          onSelectClassB={handleSelectClassB}
          onUpdateCell={updateCell}
          onApplyEdits={applyEdits}
          onCancelEdit={cancelEdit}
          onStartEdit={handleStartEdit}
        />
      </div>

      <HistoryPanel roundHistory={roundHistory} onClear={() => setRoundHistory([])} />
      <AppFooter />
    </div>
  )
}
