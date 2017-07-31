using System;
using Checkout.Cart.Abstracts;
using System.ComponentModel.DataAnnotations;

namespace Checkout.Cart.Models
{
    public class Item
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name cannot be empty")]
        public string Name { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value should be non negative")]
        public int Quantity { get; set; }
    }
}
