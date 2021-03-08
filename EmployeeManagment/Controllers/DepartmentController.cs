using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagment.Controllers
{
    public class DepartmentController : Controller
    {
        public string ListDepartments()
        {
            return "This is from List Departments";
        }
        public string DetailsList()
        {
            return "This is from Details List";
        }
    }
}
