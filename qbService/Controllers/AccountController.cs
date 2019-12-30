using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using qbService.Models;
using qbService.Services;
namespace qbService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContex _context;
        private string _root = "root@skylease.aero";
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ApplicationDbContex contex)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._configuration = configuration;
            this._context = contex;
        }
        [Route("GetAllUser")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public  ActionResult<IEnumerable<object>> GetAllUsers()
        {
            List<UserInfo> users = _userManager.Users.Select(x => new UserInfo
            {
                Id = x.Id,
                Email = x.Email,
                CompanyName = x.CompanyName,
                Phone = x.PhoneNumber,
                Block = (x.LockoutEnd.ToString()!=null) ? false : true
            }).ToList();
            if (users.Count == 0)
            {
                return Ok();
            }

            users.ForEach(x => x.Roles = new List<string>());

            var roles = _roleManager.Roles.ToList();
            foreach (IdentityRole item in roles)
            {
                IList<ApplicationUser> userTemps =  _userManager.GetUsersInRoleAsync(item.Name).Result;
                if (userTemps != null)
                {
                    users.ForEach(userinfo =>
                    {
                        if (userTemps.Any(userTemp => userTemp.Id == userinfo.Id))
                            userinfo.Roles.Add(item.Name);
                    });
                }
            }
            return users;
        }
        [Route("GetUserByID")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<object> GetUsersByID([FromBody]string id)
        {
            if (id == null)
                return BadRequest("ID Null");
            List<UserInfo> users = _userManager.Users.Where(x=>x.Id== id).Select(x => new UserInfo
            {
                Id = x.Id,
                Email = x.Email,
                CompanyName = x.CompanyName,
                Phone = x.PhoneNumber,
                Block = (x.LockoutEnd.ToString() != null) ? false : true
            }).ToList();

            if (users.Count == 0)
            {
                return Ok();
            }
            users.ForEach(x => x.Roles = new List<string>());

            var roles = _roleManager.Roles.ToList();
            foreach (IdentityRole item in roles)
            {
                IList<ApplicationUser> userTemps = _userManager.GetUsersInRoleAsync(item.Name).Result;
                if (userTemps != null)
                {
                    users.ForEach(userinfo =>
                    {
                        if (userTemps.Any(userTemp => userTemp.Id == userinfo.Id))
                            userinfo.Roles.Add(item.Name);
                    });
                }
            }
            return users[0];
        }

        [Route("UpdateUser")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateUser([FromBody] UserInfo userInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = _context.Users.Find(userInfo.Id);
            if (user != null)
            {
                user.Email = userInfo.Email;
                user.NormalizedEmail = userInfo.Email;
                user.UserName = userInfo.Email;
                user.NormalizedUserName = userInfo.Email;
                user.CompanyName = userInfo.CompanyName;
                user.PhoneNumber = userInfo.Phone;
                //_context.Users.Attach(user).Property(x=> x.Email)
                _context.SaveChanges();
            }
            return Ok();
        }

        //Use to valid asyn
        [Route("AnyUserByEmail")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public ActionResult<bool> GetAnyUserByEmail([FromQuery] string Email)
        {
            var result = _context.Users.Any(x => x.Email == Email);
            return result;
        }

        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser { 
                        UserName = userInfo.Email,
                        CompanyName = userInfo.CompanyName, 
                        Email = userInfo.Email, 
                        PhoneNumber = userInfo.Phone};
                    var result = await _userManager.CreateAsync(user,userInfo.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                        string callbackUrl = Url.Action(null, null, new { code = code, email = user.Email }, Request.Scheme);
                        EmailService emailService = new EmailService(_configuration);
                        string body = "Please clicking here to activate your account: <a href=\"" + callbackUrl.Replace("api/Account/Create", "forgotPassword") + "\">link</a>";
                        emailService.SendEmailSystem(user.Email, "Activate Account Skyleaeaccess", body);
                        return Ok();
                    }
                    else
                    {
                        return BadRequest(result.Errors);
                    }
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPost]
        [Route("Remove")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApplicationUser>> RemoveUser([FromBody] UserInfo userInfo)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userInfo.Email);
                //var user = new ApplicationUser { UserName = userInfo.Email, Email = userInfo.Email };
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return user;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid User.");
                    return BadRequest(ModelState);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //[HttpGet]
        //[Route("IsAdmin")]
        //public async Task<ActionResult<List<string>>> IsAdmin()
        //{
        //    var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "UserName").Value.ToLower();
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        return BadRequest("User not found.");
        //    var resul = await _userManager.GetRolesAsync(user);
        //    return Ok(resul);
        //}

        // POST api/Account/SetPasswor

        [HttpPost]
        [Route("SetPassword")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetPassword([FromBody] PassChange Password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ApplicationUser user = null;
            if (Password.Id != null)
            {
                user = _context.Users.FirstOrDefault(x => x.Id == Password.Id);
            }
            else
            {
                var claims = User.Claims.ToList();
                user = await _userManager.FindByEmailAsync(claims.FirstOrDefault(x => x.Type == "Email").Value);
            }
            if (user == null)
            {
                return Ok("User no valid");
            }
            IdentityResult result1 = await _userManager.RemovePasswordAsync(user);
            IdentityResult result = await _userManager.AddPasswordAsync(user, Password.NewPass);

            if (!result.Succeeded)
            {
                return BadRequest(result);
            }

            return Ok();
        }

        //Roles Manayer
        [HttpGet]
        [Route("GetAllRoles")]
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public ActionResult GetAllRol()
        {
            return Ok(_roleManager.Roles.ToList());
        }

        [HttpPut]
        [Route("AddRolByUser/{id}")]
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public async Task<ActionResult> AddRolByUser(string id, [FromBody] IdentityRole role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Ok("User not Found.");
            var resul = _userManager.AddToRoleAsync(user, role.Name);
            if (!resul.Result.Succeeded)
                return Ok("Rol problem.");
            return Ok();
        }

        [HttpPut]
        [Route("RemoveRolByUser/{id}")]
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public async Task<ActionResult> RemoveRolByUser(string id, [FromBody] IdentityRole role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return Ok("User not Found.");
            var resul = _userManager.RemoveFromRoleAsync(user, role.Name);
            if (!resul.Result.Succeeded)
                return Ok("Rol problem.");
            return Ok();
        }

        [Route("Block")]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public async Task<IActionResult> BlockUser([FromBody] UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(userInfo.Email);
                    if (!userInfo.Block)
                    {
                        DateTimeOffset date = DateTimeOffset.Now;
                        date = date.AddYears(10);
                        await _userManager.SetLockoutEndDateAsync(user, date);
                    }
                    else
                    {
                        await _userManager.SetLockoutEndDateAsync(user, null);
                    }
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        //-------AllowAnonymous 
        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                if (!_context.Users.Any(x => x.UserName == _root))
                {
                    var root = new ApplicationUser
                    {
                        UserName = _root,
                        CompanyName = "Root",
                        Email = _root,
                        PhoneNumber = "0"
                    };
                    await _userManager.CreateAsync(root, _configuration["root"]);
                    await _userManager.AddToRoleAsync(root, "Admin");
                }

                var user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    return Ok("User no valid");
                }
                //string a = user.LockoutEnd.ToString();
                //await _userManager.SetLockoutEnabledAsync(user, true);
                //await _userManager.SetLockoutEndDateAsync(user,null);
                var result = await _signInManager.PasswordSignInAsync(user, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.IsLockedOut)
                {
                    return Ok("The User is BLOCK");
                }
                if (result.Succeeded)
                {
                    var rol = await _userManager.GetRolesAsync(user);
                    return BuildToken(user, rol);
                }
                else
                {
                    return Ok("Invalid Password");
                }
            }
            else
            {
                return Ok("Data error");
            }
        }
        
        [HttpPost]
        [Route("ForgotPassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword([FromBody]ForgotPass model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return Ok("That Email not exist in the database.");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user); //BuildTokenPass(user);//  
                //string ppp = Request.Scheme+"://"+ Request.Host.Value+ "/api/Account/ForgotPasswordVAlid?userId=" + user.Id+"&code="+code;
                //var url= Url.Action("fewa","sss", new { userId = user.Id, code = code },ppp, pp);
                string callbackUrl = Url.Action(null, null, new { code = code, email = user.Email }, Request.Scheme);
                EmailService emailService = new EmailService(_configuration);
                string body = "Please change your password by clicking here: <a href=\"" + callbackUrl.Replace("api/Account/ForgotPassword", "forgotPassword") + "\">link</a>";
                emailService.SendEmailSystem(user.Email, "Forgot Password Skyleaeaccess", body);
                return Ok(callbackUrl.Replace("api/Account/ForgotPassword", "forgotPassword"));
            }

            // If we got this far, something failed, redisplay form
            return Ok("Data invalid.");
        }

        [HttpPost]
        [Route("ForgotPasswordValid")]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailValid([FromBody]ForgotPasswordValid model)
        {
            if (model.Email == null || model.Token == null || model.Passwrod == null)
            {
                return Ok("Some data is Empty");
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Ok("That Email not exist in the database.");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Passwrod);
            if (result.Succeeded)
            {
                return Ok(user.Email);
            }

            return Ok("Invalid Token");
        }

        private IActionResult BuildToken(ApplicationUser userInfo, IList<string> rol)
        {
            var claims = new List<Claim>()
            {
                new Claim("UserName", userInfo.Email),
                new Claim("Email", userInfo.Email),
                new Claim("Phone", userInfo.PhoneNumber),
            };
            foreach (var item in rol)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Llave_super_secreta"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(120);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: "skylease.com",
               audience: "skylease.com",
               claims: claims,
               expires: expiration,
               signingCredentials: creds);


            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiration,
                roles = rol
            });

        }

    }
}