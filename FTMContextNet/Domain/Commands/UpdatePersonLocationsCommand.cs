using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

/// <summary>
/// Update latitude and longitude properties of the frmperson table
/// </summary>
public class UpdatePersonLocationsCommand : IRequest<CommandResult> { }