import { useEffect, useState } from 'react'
import { API } from '../api'
import { formatDate } from '../utils/format'
import {
  Typography, TextField, Button, MenuItem, Alert, Stack,
  Table, TableHead, TableBody, TableRow, TableCell, Paper
} from '@mui/material'

const emptyForm = {
  id: 0,
  code: '',
  fullName: '',
  abbreviation: '',
  createdDate: '',
  closedDate: '',
  parentId: ''
}

function Departments() {
  const [list, setList] = useState([])
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState('')

  async function load() {
    const res = await fetch(API + '/api/departments')
    const data = await res.json()
    setList(data)
  }

  useEffect(() => {
    load()
  }, [])

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value })
  }

  async function save(e) {
    e.preventDefault()
    setError('')

    const body = {
      code: form.code,
      fullName: form.fullName,
      abbreviation: form.abbreviation,
      createdDate: form.createdDate,
      closedDate: form.closedDate || null,
      parentId: form.parentId ? Number(form.parentId) : null
    }

    const isEdit = form.id !== 0
    const url = isEdit
      ? API + '/api/departments/' + form.id
      : API + '/api/departments'

    const res = await fetch(url, {
      method: isEdit ? 'PUT' : 'POST',
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

  function startEdit(d) {
    setForm({
      id: d.id,
      code: d.code,
      fullName: d.fullName || '',
      abbreviation: d.abbreviation || '',
      createdDate: d.createdDate,
      closedDate: d.closedDate || '',
      parentId: d.parentId || ''
    })
  }

  async function closeDep(id) {
    if (!window.confirm('Ликвидировать подразделение?')) return
    await fetch(API + '/api/departments/' + id + '/close', { method: 'POST' })
    load()
  }

  return (
    <div>
      <Typography variant="h5" gutterBottom>Подразделения</Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <form onSubmit={save}>
          <Stack spacing={2} sx={{ maxWidth: 400 }}>
            <TextField
              label="Код (4 символа)"
              name="code"
              value={form.code}
              onChange={handleChange}
              slotProps={{ htmlInput: { maxLength: 4 } }}
              required
            />
            <TextField
              label="Полное наименование"
              name="fullName"
              value={form.fullName}
              onChange={handleChange}
              required
            />
            <TextField
              label="Аббревиатура"
              name="abbreviation"
              value={form.abbreviation}
              onChange={handleChange}
              required
            />
            <TextField
              label="Дата создания"
              type="date"
              name="createdDate"
              value={form.createdDate}
              onChange={handleChange}
              slotProps={{ inputLabel: { shrink: true } }}
              required
            />

            {form.id !== 0 && (
              <TextField
                label="Дата ликвидации"
                type="date"
                name="closedDate"
                value={form.closedDate}
                onChange={handleChange}
                slotProps={{ inputLabel: { shrink: true } }}
              />
            )}

            <TextField
              select
              label="Вышестоящее подразделение"
              name="parentId"
              value={form.parentId}
              onChange={handleChange}
            >
              <MenuItem value="">— нет —</MenuItem>
              {list
                .filter(d => !d.closedDate && d.id !== form.id)
                .map(d => (
                  <MenuItem key={d.id} value={d.id}>
                    {d.code} {d.fullName}
                  </MenuItem>
                ))}
            </TextField>

            {error && <Alert severity="error">{error}</Alert>}

            <Stack direction="row" spacing={1}>
              <Button type="submit" variant="contained">
                {form.id === 0 ? 'Добавить' : 'Сохранить'}
              </Button>
              {form.id !== 0 && (
                <Button onClick={() => setForm(emptyForm)}>Отмена</Button>
              )}
            </Stack>
          </Stack>
        </form>
      </Paper>

      <Paper>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Код</TableCell>
              <TableCell>Наименование</TableCell>
              <TableCell>Аббревиатура</TableCell>
              <TableCell>Вышестоящее</TableCell>
              <TableCell>Создано</TableCell>
              <TableCell>Ликвидировано</TableCell>
              <TableCell></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {list.map(d => (
              <TableRow key={d.id}>
                <TableCell>{d.code}</TableCell>
                <TableCell>{d.fullName}</TableCell>
                <TableCell>{d.abbreviation}</TableCell>
                <TableCell>{d.parentName}</TableCell>
                <TableCell>{formatDate(d.createdDate)}</TableCell>
                <TableCell>{formatDate(d.closedDate)}</TableCell>
                <TableCell>
                  <Button size="small" onClick={() => startEdit(d)}>Изменить</Button>
                  {!d.closedDate && (
                    <Button size="small" color="error" onClick={() => closeDep(d.id)}>
                      Ликвидировать
                    </Button>
                  )}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </div>
  )
}

export default Departments