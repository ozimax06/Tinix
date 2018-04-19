using System.ComponentModel.DataAnnotations;

namespace Tinix.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter the user name")]
        public string UserName
        {
            get;
            set;
        }

        [Required(ErrorMessage = "Please enter the password")]
        public string Password
        {
            get;
            set;
        }

        public bool RememberMe
        {
            get;
            set;
        }
        
        public bool CredentialsCorrect
        {
            get;
            set;
        } = true;
    }
}