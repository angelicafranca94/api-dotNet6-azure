using AutoMapper;
using Bogus;
using Bogus.DataSets;
using EscNet.Cryptography.Interfaces;
using FluentAssertions;
using Manager.Core.Exceptions;
using Manager.Domain.Entities;
using Manager.Infra.Interfaces;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Manager.Services.Services;
using Manager.Tests.Configuration;
using Manager.Tests.Fixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Manager.Tests.Projects.Services;

public class UserServiceTest
{
    private readonly IUserService _sut;

    //Mocks
    private readonly IMapper _mapper;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRijndaelCryptography> _cryptoMock;

    public UserServiceTest()
    {
        _mapper = AutoMapperConfiguration.GetConfiguration();
        _userRepositoryMock = new Mock<IUserRepository>();
        _cryptoMock = new Mock<IRijndaelCryptography>();

        _sut = new UserService(
            mapper: _mapper,
            userRepository: _userRepositoryMock.Object,
            cryp: _cryptoMock.Object);
    }

    //NOMEMETODO_CONDICAO_RESULTADOESPERADO
    [Fact(DisplayName = "Create Valid User")]
    [Trait("Category", "Services")]
    public async Task Create_WhenUserIsValid_ReturnsUserDTO()
    {
        //Arrange
        var userToCreate = UserFixture.CreateValidUserDTO();

        var encryptedPassword = new Lorem().Sentence();

        var userCreated = _mapper.Map<User>(userToCreate);

        userCreated.ChangePassword(encryptedPassword);

        _userRepositoryMock.Setup(u =>
            u.GetByEmail(It.IsAny<string>())).ReturnsAsync(() => null);

        _cryptoMock.Setup(c =>
            c.Encrypt(It.IsAny<string>())).Returns(encryptedPassword);

        _userRepositoryMock.Setup(u =>
           u.Create(It.IsAny<User>())).ReturnsAsync(() => userCreated);

        //Act
        var result = await _sut.Create(userToCreate);

        //Assert
        result.Should().BeEquivalentTo(_mapper.Map<UserDTO>(userCreated));
    }

