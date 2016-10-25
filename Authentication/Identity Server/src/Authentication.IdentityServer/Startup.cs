using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Authentication.IdentityServer.Data;
using Authentication.IdentityServer.Services;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Authentication.IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this.InitializeDatabase(app);
            app.Properties["host.AppMode"] = "development";

            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
              //  app.UseBrowserLink();
            }

            // Identity API Authentication
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = this.Configuration["Authority"],
                ScopeName = "api",
                RequireHttpsMetadata = false,
                AutomaticChallenge = false
            });

            // Static files
            app.UseStaticFiles();

            // ASP.NET Identity
            app.UseIdentity();

            // Identity Server
            app.UseIdentityServer();

            // API
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            var connectionString = this.Configuration.GetConnectionString("Identity");

            // ASP.NET Identity
            services.AddDbContext<ApplicationDbContext>(options =>
                                                        options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            // MVC
            services.AddMvc();

            // Identity Server
            services.AddIdentityServer()
                .SetSigningCredential(LoadCertificate())
                    .AddConfigurationStore(builder =>
                        builder.UseSqlServer(connectionString, options =>
                            options.MigrationsAssembly(migrationsAssembly)))
                    .AddOperationalStore(builder =>
                        builder.UseSqlServer(connectionString, options =>
                            options.MigrationsAssembly(migrationsAssembly)))
                    .AddAspNetIdentity<ApplicationUser>();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        private static X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(Path.Combine(Directory.GetCurrentDirectory(), "idsrv3test.pfx"), "idsrv3test");
        }

        private async void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                Console.WriteLine("Migrating ApplicationDbContext...");
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                Console.WriteLine("Migrating PersistedGrantDbContext...");
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                Console.WriteLine("Migrating ConfigurationDbContext...");
                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();

                Console.WriteLine("Adding seed data...");
                if (!context.Clients.Any())
                {
                    foreach (var client in IdentityConfiguration.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.Scopes.Any())
                {
                    foreach (var client in IdentityConfiguration.GetScopes())
                    {
                        context.Scopes.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                if (!manager.Users.Any())
                {
                    foreach (var user in IdentityConfiguration.GetUsers())
                    {
                        var applicationUser = new ApplicationUser { UserName = user.Username, Email = user.Username };
                        foreach (var claim in user.Claims)
                        {
                            applicationUser.Claims.Add(new IdentityUserClaim<string> { ClaimType = claim.Type, ClaimValue = claim.Value });
                        }
                        await manager.CreateAsync(applicationUser, user.Password);
                    }
                }
            }
        }
    }
}