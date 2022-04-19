using Assignmentfive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Assignmentfive.Controllers
{
    public class AccountsController : Controller
    {
        Assignment5Entities db = new Assignment5Entities();
        // GET: Accounts
        public JsonResult IsUserNameAvailable(string username)
        {



            return Json(!db.Users.Any(u => u.UserName == username), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserModel model)
        {
            using (Assignment5Entities entities = new Assignment5Entities())
            {
                bool isValidUser = entities.Users.Any(u => u.UserName.ToLower() == model.UserName.ToLower() && u.UserPassword == model.UserPassword);
                if (isValidUser)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Books");
                }
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }



        }

        public ActionResult SignUp()
        {

            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User model)
        {
            using (Assignment5Entities entities = new Assignment5Entities())
            {
                entities.Users.Add(model);
                entities.SaveChanges();
            }
            return RedirectToAction("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}