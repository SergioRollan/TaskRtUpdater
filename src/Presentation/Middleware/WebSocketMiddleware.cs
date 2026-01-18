using System.Net.WebSockets;
using TaskRtUpdater.src.Infrastructure.Observer;

namespace TaskRtUpdater.src.Presentation.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                WebSocketNotificationService.AddClient(webSocket);

                try
                {
                    await ReceiveMessages(webSocket);
                }
                finally
                {
                    WebSocketNotificationService.RemoveClient(webSocket);
                }
            }
            else
            {
                await _next(context);
            }
        }

        private async Task ReceiveMessages(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "Closed by client",
                        CancellationToken.None);
                }
            }
        }
    }
}
