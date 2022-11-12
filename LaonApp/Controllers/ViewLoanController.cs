using LoanApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoanApp.Controllers
{
    public class ViewLoanController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ViewLoanController(ApplicationDbContext db)
        {
            _db = db;
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

                    ViewBag.Loans += "<tr><td>&#8358;" + lo.Borrowed.ToString("N") + "</td><td>" + sta + "</td><td> <a href='/" + lo.Id + "/view'><i class='fa fa-eye'></i>View</a> </td></tr>";
                }
            }
            return View();
        }
    }
}
