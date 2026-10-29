export function formatDate(s) {
  if (!s) return ''
  return s.split('-').reverse().join('.')
}

export function yearsText(n) {
  const last = n % 10
  const last2 = n % 100
  if (last2 >= 11 && last2 <= 14) return n + ' лет'
  if (last === 1) return n + ' год'
  if (last >= 2 && last <= 4) return n + ' года'
  return n + ' лет'
}