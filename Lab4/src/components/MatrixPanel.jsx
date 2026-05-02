import { STRATEGIES, PLAYER_CLASSES } from '../data/gameData'
import { formatPayoff, classPerksLine } from '../utils/gameUtils'

export function MatrixPanel({
  editOpen,
  bothClassesPicked,
  editMatrix,
  displayMatrix,
  lastCell,
  selectedClassA,
  selectedClassB,
  onSelectClassA,
  onSelectClassB,
  onUpdateCell,
  onApplyEdits,
  onCancelEdit,
  onStartEdit,
}) {
  return (
    <section className={`panel matrix-panel${editOpen ? ' matrix-panel--edit' : ''}`}>
      <div className="matrix-head">
        <div className="matrix-title-wrap">
          <h2>Платіжна матриця</h2>
          {editOpen && (
            <p className="matrix-edit-hint muted small">
              Редагується базова матриця (без класів). Після «Застосувати» за потреби
              оберіть класи — таблиця покаже бонуси по стратегіях.
            </p>
          )}
          {!editOpen && (
            <p className="matrix-edit-hint muted small">
              {bothClassesPicked
                ? 'У клітинці (i, j): база + бонус класу A для рядка i − бонус класу B для стовпця j (різні стратегії зміщуються по-різному).'
                : 'Показано базова матриця. Оберіть обидва класи — у таблиці з’являться значення з бонусами; без класів раунди теж можна грати по базі.'}
            </p>
          )}
        </div>
        <div className="matrix-toolbar">
          {editOpen ? (
            <>
              <button type="button" className="btn tiny primary" onClick={onApplyEdits}>
                Застосувати
              </button>
              <button type="button" className="btn tiny ghost" onClick={onCancelEdit}>
                Скасувати
              </button>
            </>
          ) : (
            <button type="button" className="btn tiny" onClick={onStartEdit}>
              Редагувати
            </button>
          )}
        </div>
      </div>

      {!editOpen && (
        <div className="matrix-classes-bar">
          <div className="matrix-class-row">
            <span className="matrix-class-label">Клас A</span>
            <div className="matrix-class-picks" role="group" aria-label="Клас гравця A">
              {PLAYER_CLASSES.map((c, idx) => (
                <button
                  key={c.id}
                  type="button"
                  title={`${c.desc}\n${classPerksLine(c.perStrategy)}`}
                  className={`chip ${selectedClassA === idx ? 'chip--on' : ''}`}
                  onClick={() => onSelectClassA(idx)}
                >
                  <span className="chip-name">{c.name}</span>
                </button>
              ))}
            </div>
          </div>
          <div className="matrix-class-row">
            <span className="matrix-class-label">Клас B</span>
            <div className="matrix-class-picks" role="group" aria-label="Клас гравця B">
              {PLAYER_CLASSES.map((c, idx) => (
                <button
                  key={c.id}
                  type="button"
                  title={`${c.desc}\n${classPerksLine(c.perStrategy)}`}
                  className={`chip ${selectedClassB === idx ? 'chip--on' : ''}`}
                  onClick={() => onSelectClassB(idx)}
                >
                  <span className="chip-name">{c.name}</span>
                </button>
              ))}
            </div>
          </div>
          <p className="muted small matrix-class-hint">
            Повторне натискання на обраний клас знімає вибір.
          </p>
        </div>
      )}

      <div className="table-scroll">
        <table className={`matrix${editOpen ? ' matrix--editing' : ''}`}>
          <thead>
            <tr>
              <th>A \ B</th>
              {STRATEGIES.map((s) => (
                <th key={s.id}>{s.name}</th>
              ))}
            </tr>
          </thead>
          <tbody>
            {STRATEGIES.map((_, i) => {
              const row = editOpen ? editMatrix[i] : displayMatrix[i]
              return (
                <tr key={STRATEGIES[i].id}>
                  <th scope="row">{STRATEGIES[i].name}</th>
                  {row.map((cell, j) => {
                    const active =
                      !editOpen && lastCell && lastCell.i === i && lastCell.j === j
                    if (editOpen) {
                      return (
                        <td key={j} className="td-input">
                          <input
                            className="cell-input"
                            type="number"
                            value={cell}
                            onChange={(e) => onUpdateCell(i, j, e.target.value)}
                            aria-label={`Базовий виграш A, ${STRATEGIES[i].name} проти ${STRATEGIES[j].name}`}
                          />
                        </td>
                      )
                    }
                    return (
                      <td
                        key={j}
                        className={active ? 'cell--active' : ''}
                        title={
                          active
                            ? `У таблиці: ${formatPayoff(cell)}${
                                i === j && cell !== 0
                                  ? ` → у раунді обидва: ${formatPayoff(-cell)}`
                                  : ''
                              }`
                            : undefined
                        }
                      >
                        {cell}
                      </td>
                    )
                  })}
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>
      <p className="muted small matrix-caption">
        {editOpen
          ? 'Під час редагування раунди вимкнено до застосування змін.'
          : bothClassesPicked
            ? 'У клітинці (i, j) — база + бонус класу A для рядка i − бонус класу B для j. Однакові стратегії та значення ≠ 0: у раунді обидва отримують протилежний знак.'
            : 'Без класів у таблиці та в раундах використовується лише базова матриця. Після вибору обох класів — ефективні значення з бонусами.'}
      </p>
    </section>
  )
}
