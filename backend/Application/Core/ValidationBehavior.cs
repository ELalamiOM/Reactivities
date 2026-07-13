using FluentValidation;
using MediatR;

namespace Application.Core;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? validator = null)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validator == null)
            return await next();

        var validationResults = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResults.IsValid)
        {
            throw new ValidationException(validationResults.Errors);
        }

        return await next();
    }
}