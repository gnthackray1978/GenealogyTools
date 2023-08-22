using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class UpdateImportStatusCommand : IRequest<CommandResult>
{
    public UpdateImportStatusCommand(int importId)
    {
        ImportId = importId;
    }

    public int ImportId { get; private set; }
}