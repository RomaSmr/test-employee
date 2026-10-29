import { API } from '../api'

function Avatar({ photoPath, gender }) {
  if (photoPath) {
    return (
      <img
        src={API + photoPath}
        alt="фото"
        width="150"
        height="180"
        style={{ objectFit: 'cover' }}
      />
    )
  }

  const color = gender === 2 ? '#d98aa8' : '#7a9cc6'

  return (
    <svg width="150" height="180" viewBox="0 0 150 180">
      <rect width="150" height="180" fill="#eee" />
      <circle cx="75" cy="65" r="30" fill={color} />
      <path d="M25 180 C25 120 125 120 125 180 Z" fill={color} />
    </svg>
  )
}

export default Avatar