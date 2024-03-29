﻿using MediatR;
using MSG.CommonTypes;

namespace FTMContextNet.Domain.Commands;

public class CreateImportCommand : IRequest<CommandResult>
{

    public CreateImportCommand(string fileName, string fileSize, bool selected)
    {
        FileName = fileName;
        FileSize = fileSize;
        Selected = selected;
    }

    public string FileName { get; private set; }

    public string FileSize { get; private set; }

    public bool Selected { get; private set; }
}