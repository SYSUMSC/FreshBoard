using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
using Westwind.AspNetCore.LiveReload;

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
            services.AddLiveReload();

            services.AddResponseCompression(options =>
            {
                options.Providers.Add<BrotliCompressionProvider>();
                options.EnableForHttps = true;
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<Data.DbContext>(options =>
                options.UseSqlite(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

            services.AddIdentityCore<FreshBoardUser>(o =>
            {
                o.Password.RequireNonAlphanumeric = false;
                o.Stores.MaxLengthForKeys = 128;
                o.User.RequireUniqueEmail = true;
            })
            .AddSignInManager()
            .AddUserManager<UserManager<FreshBoardUser>>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<Data.DbContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<TranslatedIdentityErrorDescriber>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/SignIn";
                options.LogoutPath = "/SignOut";
            });

            services.AddEntityFrameworkSqlite();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<ISmsSender, SmsSender>();
            services.AddSession();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR();

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
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
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/Hackathon", StringComparison.CurrentCultureIgnoreCase))
                    await next();
            });

            app.UseResponseCompression();

            // app.UseLiveReload();

            app.UseStaticFiles();

            app.UseRouting();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapHub<ChatHub>("/ChatHub");
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
                                    using (var db = new Data.DbContext(Configuration.GetConnectionString("DefaultConnection")))
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
