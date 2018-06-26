using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace turkcell_web_app.ViewModels
{
    public class Credentials
    {
        //used to execute klfsajdflkjafdlk jak;lfd jd 
        private string email;
        [Display(Name = "E-mail")]
        [Required]
        [EmailAddress]
        public string Email
        {
            get { return email; }
            set { email = value; }
        }


        private string pass;
        [Display(Name ="Password")]
        [Required]
        public string Pass
        {
            get { return pass; }
            set { pass = value; }
        }

    }
}