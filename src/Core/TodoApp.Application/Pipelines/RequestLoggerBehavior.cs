using MediatR;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using TodoApp.Application.Extensions;
using TodoApp.Application.Logging;

namespace TodoApp.Application.Pipelines
{
    public class RequestLoggerBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<RequestLoggerBehavior<TRequest, TResponse>> _logger;

        public RequestLoggerBehavior(ILogger<RequestLoggerBehavior<TRequest, TResponse>> logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                var ignoreRequestLogging = typeof(TRequest).ShouldIgnoreRequestLogging();

                if (!ignoreRequestLogging)
                {
                    ExecutingRequest(request);
                }

                var response = await next();

                if (!ignoreRequestLogging)
                {
                    ExecutedRequest(response);
                }

                return response;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ExecutedRequestFailed(ex);

                throw;
            }
        }

        private void ExecutingRequest(TRequest request)
        {
            _logger
                .ForContext("Data", JsonConvert.SerializeObject(request))
                .Debug("[Request<{RequestType}>] Request executing", typeof(TRequest).GetRequestName());
        }

        private void ExecutedRequest(TResponse response)
        {
            _logger
                .ForContext("Data", JsonConvert.SerializeObject(response))
                .Debug("[Request<{RequestType}>] Request executed", typeof(TRequest).GetRequestName());
        }

        private void ExecutedRequestFailed(Exception ex)
        {
            _logger
                .Error(ex, "[Request<{RequestType}>] Request executed", typeof(TRequest).GetRequestName());
        }
    }
}
