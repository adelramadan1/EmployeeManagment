using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmployeeManagment.Models;
using EmployeeManagment.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;

namespace EmployeeManagment.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        private readonly IWebHostEnvironment _host;

        public HomeController(IEmployeeRepository employeeRepository, IWebHostEnvironment host)
        {
            _employeeRepository = employeeRepository;
            _host = host;
        }
        /*[Route("")]  
        [Route("~/Home")]
        [Route("~/Home/Index")]
        */
        [AllowAnonymous]
        public IActionResult Index()
        {
            var Employees = _employeeRepository.GetEmployees();
            return View(Employees);
        }
        //[Route("~/Home/Details/{id?}")]
        [AllowAnonymous]
        public ViewResult Details(int? Id)
        {
           // throw new Exception("Error Exception in Employee details action");
            Employee employee = _employeeRepository.getEmployee(Id.Value);
            if(employee==null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound",Id.Value);
            }
            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel
            {
                Employee = _employeeRepository.getEmployee(Id ?? 1),
                PageTitle = "Show Employee in Delatls"
            };
            return View(homeDetailsViewModel);
        }
        [HttpGet]
       
        public ViewResult Create()
        {
            return View();
        }
        [HttpPost]
        
        public IActionResult Create(EmployeeCreateViewModel employee)
        {

            if (ModelState.IsValid)
            {
                string uniqueFileName =ProcessUploadFile(employee);
                Employee newemployee = new Employee
                {
                    Name = employee.Name,
                    Email = employee.Email,
                    Department = employee.Department,
                    PhotoPath = uniqueFileName

                };
                _employeeRepository.Add(newemployee);
                return RedirectToAction("Details", new { Id = newemployee.Id });
            }
            return View();
        }
        [HttpGet]
       
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.getEmployee(id);
            EmployeeEditViewModel employeeEditViewModel = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };
            return View(employeeEditViewModel);
        }
        [HttpPost]
       
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {

                Employee employee = _employeeRepository.getEmployee(model.Id);
                string uniqueFileName = ProcessUploadFile(model);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
              
                if(model.Photo!=null)
                {
                   
                    if(model.ExistingPhotoPath!=null)
                    {
                        string PathExistingPhoto = Path.Combine(_host.WebRootPath,"images",model.ExistingPhotoPath);
                        System.IO.File.Delete(PathExistingPhoto);
                    }
                    employee.PhotoPath = uniqueFileName;
                }
               
                _employeeRepository.Update(employee);
                return RedirectToAction("index");


            }

            return View();
        }

        private string ProcessUploadFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(_host.WebRootPath, "images");
                //string  = Path.Combine(RootPath, "");
                uniqueFileName = Guid.NewGuid() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (FileStream fileStream=new FileStream(filePath,FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);

                };
            }

            return uniqueFileName;
        }
    }
        
}