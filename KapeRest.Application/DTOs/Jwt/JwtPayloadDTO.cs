using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Jwt
{
    public class JwtPayloadDTO
    {
        public string id { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public IList<string> roles { get; set; }
        public string? cashierId { get; set; }
    }
}
