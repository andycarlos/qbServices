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
using Newtonsoft.Json;
using qbService.Hubs;
using qbService.Models;
using qbService.Services;

namespace qbService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QbController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<ServiceHub> _serviceHub;

        string ErrorTimer = "Timeout Exceeded";
        string ErrorDestOff = "Desktop APP is unlogin";
        string ErrorRequestNoEnd = "Last Requet no END";
        string Token = String.Empty;
        public QbController(
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

        [HttpGet]
        [Route("getCode")]
        public IActionResult GetCode([FromQuery] string name)
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            if (email != null)
            {
                Random random = new Random();
                int code = random.Next(100000, 999999);
                EmailService emailService = new EmailService(_configuration);
                string subject = "Code for Customer: " + name;
                string body = "<p class=MsoNormal align=center style='text-align:center'><span style = 'font-size:28.0pt'> Code: " + code + "</span></p>";
                emailService.SendEmailSystem(email, subject, body);
                return Ok(new { Code = code });
             }
            return BadRequest(new { Error = "User invalid" });
        }

        private Boolean TimeWait(IHubUser hubUser)
        {
            int count = 0;
            int count1 = 0;
            string tempToken = string.Empty;
            while (hubUser.Token.Count == 0 || (hubUser.Token.Count > 0 && (hubUser.Token[0] != Token && hubUser.Token[0] != Token+ ":=>Activado")))
            {
                count++;
                count1++;
                //selecionar el nuevo token a validar
                if (hubUser.Token.Count > 0 && hubUser.Token[0] != tempToken)
                {
                    count1 = 0;
                    tempToken = hubUser.Token[0];
                }
                //elimina el token por problemas de error
                if (hubUser.Token.Count > 0 && count1 > 200 && hubUser.Token[0] == tempToken)
                {
                    count1 = 0;
                    count = 0;
                    hubUser.Token.RemoveAt(0);
                }
                //tiempo de esperar exedido
                if (count > 3000)
                    return false;
                System.Threading.Thread.Sleep(10);
            }

            count = 0;
            while (hubUser.Token[0] != Token + ":=>Activado")
            {
                count++;
                if (count > 200)
                    return false;
                System.Threading.Thread.Sleep(10);
            }
            return true;
        }

        public bool HasProperty(dynamic obj,string name)
        {
            try
            {
                var value = obj[name];
                if(value!=null)
                    return true;
                return false;
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }
        private void FillItemsService(IHubUser hubUser, dynamic item, string type)//ItemServiceRet-ItemNonInventoryRet-ItemOtherChargeRet
        {
            var price = (HasProperty(item, "SalesOrPurchase") && HasProperty(item.SalesOrPurchase, "Price")) ? item.SalesOrPurchase.Price : 0.0;//evitar double NULL
            var desc = (HasProperty(item, "SalesOrPurchase") && HasProperty(item.SalesOrPurchase, "Desc")) ? item.SalesOrPurchase.Desc : "";//evitar double NULL
            if (item.IsActive == "true")
            {
                hubUser.ListItems.Add(new IQbItem
                {
                    ListID = item.ListID,
                    Name = item.Name,
                    FullName = item.FullName,
                    SalesDesc = desc,
                    SalesPrice = price,
                    Type = type
                });
            }
        }
        private void FillItemsInventory(IHubUser hubUser, dynamic item, string type)//ItemInventoryRet
        {
           
            var price = (HasProperty(item, "SalesPrice") ) ? item.SalesPrice : 0.0;//evitar double NULL
            if (item.IsActive == "true")
            {
                hubUser.ListItems.Add(new IQbItem
                {
                    ListID = item.ListID,
                    Name = item.Name,
                    FullName = item.FullName,
                    SalesDesc = item.SalesDesc,
                    SalesPrice = price,
                    Type = type
                });
            }
        }
        private void FillItemsFixedAsset(IHubUser hubUser, dynamic item, string type)//ItemFixedAssetRet
        {
            var price = (HasProperty(item, "FixedAssetSalesInfo") && HasProperty(item.FixedAssetSalesInfo, "SalesPrice")) ? item.SalesPrice : 0.0;//evitar double NULL
            var desc = (HasProperty(item, "FixedAssetSalesInfo") && HasProperty(item.FixedAssetSalesInfo, "SalesDesc")) ? item.SalesDesc: "";//evitar double NULL
            if (item.IsActive == "true")
            {
                hubUser.ListItems.Add(new IQbItem
                {
                    ListID = item.ListID,
                    Name = item.Name,
                    FullName = item.Name,
                    SalesDesc = desc,
                    SalesPrice = price,
                    Type = type
                });
            }
        }
        
        [HttpPost]
        [Route("RequestReturn")]
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
                hubUser.Token.Add(response.Token);
                while (hubUser.Token[0] != response.Token)
                { System.Threading.Thread.Sleep(10); }

                //getAllCustomer------------------------------------------
                if (response.Body.IndexOf("CustomerQueryRs") != -1)
                {
                    hubUser.ListCustomer = new List<IQbCustomer>();
                    var data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                    foreach (dynamic item in data.QBXML.QBXMLMsgsRs.CustomerQueryRs.CustomerRet)
                    {
                        hubUser.ListCustomer.Add(new IQbCustomer
                        {
                            ListID = item.ListID,
                            Name = item.Name,
                            FullName = item.FullName,
                            CreditLimit = (HasProperty(item, "CreditLimit")) ? item.CreditLimit : 0
                        }); ;
                    }
                }

                //getInvoce---------------------------------------------
                if (response.Body.IndexOf("InvoiceQueryRs") != -1)
                {
                    hubUser.ListInvoce = new List<IQbInvoce>();
                    if (response.Body.IndexOf("statusCode\":\"0\"") == -1)
                    {
                        hubUser.Token[0] += ":=>Activado";
                        return Ok();
                    }
                    try
                    {
                        var data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                        if (data.QBXML.QBXMLMsgsRs.InvoiceQueryRs.InvoiceRet is Newtonsoft.Json.Linq.JArray)
                        { 
                            foreach (dynamic item in data.QBXML.QBXMLMsgsRs.InvoiceQueryRs.InvoiceRet)
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
                        }
                        if (data.QBXML.QBXMLMsgsRs.InvoiceQueryRs.InvoiceRet is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic item = data.QBXML.QBXMLMsgsRs.InvoiceQueryRs.InvoiceRet;
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
                    }
                    catch (Exception e)
                    {
                        hubUser.Token.RemoveAt(0);
                        return Ok(e.Message);
                    }
                }

                //getItemsInventory---------------------------------------------
                if (response.Body.IndexOf("ItemQueryRs") != -1)
                {
                    hubUser.ListItems = new List<IQbItem>();
                    if (response.Body.IndexOf("statusCode\":\"0\"") == -1)
                    {
                        hubUser.Token[0] += ":=>Activado";
                        return Ok();
                    }
                    try
                    {
                        var data = JsonConvert.DeserializeObject<dynamic>(response.Body);

                        #region ItemServiceRet
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemServiceRet is Newtonsoft.Json.Linq.JArray)
                        {
                            foreach (dynamic item in data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemServiceRet)
                            {
                                FillItemsService(hubUser, item, "Service");
                            }
                        }
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemServiceRet is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic item = data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemServiceRet;
                            FillItemsService(hubUser, item, "Service");
                        }
                        #endregion
                        #region ItemNonInventoryRet
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemNonInventoryRet is Newtonsoft.Json.Linq.JArray)
                        {
                            foreach (dynamic item in data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemNonInventoryRet)
                            {
                                FillItemsService(hubUser, item, "NonInventory");
                            }
                        }
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemNonInventoryRet is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic item = data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemNonInventoryRet;
                            FillItemsService(hubUser, item, "NonInventory");
                        }
                        #endregion
                        #region ItemOtherChargeRet
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemOtherChargeRet is Newtonsoft.Json.Linq.JArray)
                        {
                            foreach (dynamic item in data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemOtherChargeRet)
                            {
                                FillItemsService(hubUser, item, "OtherCharge");
                            }
                        }
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemOtherChargeRet is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic item = data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemOtherChargeRet;
                            FillItemsService(hubUser, item, "OtherCharge");
                        }
                        #endregion
                        #region ItemInventoryRet
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemInventoryRet is Newtonsoft.Json.Linq.JArray)
                        {
                            foreach (dynamic item in data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemInventoryRet)
                            {
                                FillItemsInventory(hubUser, item, "Inventory");
                            }
                        }
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemInventoryRet is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic item = data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemInventoryRet;
                            FillItemsInventory(hubUser, item, "Inventory");
                        }
                        #endregion
                        #region ItemFixedAssetRet
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemFixedAssetRet is Newtonsoft.Json.Linq.JArray)
                        {
                            foreach (dynamic item in data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemFixedAssetRet)
                            {
                                FillItemsFixedAsset(hubUser, item, "FixedAsset");
                            }
                        }
                        if (data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemFixedAssetRet is Newtonsoft.Json.Linq.JObject)
                        {
                            dynamic item = data.QBXML.QBXMLMsgsRs.ItemQueryRs.ItemFixedAssetRet;
                            FillItemsFixedAsset(hubUser, item, "FixedAsset");
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        hubUser.Token.RemoveAt(0);
                        return Ok(e.Message);
                    }
                }

                //createSaleOrder
                if (response.Body.IndexOf("SalesOrderAddRs") != -1)
                {
                    if (response.Body.IndexOf("statusCode\":\"0\"") == -1)
                    {
                        var data = JsonConvert.DeserializeObject<dynamic>(response.Body);
                        hubUser.StatusMessage = data.QBXML.QBXMLMsgsRs.SalesOrderAddRs["@statusMessage"];
                        hubUser.StatusCode = false;
                        hubUser.Token[0] += ":=>Activado";
                        return Ok();
                    }
                    else
                        hubUser.StatusCode = true;
                }
            }
            hubUser.Token[0] += ":=>Activado";
            return Ok();
        }

        [HttpGet]
        [Route("getAllCustomers")]
        public async Task<IActionResult> getAllCustomer()
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            IHubUser hubUser = null;
            if (email != null)
            {
                hubUser = HubUser.HubUsers.FirstOrDefault(x => x.Email == email);
            }
            if (hubUser != null)
            {
                string query = $"<?xml version=\"1.0\"?><?qbxml version=\"2.0\"?><QBXML><QBXMLMsgsRq onError = \"continueOnError\"><CustomerQueryRq requestID = \"2\" /></QBXMLMsgsRq ></QBXML>";
                Token = Guid.NewGuid().ToString();
                await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query, Token);
                if (TimeWait(hubUser))
                {
                    List<IQbCustomer> resul = new List<IQbCustomer>();
                    resul.AddRange(hubUser.ListCustomer);
                    if (hubUser.Token[0] == Token + ":=>Activado")
                        hubUser.Token.RemoveAt(0);
                    return Ok(resul);
                }
                else
                    return BadRequest(new { Error = ErrorTimer });
            }
            return BadRequest(new { Error = ErrorDestOff });
        }
        [HttpGet]
        [Route("getItems")]
        public async Task<IActionResult> getItemsInventory()
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            IHubUser hubUser = null;
            if (email != null)
            {
                hubUser = HubUser.HubUsers.FirstOrDefault(x => x.Email == email);
            }
            if (hubUser != null)
            {
                string query = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"13.0\"?><QBXML><QBXMLMsgsRq onError=\"stopOnError\"><ItemQueryRq><ActiveStatus>ActiveOnly</ActiveStatus></ItemQueryRq></QBXMLMsgsRq></QBXML> ";
                Token = Guid.NewGuid().ToString();
                await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query, Token);
                if (TimeWait(hubUser))
                {
                    List<IQbItem> resul = new List<IQbItem>();
                    resul.AddRange(hubUser.ListItems);
                    if (hubUser.Token[0] == Token + ":=>Activado")
                        hubUser.Token.RemoveAt(0);
                    return Ok(resul);
                }
                else
                    return BadRequest(new { Error = ErrorTimer });
            }
            return BadRequest(new { Error = ErrorDestOff });
        }

        [HttpPost]
        [Route("getInvoces")]
        public async Task<IActionResult> getInvoce([FromBody]IQbInvoceFilter filter)
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            IHubUser hubUser = null;
            if (email != null)
            {
                hubUser = HubUser.HubUsers.FirstOrDefault(x => x.Email == email);
            }
            if (hubUser != null)
            {
                string customerID = String.Empty;
                if(filter.CustomerID!=String.Empty)
                   customerID ="<EntityFilter><ListID >" + filter.CustomerID + "</ListID></EntityFilter>";

                string paidStatus = String.Empty;
                if (filter.PaidStatus != String.Empty)
                    paidStatus = "<PaidStatus>"+ filter.PaidStatus + "</PaidStatus>";
                
                string query = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"13.0\"?><QBXML><QBXMLMsgsRq onError=\"stopOnError\"><InvoiceQueryRq>"+customerID+paidStatus+"</InvoiceQueryRq></QBXMLMsgsRq></QBXML>";
                Token = Guid.NewGuid().ToString();
                await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query, Token);
                if (TimeWait(hubUser))
                {
                    if (filter.Overdue)
                        hubUser.ListInvoce = hubUser.ListInvoce.Where(x => x.DueDate.Date < DateTime.Now.Date).ToList();

                    List<IQbInvoce> resul = new List<IQbInvoce>();
                    resul.AddRange(hubUser.ListInvoce);
                    if (hubUser.Token[0] == Token + ":=>Activado")
                        hubUser.Token.RemoveAt(0);
                    return Ok(new { Invoces = resul, Id = filter.CustomerID , DateNow = DateTime.Now.Date });
                }
                else
                    return BadRequest(new { Error = ErrorTimer });
            }
            return BadRequest(new { Error = ErrorDestOff });
        }

        [HttpPost]
        [Route("CreateSaleOrder")]
        public async Task<IActionResult> CreateSaleOrder([FromBody]IQbSaleOrder body)
        {
            var email = User.Claims.ToList().FirstOrDefault(x => x.Type == "Email").Value.ToLower();
            IHubUser hubUser = null;
            if (email != null)
            {
                hubUser = HubUser.HubUsers.FirstOrDefault(x => x.Email == email);
            }
            if (hubUser != null)
            {
                string customerID = String.Empty;
                if (body.CustomerRefListID != String.Empty)
                    customerID = "<CustomerRef><ListID>" + body.CustomerRefListID + "</ListID></CustomerRef>";

                string SalesOrderLineAdd = String.Empty;
                if (body.SalesOrderLineAdd != null)
                {
                    foreach (var item in body.SalesOrderLineAdd)
                    {
                        SalesOrderLineAdd += "<SalesOrderLineAdd><ItemRef><ListID>" + item.ItemRefListID + "</ListID></ItemRef>" + "<Quantity>" + item.Quantity + "</Quantity></SalesOrderLineAdd>";
                    }
                }

                string query = $"<?xml version=\"1.0\" encoding=\"utf-8\"?><?qbxml version=\"13.0\"?><QBXML><QBXMLMsgsRq onError=\"stopOnError\"><SalesOrderAddRq><SalesOrderAdd>" + customerID + SalesOrderLineAdd + "</SalesOrderAdd></SalesOrderAddRq></QBXMLMsgsRq></QBXML>";
                Token = Guid.NewGuid().ToString();
                await _serviceHub.Clients.Client(hubUser.ConectionId).SendAsync("runQuery", query, Token);
                if (TimeWait(hubUser))
                {
                    bool statu = (hubUser.StatusCode)?true:false;
                    if (hubUser.Token[0] == Token + ":=>Activado")
                        hubUser.Token.RemoveAt(0);
                    if (statu)
                        return Ok();
                    else
                        return BadRequest(new { Error = hubUser.StatusMessage });
                }
                else
                    return BadRequest(new { Error = ErrorTimer });
            }
            return BadRequest(new { Error = ErrorDestOff });
        }
    }
}