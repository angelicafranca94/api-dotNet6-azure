using AutoMapper;
using EscNet.Hashers.Interfaces.Algorithms;
using Manager.Core.Exceptions;
using Manager.Domain.Entities;
using Manager.Infra.Interfaces;
using Manager.Services.DTO;
using Manager.Services.Interfaces;

namespace Manager.Services.Services;

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly IArgon2IdHasher _hasher;


    public UserService(IMapper mapper, 
        IUserRepository userRepository,
       IArgon2IdHasher hasher)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _hasher = hasher;
    }

    public async Task<UserDTO> Create(UserDTO userDTO)
    {
        var userExists = await _userRepository.GetByEmail(userDTO.Email);

        if (userExists != null)
            throw new DomainException("Já existe um usuário cadastrado com o email informado");

        var user = _mapper.Map<User>(userDTO);
        user.Validate();
        user.SetPassword(_hasher.Hash(user.Password));

        var userCreated = await _userRepository.Create(user);

        return _mapper.Map<UserDTO>(userCreated);
    }

    public async Task<UserDTO> Update(UserDTO userDTO)
    {
        var userExists = await _userRepository.GetById(userDTO.Id);

        if (userExists == null)
            throw new DomainException("Não existe nenhum usuário com o id informado");

        var user = _mapper.Map<User>(userDTO);
        user.Validate();
        user.SetPassword(_hasher.Hash(user.Password));

        var userCreated = await _userRepository.Update(user);

        return _mapper.Map<UserDTO>(userCreated);
    }

    public async Task Delete(long id)
    {
        await _userRepository.Delete(id);
    }

    public async Task<UserDTO> GetById(long id)
    {
        var user = await _userRepository.GetById(id);

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<List<UserDTO>> GetAll()
    {
        var allUsers = await _userRepository.GetAll();
        return _mapper.Map<List<UserDTO>>(allUsers);
    }

    public async Task<UserDTO> GetByEmail(string email)
    {
        var user = await _userRepository.GetByEmail(email);

        return _mapper.Map<UserDTO>(user);
    }

    public async Task<List<UserDTO>> SearchByName(string name)
    {
        var allUsers = await _userRepository.SearchByName(name);
        return _mapper.Map<List<UserDTO>>(allUsers);
    }

    public async Task<List<UserDTO>> SearchByEmail(string email)
    {
        var allUsers = await _userRepository.SearchByEmail(email);
        return _mapper.Map<List<UserDTO>>(allUsers);
    }
}
