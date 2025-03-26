using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sanayii.Controllers.ViewModel;
using Sanayii.Core.Entities;
using System.Linq;
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

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AppUser user = await userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                return BadRequest("User not found!");
            }

            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return Ok("Login Successfully");
            }
            else
            {
                return BadRequest("Invalid username or password");
            }
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
    }
}
