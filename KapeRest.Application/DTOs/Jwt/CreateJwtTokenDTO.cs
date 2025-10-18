using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Jwt
{
    public class CreateJwtTokenDTO
    {
        public string token { get; set; }
        public string refreshToken { get; set; }    
    }
}
