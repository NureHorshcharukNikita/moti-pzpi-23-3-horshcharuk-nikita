import { PLAYER_CLASSES } from '../data/gameData'
import { classPerksLine } from '../utils/gameUtils'

export function ClassesHeroPanel() {
  return (
    <section className="panel">
      <h2>Класи героїв</h2>
      <p className="muted small class-hero-intro">
        Кожен клас підсилює лише певні архетипи стратегій у платіжній матриці: до
        клітинки (i, j) додається бонус класу A для рядка i та віднімається бонус
        класу B для стовпця j. Обрати клас можна над матрицею — на кнопці лише
        назва; повний текст — у підказці при наведенні.
      </p>
      <ul className="strategy-grid">
        {PLAYER_CLASSES.map((c) => (
          <li key={c.id} className="strategy-card strategy-card--hero">
            <span className="strategy-meta">
              {c.short} · {c.name}
            </span>
            <strong className="class-card-head">{classPerksLine(c.perStrategy)}</strong>
            <p>{c.desc}</p>
          </li>
        ))}
      </ul>
    </section>
  )
}
