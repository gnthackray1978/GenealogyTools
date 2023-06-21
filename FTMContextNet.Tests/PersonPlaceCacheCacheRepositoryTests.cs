using FTMContextNet.Domain.Caching;
using PlaceLibNet.Domain;

namespace FTMContextNet.Tests;

public class PersonPlaceCacheCacheRepositoryTests
{
    [Fact]
    public void Items_ReturnsCorrectNumber_WhenSeeded()
    {
        var testData = new List<string>()
        {
            "Canada",
            "Nottingham, Nottinghamshire, England",
            "",
            "Lincolnshire, England",
            "Hucknall Torkard, Nottinghamshire, England",
            "Rumworth, Lancashire, England",
            "xxx,, Lancashire, England",
            "abc,/ Lancashire, England",
        };

        var ppc = new PersonPlaceCache(testData, new PlaceNameFormatter());

        ppc.Items.Should().HaveCount(5);
    }

    [Fact]
    public void Item_ReturnCorrectFormat_WhenSeeded()
    {
        var testData = new List<string>()
        {
            "abc,/ Lancashire, England",
            "xxx,, Lancashire, England",
            "Hucknall Torkard, Nottinghamshire, England"
        };

        var ppc = new PersonPlaceCache(testData, new PlaceNameFormatter());

        ppc.Items[0].PlaceFormatted.Should().Be("abc/Lancashire/England");
        ppc.Items[1].PlaceFormatted.Should().Be("xxx/Lancashire/England");
        ppc.Items[2].PlaceFormatted.Should().Be("Hucknall Torkard/Nottinghamshire/England");
    }

    [Fact]
    public void StatisticProperties_ReturnsCorrectNumber_WhenSeeded()
    {
        var testData = new List<string>()
        {
            "Canada",
            "Nottingham, Nottinghamshire, England",
            "Nottingham, Nottinghamshire, England",
            "",
            "Lincolnshire, England",
            "Hucknall Torkard, Nottinghamshire, England",
            "Rumworth, Lancashire, England",
            "xxx,, Lancashire, England",
            "abc,/ Lancashire, England",
        };

        var ppc = new PersonPlaceCache(testData, new PlaceNameFormatter());

        ppc.Count.Should().Be(5);

        ppc.InvalidLocationsCount.Should().Be(3);

        ppc.DuplicateLocationsCount.Should().Be(1);
    }
}