using MediatR;
using Telegram.Bot.Types;

namespace DrakkarVpn.Bot.Application.Features.RegionCallback;

public sealed record RegionCallbackCommand(CallbackQuery CallbackQuery) : IRequest<Unit>;
