import { Link } from 'react-router-dom'
import { AppBar, Toolbar, Typography, Button } from '@mui/material'

function Navbar() {
  return (
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
  )
}

export default Navbar