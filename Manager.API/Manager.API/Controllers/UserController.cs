using AutoMapper;
using Manager.API.Utilities;
using Manager.API.ViewModels;
using Manager.Core.Exceptions;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers;

[ApiController]
[Route("/api/v1/users/")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost]
   [Route("create")]
   public async Task<IActionResult> Create([FromBody]CreateUserViewModel model)
    {
        try
        {
            var userDTO = _mapper.Map<UserDTO>(model);
            var userCreated = _userService.Create(userDTO);


            return Ok(new ResultViewModel
            {
                Message = "Usuário Criado com sucesso!",
                Sucess = true,
                Data = userCreated
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception ex)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }

    [HttpGet]
    [Route("find/{id}")]
    public async Task<IActionResult> FindById()
    {
        return Ok();
    }

}
