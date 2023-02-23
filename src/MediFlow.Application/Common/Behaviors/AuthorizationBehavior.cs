using MediatR;
using MediFlow.Application.Common.Exceptions;
using MediFlow.Application.Common.Interfaces;
using MediFlow.Application.Common.Security;

namespace MediFlow.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    
    public AuthorizationBehavior(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), true);
        
        if (authorizeAttributes.Any())
        {
            if (!_currentUserService.IsAuthenticated)
                throw new UnauthorizedException();
                
            var authorizeAttribute = (AuthorizeAttribute)authorizeAttributes.First();
            
            if (!string.IsNullOrEmpty(authorizeAttribute.Roles))
            {
                var roles = authorizeAttribute.Roles.Split(',');
                var hasRole = roles.Any(role => _currentUserService.IsInRole(role.Trim()));
                
                if (!hasRole)
                    throw new ForbiddenAccessException();
            }
        }
        
        return await next();
    }
}