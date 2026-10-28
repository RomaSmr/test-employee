import { useState } from 'react'
import { API } from './api'
import {
  Typography, TextField, Button, MenuItem, Alert, Stack, Box,
  Table, TableHead, TableBody, TableRow, TableCell, Paper
} from '@mui/material'

function formatDate(s) {
  if (!s) return ''
  return s.split('-').reverse().join('.')
}

const typeNames = {
  1: 'Приём',
  2: 'Перевод',
  3: 'Увольнение'
}

function Report() {
  const [from, setFrom] = useState('')
  const [to, setTo] = useState('')
  const [type, setType] = useState('')
  const [list, setList] = useState(null)
  const [error, setError] = useState('')

  async function build(e) {
    e.preventDefault()
    setError('')

    let url = API + '/api/reports?from=' + from + '&to=' + to
    if (type) {
      url += '&type=' + type
    }

    const res = await fetch(url)
    if (!res.ok) {
      setError(await res.text())
      return
    }

    setList(await res.json())
  }

  return (
    <div>
      <Typography variant="h5" gutterBottom>Отчёт по кадровым событиям</Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <form onSubmit={build}>
          <Stack spacing={2} sx={{ maxWidth: 400 }}>
            <TextField
              label="Период с"
              type="date"
              value={from}
              onChange={e => setFrom(e.target.value)}
              slotProps={{ inputLabel: { shrink: true } }}
              required
            />
            <TextField
              label="по"
              type="date"
              value={to}
              onChange={e => setTo(e.target.value)}
              slotProps={{ inputLabel: { shrink: true } }}
              required
            />
            <TextField
              select
              label="Тип события"
              value={type}
              onChange={e => setType(e.target.value)}
            >
              <MenuItem value="">Все</MenuItem>
              <MenuItem value="1">Приём</MenuItem>
              <MenuItem value="2">Перевод</MenuItem>
              <MenuItem value="3">Увольнение</MenuItem>
            </TextField>

            {error && <Alert severity="error">{error}</Alert>}

            <Box>
              <Button type="submit" variant="contained">Сформировать</Button>
            </Box>
          </Stack>
        </form>
      </Paper>

      {list && (
        <Paper>
          <Box sx={{ p: 2 }}>
            <Typography variant="subtitle1">
              Найдено записей: {list.length}
            </Typography>
          </Box>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Дата</TableCell>
                <TableCell>Событие</TableCell>
                <TableCell>Таб. №</TableCell>
                <TableCell>ФИО</TableCell>
                <TableCell>Подразделение</TableCell>
                <TableCell>Должность</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {list.map(r => (
                <TableRow key={r.id} hover>
                  <TableCell>{formatDate(r.eventDate)}</TableCell>
                  <TableCell>{typeNames[r.type]}</TableCell>
                  <TableCell>{r.tabNumber}</TableCell>
                  <TableCell>{r.lastName} {r.firstName} {r.middleName}</TableCell>
                  <TableCell>{r.departmentName}</TableCell>
                  <TableCell>{r.position}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </Paper>
      )}
    </div>
  )
}

export default Report