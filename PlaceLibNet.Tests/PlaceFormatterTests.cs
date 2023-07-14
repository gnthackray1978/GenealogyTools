using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Tests;

public class PlaceFormatterTests
{
    [Fact]
    public void String_ShouldBeCorrectFormat_WhenFormatCalled()
    {
        var cs = new PlaceNameFormatter();

        var x = cs.Format("fred");

        x.Should().Be("fred");
    }
}