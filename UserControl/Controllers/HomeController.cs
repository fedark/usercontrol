﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserControl.Data;
using UserControl.Models;

namespace UserControl.Controllers
{
    public class HomeController : Controller
    {
		public HomeController()
        {
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "User");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}