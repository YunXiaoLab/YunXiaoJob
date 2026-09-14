using YunXiaoJob.Domain.Entities;
namespace YunXiaoJob.Application.Interfaces.Repositories;
public interface INotificationRepository { Task<IReadOnlyList<Notification>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default); Task<Notification?> GetByIdAsync(Guid notificationId, CancellationToken cancellationToken = default); Task AddAsync(Notification notification, CancellationToken cancellationToken = default); }
