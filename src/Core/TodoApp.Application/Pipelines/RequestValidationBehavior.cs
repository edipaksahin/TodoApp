using FluentValidation;
using MediatR;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Extensions;
using TodoApp.Application.Logging;

namespace TodoApp.Application.Pipelines
{
    public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<RequestValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public RequestValidationBehavior(
            ILogger<RequestValidationBehavior<TRequest, TResponse>> logger,
            IEnumerable<IValidator<TRequest>> validators)
        {
            _logger = logger;
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = _validators
                .Select(validator => validator.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                _logger
                    .ForContext("RequestType", $"Request<{typeof(TRequest).GetRequestName()}>")
                    .ForContext(nameof(failures), JsonConvert.SerializeObject(failures))
                    .Debug("Failed to validate request");
                throw new ValidationException(failures);
            }

            return next();
        }
    }
}
