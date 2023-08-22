using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class DeleteImportCommand : IRequest<CommandResult>
{
    public DeleteImportCommand(int importId)
    {
        ImportId = importId;
    }

    public int ImportId { get; private set; }
}