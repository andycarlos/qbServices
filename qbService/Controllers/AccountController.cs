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
using Microsoft.EntityFrameworkCore;
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
        //private string _root = "root@skylease.aero";
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
        [Authorize(Roles = "Admin, QbAdmin")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllUsers()
        {
            //User.Claims.ToList().FirstOrDefault(x => x.Type == "EmailMain").Value.ToLower() ==
            //    User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower()

            List<UserInfo> users = new List<UserInfo>();
            bool rolAdmin = User.Claims.Where(x => x.Type == ClaimTypes.Role).Any(y => y.Value == "Admin");
            if (!rolAdmin)
            {
                var companyName = User.Claims.ToList().FirstOrDefault(x => x.Type == "CompanyName").Value.ToLower();
                users = _userManager.Users.Where(y => y.TypeUser != null && y.CompanyName == companyName).Select(x => new UserInfo
                {
                    Id = x.Id,
                    Email = x.Email,
                    CompanyName = x.CompanyName,
                    Phone = x.PhoneNumber,
                    Name = x.Name,
                    TypeUser = x.TypeUser
                    //Block = (x.LockoutEnd.ToString() != null) ? false : true
                }).ToList();
                //if (users.Count == 0)
                //{
                //    return Ok();
                //}
            }
            else
            {
                users = _userManager.Users.Select(x => new UserInfo
                {
                    Id = x.Id,
                    Email = x.Email,
                    CompanyName = x.CompanyName,
                    Phone = x.PhoneNumber,
                    Block = (x.LockoutEnd.ToString() != null) ? false : true
                }).ToList();
                //if (users.Count == 0)
                //{
                //    return Ok();
                //}
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
            return  users;
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

        //Use to valid Submit
        [Route("AnyCompanyName")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult<bool> AnyCompanyName([FromQuery] string CompanyName)
        {
            var result = _context.Users.Any(x => x.CompanyName == CompanyName);
            return result;
        }

        //Faltan lo EMAIL
        [Route("Create")]
        [HttpPost]
        [Authorize(Roles = "Admin, QbAdmin")]
        public async Task<IActionResult> CreateUser([FromBody] UserInfo userInfo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new ApplicationUser
                    {
                        UserName = userInfo.Email,
                        CompanyName = (userInfo.CompanyName != "" && userInfo.CompanyName != null) ? userInfo.CompanyName : User.Claims.ToList().FirstOrDefault(x => x.Type == "CompanyName").Value.ToLower(),
                        Email = userInfo.Email,
                        PhoneNumber = userInfo.Phone,
                        CreateTimer = DateTime.Now,
                        TypeUser =  userInfo.TypeUser ,
                        Name = userInfo.Name,
                        ListID =  userInfo.ListID,
                    };
                    string pass = userInfo.Password;
                    if (userInfo.Password == "" || userInfo.Password == null)
                    {
                        pass = Guid.NewGuid().ToString();
                    }
                    var result = await _userManager.CreateAsync(user, pass);
                    if (result.Succeeded )
                    {
                        try
                        {
                            ////Cambiar
                            //if (userInfo.Email == "acalfonso86@gmail.com")
                            //{
                                await _userManager.AddToRoleAsync(user, "QbAdmin");
                                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                                string callbackUrl = Url.Action(null, null, new { code = code, email = user.Email }, Request.Scheme);
                                EmailService emailService = new EmailService(_configuration);
                                string body = "Please clicking here to activate your account: <a href=\"" + callbackUrl.Replace("api/Account/Create", "forgotPassword") + "\">link</a>";
                                emailService.SendEmailSystem(user.Email, user.Name + " Activate account", body);
                            //}
                            return Ok();
                        }
                        catch (Exception e)
                        {
                            return BadRequest(e.Message);
                        }
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
        [Authorize(Roles = "Admin, QbAdmin")]
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
        [Authorize(Roles = "Admin, QbAdmin")]
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
                return BadRequest("User no valid");
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
        public ActionResult GetAllRol()
        {
            return Ok(_roleManager.Roles.ToList());
        }

        [HttpGet]
        [Route("GetAllRolesUser")]
        [Authorize(Roles = "Admin, QbAdmin")]
        public async Task<ActionResult> GetAllRolesUser()
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "EmailMain").Value.ToLower();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("User not Found.");
            var roles = await _userManager.GetRolesAsync(user);
            return Ok( _roleManager.Roles.ToList().Where(x=> roles.Contains(x.Name)).ToList() );
        }

        [HttpPost]
        [Route("AddRolByUser/{id}")]
        [Authorize(Roles = "Admin, QbAdmin")]
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

        [HttpPost]
        [Route("RemoveRolByUser/{id}")]
        [Authorize(Roles = "Admin, QbAdmin")]
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
                if (!_context.Users.Any(x => x.UserName == _configuration["email"]))
                {
                    var root = new ApplicationUser
                    {
                        UserName = _configuration["email"],
                        CompanyName = "Root",
                        Email = _configuration["email"],
                        PhoneNumber = "0",
                        CreateTimer = DateTime.Now
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
                string url = callbackUrl.Replace("api/Account/ForgotPassword", "forgotPassword");
                string body = Body(url);// "Please change your password by clicking here: <a href=\"" + callbackUrl.Replace("api/Account/ForgotPassword", "forgotPassword") + "\">link</a>";
                emailService.SendEmailSystem(user.Email, "Forgot Password QuickBoos Services Account", body);
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
                new Claim("Email", userInfo.Email),
                new Claim("CompanyName", userInfo.CompanyName),
            };

            if (userInfo.TypeUser!=null)
            {
                //var companyName = User.Claims.ToList().FirstOrDefault(x => x.Type == "CompanyName").Value.ToLower();
                var user =  _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.CompanyName == userInfo.CompanyName && x.TypeUser == null).Result;
                if (user != null)
                {
                    claims.Add(new Claim("EmailMain", user.Email));
                }
            }
            else
                claims.Add(new Claim("EmailMain", userInfo.Email));

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
                roles = rol,
                type = userInfo.TypeUser,
                listID = userInfo.ListID,
            });

        }


        private string Body(string url)
        {
            string body = @"
<!doctype html>
<html style=""width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;"">
 <head> 
  <meta charset=""UTF-8""> 
  <meta content=""width=device-width, initial-scale=1"" name=""viewport""> 
  <meta name=""x-apple-disable-message-reformatting""> 
  <meta http-equiv=""X-UA-Compatible"" content=""IE=edge""> 
  <meta content=""telephone=no"" name=""format-detection""> 
  <title>Nuevo correo electrónico</title> 
  <!--[if (mso 16)]>
    <style type=""text/css"">
    a {text-decoration: none;}
    </style>
    <![endif]--> 
  <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]--> 
  <style type=""text/css"">
@media only screen and (max-width:600px) {p, ul li, ol li, a { font-size:16px!important; line-height:150%!important } h1 { font-size:30px!important; text-align:center; line-height:120%!important } h2 { font-size:26px!important; text-align:center; line-height:120%!important } h3 { font-size:20px!important; text-align:center; line-height:120%!important } h1 a { font-size:30px!important } h2 a { font-size:26px!important } h3 a { font-size:20px!important } .es-menu td a { font-size:16px!important } .es-header-body p, .es-header-body ul li, .es-header-body ol li, .es-header-body a { font-size:16px!important } .es-footer-body p, .es-footer-body ul li, .es-footer-body ol li, .es-footer-body a { font-size:16px!important } .es-infoblock p, .es-infoblock ul li, .es-infoblock ol li, .es-infoblock a { font-size:12px!important } *[class=""gmail-fix""] { display:none!important } .es-m-txt-c, .es-m-txt-c h1, .es-m-txt-c h2, .es-m-txt-c h3 { text-align:center!important } .es-m-txt-r, .es-m-txt-r h1, .es-m-txt-r h2, .es-m-txt-r h3 { text-align:right!important } .es-m-txt-l, .es-m-txt-l h1, .es-m-txt-l h2, .es-m-txt-l h3 { text-align:left!important } .es-m-txt-r img, .es-m-txt-c img, .es-m-txt-l img { display:inline!important } .es-button-border { display:block!important } a.es-button { font-size:20px!important; display:block!important; border-left-width:0px!important; border-right-width:0px!important } .es-btn-fw { border-width:10px 0px!important; text-align:center!important } .es-adaptive table, .es-btn-fw, .es-btn-fw-brdr, .es-left, .es-right { width:100%!important } .es-content table, .es-header table, .es-footer table, .es-content, .es-footer, .es-header { width:100%!important; max-width:600px!important } .es-adapt-td { display:block!important; width:100%!important } .adapt-img { width:100%!important; height:auto!important } .es-m-p0 { padding:0px!important } .es-m-p0r { padding-right:0px!important } .es-m-p0l { padding-left:0px!important } .es-m-p0t { padding-top:0px!important } .es-m-p0b { padding-bottom:0!important } .es-m-p20b { padding-bottom:20px!important } .es-mobile-hidden, .es-hidden { display:none!important } .es-desk-hidden { display:table-row!important; width:auto!important; overflow:visible!important; float:none!important; max-height:inherit!important; line-height:inherit!important } .es-desk-menu-hidden { display:table-cell!important } table.es-table-not-adapt, .esd-block-html table { width:auto!important } table.es-social { display:inline-block!important } table.es-social td { display:inline-block!important } }
#outlook a {
	padding:0;
}
.ExternalClass {
	width:100%;
}
.ExternalClass,
.ExternalClass p,
.ExternalClass span,
.ExternalClass font,
.ExternalClass td,
.ExternalClass div {
	line-height:100%;
}
.es-button {
	mso-style-priority:100!important;
	text-decoration:none!important;
}
a[x-apple-data-detectors] {
	color:inherit!important;
	text-decoration:none!important;
	font-size:inherit!important;
	font-family:inherit!important;
	font-weight:inherit!important;
	line-height:inherit!important;
}
.es-desk-hidden {
	display:none;
	float:left;
	overflow:hidden;
	width:0;
	max-height:0;
	line-height:0;
	mso-hide:all;
}
</style> 
 </head> 
 <body style=""width:100%;font-family:arial, 'helvetica neue', helvetica, sans-serif;-webkit-text-size-adjust:100%;-ms-text-size-adjust:100%;padding:0;Margin:0;""> 
  <div class=""es-wrapper-color"" style=""background-color:#F6F6F6;""> 
   <!--[if gte mso 9]>
			<v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t"">
				<v:fill type=""tile"" color=""#f6f6f6""></v:fill>
			</v:background>
		<![endif]--> 
   <table cellpadding=""0"" cellspacing=""0"" class=""es-wrapper"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;padding:0;Margin:0;width:100%;height:100%;background-repeat:repeat;background-position:center top;""> 
     <tr style=""border-collapse:collapse;""> 
      <td valign=""top"" style=""padding:0;Margin:0;""> 
       <table cellpadding=""0"" cellspacing=""0"" class=""es-content"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;""> 
         <tr style=""border-collapse:collapse;""> 
          <td align=""center"" bgcolor=""transparent"" style=""padding:0;Margin:0;background-color:transparent;""> 
           <table bgcolor=""#b1807e"" class=""es-content-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#B1807E;""> 
             <tr style=""border-collapse:collapse;""> 
              <td align=""left"" bgcolor=""#01B5F0"" style=""padding:0;Margin:0;padding-bottom:5px;padding-left:20px;padding-right:20px;background-color:#01B5F0;""> 
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> 
                 <tr style=""border-collapse:collapse;""> 
                  <td width=""560"" align=""center"" valign=""top"" style=""padding:0;Margin:0;""> 
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:17px;"" role=""presentation""> 
                     <tr style=""border-collapse:collapse;""> 
                      <td align=""center"" height=""7"" style=""padding:0;Margin:0;""></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table> 
       <table cellpadding=""0"" cellspacing=""0"" class=""es-content"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;""> 
         <tr style=""border-collapse:collapse;""> 
          <td align=""center"" style=""padding:0;Margin:0;""> 
           <table bgcolor=""#ffffff"" class=""es-content-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#FFFFFF;""> 
             <tr style=""border-collapse:collapse;""> 
              <td align=""left"" bgcolor=""black"" style=""Margin:0;padding-top:20px;padding-bottom:20px;padding-left:20px;padding-right:20px;background-color:#2B323C;""> 
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> 
                 <tr style=""border-collapse:collapse;""> 
                  <td width=""560"" align=""center"" valign=""top"" style=""padding:0;Margin:0;""> 
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" role=""presentation"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> 
                     <tr style=""border-collapse:collapse;""> 
                      <td align=""center"" class=""es-m-txt-c"" bgcolor=""transparent"" style=""padding:5px;Margin:0;""><h3 style=""Margin:0;line-height:24px;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:20px;font-style:normal;font-weight:normal;color:#FFFFFF;"">PROFESSIONAL</h3></td> 
                     </tr> 
                     <tr style=""border-collapse:collapse;""> 
                      <td align=""center"" style=""padding:0;Margin:0;""><h1 style=""Margin:0;line-height:46px;mso-line-height-rule:exactly;font-family:helvetica, 'helvetica neue', arial, verdana, sans-serif;font-size:38px;font-style:normal;font-weight:normal;color:#01B5F0;""><strong>QuickBooks Services</strong></h1></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table> 
       <table cellpadding=""0"" cellspacing=""0"" class=""es-content"" align=""center"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;table-layout:fixed !important;width:100%;""> 
         <tr style=""border-collapse:collapse;""> 
          <td align=""center"" bgcolor=""transparent"" style=""padding:0;Margin:0;background-color:transparent;""> 
           <table bgcolor=""#b1807e"" class=""es-content-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;background-color:#B1807E;""> 
             <tr style=""border-collapse:collapse;""> 
              <td align=""left"" bgcolor=""#01B5F0"" style=""padding:0;Margin:0;padding-top:40px;padding-bottom:40px;background-color:#01B5F0;""> 
               <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:collapse;border-spacing:0px;""> 
                 <tr style=""border-collapse:collapse;""> 
                  <td width=""600"" align=""center"" valign=""top"" style=""padding:0;Margin:0;""> 
                   <table cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""mso-table-lspace:0pt;mso-table-rspace:0pt;border-collapse:separate;border-spacing:0px;border-radius:17px;"" role=""presentation""> 
                     <tr style=""border-collapse:collapse;""> 
                      <td align=""center"" style=""padding:0;Margin:0;""><h4 style=""Margin:0;line-height:19px;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;color:#EFEFEF;font-size:16px;"">Please clicking here to activate your account</h4></td> 
                     </tr> 
                     <tr style=""border-collapse:collapse;""> 
                      <td align=""center"" height=""7"" style=""padding:0;Margin:0;""></td> 
                     </tr> 
                     <tr style=""border-collapse:collapse;""> 
                      <td align=""center"" style=""padding:10px;Margin:0;""><span class=""es-button-border"" style=""border-style:solid;border-color:#2061CD;background:#0B5394;border-width:2px;display:inline-block;border-radius:30px;width:auto;""><a href="""+url+@""" class=""es-button"" target=""_blank"" style=""mso-style-priority:100 !important;text-decoration:none;-webkit-text-size-adjust:none;-ms-text-size-adjust:none;mso-line-height-rule:exactly;font-family:arial, 'helvetica neue', helvetica, sans-serif;font-size:18px;color:#FFFFFF;border-style:solid;border-color:#0B5394;border-width:5px 20px;display:inline-block;background:#0B5394;border-radius:30px;font-weight:normal;font-style:normal;line-height:22px;width:auto;text-align:center;"">Activar Account</a></span></td> 
                     </tr> 
                   </table></td> 
                 </tr> 
               </table></td> 
             </tr> 
           </table></td> 
         </tr> 
       </table></td> 
     </tr> 
   </table> 
  </div>  
 </body>
</html>
";

            return body;
        }
    }
}