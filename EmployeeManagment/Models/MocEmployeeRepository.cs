using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class MocEmployeeRepository 
    {
        private List<Employee> _employeeList;
        public MocEmployeeRepository()
        {
            this._employeeList = new List<Employee>()
            {
                new Employee(){ Id=1,Name="Mary",Department=Dept.HR,Email="mary@gmail.com"},
                new Employee(){ Id=2,Name="Jon",Department=Dept.IT,Email="Jon@gmail.com"},
                new Employee(){ Id=3, Name="Ram",Department=Dept.None , Email="ram@gmail.com"},
                new Employee(){ Id=4,Name="Suman",Department=Dept.Payroll,Email="suman@gmail.com"}
            };
        }

        public Employee Add(Employee employee)
        {
            employee.Id = _employeeList.Max(x => x.Id) + 1;
            _employeeList.Add(employee);
            return employee;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return this._employeeList;
        }

        public Employee GetEmployee(int Id)
        {
            return _employeeList.FirstOrDefault(x => x.Id == Id);
        }
    }
}
