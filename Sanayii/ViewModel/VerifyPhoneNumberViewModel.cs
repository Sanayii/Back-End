﻿using System.ComponentModel.DataAnnotations;

namespace Sanayii.ViewModel
{
    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Verification code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Verification code must be 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Verification code must be numeric.")]
        [Display(Name = "Verification Code")]
        public string Token { get; set; }
    }
}
