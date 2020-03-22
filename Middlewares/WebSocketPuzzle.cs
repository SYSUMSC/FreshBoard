using FreshBoard.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreshBoard.Middlewares
{
    public static class WebSocketPuzzleExtensions
    {
        public static IApplicationBuilder UseWebSocketPuzzle(this IApplicationBuilder app)
        {
            app.UseMiddleware<WebSocketPuzzle>();
            return app;
        }
    }
    public class WebSocketPuzzle : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.WebSockets.IsWebSocketRequest && context.Request.Path == "/MSCHome")
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await PuzzleExecutor(context, webSocket);
            }
            else
            {
                await next(context);
            }
        }

        private async Task PuzzleExecutor(HttpContext context, WebSocket webSocket)
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
                            await webSocket.SendAsync(Encoding.UTF8.GetBytes("Here is an important key: " + guid), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else
                        {
                            if (hasKey)
                            {
                                if (text == guid)
                                {
                                    var db = context.RequestServices.GetRequiredService<FreshBoardDbContext>();
                                    var answer = await db.Problem.FirstOrDefaultAsync(i => i.Level == 10 && i.Title == "Greetings");
                                    if (answer != null)
                                    {
                                        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"Wow! {guid}! That's it! Congratulations! Here is the answer: {answer.Answer}. Bye~"), WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                    else
                                    {
                                        await webSocket.SendAsync(Encoding.UTF8.GetBytes($"Wow! {guid}! That's it! Congratulations! But unfortunately I don't know the answer. Bye~"), WebSocketMessageType.Text, true, CancellationToken.None);
                                    }
                                }
                                else
                                {
                                    await webSocket.SendAsync(Encoding.UTF8.GetBytes("What's this?"), WebSocketMessageType.Text, true, CancellationToken.None);
                                }
                            }
                            else
                            {
                                await webSocket.SendAsync(Encoding.UTF8.GetBytes("Sorry, I don't know what you are talking about."), WebSocketMessageType.Text, true, CancellationToken.None);
                            }
                        }
                    }
                }

                result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
