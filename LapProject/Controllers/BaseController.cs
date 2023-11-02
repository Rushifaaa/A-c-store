﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LapProject.Controllers
{
    public class BaseController : Controller
    {
        protected string CustomerEmail => User.Identity.Name;

        [NonAction]
        protected ActionResult SafeRedirect(string url)
        {
            if (Url.IsLocalUrl(url)) return Redirect(url);

            return RedirectToAction("Index", "Home");
        }
    }
}