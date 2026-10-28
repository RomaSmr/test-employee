import { createTheme } from '@mui/material/styles'

const theme = createTheme({
  palette: {
    primary: {
      main: '#4f7cbf',
      light: '#7ea0d6',
      dark: '#3a5f96',
      contrastText: '#fff'
    },
    background: {
      default: '#d7e5f5',
      paper: '#ffffff'
    }
  },

  shape: {
    borderRadius: 6
  },

  typography: {
    fontFamily: "'Roboto', 'Helvetica', 'Arial', sans-serif"
  },

  components: {
    MuiOutlinedInput: {
      styleOverrides: {
        root: {
          borderRadius: 12
        }
      }
    },

    MuiButton: {
      styleOverrides: {
        root: {
          borderRadius: 12,
          textTransform: 'none'
        }
      }
    },

    MuiChip: {
      styleOverrides: {
        root: {
          borderRadius: 15
        }
      }
    },

    MuiPaper: {
      styleOverrides: {
        rounded: {
          borderRadius: 15
        }
      }
    }
  }
})

export default theme