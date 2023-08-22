using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class CreatePersonOriginListCommand : IRequest<CommandResult> { }