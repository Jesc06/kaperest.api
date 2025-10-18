using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.CurrentUserService
{
    public interface ICurrentUser
    {
        string? Email { get; }
        string? UserId { get; }
    }
}
