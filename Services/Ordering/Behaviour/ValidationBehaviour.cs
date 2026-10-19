using FluentValidation;
using MediatR;

namespace Ordering.Behaviour
{

    using FluentValidation.Results;

    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            this._validators = validators;
        }
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (this._validators.Any())
            {
                ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);
                //Run all the validations one by one and return the Validation Result
                ValidationResult[] validationResults = await Task.WhenAll(this._validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                //now need to check for any failures
                List<ValidationFailure> failures = validationResults.SelectMany(e => e.Errors).Where(f => f != null).ToList();
                if (failures.Count > 0)
                {
                    throw new ValidationException(failures);
                }
            }
            //On success case
            return await next();
        }
    }
}
