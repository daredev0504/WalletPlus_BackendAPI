using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WalletPlusIncAPI.Helpers;
using WalletPlusIncAPI.Models.Dtos.Currency;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Controllers
{
    /// <summary>
    /// Currency Controller
    /// </summary>
    public class CurrencyController : BaseApiController
    {
        private readonly ICurrencyService _currencyService;
        private readonly IMapper _mapper;

       /// <summary>
       /// 
       /// </summary>
       /// <param name="currencyService"></param>
       /// <param name="mapper"></param>
        public CurrencyController(ICurrencyService currencyService, IMapper mapper)
        {
           _currencyService = currencyService;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all country codes
        /// </summary>
        /// <returns></returns>
      
        [HttpGet("GetAllCurrencies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCurrencies()
        {
            var currencies = await _currencyService.GetAllCurrencies();
            var data = currencies.Data;
            var dataToReturn = _mapper.Map<List<CurrencyReadDto>>(data);

            return Ok(ResponseMessage.Message("List of all Currencies and their slug code", null, dataToReturn));
        }
    }
}