using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RememberText.Domain.Entities.Identity
{
    public class User : IdentityUser
    {
        [StringLength(256)]
        public string Nickname { get; set; }
        [StringLength(256)]
        public string PreffLang { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastLoginTime { get; set; }
        public int LimitOfText { get; set; }
        [Required]
        public int YearOfBirth { get; set; }

        public const string RTGenAdministrator = "GenAdministrator";
        public const string RTAdministrator = "Administrators";
        public const string RTUser = "Users";
        public const string AdminDefaultPassword = "9B3t&r0Yqz$!";
        public const string AdminEmail = "yury.podlevskikh@gmail.com";
    }
}
