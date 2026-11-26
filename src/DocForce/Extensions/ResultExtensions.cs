using DocForce.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace DocForce.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
        => result.IsSuccess
            ? new NoContentResult()
            : result.Error!.ToActionResult();

    public static IActionResult ToActionResult<T>(this Result<T> result)
        => result.IsSuccess
            ? new OkObjectResult(result.Value)
            : result.Error!.ToActionResult();

    public static IActionResult ToActionResult(this BaseError error)
    {
        var statusCode = error.ErrorType switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
            ErrorType.BadRequest => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError,
        };

        var problemDetails = new ProblemDetails
        {
            Title = error.Code,
            Detail = error.Description,
            Status = statusCode
        };

        if (error.Metadata is not null)
            foreach (var (key, value) in error.Metadata)
                problemDetails.Extensions[key] = value;

        return new ObjectResult(problemDetails) { StatusCode = statusCode };
    }
}
