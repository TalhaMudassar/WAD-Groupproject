using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    // Admin Controller
    public class AdminController : Controller
    {
        private readonly Ecommercecontext _context = new Ecommercecontext();

        // Admin Dashboard
        public ActionResult Index()
        {
            // Ccheck the user is admin 
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var products = _context.Products.ToList();
            return View(products);
        }

        // Add Product 
        [HttpGet]
        public ActionResult AddProduct()
        {
            // Check if the user is an admin
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new Products());
        }

        // Add Product with image
        [HttpPost]
        public ActionResult AddProduct(Products product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Generate a unique file name
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    // Define the path where the image will be saved
                    string imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);

                    // Save the image to the server
                    ImageFile.SaveAs(imagePath);

                    // Store the relative path in the database
                    product.ImagePath = "~/Images/" + fileName;
                }

                _context.Products.Add(product);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // Edit Product 
        [HttpGet]
        public ActionResult EditProduct(int id)
        {
            // Check if the user is an admin
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _context.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        // Edit Product 
        [HttpPost]
        public ActionResult EditProduct(Products product, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = _context.Products.Find(product.Id);
                if (existingProduct != null)
                {
                    // Update the products
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.Price = product.Price;
                    existingProduct.Stock = product.Stock;

                    //image upload
                    if (ImageFile != null && ImageFile.ContentLength > 0)
                    {
                        // generate a unique file name for image
                        string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                        string extension = Path.GetExtension(ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                        // define the path where the image will be saved
                        string imagePath = Path.Combine(Server.MapPath("~/Images/"), fileName);

                       
                        ImageFile.SaveAs(imagePath);

                        
                        existingProduct.ImagePath = "~/Images/" + fileName;
                    }

                   
                    _context.SaveChanges();

                   
                    return RedirectToAction("Index");
                }
                else
                {
                    // If the product doesn't exist, return a 404 error
                    return HttpNotFound();
                }
            }

            // If the model state is invalid, return to the edit view with the current model
            return View(product);
        }

        // Delete Product
        public ActionResult DeleteProduct(int id)
        {
            // Check if the user is an admin
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            var product = _context.Products.Find(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}