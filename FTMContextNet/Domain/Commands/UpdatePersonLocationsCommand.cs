using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class UpdatePersonLocationsCommand : IRequest<CommandResult> { }