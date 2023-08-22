using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class CreateTreeGroupMappingsCommand : IRequest<CommandResult> { }