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
    public class WishList_ItemController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WishList_Item
        public ActionResult Index()
        {
            //var wishList_Items = db.WishList_Items.Include(w => w.Bank_Account);
            var wishList_Items = db.WishList_Items.Where(w => w.Bank_Account.Owner == User.Identity.Name || w.Bank_Account.Administrator == User.Identity.Name || w.Bank_Account.Recipient == User.Identity.Name);
            return View(wishList_Items.ToList());
        }

        // GET: WishList_Item/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList_Item wishList_Item = db.WishList_Items.Find(id);
            if (wishList_Item == null)
            {
                return HttpNotFound();
            }
            if (!wishList_Item.Bank_Account.isOwnerorRecipient(User.Identity.Name))
            {
                return HttpNotFound();
            }
            return View(wishList_Item);
        }

        // GET: WishList_Item/Create
        public ActionResult Create()
        {
            var OwnerBank = db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList();
            if(OwnerBank == null || OwnerBank.Count == 0)
            {
                OwnerBank = db.Bank_Accounts.Where(x => x.Recipient == User.Identity.Name).ToList();
            }

            ViewBag.Bank_AccountId = new SelectList(OwnerBank, "Id", "Name");
            return View();
        }

        // POST: WishList_Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DateAdded,Cost,Description,Link,Purchased,Bank_AccountId")] WishList_Item wishList_Item)
        {
            if (ModelState.IsValid)
            {
                db.WishList_Items.Add(wishList_Item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var OwnerBank = db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList();
            if (OwnerBank == null || OwnerBank.Count == 0)
            {
                OwnerBank = db.Bank_Accounts.Where(x => x.Recipient == User.Identity.Name).ToList();
            }
            ViewBag.Bank_AccountId = new SelectList(OwnerBank, "Id", "Owner", wishList_Item.Bank_AccountId);
            return View(wishList_Item);
        }

        // GET: WishList_Item/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList_Item wishList_Item = db.WishList_Items.Find(id);
            if (wishList_Item == null)
            {
                return HttpNotFound();
            }
            if (!wishList_Item.Bank_Account.isOwnerorRecipient(User.Identity.Name))
            {
                return HttpNotFound();
            }

            var OwnerBank = db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList();
            if (OwnerBank == null || OwnerBank.Count == 0)
            {
                OwnerBank = db.Bank_Accounts.Where(x => x.Recipient == User.Identity.Name).ToList();
            }
            ViewBag.Bank_AccountId = new SelectList(OwnerBank, "Id", "Owner", wishList_Item.Bank_AccountId);
            return View(wishList_Item);
        }

        // POST: WishList_Item/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DateAdded,Cost,Description,Link,Purchased,Bank_AccountId")] WishList_Item wishList_Item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wishList_Item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var OwnerBank = db.Bank_Accounts.Where(x => x.Owner == User.Identity.Name || x.Administrator == User.Identity.Name).ToList();
            if (OwnerBank == null || OwnerBank.Count == 0)
            {
                OwnerBank = db.Bank_Accounts.Where(x => x.Recipient == User.Identity.Name).ToList();
            }
            ViewBag.Bank_AccountId = new SelectList(OwnerBank, "Id", "Owner", wishList_Item.Bank_AccountId);
            return View(wishList_Item);
        }

        // GET: WishList_Item/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishList_Item wishList_Item = db.WishList_Items.Find(id);
            if (wishList_Item == null)
            {
                return HttpNotFound();
            }
            if (!wishList_Item.Bank_Account.isOwner(User.Identity.Name))
            {
                return HttpNotFound();
            }
            return View(wishList_Item);
        }

        // POST: WishList_Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WishList_Item wishList_Item = db.WishList_Items.Find(id);
            db.WishList_Items.Remove(wishList_Item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Search(String listname)
        {
            bool SearchPerformed = false;
            var wishList_Items = db.WishList_Items.Where(w => w.Bank_Account.Owner == User.Identity.Name || w.Bank_Account.Administrator == User.Identity.Name);
            if(wishList_Items == null || wishList_Items.Count() == 0)
            {
                wishList_Items = db.WishList_Items.Where(w => w.Bank_Account.Recipient == User.Identity.Name);
            }


            if (!string.IsNullOrWhiteSpace(listname))
            {
                wishList_Items = wishList_Items.Where(x => x.Description.Contains(listname));
                SearchPerformed = true;
                decimal n;
                bool isNumeric = Decimal.TryParse(listname, out n);
                if (isNumeric)
                {
                    var price = Convert.ToDecimal(listname);
                    if (wishList_Items == null || wishList_Items.Count() == 0)
                    {
                        wishList_Items = db.WishList_Items.Where(w => w.Bank_Account.Recipient == User.Identity.Name);
                    }
                    wishList_Items = db.WishList_Items.Where(w => w.Cost == price);
                    SearchPerformed = true;
                }
            }
            else
            {
                return View("Index", wishList_Items.ToList());
            }

            if (SearchPerformed)
            {
                // return search results
                return View("Index", wishList_Items.ToList());
            }
            
                // return empty list
                return View(new List<WishList_Item>());
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
