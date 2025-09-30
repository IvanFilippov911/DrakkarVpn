using MediatR;

namespace DrakkarVpn.Core.Api.Modules.Idempotency.Application.Abstracts;

public interface IIdempotentRequest<TResponse> : IRequest<TResponse>
{
    Guid RequestId { get; }
    string ActorKey { get; }   
    string Action { get; }    
}