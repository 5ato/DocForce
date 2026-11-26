using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Domain.Common;
using DocForce.Domain.Errors;
using FluentValidation;
using System.Reflection;

namespace DocForce.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async ValueTask<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken = default)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => new ValidationFailure(f.PropertyName, f.ErrorMessage))
            .ToList();

        if (failures.Count == 0)
            return await next();

        var error = new ValidationError(failures);

        var resultType = typeof(TResponse);
        if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = resultType.GetGenericArguments()[0];
            var failureMethod = typeof(Result)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Single(m => m.Name == nameof(Result.Failure)
                            && m.IsGenericMethodDefinition
                            && m.GetParameters().Length == 1
                            && m.GetParameters()[0].ParameterType == typeof(BaseError))
                .MakeGenericMethod(innerType);

            return (TResponse)failureMethod.Invoke(null, [error])!;
        }

        return (TResponse)(object)Result.Failure(error);
    }
}
