using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DocWebSite.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email", ResourceType = typeof(Resources.DocLang))]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "VerifyCode", ResourceType = typeof(Resources.DocLang))]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "RememberBrowser", ResourceType = typeof(Resources.DocLang))]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email", ResourceType = typeof(Resources.DocLang))]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email", ResourceType = typeof(Resources.DocLang))]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.DocLang))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(Resources.DocLang))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(Resources.DocLang))]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "PasswordLengthError", ErrorMessageResourceType = typeof(Resources.DocLang), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.DocLang))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.DocLang))]
        [Compare("Password", ErrorMessage = "ConfirmPasswordNotMatch", ErrorMessageResourceType = typeof(Resources.DocLang))]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(Resources.DocLang))]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "PasswordLengthError", ErrorMessageResourceType = typeof(Resources.DocLang), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.DocLang))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.DocLang))]
        [Compare("Password", ErrorMessage = "ConfirmPasswordNotMatch", ErrorMessageResourceType = typeof(Resources.DocLang))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(Resources.DocLang))]
        public string Email { get; set; }
    }
}
