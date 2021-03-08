using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class CreateViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public Dept? Department { get; set; }
        public IFormFile Photo { get; set; }
        //public List<IFormFile> Photos { get; set; }
    }
}
