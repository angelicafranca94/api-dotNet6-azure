using Manager.API.Token;
using Manager.API.Utilities;
using Manager.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Manager.API.Controllers;

[ApiController]
[Route("/api/v1/auth/")]
public class AuthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ITokenGenerator _tokenGenerator;

    public AuthController(IConfiguration configuration, ITokenGenerator tokenGenerator)
    {
        _configuration = configuration;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost]
    [Route("login")]
    public IActionResult Login([FromBody] LoginViewModel model)
    {
        try
        {
            //Aqui é o login e senha do usuario mas não tem isso no banco
            var tokenLogin = _configuration["Jwt:Login"];
            var tokenPassword = _configuration["Jwt:Password"];

            if(model.Login == tokenLogin && model.Password == tokenPassword) 
            {
                return Ok(new ResultViewModel
                {
                   
                    Sucess = true,
                    Data = new
                    {
                        Token = _tokenGenerator.GenerateToken(),
                        TokenExpires = DateTime.UtcNow.AddHours(int.Parse(
                             _configuration["Jwt:HoursToExpire"]))
                    }
                });
            }
            else
            {
                return StatusCode(401, Responses.UnauthorizedErrorMessage());
            }
        }
        catch (Exception)
        {

            return StatusCode(500, Responses.ApplicationErrorMessage());
        }
    }
}
