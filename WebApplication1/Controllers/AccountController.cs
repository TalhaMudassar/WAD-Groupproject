using System;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private readonly Ecommercecontext _context = new Ecommercecontext();

        // Login - GET  here is my login section
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Login - POST    post login to check email and passward for furthur process
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                Session["UserId"] = user.Id;
                Session["Role"] = user.Role;

                if (user.Role == "Admin")
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // Register - GET   register section 
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // Register - POST  
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // check email already exists in the database

                var existingUser = _context.Users.SingleOrDefault(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    
                    // ager email already exist kerti ha  us ka liea error message 
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(user);
                }

            
                //ager email unique ha continue kre procedding ko with registration 
                user.Role = "Customer";
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }

         
            // model state invalid so retrun the validation error.
            return View(user);
        }

        // Contact - GET
        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }
    }
}

