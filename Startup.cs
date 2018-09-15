using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mscfreshman.Data;
using mscfreshman.Data.Identity;
using mscfreshman.Hubs;
using mscfreshman.Services;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mscfreshman
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<FreshBoardUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddErrorDescriber<TranslatedIdentityErrorDescriber>();

            services.AddEntityFrameworkSqlite();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddSession();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSignalR();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=864000");
                    ctx.Context.Response.Headers.Add("Expires", new[] { DateTime.UtcNow.AddDays(10).ToString("R") });
                }
            });

            app.UseCookiePolicy();
            app.UseSession();
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/MSCHome")
                {
                    var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await Echo(context, webSocket);
                }
                else
                {
                    await next();
                }
            });

            app.UseSignalR(routes => { routes.MapHub<ChatHub>("/ChatHub"); });

            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }


        private async Task Echo(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[128];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var hasGreeting = false;
            var hasKey = false;
            var guid = Guid.NewGuid().ToString();
            while (!result.CloseStatus.HasValue)
            {
                var text = Encoding.UTF8.GetString(buffer.Take(result.Count).ToArray()).ToLower();

                if ((text.Contains("hello") || text.Contains("hi")) && text.Contains("msc"))
                {
                    hasGreeting = true;
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("Hi~")), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    if (hasGreeting)
                    {
                        if ((text.Contains("answer") || text.Contains("solve") || text.Contains("solution") || text.Contains("hint") || text.Contains("key")) && (text.Contains("what") || text.Contains("where") || text.Contains("how")))
                        {
                            hasKey = true;
                            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("Here is an important key: " + guid)), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            if (hasKey)
                            {
                                if (text == guid)
                                {
                                    using (var db = new ApplicationDbContext(Configuration.GetConnectionString("DefaultConnection")))
                                    {
                                        var answer = await db.Problem.FirstOrDefaultAsync(i => i.Level == 10 && i.Title == "Greetings");
                                        if (answer != null)
                                        {
                                            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Wow! {guid}! That's it! Congratulations! Here is the answer: {answer.Answer}. Bye~")), WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                        else
                                        {
                                            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Wow! {guid}! That's it! Congratulations! But unfortunately I don't know the answer. Bye~")), WebSocketMessageType.Text, true, CancellationToken.None);
                                        }
                                    }
                                }
                                else
                                {
                                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("What's this?")), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                            else
                            {
                                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("Sorry, I don't know what you are talking about.")), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                }

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
