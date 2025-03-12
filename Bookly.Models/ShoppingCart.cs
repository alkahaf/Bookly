using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bookly.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public int ProductId { get; set; }
        [Range(1,1000,ErrorMessage ="Please enter a value between 1 and 1000")]
        public int Count { get; set; }
        
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUser")]
        [ValidateNever]
        public string ApplicationUser { get; set; }
    }
}
