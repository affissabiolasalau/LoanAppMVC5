using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using LoanApp.Models;
using System.Web.Helpers;
using System.Security.Cryptography;

using Microsoft.AspNetCore.Http;
using System.Web;

namespace LoanApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _db;
        
        public AuthController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IEnumerable<Account> Accounts { get; set; }

        public IActionResult Index()
        {
            var userid = this.HttpContext.Session.GetString("UserID");
            if(userid != null)
            {
                return RedirectToAction("Index", "Dashboard"); //return Redirect("Dashboard");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Login lg)
        {
           
            if (ModelState.IsValid)
            {
                var varPhone = _db.Auth.Where(x => x.Phone == lg.Phone).FirstOrDefault();
                var checkPhone = _db.ConfirmEmail.Where(y => y.Phone == lg.Phone).FirstOrDefault();

                try
                {
                    if (checkPhone != null)
                    {
                        if (checkPhone.IsConfirmed == 1)
                        {
                            var check = _db.Auth.Where(x => x.Phone == lg.Phone).FirstOrDefault();
                            var checkPassword = Crypto.VerifyHashedPassword(check.Password, lg.Password);

                            const string SessionId = "UserID";
                            const string SessionEmail = "UserEmail";
                            if (check != null && checkPassword)
                            {
                                HttpContext.Session.SetString(SessionId, check.Id.ToString());
                                HttpContext.Session.SetString(SessionEmail, check.Email.ToString());
                                //return Redirect("Dashboard");
                                return RedirectToAction("Index", "Dashboard");

                                string statusMsg = "Login successful.";
                                ViewBag.message = "<div class='alert alert-success' role='alert'>" + statusMsg + "</ div >";
                            }
                            else
                            {
                                string statusMsg = "You entered a wrong phone number/PIN.";
                                ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                                return View();
                            }
                        }

                        if (checkPhone.IsConfirmed == 0)
                        {

                                var getUrl = _db.ConfirmEmail.Where(x => x.Phone == checkPhone.Phone).FirstOrDefault();
                                string getUrl_ = getUrl.UrlParameter;
                                ViewBag.urlParameter = getUrl.UrlParameter;
                                return View();

                        }
                    }
                    else
                    {
                        string statusMsg = "Invalid Phone Number or PIN";
                        ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                        return View();
                    }
                        
                }
                catch (Exception)
                {

                    if (varPhone == null)
                    {
                        string statusMsg = "Invalid Phone Number or PIN";
                        ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                        return View();
                    }
                    else if (checkPhone == null)
                    {
                        string statusMsg = "Invalid Email or PIN";
                        ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                        return View();
                    }
                    else
                    {
                        string statusMsg = "An unknown error occured. Please make sure you entered correct login details.";
                        ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                        return View();
                    }

                }



            }
            return View();

        }

        public IActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Signup(Auth uc)
        {
            var checkPhone = _db.Auth.Where(x => x.Phone == uc.Phone).FirstOrDefault();
            var checkEmail = _db.Auth.Where(x => x.Phone == uc.Email).FirstOrDefault();

            if (checkPhone != null)
            {
                string statusMsg = "Phone Number already in use";
                ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                return View();
            }
            if (checkEmail != null)
            {
                string statusMsg = "Email already in use";
                ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                return View();
            }
            else
            {
                //int ckNum = Int32.Parse(uc.Phone);
                int z;
                bool ckNumb = int.TryParse(uc.Phone, out z);
                bool ckPass = int.TryParse(uc.Password, out z);
                /*if (ckNumb == false)
                {
                    string statusMsg_ = "Phone number should be numeric"+ ckNumb;
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg_ + "</ div >";
                    return View();
                }*/
                if(uc.Phone.Substring(0, 1) != "0")
                {
                    string statusMsg_ = "Phone number should start with 0";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg_ + "</ div >";
                    return View();
                }
                /*
                else if (ckPass == false)
                {
                    string statusMsg_ = "PIN should be numeric";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg_ + "</ div >";
                    return View();
                }*/
                else if (uc.Password.Length > 4)
                {
                    string statusMsg_ = "PIN should be 4 digits maximum";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg_ + "</ div >";
                    return View();
                }

                else if (uc.Password.Length < 4)
                {
                    string statusMsg_ = "PIN should be at least 4 digits";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg_ + "</ div >";
                    return View();
                }

                else
                {
                    uc.Password = Crypto.HashPassword(uc.Password);
                    uc.RepeatPassword = Crypto.HashPassword(uc.RepeatPassword);
                    _db.Add(uc);

                    var success = await _db.SaveChangesAsync();

                    if (success > 0)
                    {
                        Random random = new Random();
                        int rand = random.Next(10000, 99999);

                        var bytes = new Byte[30];
                        random.NextBytes(bytes);
                        var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
                        var hexStr = String.Concat(hexArray);

                        var getId = _db.Auth.Where(x => x.Phone == uc.Phone).FirstOrDefault();

                        //save to confirm email
                        ConfirmEmail confirmEmail = new ConfirmEmail();
                        confirmEmail.Email = uc.Email;
                        confirmEmail.Phone = uc.Phone;
                        confirmEmail.UserId = getId.Id;
                        confirmEmail.Code = rand;
                        confirmEmail.UrlParameter = hexStr;
                        _db.ConfirmEmail.Add(confirmEmail);
                        //_db.SaveChanges();
                        var y = await _db.SaveChangesAsync();
                        
                        if(y > 0)
                        {
                            
                            var getUrl = _db.ConfirmEmail.Where(x => x.Phone == uc.Phone).FirstOrDefault();
                            ViewBag.urlParameter = getUrl.UrlParameter;
                            //return RedirectToAction("ConfirmSignup/{0}");
                            return View();
                        }
                    }

                    return View();
                    /*                    string statusMsg = "Account Created. A mail has been sent to " + uc.Email + ", please check to confirm your registration.";
                                        ViewBag.message = "<div class='alert alert-success' role='alert'>" + statusMsg + "</ div >";
                                        return View(); */
                }
                
            }
            
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
//            return View();
        }


        public IActionResult Password()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(Auth pass)
        {
            Random random = new Random();
            var bytes = new Byte[30];
            random.NextBytes(bytes);
            var hexArray = Array.ConvertAll(bytes, x => x.ToString("X2"));
            var hexStr = String.Concat(hexArray);

            var checkEmail = _db.Auth.Where(x => x.Phone == pass.Phone).FirstOrDefault();
            if (checkEmail == null)
            {
                string statusMsg = "No account found!";
                ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                return View();
            }
            else
            {


                    var getId = _db.Auth.Where(x => x.Phone == pass.Phone).FirstOrDefault();
                    var chkDB = _db.PasswordReset.Where(x => x.Phone == pass.Phone && x.Status == 0).FirstOrDefault();

                /// Auto increment manually
                int pk;

                try
                {
                    int max = _db.PasswordReset.Max(p => p.Id);
                    pk = max + 1;

                }
                catch (Exception)
                {

                    pk = 1;
                }

                if(chkDB != null)
                {
                    PasswordReset pas = _db.PasswordReset.FirstOrDefault(x => x.Phone == pass.Phone && x.Status == 0);
                    _db.Remove(pas);
                    var removal = await _db.SaveChangesAsync();
                    if(removal > 0)
                    {
                        //save to confirm email
                        var em = _db.Auth.Where(x => x.Phone == pass.Phone).FirstOrDefault();

                        PasswordReset password = new PasswordReset();
                        password.Id = pk;
                        password.Email = em.Email;
                        password.Phone = pass.Phone;
                        password.UserId = getId.Id;
                        password.Status = 0;
                        password.Hash = hexStr;
                        _db.PasswordReset.Add(password);
                        //               _db.Add(password);
                        var success = await _db.SaveChangesAsync();

                        if (success > 0)
                        {
                            string statusMsg = "A link will be sent to you in few minutes. Once receive, please click the link to complete password reset";
                            ViewBag.message = "<div class='alert alert-success' role='alert'>" + statusMsg + "</ div >";
                            return View();

                        }
                    }
                }

                else
                {
                    //save to confirm email
                    var em = _db.Auth.Where(x => x.Phone == pass.Phone).FirstOrDefault();

                    PasswordReset password = new PasswordReset();
                    password.Id = pk;
                    password.Email = em.Email;
                    password.Phone = pass.Phone;
                    password.UserId = getId.Id;
                    password.Status = 0;
                    password.Hash = hexStr;
                    _db.PasswordReset.Add(password);
                    //               _db.Add(password);
                    var success = await _db.SaveChangesAsync();

                    if (success > 0)
                    {
                        string statusMsg = "A link will be sent to you in few minutes. Once receive, please click the link to complete password reset";
                        ViewBag.message = "<div class='alert alert-success' role='alert'>" + statusMsg + "</ div >";
                        return View();

                    }
                }

            }
            return View();
        }


        public IActionResult Reset()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reset(string id, Auth res)
        {
            string link = id;

            ViewBag.id = link;

            var checkHash = _db.PasswordReset.Where(x => x.Hash == link).FirstOrDefault();
            if (checkHash == null)
            {
                //string statusMsg = "No account found!";
                //ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                return Content("Invalid Reset Passoword Link");
            }
            if (checkHash != null)
            {
                if (checkHash.Status == 1)
                {
                    return Content("Password Reset Has Link Expired.");
                }
                if (checkHash.Status == 0)
                {
                    //Update user password
                    Auth aut = _db.Auth.FirstOrDefault(x => x.Id == checkHash.UserId);
                    aut.Password = Crypto.HashPassword(res.Password); ;
                    _db.SaveChanges();

                    //Update Password Reset Table
                    PasswordReset pr = _db.PasswordReset.FirstOrDefault(x => x.Hash == link);
                    pr.Status = 1;
                    var success = await _db.SaveChangesAsync();
                    
                    if(success > 0)
                    {
                        return RedirectToAction("Index", "Auth");
                    }
                }
                

            }
            return View();
        }



        public IActionResult ConfirmSignup(string id)
        {
            var checkHash = _db.ConfirmEmail.Where(x => x.UrlParameter == id).FirstOrDefault();
            if (checkHash == null)
            {
                return RedirectToAction("Index", "Auth");
            }
            if (checkHash != null)
            {
                if (checkHash.IsConfirmed == 1)
                {
                    return RedirectToAction("Index", "Auth");
                }

            }
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmSignup(string id, ConfirmEmail con)
        {
            var checkHash = _db.ConfirmEmail.Where(x => x.UrlParameter == id).FirstOrDefault();
            if (checkHash == null)
            {
                return Content("An error Occured!");
            }

            if (checkHash != null)
            {
                if(checkHash.IsConfirmed == 1)
                {
                    return RedirectToAction("Index", "Auth");
                }
                else if (checkHash.IsConfirmed == 0 && checkHash.Code == con.Code)
                {
                    //Update Password Reset Table
                    ConfirmEmail ce = _db.ConfirmEmail.FirstOrDefault(x => x.UrlParameter == id);
                    ce.IsConfirmed = 1;
                    var success = await _db.SaveChangesAsync();

                    if (success > 0)
                    {
                        var check = _db.Auth.Where(x => x.Phone == checkHash.Phone).FirstOrDefault();

                        const string SessionId = "UserID";
                        const string SessionEmail = "UserEmail";
                        if (check != null)
                        {

                            int pk;

                            try
                            {
                                int max = _db.Account.Max(p => p.Id);
                                pk = max + 1;

                            }
                            catch (Exception)
                            {

                                pk = 1;
                            }

                            HttpContext.Session.SetString(SessionId, check.Id.ToString());
                            HttpContext.Session.SetString(SessionEmail, check.Email.ToString());

                            Account acct = new Account();
                            acct.Id = pk;
                            acct.UserId = check.Id;
                            acct.Photo = null;
                            acct.FirstName = "";
                            acct.LastName = "";
                            acct.Gender = "";
                            acct.State = "";
                            acct.City = "";
                            acct.Email = "";
                            acct.Phone = "";
                            //accc.Dob = "";
                            acct.BVN = "";
                            acct.DateAdded = DateTime.Now;
                            _db.Account.Add(acct);
                            _db.SaveChanges();

                            return RedirectToAction("Index", "Dashboard");

                        }

                    }

                    
                }
                else
                {
                    string statusMsg = "You entered an invalid code";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                    return View();
                }
                
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResendCode(string id, ConfirmEmail conf)
        {
            var checkHash = _db.ConfirmEmail.Where(x => x.UrlParameter == id).FirstOrDefault();
            string getParameter = checkHash.UrlParameter;
            string urlPath = "/Auth/ConfirmSignup/"+ getParameter + "";
            
            var resendBtn = conf.Resend;

            if (resendBtn == "resend")
            {
                string sendMsgd = "Use this code " + checkHash.Code + " to verify your account on myloanApp  ";
                string statusMsg = "Code has been resent. Please wait for some time.";
                ViewBag.message = "<div class='alert alert-info' role='alert'>" + statusMsg + "</ div >";
                
            }
            return View();
            //return base.Content("<script>window.location.replace('"+urlPath+"');</script >", "text/html");
        }


        }
}
