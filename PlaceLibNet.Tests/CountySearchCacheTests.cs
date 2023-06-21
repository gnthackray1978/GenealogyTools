using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Tests
{

    //PlaceLibCoordCache


    public class CountySearchCacheTests
    {
        [Fact]
        public void String_ShouldBeCorrectCounty_WhenTownSearchedFor()
        {
            var places = new List<Places>()
            {
                new(){Id = 0,Placesort = "stafford",Ctyhistnm = "Staffordshire", Place15nm ="Staffordshire"},
                new(){Id = 1,Placesort = "birmingham",Ctyhistnm = "Warwickshire", Place15nm ="Birmingham"},
                new(){Id = 2,Placesort = "fredblogs",Ctyhistnm = "Lincolnshire", Place15nm ="Fred Blogs"},

            };

            var cs = new CountySearch(places,new PlaceNameFormatter());

            var x = cs.Search("Birmingham");

            x.Should().Be("Warwickshire");
        }

        [Fact]
        public void String_ShouldBeCorrectCounty_WhenMultiPartTownSearchedFor()
        {
            var places = new List<Places>()
            {
                new(){Id = 0,Placesort = "stafford",Ctyhistnm = "Staffordshire", Place15nm ="Staffordshire"},
                new(){Id = 1,Placesort = "birmingham",Ctyhistnm = "Warwickshire", Place15nm ="Birmingham"},
                new(){Id = 2,Placesort = "fredblogs",Ctyhistnm = "Lincolnshire", Place15nm ="Fred Blogs"},

            };

            var cs = new CountySearch(places, new PlaceNameFormatter());

            var x = cs.Search("Fred Blogs");

            x.Should().Be("Lincolnshire");
        }

        [Fact]
        public void String_ShouldBeCorrectCounty_WhenLowerCaseMultiPartTownSearchedFor()
        {
            var places = new List<Places>()
            {
                new(){Id = 0,Placesort = "stafford",Ctyhistnm = "Staffordshire", Place15nm ="Staffordshire"},
                new(){Id = 1,Placesort = "birmingham",Ctyhistnm = "Warwickshire", Place15nm ="Birmingham"},
                new(){Id = 2,Placesort = "fredblogs",Ctyhistnm = "Lincolnshire", Place15nm ="Fred Blogs"},

            };

            var cs = new CountySearch(places, new PlaceNameFormatter());

            var x = cs.Search("fred blogs");

            x.Should().Be("Lincolnshire");
        }
    }
}