using Microsoft.EntityFrameworkCore;
using YunXiaoJob.Application.Interfaces.Repositories;
using YunXiaoJob.Domain.Entities;
using YunXiaoJob.Infrastructure.Data;
namespace YunXiaoJob.Infrastructure.Repositories;
public class NotificationRepository : INotificationRepository
{
    private readonly ApplicationDbContext _context; public NotificationRepository(ApplicationDbContext context) => _context = context;
    public async Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default) => await _context.Notifications.Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);
    public Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default) => _context.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId, cancellationToken);
    public Task AddAsync(Notification notification, CancellationToken cancellationToken = default) => _context.Notifications.AddAsync(notification, cancellationToken).AsTask();
}
