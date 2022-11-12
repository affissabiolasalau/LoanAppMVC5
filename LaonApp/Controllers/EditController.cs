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
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace LoanApp.Controllers
{
    public class EditController : Controller
    {
        private readonly ApplicationDbContext _db;
        public IEnumerable<Account> Accounts { get; set; }

        public EditController(ApplicationDbContext db)
        {
            _db = db;
        }

         
        public IActionResult Upload()
        {
            var userid = this.HttpContext.Session.GetString("UserID");

            int uId = int.Parse(userid);

            if (userid == null)
            {
                return Redirect("Auth/");
            }

            return View();
        }
            [HttpPost]
        public IActionResult Upload(IFormFile files)
        {
            var userid = this.HttpContext.Session.GetString("UserID");

            int uId = int.Parse(userid);

            Account Acct = _db.Account.FirstOrDefault(x => x.UserId == uId);

            if (files != null)
            {
                if (files.Length > 0)
                {
                    //Getting FileName
                    var fileName = Path.GetFileName(files.FileName);

                    //Assigning Unique Filename (Guid)
                    var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                    //Getting file Extension
                    var fileExtension = Path.GetExtension(fileName);

                    // concatenating  FileName + FileExtension
                    var newFileName = String.Concat(myUniqueFileName, fileExtension);

                    // Combines two strings into a path.
                    var filepath =
            new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads")).Root + $@"\{newFileName}";

                    using (FileStream fs = System.IO.File.Create(filepath))
                    {
                        files.CopyTo(fs);
                        fs.Flush();
                    }

                    if (_db.Account.Where(x => x.UserId == uId).FirstOrDefault() == null)
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

                        Account acct = new Account();
                        acct.Id = pk;
                        acct.UserId = int.Parse(userid);
                        acct.Photo = newFileName;
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

                    }
                    else 
                    { 
                        //Update
                         
                        Account acct = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
                        acct.Photo = newFileName;
                        acct.DateAdded = DateTime.Now;
                        _db.SaveChanges();
                    }

                    return RedirectToAction("index",  "Edit");
                }
            }
            return View(Acct);
        } 

        public IActionResult Index()
        {
            int accountStatus = 1;
            int uId;

            var userid = this.HttpContext.Session.GetString("UserID");

            if (userid != null)
            {
                uId = int.Parse(userid);
            }
            else 
            {
                return Redirect("/");
            }
            


            if(uId > 0)
            {
                var auth = _db.Auth.Where(x => x.Id == uId).FirstOrDefault();

                ViewBag.Email = auth.Email;

                var checkAcct = _db.Account.Where(x => x.UserId == auth.Id).FirstOrDefault();

                if (checkAcct == null)
                {
                    accountStatus = 0;
                }
            

            ViewBag.PageTitle = accountStatus;
                
            var editing = _db.Account.Where(x => x.UserId == auth.Id).FirstOrDefault();

            if(editing != null)
            {
               var edit = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
                ViewBag.Photo = edit.Photo;
                ViewBag.BVN = edit.BVN;
                ViewBag.City = edit.City;
                ViewBag.State = edit.State;
            }
            else
            {
                ViewBag.Photo = "";
                ViewBag.BVN = "";
                ViewBag.City = "";
                ViewBag.State = "";
            }

            }

            ViewBag.Photo = "";
            ViewBag.BVN = "";
            ViewBag.City = "";
            ViewBag.State = "";
            
            if (_db.Account.Where(x => x.UserId == uId).FirstOrDefault() == null)
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

                Account accc = new Account();
                accc.Id = pk;
                accc.UserId = int.Parse(userid);
                accc.FirstName = "";
                accc.LastName = "";
                accc.Gender = "";
                accc.State = "";
                accc.City = "";
                accc.Email = "";
                accc.Phone = "";
                //accc.Dob = "";
                accc.BVN = "";
                accc.DateAdded = DateTime.Now;
                _db.Account.Add(accc);
                _db.SaveChanges();

                int pk2;
                try
                {
                    int max = _db.Wallet.Max(p => p.Id);
                    pk2 = max + 1;

                }
                catch (Exception)
                {

                    pk2 = 1;
                }

                Wallet wall = new Wallet();
                wall.Id = pk2;
                wall.UserId = int.Parse(userid);
                wall.Balance = 0;
                wall.Bonus = 0;
                wall.DateAdded = DateTime.Now;
                _db.Wallet.Add(wall);
                _db.SaveChangesAsync(); // _db.SaveChanges();

            }
            

            if (userid == null)
            {
                return Redirect("Auth/");
            }

          try
                      {
                          Account acctx = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
                          //ViewBag.Account = Acct;
                          ViewBag.PageTitle = "Edit Profile";
                          return View(acctx);
                      }
                      catch (Exception)
                      {

                          ViewBag.PageTitle = "Edit Profile";
                          return View();
                      }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Account acc, Wallet wa)
        {
            var userid = this.HttpContext.Session.GetString("UserID");
            int uId = int.Parse(userid);
            
            Account Acct = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();

            Account up = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();

            var useremail = this.HttpContext.Session.GetString("UserEmail");
           
            if(_db.Account.Where(x => x.UserId == uId).FirstOrDefault() == null)
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

                Account accc = new Account();
                accc.Id = pk;
                accc.UserId = int.Parse(userid);
                accc.FirstName = acc.FirstName;
                accc.LastName = acc.LastName;
                accc.Gender = acc.Gender;
                accc.State = acc.State;
                accc.City = acc.City;
                accc.Email = useremail;
                accc.Phone = acc.Phone;
                accc.Dob = acc.Dob;
                accc.BVN = acc.BVN;
                accc.DateAdded = DateTime.Now;
                _db.Account.Add(accc);
                _db.SaveChanges();

                int pk2;
                try
                {
                    int max = _db.Wallet.Max(p => p.Id);
                    pk2 = max + 1;

                }
                catch (Exception)
                {

                    pk2 = 1;
                }

                Wallet wall = new Wallet();
                wall.Id = pk2;
                wall.UserId = int.Parse(userid);
                wall.Balance = 0;
                wall.Bonus = 0;
                wall.DateAdded = DateTime.Now;
                _db.Wallet.Add(wall);

                var saved = await _db.SaveChangesAsync(); // _db.SaveChanges();

                if (saved > 0)
                {
                    string statusMsg = "Updated.";
                    ViewBag.message = "<div class='alert alert-success' role='alert'>" + statusMsg + "</ div >";
                    return View(Acct);
                }
                else
                {
                    string statusMsg = "An error occcured. Please try again.";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                    return View(Acct);
                }
            }

            else
            {
                Account accc = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
                //Account accc = new Account();
//                accc.UserId = int.Parse(userid);
                accc.FirstName = acc.FirstName;
                accc.LastName = acc.LastName;
                accc.Gender = acc.Gender;
                accc.State = acc.State;
                accc.City = acc.City;
                accc.Email = useremail;
                accc.Phone = acc.Phone;
                accc.Dob = acc.Dob;
                accc.BVN = acc.BVN;
                accc.DateAdded = DateTime.Now;
//                _db.Account.Add(acc);
//                _db.SaveChanges();

                var saved = await _db.SaveChangesAsync(); // _db.SaveChanges();

                if (saved > 0)
                {
                    string statusMsg = "Updated.";
                    ViewBag.message = "<div class='alert alert-success' role='alert'>" + statusMsg + "</ div >";
                    return View(Acct);
                }
                else
                {
                    string statusMsg = "An error occcured. Please try again.";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                    return View(Acct);
                }

            }

            //   return View(Acct);
        }
        

    }
}
