
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using RealTimeApp.Hubs;

namespace RealTimeApp
{
    public class PostgresListener : BackgroundService
    {
        private readonly string _connectionString;
        private readonly IHubContext<YourHub> _hubContext;

        public PostgresListener(IConfiguration configuration, IHubContext<YourHub> hubContext)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _hubContext = hubContext;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(stoppingToken);

            await using var listenCommand = new NpgsqlCommand("LISTEN events;", connection);
            await listenCommand.ExecuteNonQueryAsync(stoppingToken);

            connection.Notification += async (o, e) =>
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", "payload");
            };

            while (!stoppingToken.IsCancellationRequested)
            {
                await connection.WaitAsync(stoppingToken);
            }
        }
    }
}
