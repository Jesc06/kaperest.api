using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Jwt
{
    public class JwtRefreshRequestDTO
    {
        public string requestToken { get; set; }    
        public string requestRefreshToken { get; set; }
    }
}
