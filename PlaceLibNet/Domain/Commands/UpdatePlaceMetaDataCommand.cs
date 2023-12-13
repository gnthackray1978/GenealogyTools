using MediatR;
using MSG.CommonTypes;

namespace PlaceLibNet.Domain.Commands;

/// <summary>
/// Update place cache lat long
/// Update place cache county
/// Update place cache bad data where appropriate
/// </summary>
public class UpdatePlaceMetaDataCommand : IRequest<CommandResult>
{
}