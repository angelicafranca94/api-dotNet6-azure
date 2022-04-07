using AutoMapper;
using Manager.API.Utilities;
using Manager.API.ViewModels;
using Manager.Core.Exceptions;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers;

[ApiController]
[Route("/api/v1/user/")]
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
    public async Task<IActionResult> Create([FromBody] CreateUserViewModel model)
    {
        try
        {
            var userDTO = _mapper.Map<UserDTO>(model);
            var userCreated = await _userService.Create(userDTO);


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
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    } 
    
    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> Update([FromBody] UpdateUserViewModel model)
    {
        try
        {
            var userDTO = _mapper.Map<UserDTO>(model);
            var userUpdated = await _userService.Update(userDTO);


            return Ok(new ResultViewModel
            {
                Message = "Usuário Atualizado com sucesso!",
                Sucess = true,
                Data = userUpdated
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
           await _userService.Delete(id);

            return Ok(new ResultViewModel
            {
                Message = "Usuário Removido com sucesso!",
                Sucess = true,
                Data = null
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }

    [HttpGet]
    [Route("get/{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var user = await _userService.GetById(id);
            
            if(user == null)
                return NotFound(new ResultViewModel
                {
                    Message = "Usuário não encontrado!",
                    Sucess = true,
                    Data = null
                });

            return Ok(new ResultViewModel
            {
                Message = "",
                Sucess = true,
                Data = user
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }
    
    
    [HttpGet]
    [Route("get")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var allUsers = await _userService.GetAll();
          
            return Ok(new ResultViewModel
            {
                Message = "",
                Sucess = true,
                Data = allUsers
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }

    [HttpGet]
    [Route("search-by-name")]
    public async Task<IActionResult> SearchByName([FromQuery] string name)
    {
        try
        {
            var allUsers = await _userService.SearchByName(name);

            if (allUsers.Count == 0)
                return NotFound(new ResultViewModel
                {
                    Message = "Nenhum usuário encontrado com esse nome!",
                    Sucess = true,
                    Data = null
                });

            return Ok(new ResultViewModel
            {
                Message = "",
                Sucess = true,
                Data = allUsers
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }

    [HttpGet]
    [Route("search-by-email")]
    public async Task<IActionResult> SearchByEmail([FromQuery] string email)
    {
        try
        {
            var allUsers = await _userService.SearchByEmail(email);

            if (allUsers.Count == 0)
                return NotFound(new ResultViewModel
                {
                    Message = "Nenhum usuário encontrado com esse email!",
                    Sucess = true,
                    Data = null
                });

            return Ok(new ResultViewModel
            {
                Message = "",
                Sucess = true,
                Data = allUsers
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(Responses.DomainErrorMessage(ex.Message, ex.Errors));
        }
        catch (Exception)
        {
            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }
}
