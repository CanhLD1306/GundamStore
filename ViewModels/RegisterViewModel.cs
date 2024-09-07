﻿using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GundamStore.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8-20 characters")]
        [RegularExpression(@"(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\w\d\s]).{8,}", 
        ErrorMessage = "Password must have at least one digit, one lowercase letter, one uppercase letter, and one special character.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The Confirmation password do not match.")]
        [StringLength(20)]
        public string? ConfirmPassword { get; set; }
    }
}
