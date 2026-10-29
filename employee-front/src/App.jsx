import { Container, CssBaseline } from '@mui/material'
import Navbar from './components/Navbar'
import AppRoutes from './routes/AppRoutes'

function App() {
  return (
    <div>
      <CssBaseline />
      <Navbar />
      <Container sx={{ mt: 3, mb: 5 }}>
        <AppRoutes />
      </Container>
    </div>
  )
}

export default App