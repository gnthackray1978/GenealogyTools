using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class CreateTreeRecordCommand : IRequest<CommandResult> { }