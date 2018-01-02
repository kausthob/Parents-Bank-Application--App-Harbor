using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Parents_Bank_Application.Models;

namespace Parents_Bank_Application.Controllers
{
    [Authorize]
    public class Bank_AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public Boolean ShowAffordable { get; set; }
        public Boolean ShowNonAffordable { get; set; }
        
        // GET: Bank_Account
        public ActionResult Index()
        {
            var Owners = db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList();
            if (Owners != null && Owners.Count > 0)
                return View(Owners);

            var Recipient = db.Bank_Accounts.Where(x => x.Recipient == User.Identity.Name).ToList();
            if (Recipient != null && Recipient.Count > 0)
                return RedirectToAction("Details", new { id = Recipient.First().Id });
            
            return View(new List<Bank_Account>());
            //ViewBag.LastTransactionDate = db.Transactions.OrderBy(x => x.TransactionDate).First().TransactionDate;
            //var BankAccounts = db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList();
            //return View(db.Bank_Accounts.ToList());
            //return View(BankAccounts);
        }

        // GET: Bank_Account/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank_Account bank_Account = db.Bank_Accounts.Find(id);
            if (bank_Account == null)
            {
                return HttpNotFound();
            }
            if(!bank_Account.isOwnerorRecipient(User.Identity.Name))
            {
                return HttpNotFound();
            }
            decimal balance = bank_Account.TotalBalance();
            ViewBag.ShowAffordable = false;
            ViewBag.ShowNonAffordable = false;
            foreach (var w in bank_Account.Wish_List_Items)
            {
                if (balance == w.Cost || balance > w.Cost)
                {
                    ViewBag.ShowAffordable = true;
                }
                if (balance < w.Cost)
                {
                    ViewBag.ShowNonAffordable = true;
                }
            }
            return View(bank_Account);
            /*if (bank_Account.isOwnerorAdministrator(User.Identity.Name))
            {
                return View(bank_Account);
            }
            else
            {
                return HttpNotFound();
            }*/
        }

        // GET: Bank_Account/Create
        public ActionResult Create()
        {
            var BankRec = db.Bank_Accounts.Where(w => w.Recipient == User.Identity.Name);
            if (BankRec != null && BankRec.Count() > 0)
            {
                return HttpNotFound();
            }
            return View();
        }

        // POST: Bank_Account/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Owner,Recipient,Name,InterestRate")] Bank_Account bank_Account)
        {
            bank_Account.Administrator = "admin@uc.edu";
            bank_Account.Owner = User.Identity.Name;

            /*
        - A recipient user can only have 1 recipient account
        - A recipient cannot be an owner of another account
        - An owner cannot be a recipient of another account
             */
            if (bank_Account.Owner.ToUpper().Trim() == bank_Account.Recipient.ToUpper().Trim())
            {
                ModelState.AddModelError("Recipient", "Owner and Recipient cannot be same email address");
            }

            List<Bank_Account> ReciepientLst = db.Bank_Accounts.Where(x => x.Recipient == bank_Account.Owner).ToList();
            if(ReciepientLst.Count > 0)
            {
                ModelState.AddModelError("Owner", "An owner cannot be a recipient of another account");
            }

            List<Bank_Account> OwnerLst = db.Bank_Accounts.Where(x => x.Owner == bank_Account.Recipient).ToList();
            if (OwnerLst.Count > 0)
            {
                ModelState.AddModelError("Recipient", "A recipient cannot be an owner of another account");
            }

            List<Bank_Account> OwnerLst1 = db.Bank_Accounts.Where(x => x.Recipient == bank_Account.Recipient).ToList();
            if (OwnerLst1.Count > 0)
            {
                ModelState.AddModelError("Recipient", "A recipient user can have only 1 reciepient account");
            }

            if (ModelState.IsValid)
            {
                db.Bank_Accounts.Add(bank_Account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bank_Account);
        }

        // GET: Bank_Account/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank_Account bank_Account = db.Bank_Accounts.Find(id);
            if (bank_Account == null)
            {
                return HttpNotFound();
            }
            if (!bank_Account.isOwnerorRecipient(User.Identity.Name))
            {
                return HttpNotFound();
            }
            var BankRec = db.Bank_Accounts.Where(w => w.Recipient == User.Identity.Name);
            if (BankRec != null && BankRec.Count() > 0)
            {
                return HttpNotFound();
            }
            return View(bank_Account);
        }

        // POST: Bank_Account/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Owner,Recipient,Name,InterestRate")] Bank_Account bank_Account)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bank_Account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bank_Account);
        }

        // GET: Bank_Account/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bank_Account bank_Account = db.Bank_Accounts.Find(id);
            if (bank_Account == null)
            {
                return HttpNotFound();
            }
            if (!bank_Account.isOwner(User.Identity.Name))
            {
                return HttpNotFound();
            }
            return View(bank_Account);
        }

        // POST: Bank_Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Bank_Account bank_Account = db.Bank_Accounts.Find(id);
            if (bank_Account.TotalBalance() != 0)
            {
                ModelState.AddModelError("Amount", "An account can only be deleted if there is a zero balance");
                return View(bank_Account);
            }
            if (ModelState.IsValid)
            {
                db.Bank_Accounts.Remove(bank_Account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bank_Account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
