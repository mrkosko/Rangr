﻿using System.Web.Mvc;

namespace App.WebAPI.Controllers.Infrastructure
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("~/help");
        }
    }
}
