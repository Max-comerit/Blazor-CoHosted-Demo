using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Responses;
public abstract class ApiBaseResponse
{
    public bool Success { get; set; }
    protected ApiBaseResponse(bool success) => Success = success;

    public TResultType GetOkResult<TResultType>()
    {
        if(this is ApiOkResponse<TResultType> apiOkResponse)
        {
            return apiOkResponse.Result;
        }

        throw new InvalidOperationException($"Response type {this.GetType().Name} is not ApiOkRespone");
    }
}

public sealed class ApiOkResponse<TResult> : ApiBaseResponse
{
    public TResult Result { get; set; }
    public ApiOkResponse(TResult result) : base(true) 
    {
        Result = result;
    }

}

public abstract class ApiNotFoundResponse : ApiBaseResponse
{
    public string Message { get; }
    protected ApiNotFoundResponse(string message) : base(false)
    {
        Message = message;
    }
}

public  class CompanyNotFoundResponse : ApiNotFoundResponse
{
    public CompanyNotFoundResponse(int id) : base($"Company with {id} is not found")
    {
        
    }
}

