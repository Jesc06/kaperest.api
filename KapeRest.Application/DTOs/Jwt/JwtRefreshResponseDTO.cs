using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Jwt
{
    public class JwtRefreshResponseDTO
    {
        public string responseToken { get; set; }
        public string responseRefreshToken { get; set; }
    }
}
