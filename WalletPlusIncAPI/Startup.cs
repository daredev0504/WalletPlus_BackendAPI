using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using WalletPlusIncAPI.Data.Data;
using WalletPlusIncAPI.Extensions;
using WalletPlusIncAPI.Filters;
using WalletPlusIncAPI.Models.Entities;

namespace WalletPlusIncAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.ConfigureSwagger();
            services.ConfigureIdentityPassword();
            //services.ConfigureDbContext(Configuration);
            services.ConfigureDbContextForPostgresql(Configuration);

                services.AddCors(options =>
                {
                    options.AddPolicy("CorsPolicy", builder =>
                        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSignalR();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            
            services.ConfigureAddIdentity();
            
            services.ConfigureAuthManager();
            services.ConfigureJwt(Configuration);
            services.ConfigureLoggerService();
            services.ConfigureWalletService();
            services.ConfigureTransactionService();
            services.ConfigureFundingService();
            services.ConfigureCurrencyService();
            services.ConfigureWalletRepository();
            services.ConfigureTransactionRepository();
            services.ConfigureCurrencyRepository();
            services.ConfigureFundsRepository();
            services.ConfigureAppUserService();
            services.AddScoped<ValidationFilterAttribute>();
          
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WalletManagementAPI v1"));
            //app.UseHttpsRedirection();

            app.UseRouting();

            PreSeeder.Seed(context, roleManager, userManager).Wait();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapHub<ChatHub>("/hubs/chat");
            });
        }
    }
}
