using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RememberText.ViewModels
{
    public class IpAddressesViewModel
    {
        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }
        [Display(Name = "Ip")]
        [Required]
        public string Ip { get; set; }
        [Display(Name = "Country")]
        public string CountryName { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
