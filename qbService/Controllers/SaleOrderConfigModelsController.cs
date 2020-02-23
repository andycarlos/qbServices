using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qbService.Models;

namespace qbService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleOrderConfigModelsController : ControllerBase
    {
        private readonly ApplicationDbContex _context;

        public SaleOrderConfigModelsController(ApplicationDbContex context)
        {
            _context = context;
        }

        // GET: api/SaleOrderConfigModels
        [HttpGet]
        public async Task<ActionResult> GetSaleOrderConfigModel()
        {
            var emailMain = User.Claims.ToList().FirstOrDefault(x => x.Type == "EmailMain").Value.ToLower();
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == emailMain);
            if (user != null)
            {
                var saleOrderConfig = await _context.SaleOrderConfigModel.AsNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
                if (saleOrderConfig != null)
                    return Ok( new { CreditLimit = saleOrderConfig.CreditLimit, DaysNextDueDate = saleOrderConfig.DaysNextDueDate, InvocesDueDate = saleOrderConfig.InvocesDueDate });
                else
                    return Ok(new { CreditLimit = false, DaysNextDueDate = 0, InvocesDueDate = 0 });
            }
            return BadRequest(new { Error = "User equal NULL"});
        }
        [HttpPost]
        public async Task<ActionResult> PostSaleOrderConfigModel(SaleOrderConfigModel1 saleOrderConfigModel)
        {
            var emailMain = User.Claims.ToList().FirstOrDefault(x => x.Type == "EmailMain").Value.ToLower();
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Email == emailMain);
            if (user != null)
            {
                try
                {
                    var saleOrderConfig = await _context.SaleOrderConfigModel.AsNoTracking().FirstOrDefaultAsync(x => x.ApplicationUserId == user.Id);
                    if (saleOrderConfig != null)
                    {
                        saleOrderConfig.CreditLimit = saleOrderConfigModel.CreditLimit;
                        saleOrderConfig.DaysNextDueDate = saleOrderConfigModel.DaysNextDueDate;
                        saleOrderConfig.InvocesDueDate = saleOrderConfigModel.InvocesDueDate;
                        _context.Entry(saleOrderConfig).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        //_context.SaleOrderConfigModel.AsNoTracking();
                        SaleOrderConfigModel saleOrderConfigTemp = new SaleOrderConfigModel()
                        {
                            CreditLimit = saleOrderConfigModel.CreditLimit,
                            DaysNextDueDate = saleOrderConfigModel.DaysNextDueDate,
                            InvocesDueDate = saleOrderConfigModel.InvocesDueDate,
                            ApplicationUserId = user.Id
                        };
                        _context.SaleOrderConfigModel.Add(saleOrderConfigTemp);
                        await _context.SaveChangesAsync();
                    }
                    return Ok();
                }
                catch (Exception e)
                {
                    return BadRequest(e);
                }
            }
            return BadRequest(new { Error = "User equal NULL"});
        }
    }
}
