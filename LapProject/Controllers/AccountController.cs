using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LapProject.Data;
using LapProject.Models.Account;
using LapProject.Services;


namespace LapProject.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }



        [HttpPost]
        public ActionResult Register(RegisterVm vm)
        {
            //1.: Model validieren (Alle Daten vorhanden?)
            //Data ist valide, wenn länger als 1 Zeichen. Passwörter müssen Übereinstimmen. Email darf nicht bereits vergeben sein.
            if (!isDataValid(vm.Title))
            {
                return View(vm);
            }
            if (!isDataValid(vm.FirstName))
            {
                return View(vm);
            }
            if (!isDataValid(vm.LastName))
            {
                return View(vm);
            }
            if (!isDataValid(vm.Street))
            {
                return View(vm);
            }
            if (!isDataValid(vm.ZipCode))
            {
                return View(vm);
            }
            if (!isDataValid(vm.City))
            {
                return View(vm);
            }
            if (!isDataValid(vm.Email))
            {
                return View(vm);
            }
            if (AccountService.emailExists(vm.Email))
            {
                ModelState.AddModelError("CustomError", "Diese Email ist bereits in Verwendung!");
                return View(vm);
            }
            if (!isDataValid(vm.Password))
            {
                return View(vm);
            }
            if (vm.Password != vm.PasswordCheck)
            {
                ModelState.AddModelError("CustomError", "Passworteingaben müssen übereinstimmen!");
                return View(vm);
            }

            //2.: AGBs akzeptiert?
            // AGBs muss zugestimmt werden.
            if (vm.AcceptedTos != "on")
            {
                ModelState.AddModelError("CustomError", "Den AGBs muss zugestimmt werden!");
                return View(vm);
            }

            //3.: Via Service Benutzer in Db speichern
            //Daten von Viewmodel in Entitätsmodel mappen

            var newCustomer = new Customer();
            newCustomer.Title = vm.Title;
            newCustomer.FirstName = vm.FirstName;
            newCustomer.LastName = vm.LastName;
            newCustomer.Street = vm.Street;
            newCustomer.ZipCode = vm.ZipCode;
            newCustomer.City = vm.City;

            newCustomer.Email = vm.Email;

            AccountService.Register(newCustomer, vm.Password);

            return RedirectToAction("Login");
        }
        private bool isDataValid(string data)
        {
            if(data == null || data.Length < 2)
            {
                ModelState.AddModelError("CustomError", "Alle Felder müssen befüllt sein und mindestens 2 Zeichen enthalten!");
                return false;
            }
            return true;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password, string returnUrl)
        {
            if (!isDataValid(email))
            {
                return View();
            }
            if (!isDataValid(password))
            {
                return View();
            }
            //1. Prüfen ob sich Benutzer anmelden darf
            //Falls die Zugangsdaten ungültig sind, wird eine Info diesbezüglich zurückgegeben.
            if (!AccountService.CanLogin(email, password))
            {
                ModelState.AddModelError("CustomError", "Ungültige Zugangsdaten!");
                return View("Login");
            }

            //2. Wenn ja, Cookie mit Login-Ticket geben
            AuthenticateUser(email);

            return SafeRedirect(returnUrl);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [NonAction]
        private void AuthenticateUser(string email)
        {
            var now = DateTime.Now;

            //1. Ticket erzeugen
            var ticket = new FormsAuthenticationTicket(
                0,                  //Versionsnummer des Tickets (für uns egal)
                email,              //(Einzigeratiger) Name
                now,                //Zeitpunkt der Ausstellung
                now.AddMinutes(30), //Zeitpunkt ab dem das Ticket ungültig ist
                true,               //Angemeldet bleiben?
                ""                 //Platz für beliebige Informationen
                );

            //2. Ticket verschlüsseln
            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            //3. Ticket in ein Cookie legen
            var cookie = new HttpCookie(
                FormsAuthentication.FormsCookieName,
                encryptedTicket
                );

            //4. Cookie dem Benutzer geben
            Response.Cookies.Add(cookie);
        }
    }
}