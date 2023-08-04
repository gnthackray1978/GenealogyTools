using System;

namespace PlaceLibNet.Domain.Caching;

public class CommandResult
{
    public string Id { get; set; }
    public string Message { get; set; }
    public CommandResultType CommandResultType { get; set; }


    public CommandResult()
    {
    }

    private CommandResult(string failureReason, string id)
    {
        FailureReason = failureReason;
        Id = id;
    }

    //public static CommandResult Success { get; } = new CommandResult();

    public string FailureReason { get; }

    public bool IsSuccess => CommandResultType == CommandResultType.Success;


    public static implicit operator bool(CommandResult result)
    {
        if (result == null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        return result.IsSuccess;
    }


    public static CommandResult SuccessWithId(string id)
    {
        return new CommandResult(string.Empty, id);
    }

    public static CommandResult Success()
    {
        return new CommandResult()
        {
            CommandResultType = CommandResultType.Success
        };
    }

    public static CommandResult Fail(string reason)
    {
        return new CommandResult(reason, string.Empty);
    }

    public static CommandResult Fail(CommandResultType reason, string message = "")
    {
        return new CommandResult()
        {
            CommandResultType = reason,
            Message = message
        };
    }

    public bool ToBoolean()
    {
        return IsSuccess;
    }
}