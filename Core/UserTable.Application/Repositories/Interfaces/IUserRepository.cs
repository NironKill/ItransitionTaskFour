using UserTable.Application.DTOs;

namespace UserTable.Application.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task Unlock(ICollection<string> emails);
        Task Lock(ICollection<string> emails);
        Task Remove(ICollection<string> emails);
        Task Update(string email);

        Task<ICollection<UserDTO>> GetAll();
        Task<UserDTO> GetByEmail(string email);
    }
}
