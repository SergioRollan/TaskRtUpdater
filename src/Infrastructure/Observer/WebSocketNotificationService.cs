using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TaskRtUpdater.src.Enums;

namespace TaskRtUpdater.src.Infrastructure.Observer
{
    public class WebSocketNotificationService : INotificationService
    {
        private static readonly List<WebSocket> _clients = new();
        private readonly ILogger<WebSocketNotificationService> _logger;

        public WebSocketNotificationService(ILogger<WebSocketNotificationService> logger)
        {
            _logger = logger;
        }

        public static void AddClient(WebSocket socket)
        {
            lock (_clients)
            {
                _clients.Add(socket);
            }
        }

        public static void RemoveClient(WebSocket socket)
        {
            lock (_clients)
            {
                _clients.Remove(socket);
            }
        }

        public async Task NotifyTaskCreatedAsync(int taskId, string title, string description)
        {
            var message = new TaskEventMessage(TaskEventType.TaskCreated, taskId, description: description);
            await SendMessageToAllClientsAsync(message);
            _logger.LogInformation($"TaskCreated notification sent for TaskId: {taskId}");
        }

        public async Task NotifyTaskUpdatedAsync(int taskId, string status)
        {
            var message = new TaskEventMessage(TaskEventType.TaskUpdated, taskId, status: status);
            await SendMessageToAllClientsAsync(message);
            _logger.LogInformation($"TaskUpdated notification sent for TaskId: {taskId}, Status: {status}");
        }

        public async Task NotifyTaskDeletedAsync(int taskId)
        {
            var message = new TaskEventMessage(TaskEventType.TaskDeleted, taskId);
            await SendMessageToAllClientsAsync(message);
            _logger.LogInformation($"TaskDeleted notification sent for TaskId: {taskId}");
        }

        private async Task SendMessageToAllClientsAsync(TaskEventMessage message)
        {
            var json = JsonSerializer.Serialize(message, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var bytes = Encoding.UTF8.GetBytes(json);
            var clientsToRemove = new List<WebSocket>();

            lock (_clients)
            {
                foreach (var client in _clients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        try
                        {
                            var task = client.SendAsync(
                                new ArraySegment<byte>(bytes),
                                WebSocketMessageType.Text,
                                true,
                                CancellationToken.None);

                            if (!task.Wait(TimeSpan.FromSeconds(5))) // 5 seconds timeout
                            {
                                _logger.LogWarning("WebSocket send timeout, marking client for removal");
                                clientsToRemove.Add(client);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error sending WebSocket message");
                            clientsToRemove.Add(client);
                        }
                    }
                    else
                    {
                        clientsToRemove.Add(client);
                    }
                }

                foreach (var client in clientsToRemove)
                {
                    _clients.Remove(client);
                }
            }
        }
    }
}