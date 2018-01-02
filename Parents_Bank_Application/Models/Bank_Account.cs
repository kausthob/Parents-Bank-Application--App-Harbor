using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
//using Gtt.InterestCalculator;

namespace Parents_Bank_Application.Models
{
    public class Bank_Account
    {

        public int Id { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        public string Owner { get; set; }
        [EmailAddress(ErrorMessage = "Please enter a valid Email Address")]
        [Required]
        public string Recipient { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime OpenDate { get;}
        [Range(1, 99, ErrorMessage = "Interest rate cannot be 0% or below and 100% or above")]
        public int InterestRate { get; set; }

        public String Administrator { get; set; }
        public virtual List<Transaction> Transactions { get; set; }
        public virtual List<WishList_Item> Wish_List_Items { get; set; }

        //public Decimal Balance { get; set; }


        public Bank_Account()
        {
            OpenDate = DateTime.Now;
        }
        public decimal CurrentBalanceWithoutInterest()
        {
            // sum all the total of all of the transactions
            decimal total = Transactions.Sum(x => x.Amount);
            return total;
        }
        public decimal TotalBalance()
        {
            // sum all the total of all of the transactions
            return CurrentBalanceWithoutInterest()+YearToDateInterestEarned();
        }

        public Nullable<DateTime> LastDeposit()
        {
            var TranscationLst = Transactions.OrderByDescending(x => x.TransactionDate).ToList();
            if (TranscationLst != null && TranscationLst.Count > 0)
                return TranscationLst.First(x => x.Amount > 0).TransactionDate.Date;
            else
                return null;
        }
        public decimal YearToDateInterestEarned()
        {
            double CompountInterestCal(double rate, double cpTime, double daysCount)
            {
                double InteresetCval;
                InteresetCval = Math.Pow(1 + (rate / 1200), (cpTime / daysCount));
                return InteresetCval;
            }
            double runningTotal = 0;
            if (Transactions != null && Transactions.Count > 0)
            {
                DateTime BeignDate = new DateTime(DateTime.Now.Year, 1, 1);
                DateTime EndDate = new DateTime(DateTime.Now.Year + 1, 1, 1);
                double Days = (EndDate - BeignDate).Days;
                double compoundingtime = 12;
                for (DateTime dt = BeignDate; dt.Date <= DateTime.Today.Date; dt = dt.AddDays(1))
                {
                    var dailySum = Transactions.Where(b => b.TransactionDate == dt).ToList().Sum(g => g.Amount);
                    runningTotal += (double)Transactions.Where(b => b.TransactionDate == dt).ToList().Sum(g => g.Amount);
                    if (runningTotal > 0)
                    {
                        runningTotal = (runningTotal * CompountInterestCal((double)InterestRate,compoundingtime,Days));
                    }
                }
            }
            return Math.Round((decimal)(runningTotal - (double)CurrentBalanceWithoutInterest()),2);
        }
        public Boolean isOwner(string currentuser)
        {
            if (string.IsNullOrWhiteSpace(currentuser))
            {
                return false;
            }
            if (currentuser.ToUpper().Trim() == Owner.ToUpper().Trim())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean isRecipient(string currentuser)
        {
            if (string.IsNullOrWhiteSpace(currentuser))
            {
                return false;
            }
            if (currentuser.ToUpper().Trim() == Recipient.ToUpper().Trim())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean isOwnerorRecipient(string currentuser)
        {
            if (isRecipient(currentuser) || isOwner(currentuser))
                return true;
            else
                return false;
        }
    }
}