using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using SalesWebMVC.Models;
using SalesWebMVC.Models.ViewModels;
using SalesWebMVC.Services;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace SalesWebMVC.Controllers
{
    public class SellersController : Controller
    {

        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            var list = _sellerService.FindAll();
            return View(list);
        }

        public IActionResult Create()
        {
            
            var departments = _departmentService.FindAll();
            var viewModel = new SellerFormViewModel { Departments = departments };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seller seller)
        {
            bool isNameValid = ValidateProperty(seller, nameof(Seller.Name));
            bool isEmailValid = ValidateProperty(seller, nameof(Seller.Email));
            bool isBirthDateValid = ValidateProperty(seller, nameof(Seller.BirthDate));
            bool isBaseSalaryValid = ValidateProperty(seller, nameof(Seller.BaseSalary));

            bool isValid = isNameValid && isEmailValid && isBirthDateValid && isBaseSalaryValid;

            if (!isValid)
            {
                var departments = _departmentService.FindAll();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }

            _sellerService.Insert(seller);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not not provided"});
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }

            return View(obj);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _sellerService.Remove(id);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {

            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not provided" });    
            }

            var obj = _sellerService.FindById(id.Value);

            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id not found" });
            }

            List<Department> departments = _departmentService.FindAll();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {
            bool isNameValid = ValidateProperty(seller, nameof(Seller.Name));
            bool isEmailValid = ValidateProperty(seller, nameof(Seller.Email));
            bool isBirthDateValid = ValidateProperty(seller, nameof(Seller.BirthDate));
            bool isBaseSalaryValid = ValidateProperty(seller, nameof(Seller.BaseSalary));

            bool isValid = isNameValid && isEmailValid && isBirthDateValid && isBaseSalaryValid;

            if (!isValid)
            {
                var departments = _departmentService.FindAll();
                var viewModel = new SellerFormViewModel { Seller = seller, Departments = departments };
                return View(viewModel);
            }

            if (seller.Id != id)
            {
                return RedirectToAction(nameof(Error), new { Message = "Id mismatch" });
            }
            try
            {
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException ex)
            {
                return RedirectToAction(nameof(Error), new { Message = ex.Message});
            }
        }

        public IActionResult Error(string message) { 

            var viewModel = new ErrorViewModel {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);

        }
        private bool ValidateProperty(object model, string propertyName)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null) { MemberName = propertyName };
            var results = new List<ValidationResult>();
            return Validator.TryValidateProperty(model.GetType().GetProperty(propertyName).GetValue(model), context, results);
        }

    }
}
