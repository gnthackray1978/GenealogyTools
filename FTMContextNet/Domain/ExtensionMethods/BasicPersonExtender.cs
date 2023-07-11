using FTMContextNet.Domain.Entities.NonPersistent.Matches;
using FTMContextNet.Domain.Entities.NonPersistent.Person;

namespace FTMContextNet.Domain.ExtensionMethods;

public static class BasicPersonExtender
{
    public static Item ToItem(this PersonIdentifier value)
    {
        return new Item
        {
            PersonId = value.Id,
            Origin = value.Origin,
            YearFrom = value.BirthYearFrom
        };
    }

     
}