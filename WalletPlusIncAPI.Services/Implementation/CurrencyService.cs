using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WalletPlusIncAPI.Data.DataAccess.Interfaces;
using WalletPlusIncAPI.Models.Entities;
using WalletPlusIncAPI.Services.Interfaces;

namespace WalletPlusIncAPI.Services.Implementation
{
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyRepository _currencyRepository;

        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }


        public async Task<string> GetCurrencyCode(int? currencyId)
        {
            var currency = await GetCurrencyById(currencyId);
            var code = currency.Data.Code;
          
            if (code != null)
            {
                return code;
            }

            return String.Empty;
        }
        
        public async Task<ServiceResponse<bool>> CurrencyExist(int currencyId)
        {
            var response = new ServiceResponse<bool>();
            var currencyExist = await _currencyRepository.CurrencyExist(currencyId);
            if (currencyExist)
            {
                response.Message = "currency found";
                response.Success = true;
                return response;
            }
            response.Message = "An error occured";
            response.Success = false;

            return response;
        }

        public async Task<ServiceResponse<Currency>> GetCurrencyById(int? id)
        {
            var response = new ServiceResponse<Currency>();
            var currency = await _currencyRepository.GetById(id);
            if (currency != null)
            {
                response.Message = "currency returned";
                response.Success = true;
                return response;
            }
            response.Message = "currency not found";
            response.Success = false;

            return response;
        }

        public async Task<ServiceResponse<List<Currency>>> GetAllCurrencies()
        {
            var response = new ServiceResponse<List<Currency>>();
            var currencies = await _currencyRepository.GetAll().ToListAsync();
           
            if (currencies != null)
            {
                var orderedCurrencies = currencies.OrderBy(x => x.Code).ToList();
                response.Message = "currency returned";
                response.Data = orderedCurrencies;
                response.Success = true;
                return response;
            }
            response.Message = "currency not found";
            response.Success = false;

            return response;
        }

        public async Task<ServiceResponse<bool>> AddCurrency(Currency currency)
        {
            var response = new ServiceResponse<bool>();
            var result = await _currencyRepository.Add(currency);
            if (result)
            {
                response.Message = "currency created";
                response.Success = true;
                return response;
            }
            response.Message = "An error occured";
            response.Success = false;

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteCurrency(int id)
        {
            var response = new ServiceResponse<bool>();
            var result = await _currencyRepository.DeleteById(id);
            if (result)
            {
                response.Message = "currency deleted";
                response.Success = true;
                return response;
            }
            response.Message = "An error occured";
            response.Success = false;

            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateCurrency(Currency currency)
        {
            var response = new ServiceResponse<bool>();
            var result = await _currencyRepository.Modify(currency);
            if (result)
            {
                response.Message = "currency updated";
                response.Success = true;
                return response;
            }
            response.Message = "An error occured";
            response.Success = false;

            return response;
        }
    }
}
