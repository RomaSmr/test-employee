import { Routes, Route, Link } from 'react-router-dom'
import Departments from './Departments'
import Employees from './Employees'
import EmployeeCard from './EmployeeCard'
import Report from './Report'
import './App.css'
import {CssBaseline, AppBar, Toolbar, Typography, Button, Container, Box} from '@mui/material'

function App() {
  return (
    <div>
      <CssBaseline />

      <AppBar position="static">
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            Учёт сотрудников
          </Typography>
          <Button color="inherit" component={Link} to="/">Сотрудники</Button>
          <Button color="inherit" component={Link} to="/departments">Подразделения</Button>
          <Button color="inherit" component={Link} to="/report">Отчёт</Button>
        </Toolbar>
      </AppBar>

      <Container sx={{ mt: 3, mb: 5, flexGrow: 1}}>
        <Routes>
          <Route path="/" element={<Employees />} />
          <Route path="/employees/:id" element={<EmployeeCard />} />
          <Route path="/departments" element={<Departments />} />
          <Route path="/report" element={<Report />} />
        </Routes>
      </Container>
    </div>
  )
}

export default App