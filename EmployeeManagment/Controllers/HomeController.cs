

using EmployeeManagment.Models;
using EmployeeManagment.Security;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;

namespace EmployeeManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly ILogger<HomeController> logger;
        // It is through IDataProtector interface Protect and Unprotect methods,
        // we encrypt and decrypt respectively
        private readonly IDataProtector protector;

        // It is the CreateProtector() method of IDataProtectionProvider interface
        // that creates an instance of IDataProtector. CreateProtector() requires
        // a purpose string. So both IDataProtectionProvider and the class that
        // contains our purpose strings are injected using the contructor
        public HomeController(IEmployeeRepository employeeRepository ,IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, IDataProtectionProvider dataProtectionProvider, DataProtectionPurposeStrings dataProtectionPurposeStrings)
        {
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
            // Pass the purpose string as a parameter
            this.protector = dataProtectionProvider.CreateProtector(
                dataProtectionPurposeStrings.EmployeeIdRouteValue);
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            var model = _employeeRepository.GetAllEmployees()
                            .Select(e =>
                            {
                                // Encrypt the ID value and store in EncryptedId property
                                e.EncryptedId = protector.Protect(e.Id.ToString());
                                return e;
                            });
            if (model !=null)
            {
                return View(model);
            }
            return View(null);
        }
        public ViewResult Details(string Id)
        {
            // Decrypt the employee id using Unprotect method
            string decryptedId = protector.Unprotect(Id);
            Employee employee = _employeeRepository.GetEmployee(Convert.ToInt32(decryptedId));
            employee.EncryptedId = Id;
            if (employee ==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", decryptedId);
            }
            return View(employee);
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EmployeeEditViewModel model )
        {
            if (ModelState.IsValid)
            {
                //For single image upload code  make custom function for code reuseablity/
                string uniqueFileName = ProcessUplodedFile(model);

                //For multiple image upload code / 
                //This is single table example real time wold need to create two tables to store the data first for employee 
                //and second for store multiple imges only single employee
                //if (createViewModel.Photos != null && createViewModel.Photos.Count > 0)
                //{
                //    // Loop thru each selected file
                //    foreach (IFormFile photo in createViewModel.Photos)
                //    {
                //        // The file must be uploaded to the images folder in wwwroot
                //        // To get the path of the wwwroot folder we are using the injected
                //        // IHostingEnvironment service provided by ASP.NET Core
                //        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                //        // To make sure the file name is unique we are appending a new
                //        // GUID value and and an underscore to the file name
                //        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //        // Use CopyTo() method provided by IFormFile interface to
                //        // copy the file to wwwroot/images folder
                //        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                //    }
                //}
                Employee employee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    // Store the file name in PhotoPath property of the employee object
                    // which gets saved to the Employees database table
                    PhotoPath = uniqueFileName
                };
                _employeeRepository.Add(employee);
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        private string ProcessUplodedFile(EmployeeEditViewModel model)
        {
            string uniqueFileName = null;
            // If the Photo property on the incoming model object is not null, then the user
            // has selected an image to upload.

            //For single image upload code
            if (model.Photo != null)
            {

                // The image must be uploaded to the images folder in wwwroot
                // To get the path of the wwwroot folder we are using the inject
                // HostingEnvironment service provided by ASP.NET Core
                string uploadFolder = Path.Combine(hostingEnvironment.WebRootPath, "img");
                // To make sure the file name is unique we are appending a new
                // GUID value and and an underscore to the file name
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                // Use CopyTo() method provided by IFormFile interface to
                // copy the file to wwwroot/images folder
                model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            return uniqueFileName;
        }

        [HttpGet]
        public IActionResult Edit(string Id)
        {
            // Decrypt the employee id using Unprotect method
            string decryptedId = protector.Unprotect(Id);
            Employee employee = _employeeRepository.GetEmployee(Convert.ToInt32(decryptedId));
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id= Id,
                Name =employee.Name,
                Email=employee.Email,
                Department=employee.Department,
                ExistingPhotoPath=employee.PhotoPath

            };

            return View(employeeEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViewModel editModel)
        {
            if (ModelState.IsValid)
            {
                // Decrypt the employee id using Unprotect method
                string decryptedId = protector.Unprotect(Convert.ToString(editModel.Id));
                Employee employee = _employeeRepository.GetEmployee(Convert.ToInt32(decryptedId));
                employee.Name = editModel.Name;
                employee.Email = editModel.Email;
                employee.Department = editModel.Department;
                if (editModel.Photo != null)
                {
                    if (editModel.ExistingPhotoPath!=null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath, "img", editModel.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUplodedFile(editModel);
                }
                Employee upldetedEmployee = _employeeRepository.Update(employee);
                return RedirectToAction("index");
            }
            return View(editModel);
        }
        public IActionResult Delete(string Id)
        {
            // Decrypt the employee id using Unprotect method
            string decryptedId = protector.Unprotect(Id);
            var emp = _employeeRepository.Delete(Convert.ToInt32(decryptedId));
            return View("index");
        }
    }
}
