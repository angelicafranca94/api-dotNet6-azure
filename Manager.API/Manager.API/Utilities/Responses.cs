using Manager.API.ViewModels;

namespace Manager.API.Utilities;

public static class Responses
{
    public static ResultViewModel ApplicationErrorMessage()
    {
        return new ResultViewModel
        {
            Message = "Ocorreu algum erro interno na aplicação, por favor tente mais tarde!",
            Sucess = false,
            Data = null
        };
    }
    
    public static ResultViewModel DomainErrorMessage(string message)
    {
        return new ResultViewModel
        {
            Message = message,
            Sucess = false,
            Data = null
        };
    }
    
    public static ResultViewModel DomainErrorMessage(string message, IReadOnlyCollection<string> erros)
    {
        return new ResultViewModel
        {
            Message = message,
            Sucess = false,
            Data = erros
        };
    }

    public static ResultViewModel UnauthorizedErrorMessage()
    {
        return new ResultViewModel
        {
            Message = "Login ou Senha incorreta!",
            Sucess = false,
            Data = null
        };
    }
}
