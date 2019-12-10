using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using qbService.Hubs;
using qbService.Models;

namespace qbService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HubController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ServiceHub> _serviceHub;

        public HubController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IHubContext<ServiceHub> serviceHub)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._configuration = configuration;
            this._serviceHub = serviceHub;

        }
        //from angular
        [HttpGet]
        [Route("GetLisCustomer")]
        public async Task<IActionResult> GetLitsCustomer([FromQuery]string funcion, [FromHeader]string id)
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            //var user = await _userManager.FindByEmailAsync(email);
            IHubUser hubUser = null;
            if (email != null)
            {
               hubUser = HubUser.HubUsers.FirstOrDefault(x => x.Email == email);
            }
            if (hubUser != null)
            {
                string ErrorVar = "Tiempo de esperar Exedido";
                string query = String.Empty;
                switch (funcion)
                {
                    case "getAllCustomer": 
                        {
                            query = $"<?xml version=\"1.0\"?><?qbxml version=\"2.0\"?><QBXML><QBXMLMsgsRq onError = \"continueOnError\"><CustomerQueryRq requestID = \"2\" /></QBXMLMsgsRq ></QBXML>";
                            hubUser.IsEditCustomer = true;
                            await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query);
                            return (TimeWait(hubUser, "IsEditCustomer"))? Ok(hubUser.ListCustomer): Ok(new { Error = ErrorVar });
                        }
                    case "getAllItemInventory":
                        {
                            query = $"<?xml version=\"1.0\"?><?qbxml version=\"8.0\"?><QBXML><QBXMLMsgsRq onError=\"stopOnError\"><ItemInventoryQueryRq requestID = \"1234\" /></QBXMLMsgsRq ></QBXML>";
                            hubUser.IsEditItemInventory = true;
                            await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query);
                            return (TimeWait(hubUser, "IsEditItemInventory")) ? Ok(hubUser.ListItemInventory) : Ok(new { Error = ErrorVar });
                        }
                    case "getAllInvoceByUserID":
                        {
                            if (id!=null)
                            {
                                //80000001-1573153390
                                //80000005-1573165105
                                if (hubUser.IsEditInvoce)
                                {
                                    return Ok(new { Error = "Last Reques no END" });
                                }
                                hubUser.IsEditInvoce = true;
                                query = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"13.0\"?><QBXML><QBXMLMsgsRq onError=\"stopOnError\"><InvoiceQueryRq><EntityFilter><ListID >"+id+"</ListID></EntityFilter></InvoiceQueryRq></QBXMLMsgsRq></QBXML>";
                                await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query);
                                return (TimeWait(hubUser, "IsEditInvoce")) ? Ok(new { Invoces = hubUser.ListInvoce, Id = id }) : Ok(new { Error = ErrorVar });
                            }
                            return Ok(new { Error = "ID = NULL" });
                        }
                    default:
                        return Ok(new { Error = "Funcion "+funcion.ToUpper()+" no valid" });
                }
            }
            return Ok();
        }
        
        private Boolean TimeWait(IHubUser hubUser,string isEdid)
        {
            int count = 0;
            do
            {
                count++;
                if (count > 1000)
                {
                    hubUser[isEdid] = false;
                    return false;
                }
                System.Threading.Thread.Sleep(10);
            } while ((Boolean)hubUser[isEdid] == true);
            return true;
        }

        //from wpf
        [HttpPost]
        [Route("runQueryReturn")]
        public IActionResult RunQueryReturn([FromBody] IHubData response)
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            IHubUser hubUser = null;
            if (email != null)
            {
                hubUser = HubUser.HubUsers.FirstOrDefault(x => x.Email == email);
            }
            if (hubUser != null)
            {
                StringReader stringReader = new StringReader(response.data);
                //getAllCustomer
                if (response.data.IndexOf("CustomerQueryRs") != -1)
                {
                    hubUser.ListCustomer = new List<IQbCustomer>();
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Models.Customer.QBXML));
                    Models.Customer.QBXML awb = (Models.Customer.QBXML)xmlSerializer.Deserialize(stringReader);
                    foreach (var item in awb.QBXMLMsgsRs.CustomerQueryRs.CustomerRet)
                    {
                        hubUser.ListCustomer.Add(new IQbCustomer { 
                            ListID = item.ListID, 
                            Name = item.Name, 
                            FullName = item.FullName });
                    }
                    hubUser.IsEditCustomer = false;
                }
                //getAllItemInventory
                if (response.data.IndexOf("ItemInventoryQueryRs") != -1)
                {
                    hubUser.ListItemInventory = new List<IQbItemInventory>();
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Models.ItemInventory.QBXML));
                    Models.ItemInventory.QBXML awb = (Models.ItemInventory.QBXML)xmlSerializer.Deserialize(stringReader);
                    foreach (var item in awb.QBXMLMsgsRs.ItemInventoryQueryRs.ItemInventoryRet)
                    {
                        hubUser.ListItemInventory.Add(new IQbItemInventory { ListID = item.ListID, Name = item.Name, FullName = item.FullName });
                    }
                    hubUser.IsEditItemInventory = false;
                }
                //getAllInvoceByUserID
                if (response.data.IndexOf("InvoiceQueryRs") != -1)
                {
                    hubUser.ListInvoce = new List<IQbInvoce>();
                    if (response.data.IndexOf("statusCode=\"0\"") == -1)
                    {
                        hubUser.IsEditInvoce = false;
                        return Ok();
                    }
                    try
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Models.Invoce.QBXML));
                        Models.Invoce.QBXML awb = (Models.Invoce.QBXML)xmlSerializer.Deserialize(stringReader);
                        foreach (var item in awb.QBXMLMsgsRs.InvoiceQueryRs.InvoiceRet)
                        {
                            hubUser.ListInvoce.Add(new IQbInvoce
                            {
                                TxnID = item.TxnID,
                                TxnDate = item.TxnDate,
                                DueDate = item.DueDate,
                                BalanceRemaining = item.BalanceRemaining,
                                IsPaid = item.IsPaid,
                                RefNumber = item.RefNumber,
                                Subtotal = item.Subtotal
                            });
                        }
                        hubUser.IsEditInvoce = false;
                    }
                    catch (Exception e)
                    {
                        return Ok(e);
                    }
                }
            }
            return Ok();
        }

        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] IHubUser userInfo)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userInfo.Email);
                if (user == null)
                {
                    return Ok("User no valid");
                }
                var result = await _signInManager.PasswordSignInAsync(user, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.IsLockedOut)
                {
                    return Ok("The User is BLOCK");
                }
                if (result.Succeeded)
                {
                    HubUser.HubUsers.RemoveAll(x => x.Email == userInfo.Email);
                    HubUser.HubUsers.Add(new IHubUser() { ConectionId = userInfo.ConectionId, Email = userInfo.Email });
                    return BuildToken(user);
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
        private IActionResult BuildToken(ApplicationUser userInfo)
        {
            var claims = new List<Claim>()
            {
                new Claim("Email", userInfo.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Llave_super_secreta"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(24);

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
            });

        }
    }
}