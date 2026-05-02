export function computeMaximin(matrix) {
  if (!matrix?.length || !matrix[0]?.length) {
    return { alpha: null, rowIndices: [], rowMins: [] }
  }
  const rowMins = matrix.map((row) => Math.min(...row))
  const alpha = Math.max(...rowMins)
  const rowIndices = rowMins
    .map((m, i) => (m === alpha ? i : -1))
    .filter((i) => i >= 0)
  return { alpha, rowIndices, rowMins }
}
