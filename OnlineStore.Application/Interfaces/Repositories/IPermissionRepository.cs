using OnlineStore.Domain.Entities;

namespace OnlineStore.Application.Interfaces.Repositories
{
    public interface IPermissionRepository
    {
        Task<IReadOnlyCollection<Permission>> GetByIdsAsync(IReadOnlyCollection<int> permissionIds);
        Task<Permission?> GetByIdAsync(int id);
        Task<IReadOnlyCollection<Permission>> GetAllAsync();
        Task<Permission> CreateAsync(Permission permission);
    }
}
