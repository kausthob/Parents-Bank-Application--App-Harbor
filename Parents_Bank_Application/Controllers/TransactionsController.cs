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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            var transactionsRec = db.Transactions.Where(w => w.Bank_Account.Recipient == User.Identity.Name);
            if(transactionsRec != null && transactionsRec.Count() > 0)
            {
                return HttpNotFound();
            }
            //var transactions = db.Transactions.Include(t => t.Bank_Account);
            var transactions = db.Transactions.Where(w => w.Bank_Account.Owner == User.Identity.Name || w.Bank_Account.Administrator == User.Identity.Name);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.Bank_Account.isOwner(User.Identity.Name))
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            var transactionsRec = db.Transactions.Where(w => w.Bank_Account.Recipient == User.Identity.Name);
            if (transactionsRec != null && transactionsRec.Count() > 0)
            {
                return HttpNotFound();
            }
            ViewBag.Bank_AccountId = new SelectList(db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList(), "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Bank_AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {

            decimal Trans = transaction.Amount;
            decimal bal = db.Bank_Accounts.Find(transaction.Bank_AccountId).TotalBalance();
            if (Trans < 0 && Math.Abs(Trans) > bal)
            {
                ModelState.AddModelError("Amount", "A debit cannot be for more that the current account balance");
            }

            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Bank_AccountId = new SelectList(db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList(), "Id", "Owner", transaction.Bank_AccountId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.Bank_Account.isOwner(User.Identity.Name))
            {
                return HttpNotFound();
            }

            var transactionsRec = db.Transactions.Where(w => w.Bank_Account.Recipient == User.Identity.Name);
            if (transactionsRec != null && transactionsRec.Count() > 0)
            {
                return HttpNotFound();
            }

            ViewBag.Bank_AccountId = new SelectList(db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList(), "Id", "Owner", transaction.Bank_AccountId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Bank_AccountId,TransactionDate,Amount,Note")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Bank_AccountId = new SelectList(db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList(), "Id", "Owner", transaction.Bank_AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.Bank_Account.isOwner(User.Identity.Name))
            {
                return HttpNotFound();
            }

            var transactionsRec = db.Transactions.Where(w => w.Bank_Account.Recipient == User.Identity.Name);
            if (transactionsRec!= null && transactionsRec.Count() > 0)
            {
                return HttpNotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
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
