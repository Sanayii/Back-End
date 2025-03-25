﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sanayii.Controllers.ViewModel;
using Sanayii.Core.Entities;
using Sanayii.Services;
using System;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sanayii.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly EmailSenderService emailSender;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, EmailSenderService emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.emailSender = emailSender;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
                return BadRequest(new { message = "Invalid username or password." });

            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
                return Ok(new { message = "Login successful." });

            return BadRequest(new { message = "Invalid username or password." });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                UserName = model.Username,
                Email = model.Email,
                FName = model.FName,
                LName = model.LName,
                City = model.City,
                Street = model.Street,
                Governate = model.Government
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors.Select(e => e.Description));

            await userManager.AddToRoleAsync(user, "Customer");

            await SendConfirmationEmail(user);
            await signInManager.SignInAsync(user, false);

            return Ok(new { message = "Registration successful. Please confirm your email." });
        }

        [HttpGet("ConfirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Invalid confirmation link." });

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            try
            {
                // Decode the Base64 token
                var decodedBytes = Convert.FromBase64String(WebUtility.UrlDecode(token));
                var decodedToken = System.Text.Encoding.UTF8.GetString(decodedBytes);

                var result = await userManager.ConfirmEmailAsync(user, decodedToken);
                if (result.Succeeded)
                    return Ok(new { message = "Email confirmed successfully." });

                return BadRequest(new
                {
                    message = "Email confirmation failed. The link may have expired or is invalid.",
                    action = "Please request a new confirmation email.",
                    resendUrl = $"{Request.Scheme}://{Request.Host}/api/Account/ResendConfirmationEmail?email={user.Email}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while confirming your email.", error = ex.Message });
            }
        }

        private async Task SendConfirmationEmail(AppUser user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            // Proper Encoding (Convert to Base64)
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            var encodedToken = WebUtility.UrlEncode(Convert.ToBase64String(tokenBytes));

            var confirmationLink = $"{Request.Scheme}://{Request.Host}/api/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            var subject = "Confirm Your Email - Sanayii";
            var messageBody = $@"
                    <div style=""font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#333;"">
                        <p>Hi {user.FName} {user.LName},</p>
                        <p>Thank you for registering with <strong>Sanayii</strong>. Please confirm your email by clicking the button below:</p>
                        <p>
                            <a href=""{HtmlEncoder.Default.Encode(confirmationLink)}"" 
                               style=""background-color:#007bff;color:#fff;padding:10px 20px;text-decoration:none;
                                      font-weight:bold;border-radius:5px;display:inline-block;"">
                                Confirm Email
                            </a>
                        </p>
                        <p>If the button doesn’t work, copy and paste this URL into your browser:</p>
                        <p><a href=""{HtmlEncoder.Default.Encode(confirmationLink)}"" style=""color:#007bff;text-decoration:none;"">{HtmlEncoder.Default.Encode(confirmationLink)}</a></p>
                        <p>If you did not sign up for this account, please ignore this email.</p>
                        <p>Best Regards,<br />The Sanayii Team</p>
                    </div>
                ";

            await emailSender.SendEmailAsync(user.Email, subject, messageBody, true);
        }

        [HttpPost("ResendConfirmationEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Please provide a valid email address." });

            var user = await userManager.FindByEmailAsync(email);
          
            if (user == null)
                return NotFound(new { message = "User not found." });
            var res = await userManager.IsEmailConfirmedAsync(user);
            if (res)
            {
                return Ok(new { message = "This Email is Already confirmed!" });
            }

            try
            {
                await SendConfirmationEmail(user);
                return Ok(new { message = "A new confirmation email has been sent." });
            }
            catch
            {
                return StatusCode(500, new { message = "An error occurred while sending the confirmation email. Please try again later." });
            }
        }
        private async Task SendForgotPasswordEmail(string email, AppUser user)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token); 

            var passwordResetLink = Url.Action("ResetPassword", "Account",
                new { Email = email, Token = encodedToken }, protocol: HttpContext.Request.Scheme);

            var safeLink = HtmlEncoder.Default.Encode(passwordResetLink);

            var subject = "Reset Your Password";

            var messageBody = $@"
                <div style=""font-family: Arial, Helvetica, sans-serif; font-size: 16px; color: #333; line-height: 1.5; padding: 20px;"">
                    <h2 style=""color: #007bff; text-align: center;"">Password Reset Request</h2>
                    <p>Hi {user.FName} {user.LName},</p>

                    <p>We received a request to reset your password for your <strong>Sanayii</strong> account. Click the button below to reset your password:</p>

                    <div style=""text-align: center; margin: 20px 0;"">
                        <a href=""{safeLink}"" 
                           style=""background-color: #007bff; color: #fff; padding: 10px 20px; text-decoration: none; font-weight: bold; border-radius: 5px; display: inline-block;"">
                            Reset Password
                        </a>
                    </div>

                    <p>If the button above doesn’t work, copy and paste this URL into your browser:</p>
                    <p style=""background-color: #f8f9fa; padding: 10px; border: 1px solid #ddd; border-radius: 5px;"">
                        <a href=""{safeLink}"" style=""color: #007bff; text-decoration: none;"">{safeLink}</a>
                    </p>

                    <p>If you did not request this reset, please ignore this email.</p>

                    <p>Thank you,<br />The Sanayii Team</p>
                </div>";

            await emailSender.SendEmailAsync(email, subject, messageBody, IsBodyHtml: true);
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await SendForgotPasswordEmail(user.Email, user);
                return Ok(new { message = "Email sent successfully." });
            }
            return NotFound(new { message = "User not found." });
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var decodedToken = WebUtility.UrlDecode(WebUtility.UrlDecode(model.Token));

            var result = await userManager.ResetPasswordAsync(user, decodedToken, model.Password);
            if (result.Succeeded)
            {
                return Ok(new { message = "Password reset successfully." });
            }

            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }
    }
}
