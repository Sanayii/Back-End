using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Sanayii.Core.Entities;
using Sanayii.Services;
using Sanayii.ViewModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using UAParser;

namespace Sanayii.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly EmailSenderService emailSender;
        private readonly SMSSender smsSender;
        private readonly RoleManager<IdentityRole> roleManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, SMSSender smsSender, EmailSenderService emailSender, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.smsSender = smsSender;
            this.emailSender = emailSender;
            this.roleManager = roleManager;
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username or email
            AppUser user = await userManager.FindByNameAsync(model.Username) ?? await userManager.FindByEmailAsync(model.Username);
            if (user == null)
            {
                return BadRequest("Invalid username or password");
            }

            // Check if email is confirmed (if required)
            if (!user.EmailConfirmed)
            {
                return BadRequest("Email is not confirmed. Please check your inbox.");
            }

            // Check password
            var result = await signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    return BadRequest("User account is locked out. Please try again later.");
                if (result.IsNotAllowed)
                    return BadRequest("Login not allowed for this user.");
                return BadRequest("Invalid username or password");
            }

            // Create JWT Token
            var userClaims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email)
    };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("L6scvGt8D3yU5vAqZt9PfMxW2jNkRgT7!@#$%"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5127/",
                audience: "http://localhost:4200/",
                expires: DateTime.UtcNow.AddHours(1),
                claims: userClaims,
                signingCredentials: creds
            );

            return Ok(new
            {
                message = "Login successful",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            var role1 = new IdentityRole() { Name = "Customer" };
            var role2 = new IdentityRole() { Name = "Admin" };
            var role3 = new IdentityRole() { Name = "Artisan" };

            await roleManager.CreateAsync(role1);
            await roleManager.CreateAsync(role2);
            await roleManager.CreateAsync(role3);



            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            var res = await userManager.CreateAsync(user, model.Password);
            if (res.Succeeded)
            {
                var resRole = await userManager.AddToRoleAsync(user, "Customer");
                if (resRole.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return Ok("Registered Successfully");
                }
                else
                {
                    return BadRequest("Failed to assign role");
                }
            }
            else
            {
                return BadRequest(res.Errors.Select(e => e.Description));
            }
        }
        [AllowAnonymous]
        [HttpGet("ExternalLogin")]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl }, Request.Scheme);
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        /// //////////////////////////////////////////////////////////////
        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return BadRequest("External login information is unavailable.");
            }

            var user = await userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                // Create a new user if they do not exist
                user = new AppUser
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    FName = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                    LName = info.Principal.FindFirstValue(ClaimTypes.Surname),
                    City = "Unknown",
                    Street = "Unknown",
                    Governate = "Unknown"
                };

                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest("User creation failed.");
                }

                await userManager.AddLoginAsync(user, info);
            }

            await signInManager.SignInAsync(user, isPersistent: false);

            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("L6scvGt8D3yU5vAqZt9PfMxW2jNkRgT7!@#$%"); // Use a secure key

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { token = tokenHandler.WriteToken(token) });
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
        [Authorize]
        [HttpPost("SendPhoneVerificationCode")]
        public async Task<IActionResult> SendPhoneVerificationCode(ConfirmPhoneNumberViewModel model)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(model.PhoneNumber))
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { message = "User not found. Please log in again." });
                }

                var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);

                var result = await smsSender.SendSmsAsync(model.PhoneNumber,
                    $"Your verification code is: {token}. Please enter this code to confirm your phone number.");

                if (result)
                {
                    return Ok(new { message = "A verification code has been sent to your phone number. Please enter the code to complete verification." });
                }
                else
                {
                    return BadRequest(new { message = "We encountered an issue while sending the verification code. Please try again later." });
                }
            }
            return BadRequest(new { message = "There were errors in your submission. Please correct them and try again." });
        }

        [Authorize]
        [HttpPost("ResendPhoneVerificationCode")]
        public async Task<IActionResult> ResendPhoneVerificationCode()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { message = "User not found. Please log in again." });
            }

            if (string.IsNullOrEmpty(user.PhoneNumber))
            {
                return BadRequest(new { message = "Your phone number is not set. Please update your phone number to continue." });
            }

            var token = await userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
            try
            {
                var result = await smsSender.SendSmsAsync(user.PhoneNumber,
                    $"Your new verification code is: {token}. Please enter this code to verify your phone number.");

                if (result)
                {
                    return Ok(new { message = "A new verification code has been sent to your phone. Please enter the code below to confirm your phone number." });
                }
                else
                {
                    return BadRequest(new { message = "We were unable to resend the verification code. Please try again later." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "An error occurred while sending the verification code. Please try again later." });
            }
        }

        [Authorize]
        [HttpPost("VerifyPhoneNumber")]
        public async Task<IActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data submitted.", errors = ModelState });
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null || string.IsNullOrEmpty(user.PhoneNumber))
            {
                return NotFound(new { message = "User not found. Please log in again." });
            }

            var isTokenValid = await userManager.VerifyChangePhoneNumberTokenAsync(user, model.Token, user.PhoneNumber);

            if (isTokenValid)
            {
                user.PhoneNumberConfirmed = true;
                var updateResult = await userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    return Ok(new { message = "Your phone number has been successfully verified." });
                }
                else
                {
                    return BadRequest(new { message = "An error occurred while confirming your phone number. Please try again." });
                }
            }
            else
            {
                return BadRequest(new { message = "The token has expired or is invalid. Please request a new verification code." });
            }
        }

        private async Task SendPasswordChangedNotificationEmail(string email, AppUser user, string device)
        {
            var subject = "Your Password Has Been Changed";

            var messageBody = $@"
        <div style=""font-family: Arial, Helvetica, sans-serif; font-size: 16px; color: #333; line-height: 1.5; padding: 20px;"">
            <h2 style=""color: #007bff; text-align: center;"">Password Change Notification</h2>
            <p style=""margin-bottom: 20px;"">Hi {user.FName} {user.LName},</p>

            <p>We wanted to let you know that your password for your <strong>Sanayii</strong> account was successfully changed.</p>

            <div style=""margin: 20px 0; padding: 10px; background-color: #f8f9fa; border: 1px solid #ddd; border-radius: 5px;"">
                <p><strong>Date and Time:</strong> {DateTime.UtcNow:dddd, MMMM dd, yyyy HH:mm} UTC</p>
                <p><strong>Device:</strong> {device}</p>
            </div>

            <p>If you did not make this change, please reset your password immediately or contact support for assistance:</p>

            <p style=""margin-top: 30px;"">Thank you,<br />The Sanayii Team</p>
        </div>
        ";

            await emailSender.SendEmailAsync(email, subject, messageBody, IsBodyHtml: true);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data submitted.", errors = ModelState });
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { message = "User not found. Please log in again." });
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                var device = GetDeviceInfo(HttpContext);

                await SendPasswordChangedNotificationEmail(user.Email, user, device);

                await signInManager.RefreshSignInAsync(user);

                return Ok(new { message = "Password changed successfully. A confirmation email has been sent." });
            }
            else
            {
                return BadRequest(new { message = "An error occurred while changing your password. Please try again." });
            }
        }
        private string GetDeviceInfo(HttpContext httpContext)
        {
            try
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

                if (string.IsNullOrEmpty(userAgent))
                {
                    return "Unknown Device";
                }

                var parser = Parser.GetDefault();
                var clientInfo = parser.Parse(userAgent);

                var os = clientInfo.OS.ToString();
                var browser = clientInfo.UA.ToString();

                return $"{browser} on {os}";
            }
            catch (Exception ex)
            {
                return "Unknown Device";
            }
        }
    }
}