using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;

namespace BattleIntel.Web.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public DateTime JoinDateUTC { get; set; }
    }

    public class UserIndexModel
    {
        public UserModel User { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Open ID")]
        public string OpenIdProvider { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public LoginModel()
        {
            RememberMe = true;
        }
    }

    public class UserEditModel
    {
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }
    }

    public class UserDataModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
    }


}
