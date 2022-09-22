using FTMContext;
using System.Collections.Generic;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class FTMPersonView
    {
        public static FTMPersonView Create(int idCounter,int personId, string foreName, string surname,
                                    ProcessDateReturnType processDateReturn, 
                                    ProcessLocationReturnType associatedLocationData, 
                                    List<int> parents, string origin) {
            var fTMPersonView = new FTMPersonView()
            {
                Id = idCounter,
                PersonId = personId,
                BirthFrom = processDateReturn.YearFrom,
                BirthTo = processDateReturn.YearTo,
                AltLat = associatedLocationData.AltLocationLat,
                AltLocation = associatedLocationData.AltLocation,
                AltLocationDesc = "n/a",
                AltLong = associatedLocationData.AltLocationLong,
                BirthLocation = associatedLocationData.BirthLocation,
                BirthLat = associatedLocationData.BirthLocationLat,
                BirthLong = associatedLocationData.BirthLocationLong,
                FirstName = foreName,
                Surname = surname,
                Origin = origin,
                LinkedLocations = associatedLocationData.LocationString,
                FatherId = parents[0],
                MotherId = parents[1]
            };

            return fTMPersonView;
        }

        public int Id { get; set; }

        public string FirstName { get; set; }
        public string Surname { get; set; }

        public int BirthFrom { get; set; }
        public int BirthTo { get; set; }

        public string BirthLocation { get; set; }
        public double BirthLat { get; set; }
        public double BirthLong { get; set; }

        public string AltLocationDesc { get; set; }
        public string AltLocation { get; set; }
        public double AltLat { get; set; }
        public double AltLong { get; set; }

        public string Origin { get; set; }

        public int PersonId { get; set; }

        public int FatherId { get; set; }

        public int MotherId { get; set; }

        public string LinkedLocations { get; set; }
    }
}