namespace FTMContextNet.Domain.Entities.Source
{


    public partial class Person 
    {
        public Person()
        {
        }

        public int Id { get; set; }
        public string Sex { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public int? BirthPlaceId { get; set; }
        public int? BirthFactId { get; set; }
        public string DeathDate { get; set; }
        public string DeathPlace { get; set; }
        public int? DeathPlaceId { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string NameSuffix { get; set; }
        public string FullName { get; set; }

    }
}
