using System;
using FluentValidation;
using MediatR;

namespace Application.Core;


public class ValidationBehavior<TRequest, TResponse> (IValidator<TRequest> validators = null) :
   IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    //private readonly IEnumerable<IValidator<TRequest>> _validators;

   // public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
   // {
   //     _validators = validators;
   // }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(validators == null)
            return await next();

          //  var context = new ValidationContext<TRequest>(request);
            var validationResults = await validators.ValidateAsync(request, cancellationToken);
          
        if (validationResults.IsValid)
        {
            throw new ValidationException(validationResults.Errors);
        }
           
           return await next();
    }
}