    [Fact(DisplayName = "Create When User Exists")]
    [Trait("Category", "Services")]
    public void Create_WhenUserExists_ThrowsNewDomainException()
    {
        //Arrange
        var userToCreate = UserFixture.CreateValidUserDTO();
        var userExists = UserFixture.CreateValidUser();

        _userRepositoryMock.Setup(u => u.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync(userExists);

        //Act
        Func<Task<UserDTO>> act = async () =>
        {
            return await _sut.Create(userToCreate);
        };

        //Assert
        act.Should().ThrowAsync<DomainException>()
            .WithMessage("Já exist um usuário cadastrado com o email informado");
    }

    [Fact(DisplayName = "Create When User is Invalid")]
    [Trait("Category", "Services")]
    public void Create_WhenUserIsInvalid_ThrowsNewDomainException()
    {
        //Arrange
        var userToCreate = UserFixture.CreateInvalidUserDTO();

        _userRepositoryMock.Setup(u =>
        u.GetByEmail(It.IsAny<string>())).ReturnsAsync(() => null);

        Func<Task<UserDTO>> act = async () =>
        {
            return await _sut.Create(userToCreate);
        };

        //Act
        act.Should().ThrowAsync<DomainException>();
    }

    [Fact(DisplayName = "Update Valid User")]
    [Trait("Category", "Services")]
    public async Task Update_WhenUserIsValid_ReturnUserDTO()
    {
        //Arange
        var oldUser = UserFixture.CreateValidUser();
        var userToUpdate = UserFixture.CreateValidUserDTO();
        var userUpdated = _mapper.Map<User>(userToUpdate);

        var encryptedPassword = new Lorem().Sentence();

        _userRepositoryMock.Setup(u =>
            u.GetById(It.IsAny<long>())).ReturnsAsync(() => oldUser);

        _cryptoMock.Setup(u =>
            u.Encrypt(It.IsAny<string>())).Returns(encryptedPassword);

        _userRepositoryMock.Setup(u =>
            u.Update(It.IsAny<User>())).ReturnsAsync(() => userUpdated);

        //Act
        var result = await _sut.Update(userToUpdate);

        //Assert
        result.Should().BeEquivalentTo(_mapper.Map<UserDTO>(userUpdated));
    }

    [Fact(DisplayName = "Update When User Not Exists")]
    [Trait("Category", "Services")]
    public void Update_WhenUserNotExists_ThrowsNewDomainException()
    {
        //Arrange
        var userToUpdate = UserFixture.CreateValidUserDTO();

        _userRepositoryMock.Setup(u =>
            u.GetById(It.IsAny<long>())).ReturnsAsync(() => null);

        //Act
        Func<Task<UserDTO>> act = async () =>
        {
            return await _sut.Update(userToUpdate);
        };


        //Assert
        act.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("O id informado para o usuário não existe");

    }

    [Fact(DisplayName = "Update When User is Invalid")]
    [Trait("Category", "Services")]
    public void Update_WhenUserIsInvalid_ThrowsNewDomainsException()
    {
        //Arrange
        var oldUser = UserFixture.CreateValidUser();
        var userToUpdate = UserFixture.CreateInvalidUserDTO();

        _userRepositoryMock.Setup(u =>
            u.GetById(It.IsAny<long>())).ReturnsAsync(() => oldUser);

        //Act
        Func<Task<UserDTO>> act = async () =>
        {
            return await _sut.Update(userToUpdate);
        };

        //Assert
        act.Should().ThrowAsync<DomainException>();
    }

    [Fact(DisplayName = "Delete User")]
    [Trait("Category", "Services")]
    public async Task Delete_WhenUserExists_DeleteUser()
    {
        // Arrange
        var userId = new Randomizer().Int(0, 1000);

        _userRepositoryMock.Setup(x => x.Delete(It.IsAny<int>()))
            .Verifiable();

        // Act
        await _sut.Delete(userId);

        // Assert
        _userRepositoryMock.Verify(x => x.Delete(userId), Times.Once);
    }

    [Fact(DisplayName = "Get By Id")]
    [Trait("Category", "Services")]
    public async Task GetById_WhenUserExists_ReturnsUserDTO()
    {
        // Arrange
        var userId = new Randomizer().Int(0, 1000);
        var userFound = UserFixture.CreateValidUser();

        _userRepositoryMock.Setup(x => x.GetById(userId))
        .ReturnsAsync(() => userFound);

        // Act
        var result = await _sut.GetById(userId);

        // Assert
        result.Should()
            .BeEquivalentTo(_mapper.Map<UserDTO>(userFound));
    }

    [Fact(DisplayName = "Get By Id When User Not Exists")]
    [Trait("Category", "Services")]
    public async Task GetById_WhenUserNotExists_ReturnsEmptyOptional()
    {
        // Arrange
        var userId = new Randomizer().Int(0, 1000);

        _userRepositoryMock.Setup(x => x.GetById(userId))
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.GetById(userId);

        // Assert
        result.Should()
            .BeNull();
    }

    [Fact(DisplayName = "Get By Email")]
    [Trait("Category", "Services")]
    public async Task GetByEmail_WhenUserExists_ReturnsUserDTO()
    {
        // Arrange
        var userEmail = new Internet().Email();
        var userFound = UserFixture.CreateValidUser();

        _userRepositoryMock.Setup(x => x.GetByEmail(userEmail))
        .ReturnsAsync(() => userFound);

        // Act
        var result = await _sut.GetByEmail(userEmail);

        // Assert
        result.Should()
            .BeEquivalentTo(_mapper.Map<UserDTO>(userFound));
    }

    [Fact(DisplayName = "Get By Email When User Not Exists")]
    [Trait("Category", "Services")]
    public async Task GetByEmail_WhenUserNotExists_ReturnsEmptyOptional()
    {
        // Arrange
        var userEmail = new Internet().Email();

        _userRepositoryMock.Setup(x => x.GetByEmail(userEmail))
         .ReturnsAsync(() => null);

        // Act
        var result = await _sut.GetByEmail(userEmail);

        // Assert
        result.Should()
            .BeNull();
    }

    [Fact(DisplayName = "Get All Users")]
    [Trait("Category", "Services")]
    public async Task GetAllUsers_WhenUsersExists_ReturnsAListOfUserDTO()
    {
        // Arrange
        var usersFound = UserFixture.CreateListValidUser();

        _userRepositoryMock.Setup(x => x.GetAll())
            .ReturnsAsync(() => usersFound);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should()
            .BeEquivalentTo(_mapper.Map<List<UserDTO>>(usersFound));
    }

    [Fact(DisplayName = "Get All Users When None User Found")]
    [Trait("Category", "Services")]
    public async Task GetAllUsers_WhenNoneUserFound_ReturnsEmptyList()
    {
        // Arrange

        _userRepositoryMock.Setup(x => x.GetAll())
            .ReturnsAsync(() => null);

        // Act
        var result = await _sut.GetAll();

        // Assert
        result.Should()
            .BeEmpty();
    }

    [Fact(DisplayName = "Search By Name")]
    [Trait("Category", "Services")]
    public async Task SearchByName_WhenAnyUserFound_ReturnsAListOfUserDTO()
    {
        // Arrange
        var nameToSearch = new Name().FirstName();
        var usersFound = UserFixture.CreateListValidUser();

        _userRepositoryMock.Setup(x => x.SearchByName(nameToSearch))
         .ReturnsAsync(() => usersFound);

        // Act
        var result = await _sut.SearchByName(nameToSearch);

        // Assert
        result.Should()
            .BeEquivalentTo(_mapper.Map<List<UserDTO>>(usersFound));
    }

    [Fact(DisplayName = "Search By Name When None User Found")]
    [Trait("Category", "Services")]
    public async Task SearchByName_WhenNoneUserFound_ReturnsEmptyList()
    {
        // Arrange
        var nameToSearch = new Name().FirstName();

        _userRepositoryMock.Setup(x => x.SearchByName(nameToSearch))
         .ReturnsAsync(() => null);

        // Act
        var result = await _sut.SearchByName(nameToSearch);

        // Assert
        result.Should()
            .BeEmpty();
    }

    [Fact(DisplayName = "Search By Email")]
    [Trait("Category", "Services")]
    public async Task SearchByEmail_WhenAnyUserFound_ReturnsAListOfUserDTO()
    {
        // Arrange
        var emailSoSearch = new Internet().Email();
        var usersFound = UserFixture.CreateListValidUser();

        _userRepositoryMock.Setup(x => x.SearchByEmail(emailSoSearch))
         .ReturnsAsync(() => usersFound);

        // Act
        var result = await _sut.SearchByEmail(emailSoSearch);

        // Assert
        result.Should()
            .BeEquivalentTo(_mapper.Map<List<UserDTO>>(usersFound));
    }

    [Fact(DisplayName = "Search By Email When None User Found")]
    [Trait("Category", "Services")]
    public async Task SearchByEmail_WhenNoneUserFound_ReturnsEmptyList()
    {
        // Arrange
        var emailSoSearch = new Internet().Email();

        _userRepositoryMock.Setup(x => x.SearchByEmail(emailSoSearch))
         .ReturnsAsync(() => null);

        // Act
        var result = await _sut.SearchByEmail(emailSoSearch);

        // Assert
        result.Should()
            .BeEmpty();
    }


}
