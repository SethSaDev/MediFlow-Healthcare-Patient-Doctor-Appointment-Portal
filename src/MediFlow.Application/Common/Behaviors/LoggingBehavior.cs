using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MediFlow.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();
        
        _logger.LogInformation("Processing {RequestName} request", requestName);
        
        try
        {
            var response = await next();
            stopwatch.Stop();
            
            _logger.LogInformation("Completed {RequestName} request in {ElapsedMilliseconds}ms", 
                requestName, stopwatch.ElapsedMilliseconds);
                
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing {RequestName} request", requestName);
            throw;
        }
    }
}