import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { API } from '../api'
import Avatar from '../components/Avatar'
import { formatDate, yearsText } from '../utils/format'
import {
  Typography, TextField, Button, MenuItem, Alert, Stack, Box, Chip, Paper
} from '@mui/material'

function EmployeeCard() {
  const { id } = useParams()
  const [emp, setEmp] = useState(null)
  const [deps, setDeps] = useState([])
  const [error, setError] = useState('')

  const [editing, setEditing] = useState(false)
  const [form, setForm] = useState({})

  const [transfer, setTransfer] = useState({ departmentId: '', position: '', date: '' })
  const [fireDate, setFireDate] = useState('')

  async function load() {
    const res = await fetch(API + '/api/employees/' + id)
    if (!res.ok) {
      setError('Сотрудника нет')
      return
    }
    setEmp(await res.json())
  }

  async function loadDeps() {
    const res = await fetch(API + '/api/departments')
    setDeps(await res.json())
  }

  useEffect(() => {
    load()
    loadDeps()
  }, [id])

  function startEdit() {
    setForm({
      tabNumber: emp.tabNumber,
      lastName: emp.lastName,
      firstName: emp.firstName,
      middleName: emp.middleName || '',
      gender: String(emp.gender),
      birthDate: emp.birthDate,
      phone: emp.phone || '',
      email: emp.email || ''
    })
    setEditing(true)
  }

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  async function saveEdit(e) {
    e.preventDefault()
    setError('')

    const res = await fetch(API + '/api/employees/' + id, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...form, gender: Number(form.gender) })
    })

    if (!res.ok) {
      setError(await res.text())
      return
    }

    setEditing(false)
    load()
  }

  async function doTransfer(e) {
    e.preventDefault()
    setError('')

    const res = await fetch(API + '/api/employees/' + id + '/transfer', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        departmentId: Number(transfer.departmentId),
        position: transfer.position,
        date: transfer.date
      })
    })

    if (!res.ok) {
      setError(await res.text())
      return
    }

    setTransfer({ departmentId: '', position: '', date: '' })
    load()
  }

  async function doFire(e) {
    e.preventDefault()
    setError('')

    if (!window.confirm('Уволить сотрудника?')) return

    const res = await fetch(API + '/api/employees/' + id + '/fire', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ date: fireDate })
    })

    if (!res.ok) {
      setError(await res.text())
      return
    }

    setFireDate('')
    load()
  }

  async function uploadPhoto(e) {
    const file = e.target.files[0]
    if (!file) return
    setError('')

    const data = new FormData()
    data.append('file', file)

    const res = await fetch(API + '/api/employees/' + id + '/photo', {
      method: 'POST',
      body: data
    })

    if (!res.ok) {
      setError(await res.text())
      return
    }

    load()
  }

  if (!emp) {
    return <Typography>{error || 'Загрузка...'}</Typography>
  }

  const fired = emp.fireDate !== null

  return (
    <div>
      <Typography variant="h5" gutterBottom>
        {emp.lastName} {emp.firstName} {emp.middleName}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

      <Paper sx={{ p: 2, mb: 3 }}>
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3} alignItems="flex-start">
          <Box sx={{ textAlign: 'center' }}>
            <Avatar photoPath={emp.photoPath} gender={emp.gender} />
            <Button
              component="label"
              variant="outlined"
              size="small"
              sx={{ mt: 1 }}
            >
              Загрузить фото
              <input type="file" accept=".jpg,.jpeg,.png" hidden onChange={uploadPhoto} />
            </Button>
          </Box>

          {!editing && (
            <Box sx={{ flexGrow: 1 }}>
              <Stack spacing={0.75}>
                <Typography>Табельный номер: <b>{emp.tabNumber}</b></Typography>
                <Typography>Пол: {emp.gender === 1 ? 'Мужской' : 'Женский'}</Typography>
                <Typography>
                  Дата рождения: {formatDate(emp.birthDate)} (возраст: {yearsText(emp.age)})
                </Typography>
                <Typography>Телефон: {emp.phone}</Typography>
                <Typography>Email: {emp.email}</Typography>
                <Typography>Подразделение: {emp.departmentName}</Typography>
                <Typography>Должность: {emp.position}</Typography>
                <Typography>Дата приёма/перевода: {formatDate(emp.hireDate)}</Typography>
                <Typography>Стаж в компании: {yearsText(emp.experience)}</Typography>
                {fired && (
                  <Typography>Дата увольнения: {formatDate(emp.fireDate)}</Typography>
                )}
                <Box sx={{ mt: 0.5 }}>
                  {fired
                    ? <Chip label="Уволен" size="small" />
                    : <Chip label="Работает" color="success" size="small" />}
                </Box>
              </Stack>

              <Button variant="contained" onClick={startEdit} sx={{ mt: 2 }}>
                Редактировать данные
              </Button>
            </Box>
          )}

          {editing && (
            <Box component="form" onSubmit={saveEdit} sx={{ flexGrow: 1 }}>
              <Stack spacing={2} sx={{ maxWidth: 400 }}>
                <TextField label="Табельный номер" name="tabNumber" value={form.tabNumber} onChange={handleChange} required />
                <TextField label="Фамилия" name="lastName" value={form.lastName} onChange={handleChange} required />
                <TextField label="Имя" name="firstName" value={form.firstName} onChange={handleChange} required />
                <TextField label="Отчество" name="middleName" value={form.middleName} onChange={handleChange} required />

                <TextField select label="Пол" name="gender" value={form.gender} onChange={handleChange}>
                  <MenuItem value="1">Мужской</MenuItem>
                  <MenuItem value="2">Женский</MenuItem>
                </TextField>

                <TextField
                  label="Дата рождения"
                  type="date"
                  name="birthDate"
                  value={form.birthDate}
                  onChange={handleChange}
                  slotProps={{ inputLabel: { shrink: true } }}
                  required
                />
                <TextField label="Телефон" name="phone" value={form.phone} onChange={handleChange} />
                <TextField label="Email" type="email" name="email" value={form.email} onChange={handleChange} />

                <Stack direction="row" spacing={1}>
                  <Button type="submit" variant="contained">Сохранить</Button>
                  <Button onClick={() => setEditing(false)}>Отмена</Button>
                </Stack>
              </Stack>
            </Box>
          )}
        </Stack>
      </Paper>

      {!fired && (
        <>
          <Paper sx={{ p: 2, mb: 3 }}>
            <Typography variant="h6" gutterBottom>Перевод</Typography>
            <form onSubmit={doTransfer}>
              <Stack spacing={2} sx={{ maxWidth: 400 }}>
                <TextField
                  select
                  label="Новое подразделение"
                  value={transfer.departmentId}
                  onChange={e => setTransfer({ ...transfer, departmentId: e.target.value })}
                  required
                >
                  {deps
                    .filter(d => !d.closedDate)
                    .map(d => (
                      <MenuItem key={d.id} value={d.id}>
                        {d.code} {d.fullName}
                      </MenuItem>
                    ))}
                </TextField>

                <TextField
                  label="Новая должность"
                  value={transfer.position}
                  onChange={e => setTransfer({ ...transfer, position: e.target.value })}
                  required
                />

                <TextField
                  label="Дата перевода"
                  type="date"
                  value={transfer.date}
                  onChange={e => setTransfer({ ...transfer, date: e.target.value })}
                  slotProps={{ inputLabel: { shrink: true } }}
                  required
                />

                <Box>
                  <Button type="submit" variant="contained">Перевести</Button>
                </Box>
              </Stack>
            </form>
          </Paper>

          <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>Увольнение</Typography>
            <form onSubmit={doFire}>
              <Stack spacing={2} sx={{ maxWidth: 400 }}>
                <TextField
                  label="Дата увольнения"
                  type="date"
                  value={fireDate}
                  onChange={e => setFireDate(e.target.value)}
                  slotProps={{ inputLabel: { shrink: true } }}
                  required
                />
                <Box>
                  <Button type="submit" variant="contained" color="error">
                    Уволить
                  </Button>
                </Box>
              </Stack>
            </form>
          </Paper>
        </>
      )}
    </div>
  )
}

export default EmployeeCard