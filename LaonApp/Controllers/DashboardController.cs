using LoanApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace LoanApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IEnumerable<Account> Accounts { get; set; }

        public IActionResult Index()
        {

            int uId;

            var userid = this.HttpContext.Session.GetString("UserID");
            uId = int.Parse(userid);

            if (userid == null)
            {
                return Redirect("Auth");
            }

            try
            {
               _db.Account.Where(x => x.UserId == uId).Single();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Edit");

            }

            try
            {
                var loanData = _db.Loans.OrderByDescending(x => x.UserId == uId).Where(x => x.UserId == uId).First();
                if (loanData.ToPay > 1 && loanData.Status == 0)
                {
                    var appdate = loanData.DateAdded.ToLongDateString();
                    var dudate = loanData.DateDue.ToLongDateString();
                    ViewBag.ToPay = loanData.Borrowed.ToString("N");
                    ViewBag.Date = $"Date Applied: {appdate}";
                    ViewBag.Pending = "Processing";
                }
                else if (loanData.ToPay > 1 && loanData.Status == 1)
                {
                    var appdate = loanData.DateAdded.ToLongDateString();
                    var dudate = loanData.DateDue.ToLongDateString();
                    ViewBag.ToPay = loanData.ToPay.ToString("N");
                    ViewBag.Date = $"Due Date: {dudate}";
                    ViewBag.Pending = "Approved";
                }
                else 
                {
                    ViewBag.ToPay = 0;
                    ViewBag.Date = "";
                    ViewBag.Pending = "";
                }
                
            }
            catch (Exception)
            {

                ViewBag.ToPay = 0;
                ViewBag.Date = "";
                ViewBag.Pending = "";
            }

            Account acct = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
            Wallet Wall = _db.Wallet.Where(x => x.UserId == uId).FirstOrDefault();

            ViewBag.Loans = "";
            string sta = "";

            var loans = _db.Loans.OrderByDescending(x => x.UserId == uId).Where(x => x.UserId == uId).ToList();

            //Loans loans = new Loans();
            //List<Loans> viewLoans = loans.ToString();
           if(loans != null)
            {
                foreach (var lo in loans)
                {
                    if (lo.Status == 0)
                    {
                        sta = "<font style='color:red;'>Processing</font>";
                    }
                    if (lo.Status == 1)
                    {
                        sta = "<font style='color:green;'>Approved</font>";
                    }

                    ViewBag.Loans += "<tr><td>&#8358;" + lo.Borrowed.ToString("N") + "</td><td>" + sta + "</td><td> <a href='/dashboard/viewloan/" + lo.Id + "'><i class='fa fa-eye'></i>View</a> </td></tr>";
                }
            }
            
            ViewBag.Wallet = Wall;
            
            return View(acct);
        }

        public IActionResult Apply()
        {

            int uId;

            var userid = this.HttpContext.Session.GetString("UserID");
            uId = int.Parse(userid);

            if (userid == null)
            {
                return Redirect("Auth");
            }

            try
            {
                _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Edit");

            }

            Account acct = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
            Wallet Wall = _db.Wallet.Where(x => x.UserId == uId).FirstOrDefault();
            Loans loan = _db.Loans.Where(x => x.UserId == uId).FirstOrDefault();
            ViewBag.Ln = loan;
            return View(acct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Apply(Loans ln)
        {
            int uId;

            var userid = this.HttpContext.Session.GetString("UserID");
            uId = int.Parse(userid);

            var varAccount = _db.Account.Where(x => x.UserId == uId).FirstOrDefault();

            
            if (userid == null)
            {
                return Redirect("Auth");
            }

            try
            {
                _db.Account.Where(x => x.UserId == uId).FirstOrDefault();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Edit");

            }

            float dailyperce = ln.Borrowed * 0.01f;
            int returning = Convert.ToInt32(dailyperce) * int.Parse(ln.Days);
            int payback = returning + ln.Borrowed;

            try
            {
                int pk;
                try
                {
                    int max = _db.Loans.Max(p => p.Id);
                    pk = max + 1;

                }
                catch (Exception)
                {

                    pk = 1;
                }

                if (varAccount == null)
                {
                    return RedirectToAction("Index", "Edit");
                }
                else if (_db.Loans.Where(x => x.UserId == uId && x.Status == 0).FirstOrDefault() != null)
                {
                    string statusMsg = "You can not apply for loan at the moment. You have a pending request.";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                }
                else if (_db.Loans.Where(x => x.UserId == uId && x.Status == 1 && x.ToPay > 0).FirstOrDefault() != null)
                {
                    string statusMsg = "Sorry, you can not apply for loan while you are owing.";
                    ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
                }
                else if(_db.Loans.Where(x => x.UserId == uId && x.ToPay > 1).FirstOrDefault() == null)
                {
                    Loans loans = new Loans();
                    loans.Id = pk;
                    loans.UserId = uId;
                    loans.Borrowed = ln.Borrowed;
                    loans.Paid = 0;
                    loans.ToPay = payback;
                    loans.Status = 0;
                    loans.Days = ln.Days;
                    loans.DateDue = DateTime.Today.AddDays(int.Parse(ln.Days));
                    loans.DateAdded = DateTime.Now;
                    _db.Loans.Add(loans);
                    _db.SaveChanges();

                    return RedirectToAction("Index", "Dashboard");

                }

            }
            catch (Exception ex)
            {
                
                string statusMsg = "Application Failed. Please, try again" +ex;
               // string statusMsg = e.Message;
                ViewBag.message = "<div class='alert alert-danger' role='alert'>" + statusMsg + "</ div >";
             
            }


            var acct = _db.Account.Where(x => x.UserId == uId);
            var Wall = _db.Wallet.Where(x => x.UserId == uId);
            ViewBag.Wallet = Wall;
            return View(acct);
        }

        [HttpGet()]
        public IActionResult ViewLoan(int id)
        {

            int uId;

            var userid = this.HttpContext.Session.GetString("UserID");
            uId = int.Parse(userid);

            ViewBag.Loans = "";
            string sta = "";

            var loans = _db.Loans.OrderByDescending(x => x.UserId == uId).Where(x => x.UserId == uId && x.Id == id).ToList();

            //Loans loans = new Loans();
            //List<Loans> viewLoans = loans.ToString();
            if (loans != null)
            {
                foreach (var lo in loans)
                {
                    if (lo.Status == 0)
                    {
                        sta = "<font style='color:red;'>Processing</font>";
                    }
                    if (lo.Status == 1)
                    {
                        sta = "<font style='color:green;'>Approved</font>";
                    }

                    ViewBag.Loans += "<tr><td>&#8358;" + lo.Borrowed.ToString("N") + "</td><td>&#8358;" + lo.ToPay.ToString("N") + "</td><td>" + sta + "</td><td>"+ lo.DateDue.ToLongDateString() + "</td></tr>";
                }
            }
            return View();
        }
    }
    }
