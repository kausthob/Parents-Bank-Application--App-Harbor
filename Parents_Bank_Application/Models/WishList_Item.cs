using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Parents_Bank_Application.Models
{
    public class WishList_Item
    {
        public int Id { get; set; }
        public DateTime DateAdded { get; set; }
        [Required]
        public decimal Cost { get; set; }
        [Required]
        public string Description { get; set; }
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string Link { get; set; }
        public Boolean Purchased { get; set; }

        public virtual Bank_Account Bank_Account { get; set; }
        public virtual int Bank_AccountId { get; set; }
    }
    /*
     A recipient cannot delete a wish list item
     */
}