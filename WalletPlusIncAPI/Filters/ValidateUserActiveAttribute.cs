using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using WalletPlusIncAPI.Services.Interfaces;

namespace Commander.API.ActionFilters
{
    public class ValidateUserActiveAttribute : IAsyncActionFilter
    {
        private readonly IAppUserService _appUserService;
        private readonly ILoggerService _logger;

        public ValidateUserActiveAttribute(IServiceProvider serviceProvider)
        {
            _appUserService = serviceProvider.GetRequiredService<IAppUserService>();
            _logger = serviceProvider.GetRequiredService<ILoggerService>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var check = await _appUserService.IsUserActiveAsync();
            if (check)
            {
                _logger.LogInfo($"user is Active");
                //context.Result = new OkObjectResult("user is active");
                await next();
            }
            else
            {
               _logger.LogInfo($"user is not Active");
                context.Result = new BadRequestObjectResult("you have been de-activated, contact admin for more information");
               
            }
        }
    }
}
