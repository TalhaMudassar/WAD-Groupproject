//Homecontroller code:
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly Ecommercecontext _context = new Ecommercecontext();

        // Display the list of products
        public ActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        // Add a product to the cart
        public ActionResult AddToCart(int id)
        {
            // Find the product by ID
            var product = _context.Products.Find(id);
            if (product != null)
            {
                // Check if the product is in stock
                if (product.Stock <= 0)
                {
                    TempData["ErrorMessage"] = "This product is not available in our stock.";
                    return RedirectToAction("Index");
                }

                // Retrieve the cart from the session or create a new one
                List<OrderItems> cart;
                if (Session["Cart"] == null)
                {
                    cart = new List<OrderItems>();
                }
                else
                {
                    cart = (List<OrderItems>)Session["Cart"];
                }

                // Check if the product already exists in the cart
                var cartItem = cart.FirstOrDefault(c => c.ProductId == id);
                if (cartItem != null)
                {
                    // If the product exists, increase the quantity
                    cartItem.Quantity++;

                    // Reduce the stock quantity
                    product.Stock--;
                }
                else
                {
                    // If the product doesn't exist, add it to the cart
                    cart.Add(new OrderItems
                    {
                        ProductId = product.Id,
                        Product = product, // Include the Product object
                        Quantity = 1,
                        Price = product.Price
                    });

                    // Reduce the stock quantity
                    product.Stock--;
                }

                // Save the updated stock quantity to the database
                _context.SaveChanges();

                // Save the updated cart back to the session
                Session["Cart"] = cart;

                // Set a success message
                TempData["SuccessMessage"] = "Product added to cart successfully!";
            }

            // Redirect back to the Index page
            return RedirectToAction("Index");
        }

        // Display the cart
        public ActionResult Cart()
        {
            // Retrieve the cart from the session or create an empty list
            List<OrderItems> cart;
            if (Session["Cart"] == null)
            {
                cart = new List<OrderItems>();
            }
            else
            {
                cart = (List<OrderItems>)Session["Cart"];
            }

            // Pass the cart to the view
            return View(cart);
        }

        // Process the checkout
        public ActionResult Checkout()
        {
            // Retrieve the cart from the session
            List<OrderItems> cart;
            if (Session["Cart"] == null)
            {
                // If the cart is empty, redirect to the home page
                return RedirectToAction("Index");
            }
            else
            {
                cart = (List<OrderItems>)Session["Cart"];
            }

            // Ensure the user is logged in
            if (Session["UserId"] == null)
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Account");
            }

            // Create a new order
            var order = new Orders
            {
                UserId = (int)Session["UserId"], // Ensure UserId is set in the session
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Quantity * c.Price),
                OrderItems = cart.Select(c => new OrderItems
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Price
                }).ToList()
            };

            // Add the order to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear the cart from the session
            Session.Remove("Cart");

            // Redirect to the order confirmation page
            return RedirectToAction("OrderConfirmation");
        }

        // Display the order confirmation page
        public ActionResult OrderConfirmation()
        {
            return View();
        }



        //Remove Cart section 
        public ActionResult RemoveFromCart(int id)
        {
            // Retrieve the cart from the session
            var cart = Session["Cart"] as List<OrderItems>;
            if (cart != null)
            {
                // Find the item to remove
                var itemToRemove = cart.FirstOrDefault(c => c.ProductId == id);
                if (itemToRemove != null)
                {
                    // Increase the stock quantity of the product
                    var product = _context.Products.Find(itemToRemove.ProductId);
                    if (product != null)
                    {
                        product.Stock += itemToRemove.Quantity; // Add the quantity back to stock
                        _context.SaveChanges();
                    }

                    // Remove the item from the cart
                    cart.Remove(itemToRemove);

                    // Save the updated cart back to the session
                    Session["Cart"] = cart;

                    // Set a success message
                    TempData["SuccessMessage"] = "Product removed from cart successfully!";
                }
            }

            // Redirect back to the Cart page
            return RedirectToAction("Cart");
        }
    }
}