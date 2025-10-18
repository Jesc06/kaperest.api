using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Jwt
{
    public class JwtRefreshRequestDTO
    {
        public string Token { get; set; }    
        public string RefreshToken { get; set; }
    }
}
