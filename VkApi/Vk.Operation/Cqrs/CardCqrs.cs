using MediatR;
using Vk.Base.Response;
using Vk.Schema;

namespace Vk.Operation;

public record CreateCardCommand(CardRequest Model) : IRequest<ApiResponse<CardResponse>>;
public record UpdateCardCommand(CardRequest Model,int Id) : IRequest<ApiResponse>;
public record DeleteCardCommand(int Id) : IRequest<ApiResponse>;
public record GetAllCardQuery() : IRequest<ApiResponse<List<CardResponse>>>;
public record GetCardByIdQuery(int Id) : IRequest<ApiResponse<CardResponse>>;
public record GetCardByCustomerIdQuery(int CustomerId) : IRequest<ApiResponse<List<CardResponse>>>;
public record GetCardByAccountIdQuery(int AccountId) : IRequest<ApiResponse<CardResponse>>;