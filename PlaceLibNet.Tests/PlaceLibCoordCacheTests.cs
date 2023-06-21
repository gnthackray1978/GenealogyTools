using System.Security.Cryptography.X509Certificates;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Tests;

public class PlaceLookupCacheTests
{
    [Fact]
    public void PlaceLookup_ShouldBeCorrect_WhenPlaceSearchedFor()
    {
        var placeLookups = new List<PlaceLookup>()
        {
            new() { PlaceId = 0, Place = @"/Coton, Cambridgeshire|//", PlaceFormatted = @"Coton/Cambridgeshire", Lat ="1", Lng = "2"},
            new() { PlaceId = 1, Place = @"//Beswick/Lancashire//England/1668440/0.9332857/-0.03845536", PlaceFormatted = @"Ringstead/Northamptonshire/England", Lat ="3", Lng = "4"},
            new() { PlaceId = 2, Place = @"/Chorlton-upon-Medlock, Manchester, Lancashire, England|//", PlaceFormatted = @"ChorltonuponMedlock/Manchester/Lancashire/England", Lat ="5", Lng = "6"}
        };
        
        var plc = new PlaceLookupCache(placeLookups, new PlaceNameFormatter());

        var f = plc.Search("Ringstead, Northamptonshire, England");

        f.PlaceId.Should().Be(1);
    }

    [Fact]
    public void Place_ShouldBeFound_WhenPlaceSearchedFor()
    {
        var placeLookups = new List<PlaceLookup>()
        {
            new() { PlaceId = 0, Place = @"/Coton, Cambridgeshire|//", PlaceFormatted = @"Coton/Cambridgeshire", Lat ="1", Lng = "2"},
            new() { PlaceId = 1, Place = @"//Beswick/Lancashire//England/1668440/0.9332857/-0.03845536", PlaceFormatted = @"Ringstead/Northamptonshire/England", Lat ="3", Lng = "4"},
            new() { PlaceId = 2, Place = @"/Chorlton-upon-Medlock, Manchester, Lancashire, England|//", PlaceFormatted = @"ChorltonuponMedlock/Manchester/Lancashire/England", Lat ="5", Lng = "6"}
        };

        var plc = new PlaceLookupCache(placeLookups, new PlaceNameFormatter());

        var f = plc.Exists("Ringstead, Northamptonshire, England");

        f.Should().Be(true);
    }
}

public class PlaceLibCoordCacheTests
{
    [Fact]
    public void String_ShouldBeCorrectCounty_WhenTownSearchedFor()
    {
        var places = new List<PlaceSearchCoordSubset>()
        {
            new() { Id = 0, Placesort = "hunsingore", Ctyhistnm = "Lincolnshire", Lat ="1", Long = "2"},
            new() { Id = 1, Placesort = "hunsingore", Ctyhistnm = "Yorkshire", Lat ="3", Long = "4"},
            new() { Id = 2, Placesort = "fredblogs", Ctyhistnm = "Lincolnshire", Lat ="5", Long = "6"}

        };

        var counties = new List<CountyDto>()
        {
            new (){Country = "",County = "yorkshire"}
        };

        var cs = new PlaceLibCoordCache(places, counties, new PlaceNameFormatter());

        List<string> searchTargets = new List<string>() 
            { "tockwith", "hunsingore" };


        var y = cs.Search(searchTargets, "Yorkshire");


        y.Id.Should().Be(1);
    }

    [Fact]
    public void Exception_ShouldBeThrown_WhenCountiesAllLowercase()
    {
        var places = new List<PlaceSearchCoordSubset>()
        {
            new() { Id = 0, Placesort = "hunsingore", Ctyhistnm = "Yorkshire", Lat ="1", Long = "2"},
            new() { Id = 1, Placesort = "york", Ctyhistnm = "Yorkshire", Lat ="3", Long = "4"},
            new() { Id = 2, Placesort = "fredblogs", Ctyhistnm = "Lincolnshire", Lat ="5", Long = "6"}

        };

        var counties = new List<CountyDto>()
        {
            new (){Country = "",County = "Yorkshire"}
        };
        
        Action a = () => new PlaceLibCoordCache(places, counties, new PlaceNameFormatter());       // null is an invalid argument
        
        a.Should().Throw<InvalidDataException>().WithMessage("Counties collection should be all lowercase");
    }


    [Fact]
    public void String_ShouldBeExpectedValue_WhenSearchedForValuePresent()
    {
        var places = new List<PlaceSearchCoordSubset>()
        {
            new() { Id = 0, Placesort = "hunsingore", Ctyhistnm = "Yorkshire", Lat ="1", Long = "2"},
            new() { Id = 1, Placesort = "york", Ctyhistnm = "Yorkshire", Lat ="3", Long = "4"},
            new() { Id = 2, Placesort = "fredblogs", Ctyhistnm = "Lincolnshire", Lat ="5", Long = "6"}
        };

        var counties = new List<CountyDto>()
        {
            new (){Country = "",County = "yorkshire"}
        };


        var searchTargets = new List<string>()
            { "tockwith", "hunsingore" };

        var x = new PlaceLibCoordCache(places, counties, new PlaceNameFormatter());  

        var y = x.Search(searchTargets, "yorkshire");

        y.Id.Should().Be(0);
    }
}