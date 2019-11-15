using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace qbService.Models
{
    public class UserInfo
    {
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
    }
    public class PassChange
    {
        public string Id { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
    }
    public class IRole
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        public string ConcurrencyStamp { get; set; }
    }
    public class ForgotPasswordValid
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Passwrod { get; set; }
    }
    public class ForgotPass
    {
        public string Email { get; set; }
    }
}
