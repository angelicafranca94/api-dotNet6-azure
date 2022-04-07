using Manager.Services.DTO;

namespace Manager.Services.Interfaces;

public interface IUserService
{
    Task<UserDTO> Create(UserDTO userDTO);
    Task<UserDTO> Update(UserDTO userDTO);
    Task Delete(long id);
    Task<UserDTO> GetById(long id);
    Task<List<UserDTO>> GetAll();
    Task<List<UserDTO>> SearchByName(string name);
    Task<List<UserDTO>> SearchByEmail(string email);
    Task<UserDTO> GetByEmail(string email);

}
