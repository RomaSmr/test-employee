import { Routes, Route } from 'react-router-dom'
import EmployeesView from '../views/EmployeesView'
import EmployeeCardView from '../views/EmployeeCardView'
import DepartmentsView from '../views/DepartmentsView'
import ReportView from '../views/ReportView'

function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<EmployeesView />} />
      <Route path="/employees/:id" element={<EmployeeCardView />} />
      <Route path="/departments" element={<DepartmentsView />} />
      <Route path="/report" element={<ReportView />} />
    </Routes>
  )
}

export default AppRoutes