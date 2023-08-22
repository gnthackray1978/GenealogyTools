﻿using MediatR;

namespace FTMContextNet.Domain.Commands;

public class CreateImportCommand : IRequest<CommandResult>
{

    public CreateImportCommand(string fileName, double fileSize, bool selected)
    {
        FileName = fileName;
        FileSize = fileSize;
        Selected = selected;
    }

    public string FileName { get; private set; }

    public double FileSize { get; private set; }

    public bool Selected { get; private set; }
}