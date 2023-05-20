using QuickGed.Types;

namespace QuickGed.Services;

public class AgeRangeCalculator
{
    private static DupeAgeInfo GetFirstChildBirth(Person person)
    {
        var list = new List<DupeAgeInfo>();

        // var children = Persons.Where(p => (p.FatherId == personId || p.MotherId == personId) && p.BirthYearFrom > 0).MinBy(o => o.BirthYearFrom);

        var children = person
            .Children
            .Cast<Person>()
            .Where(w => w.BirthYearFrom > 0)
            .MinBy(o => o.BirthYearFrom);


        if (children is { BirthYearFrom: > 0 })
        {
            list.Add(new DupeAgeInfo()
            {
                Location = children.BirthLocation,
                Type = DupeAgeInfoTypes.FirstChildBirthBap,
                Year = children.BirthYearFrom
            });
        }

        return list.Count > 0 ? list.MinBy(o => o.Year) : null;
    }

    private static DupeAgeInfo GetDeathDate(Person person)
    {
        List<DupeAgeInfo> list = new List<DupeAgeInfo>();


        if (person != null && person.DeathYear > 0)
        {
            list.Add(new DupeAgeInfo()
            {
                Location = person.DeathLocation,
                Type = DupeAgeInfoTypes.Death,
                Year = person.DeathYear
            });
        }


        if (list.Count > 0)
        {
            return list.MinBy(o => o.Year);
        }
        else
        {
            return null;
        }
    }

    public static ProcessDateReturnType GetPersonBirthDateRange(Person person)
    {
        var processDateReturnType = new ProcessDateReturnType();

        var fcb = GetFirstChildBirth(person);

        if (fcb != null)
        {
            processDateReturnType.YearFrom = fcb.Year - 50;
            processDateReturnType.YearTo = fcb.Year - 18;
            processDateReturnType.RangeString = (fcb.Year - 50).ToString() + " " + (fcb.Year - 18).ToString();

            return processDateReturnType;

        }


        var dd = GetDeathDate(person);

        if (dd != null)
        {
            processDateReturnType.YearFrom = dd.Year - 75;
            processDateReturnType.YearTo = dd.Year;
            processDateReturnType.RangeString = (dd.Year - 75).ToString() + " " + dd.Year.ToString();

            return processDateReturnType;

        }

        return processDateReturnType;
    }
}