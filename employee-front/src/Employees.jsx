import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { API } from './api'
import {
  Typography, TextField, Button, MenuItem, Alert, Stack, Chip,
  Table, TableHead, TableBody, TableRow, TableCell, Paper
} from '@mui/material'

const emptyForm = {
  tabNumber: '',
  lastName: '',
  firstName: '',
  middleName: '',
  gender: '1',
  birthDate: '',
  phone: '',
  email: '',
  departmentId: '',
  position: '',
  hireDate: ''
}

function Employees() {
  const [list, setList] = useState([])
  const [deps, setDeps] = useState([])
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState('')

  async function load() {
    const res = await fetch(API + '/api/employees')
    setList(await res.json())
  }

  async function loadDeps() {
    const res = await fetch(API + '/api/departments')
    setDeps(await res.json())
  }

  useEffect(() => {
    load()
    loadDeps()
  }, [])

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  async function hire(e) {
    e.preventDefault()
    setError('')

    const body = {
      tabNumber: form.tabNumber,
      lastName: form.lastName,
      firstName: form.firstName,
      middleName: form.middleName,
      gender: Number(form.gender),
      birthDate: form.birthDate,
      phone: form.phone,
      email: form.email,
      departmentId: Number(form.departmentId),
      position: form.position,
      hireDate: form.hireDate
    }

    const res = await fetch(API + '/api/employees', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body)
    })

    if (!res.ok) {
      setError(await res.text())
      return
    }

    setForm(emptyForm)
    load()
  }

  return (
    <div>
      <Typography variant="h5" gutterBottom>Сотрудники</Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <Typography variant="h6" gutterBottom>Принять на работу</Typography>
        <form onSubmit={hire}>
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

            <TextField
              select
              label="Подразделение"
              name="departmentId"
              value={form.departmentId}
              onChange={handleChange}
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

            <TextField label="Должность" name="position" value={form.position} onChange={handleChange} required />
            <TextField
              label="Дата приёма"
              type="date"
              name="hireDate"
              value={form.hireDate}
              onChange={handleChange}
              slotProps={{ inputLabel: { shrink: true } }}
              required
            />

            {error && <Alert severity="error">{error}</Alert>}

            <Button type="submit" variant="contained">Принять</Button>
          </Stack>
        </form>
      </Paper>

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Таб. №</TableCell>
              <TableCell>ФИО</TableCell>
              <TableCell>Подразделение</TableCell>
              <TableCell>Должность</TableCell>
              <TableCell>Статус</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {list.map(e => (
              <TableRow key={e.id} hover>
                <TableCell>{e.tabNumber}</TableCell>
                <TableCell>
                  <Link to={'/employees/' + e.id}>
                    {e.lastName} {e.firstName} {e.middleName}
                  </Link>
                </TableCell>
                <TableCell>{e.departmentName}</TableCell>
                <TableCell>{e.position}</TableCell>
                <TableCell>
                  {e.fireDate
                    ? <Chip label="Уволен" color="default" size="small" />
                    : <Chip label="Работает" color="success" size="small" />}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}

export default Employees