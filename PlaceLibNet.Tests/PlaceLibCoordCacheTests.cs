using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using FTMContextNet.Data.Repositories;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Tests;

public class PlaceLookupCacheTests
{
    [Fact]
    public void PlaceLookup_ShouldBeCorrect_WhenPlaceSearchedFor()
    {

        var mockPersistedCacheRepository = new Mock<IPlaceRepository>();
        
        var placeLookups = new List<PlaceLookup>()
        {
            new() { PlaceId = 0, Place = @"/Coton, Cambridgeshire|//", PlaceFormatted = @"Coton/Cambridgeshire", Lat ="1", Lng = "2"},
            new() { PlaceId = 1, Place = @"//Beswick/Lancashire//England/1668440/0.9332857/-0.03845536", PlaceFormatted = @"Ringstead/Northamptonshire/England", Lat ="3", Lng = "4"},
            new() { PlaceId = 2, Place = @"/Chorlton-upon-Medlock, Manchester, Lancashire, England|//", PlaceFormatted = @"ChorltonuponMedlock/Manchester/Lancashire/England", Lat ="5", Lng = "6"}
        };


        mockPersistedCacheRepository
            .Setup(s => s.GetCachedPlaces())
            .Returns(placeLookups);

        var plc = new PlaceLookupCache(mockPersistedCacheRepository.Object, new PlaceNameFormatter());
        
        plc.Load();

        var f = plc.Search("Ringstead, Northamptonshire, England");

        f.PlaceId.Should().Be(1);
    }

    [Fact]
    public void Place_ShouldBeFound_WhenPlaceSearchedFor()
    {
        var mockPersistedCacheRepository = new Mock<IPlaceRepository>();
        
        var placeLookups = new List<PlaceLookup>()
        {
            new() { PlaceId = 0, Place = @"/Wood Ditton, Cambridgeshire, England|//", PlaceFormatted = @"Wood Ditton/Cambridgeshire/England", Lat ="1", Lng = "2"},
            new() { PlaceId = 1, Place = @"Woodditton/Cambridgeshire//England/1657933/0.9110134/0.007480481", PlaceFormatted = @"WoodDitton/Cambridge/England", Lat ="3", Lng = "4"},
            new() { PlaceId = 2, Place = @"/Wood Ditton, Cambridge, England |//", PlaceFormatted = @"Woodditton/Cambridgeshire/England", Lat ="5", Lng = "6"},
            new() { PlaceId = 0, Place = @"/Wood Ditton, Cambridgeshire, England|//", PlaceFormatted = @"Wood Ditton/Cambridge/England", Lat ="1", Lng = "2"},
            new() { PlaceId = 1, Place = @"/ Woodditton, Cambridge, England |//", PlaceFormatted = @"Wood Ditton", Lat ="3", Lng = "4"},
            new() { PlaceId = 2, Place = @"Woodditton, Cambridgeshire, England", PlaceFormatted = @"Woodditton/Cambridgeshire/England", Lat ="5", Lng = "6"},
            new() { PlaceId = 2, Place = @"Wood Ditton, Cambridge, England", PlaceFormatted = @"Wood Ditton/Cambridge/England", Lat ="5", Lng = "6"}
            
        };

        mockPersistedCacheRepository
            .Setup(s => s.GetCachedPlaces())
            .Returns(placeLookups);

        var plc = new PlaceLookupCache(mockPersistedCacheRepository.Object, new PlaceNameFormatter());

        plc.Load();

        var testLocations = new List<string>()
        {
            //"Woodditton, Cambridgeshire, England",
            //"Wood Ditton, Cambridge, England",
            //"Wood Ditton, Cambridge, England",
            //"Wood Ditton, Cambridgeshire, England",
            //"Wood Ditton, Cambridge, England",
            //"Wood Ditton, Cambridgeshire, England",
            //"Wood Ditton, Cambridge, England",
            //"Wood Ditton, Cambridgeshire, England",
            "Wood Ditton, Cambridge, England",
            "Wood Ditton, Cambridge, England",
            "Wood Ditton, Cambridge, England",
            "Wood Ditton, Cambridge, England",
            "Wood Ditton, Cambridge, England",
            "Wood Ditton",
            "Wood Ditton, Cambridge, England",
            "Wood Ditton, Cambridgeshire, England",
            "Woodditton, Cambridge, England"
        };

        foreach (var l in testLocations)
        {
            TestLocation(plc, l);
        }

       
    }

    private static void TestLocation(PlaceLookupCache plc, string test)
    {
        var f = plc.Exists(test);

        if (!f)
        {
            Debug.WriteLine("x");
        }

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

        var mockPersistedCacheRepository = new Mock<IPlaceRepository>();

        mockPersistedCacheRepository
            .Setup(s => s.GetCounties(true))
            .Returns(counties);

        mockPersistedCacheRepository
            .Setup(s => s.GetPlaceLibCoords())
            .Returns(places);

        var cs = new PlaceLibCoordCache(mockPersistedCacheRepository.Object, new PlaceNameFormatter());

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

        var mockPersistedCacheRepository = new Mock<IPlaceRepository>();

        mockPersistedCacheRepository
            .Setup(s => s.GetCounties(true))
            .Returns(counties);

        mockPersistedCacheRepository
            .Setup(s => s.GetPlaceLibCoords())
            .Returns(places);

        Action a = () => new PlaceLibCoordCache(mockPersistedCacheRepository.Object, new PlaceNameFormatter());       // null is an invalid argument
        
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

        var mockPersistedCacheRepository = new Mock<IPlaceRepository>();

        mockPersistedCacheRepository
            .Setup(s => s.GetCounties(true))
            .Returns(counties);

        mockPersistedCacheRepository
            .Setup(s => s.GetPlaceLibCoords())
            .Returns(places);

        var searchTargets = new List<string>()
            { "tockwith", "hunsingore" };

        var x = new PlaceLibCoordCache(mockPersistedCacheRepository.Object, new PlaceNameFormatter());  

        var y = x.Search(searchTargets, "yorkshire");

        y.Id.Should().Be(0);
    }


    [Fact]
    public void String_ShouldBeExpectedValue_WhenSearchedForCountyValuePresent()
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

        var mockPersistedCacheRepository = new Mock<IPlaceRepository>();

        mockPersistedCacheRepository
            .Setup(s => s.GetCounties(true))
            .Returns(counties);

        mockPersistedCacheRepository
            .Setup(s => s.GetPlaceLibCoords())
            .Returns(places);

        var searchTargets = new List<string>()
            { "tockwith", "hunsingore" };

        var x = new PlaceLibCoordCache(mockPersistedCacheRepository.Object, new PlaceNameFormatter());

        var y = x.Search(searchTargets, "tockwith, yorkshire, england");

        y.Id.Should().Be(0);
    }
}