using FTMContext;
using System.Collections.Generic;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using QuickGed.Types;

namespace FTMContextNet.Domain.Entities.Persistent.Cache
{
    public partial class FTMPersonView
    {
        public static FTMPersonView Create(int idCounter, int importId, PersonSubset person) {
            var fTmPersonView = new FTMPersonView
            {
                Id = idCounter,
                PersonId = person.Id,
                BirthFrom = person.BirthYearFrom,
                BirthTo = person.BirthYearTo,
                AltLat = 0.0,
                AltLocation = !string.IsNullOrEmpty(person.DeathLocation) ? person.DeathLocation : person.Residence,
                AltLocationDesc = !string.IsNullOrEmpty(person.DeathLocation) ? "Burial" : person.ResidenceDescription,
                AltLong = 0.0,
                BirthLocation = person.BirthLocation,
                BirthLat = 0.0,
                BirthLong = 0.0,
                FirstName = person.Forename,
                Surname = person.FamilyName,
                Origin = person.Origin,
                DirectAncestor = person.IsDirectAncestor,
                LinkedLocations = person.AllLocations(),
                FatherId = person.FatherId,
                MotherId = person.MotherId,
                ImportId = importId
            };

            return fTmPersonView;
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

        public bool DirectAncestor { get; set; }

        public int PersonId { get; set; }

        public int FatherId { get; set; }

        public int MotherId { get; set; }

        public string LinkedLocations { get; set; }

        public int ImportId { get; set; }

        public bool LocationsCached { get; set; }

    }
}