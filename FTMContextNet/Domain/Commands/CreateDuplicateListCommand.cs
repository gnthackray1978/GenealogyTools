using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class CreateDuplicateListCommand : IRequest<CommandResult> { }