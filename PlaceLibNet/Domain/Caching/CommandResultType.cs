namespace PlaceLibNet.Domain.Caching;

public enum CommandResultType
{
    Success = 0,
    RecordExists = 1,
    InvalidRequest = 2,
    Unauthorized = 3
}