using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagment.Models
{
    public class Employee
    {
        //[Key,DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [NotMapped]
        public string EncryptedId { get; set; }
        [Required(AllowEmptyStrings =false,ErrorMessage ="Please enter name")]
        [DisplayName("Name")]
        [MaxLength(100,ErrorMessage ="Max name limit is 100")]
        //[MinLength(3,ErrorMessage = "Min name limit is 3")]
        //[Range(2,100,ErrorMessage ="Name between 2 and 10")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Email")]
        [DisplayName("Email")]
        [MaxLength(100, ErrorMessage = "Max name limit is 100")]
        //[MinLength(3, ErrorMessage = "Min name limit is 3")]
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Please enter valid email id")]
        //[Range(2, 100, ErrorMessage = "Email between 2 and 10")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please select Department")]
        [DisplayName("Department")]
        public Dept? Department { get; set; }
        public string PhotoPath { get; set; }
    }
}
