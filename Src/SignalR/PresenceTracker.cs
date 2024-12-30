using dating_course_api.Src.Data;
using dating_course_api.Src.Entities;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.SignalR
{
    public class PresenceTracker(DataContext context)
    {
        private readonly DataContext _context = context;

        public async Task<bool> UserConnected(int userId, string connectionId)
        {
            var isOnline = false;

            var connection = new Connection
            {
                ConnectionId = connectionId,
                UserId = userId,
                Connected = true
            };

            var existingConnection = await _context.Connections.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.ConnectionId == connectionId
            );

            if (existingConnection == null)
            {
                await _context.Connections.AddAsync(connection);
                isOnline = true;
            }
            else
            {
                existingConnection.Connected = true;
            }

            await _context.SaveChangesAsync();
            return isOnline;
        }

        public async Task<bool> UserDisconnected(int userId, string connectionId)
        {
            var isOffline = false;

            var connection = await _context.Connections.FirstOrDefaultAsync(x =>
                x.UserId == userId && x.ConnectionId == connectionId
            );

            if (connection != null)
            {
                connection.Connected = false;

                // Verificar TODAS las conexiones activas del usuario
                var activeConnections = await _context
                    .Connections.Where(x =>
                        x.UserId == userId && x.Connected && x.ConnectionId != connectionId
                    )
                    .CountAsync();

                if (activeConnections == 0)
                    isOffline = true;

                await _context.SaveChangesAsync();
            }

            return isOffline;
        }

        public async Task<int[]> GetOnlineUsers()
        {
            return await _context
                .Connections.Where(x => x.Connected)
                .Select(x => x.UserId)
                .Distinct()
                .OrderBy(x => x)
                .ToArrayAsync();
        }

        public async Task<List<string>> GetConnectionsForUser(int userId)
        {
            return await _context
                .Connections.Where(x => x.UserId == userId && x.Connected)
                .Select(x => x.ConnectionId)
                .ToListAsync();
        }
    }
}
