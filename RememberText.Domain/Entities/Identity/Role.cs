using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace RememberText.Domain.Entities.Identity
{
    public class Role : IdentityRole
    {
        public const string GenAdministrator = "GenAdministrator";
        public const string Administrator = "Administrators";
        public const string User = "Users";
    }
}
