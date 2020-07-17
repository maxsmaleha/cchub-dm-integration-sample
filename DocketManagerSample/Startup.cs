using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace DocketManagerSample
{
    public class Startup
    {
        private const string DefaultCorsPolicyName = "localhost";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            /* 
             * IMPORTANT PART! 
             * 
             * See this page to learn about AddIdentityServer
             * https://identityserver4.readthedocs.io/en/latest/quickstarts/1_client_credentials.html
             * 
             * In the tutorial, the values passed to `AddInMemoryIdentityResources`, `AddInMemoryApiScopes` and
             * `AddInMemoryClients` are separated to a separate static class called Config. We decided to use the
             * inline initialization to simplify the code. In a real-life app, you may want to structure it in a 
             * better way.
             * 
             * The `AddIdentityServer` adds a support of identity server to your app - it adds all necessary endpoints
             * according to OAuth2 specs and "connects" to your users. 
             * 
             * This article may be helpful when you are examining the code below. 
             * http://docs.identityserver.io/en/release/topics/startup.html
             */

            services.AddIdentityServer()
                /*
                 * The `AddDeveloperSigningCredential` is good for the dev purposes. By the time you go live,
                 * it makes sense to change it to `AddSigningCredential` as discussed at:
                 * https://forums.asp.net/t/2138084.aspx or https://tatvog.wordpress.com/2018/06/05/identityserver4-addsigningcredential-using-certificate-stored-in-azure-key-vault/
                 */
                .AddDeveloperSigningCredential()

                /*
                 * Here you configure what information about an identity (= user) is available to 
                 * the app through API. http://docs.identityserver.io/en/release/topics/resources.html
                 */
                .AddInMemoryIdentityResources(new List<IdentityResource>
                    {
                        new IdentityResources.OpenId(),
                        new IdentityResources.Profile(),
                        new IdentityResources.Email(),
                        new IdentityResources.Address(),
                        new IdentityResource {Name = "roles", UserClaims = {JwtClaimTypes.Role}}
                    })

                /*
                 * In this sample, you are exposing the API for BackOffice. Here you declare the API Scope
                 * available BackOffice (or, saying more generally, to the third-party apps who are going to 
                 * use your APIs). http://docs.identityserver.io/en/release/topics/resources.html#defining-api-resources
                 * 
                 * Later, we can discuss about more granular API Scopes if necessary (e.g. Products/Full Rights, Products/Read-Only, etc).
                 */
                .AddInMemoryApiScopes(new List<ApiScope>
                    {
                        new ApiScope("docket-manager", "DocketManager API")
                    })

                /*
                 * Here you specify a list of Clients (=Third-Party Applications) which are allowed to access the
                 * IdentityServer. http://docs.identityserver.io/en/release/topics/clients.html
                 * 
                 * In the beginning, here you will have a limited hardcoded list of Applications. 
                 * In addition to BackOffice, you may want to add DocketManager client or other your services. 
                 * 
                 * Later, you may consider creating a Marketplace of apps or support "private apps" like in Shopify, 
                 * BigCommerce, etc. In this case, you need to populate this list from a database (most likely, use 
                 * a DB-driven client store instead of in-memory one). 
                 */
                .AddInMemoryClients(new List<Client>
                    {
                        new Client
                        {
                            /* 
                             * The ClientId and Secret should be the same as inside the BackOffice. 
                             * That's why it is parametrized through the configuration
                             */
                            ClientId = Configuration["BackOffice:ClientId"],
                            ClientSecrets = { new Secret(Configuration["BackOffice:ApiSecret"].Sha256()) },
                            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                            AllowOfflineAccess = true,
                            AlwaysIncludeUserClaimsInIdToken = true,

                            RequireConsent = false,
                            RequirePkce = true,

                            // where to redirect to after login
                            RedirectUris = { $"{Configuration["BackOffice:BackendUrl"]}signin-docket-manager" },

                            // where to redirect to after logout
                            PostLogoutRedirectUris = { $"{Configuration["BackOffice:BackendUrl"]}signout-docket-manager" },

                            AllowedScopes = new List<string>
                            {
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile,
                                IdentityServerConstants.StandardScopes.Email,
                                IdentityServerConstants.StandardScopes.Address,
                                "roles",
                                "docket-manager"
                            }
                        }
                    })
                .AddTestUsers(TestUsers.Users);

            services.AddAuthentication()
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = Configuration["Application:Url"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        DefaultCorsPolicyName,
                        builder => builder
                            .WithOrigins(Configuration["BackOffice:FrontendUrl"])
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                    );
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Remove("X-Frame-Options");

                await next();
            });

            app.UseCors(DefaultCorsPolicyName); // Enable CORS!

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
