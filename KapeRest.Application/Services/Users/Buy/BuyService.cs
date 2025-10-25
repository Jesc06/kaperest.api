using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Users.Buy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Users.Buy
{
    public class BuyService
    {
        private readonly IBuy _buy;
        public BuyService(IBuy buy)
        {
            _buy = buy;
        }
        public async Task<string> BuyItem(BuyMenuItemDTO buy)
        {
            return await _buy.BuyMenuItemAsync(buy);
        }

    }
}
