using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Parents_Bank_Application.Models
{
    [CustomValidation(typeof(Transaction), "TransactionValue")]
    [CustomValidation(typeof(Transaction), "TransactionDateVal")]
    public class Transaction
    {
        public int Id { get; set; }
        public virtual int Bank_AccountId { get; set; }
        public virtual Bank_Account Bank_Account { get; set; }
        public DateTime TransactionDate { get; set; }

        public decimal Amount { get; set; }

        [Required]
        public string Note { get; set; }

        public static ValidationResult TransactionValue(Transaction Ts, ValidationContext context)
        {
            if (Ts != null && Ts.Amount == 0m)
            {
                return new ValidationResult("A transaction cannot be for a $0.00 amount");
            }
            return ValidationResult.Success;

        }
        public static ValidationResult TransactionDateVal(Transaction Ts, ValidationContext context)
        {
            if (Ts != null && Ts.TransactionDate > DateTime.Now)
            {
                return new ValidationResult("The transaction date cannot be in the future");
            }
            if (Ts != null && Ts.TransactionDate.Year < DateTime.Now.Year)
            {
                return new ValidationResult("The transaction date cannot be before the current year");
            }
            return ValidationResult.Success;

        }
        /*
          - A debit cannot be for more that the current account balance
         */
        public Transaction()
        {
            //Transaction date cannot be in future and cannot be before the current year
            TransactionDate = DateTime.Now;
        }
    }
}