using EmployeeManagment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.ViewModels
{
    public class EmployeeEditViewModel :CreateViewModel
    {
        public string Id { get; set; }
        public string ExistingPhotoPath { get; set; }
    }
}
