namespace FTMContextNet.Domain.Entities.Source
{
    public partial class Person
    {
        public Person()
        {
        }

        public int Id { get; set; }
        //     public Guid PersonGuid { get; set; }
        //   public int? NameFactId { get; set; }
        public string Sex { get; set; }
        //      public int? SexFactId { get; set; }
        public string BirthDate { get; set; }
        public string BirthPlace { get; set; }
        public int? BirthPlaceId { get; set; }
        public int? BirthFactId { get; set; }
        public string DeathDate { get; set; }
        public string DeathPlace { get; set; }
        public int? DeathPlaceId { get; set; }
        //  public int? DeathFactId { get; set; }
        //    public string MarriageDate { get; set; }
        //   public string MarriagePlace { get; set; }
        //   public int? MarriagePlaceId { get; set; }
        //    public int? MarriageFactId { get; set; }
        //    public int? PreferredRelId { get; set; }
        //    public int? PreferredMediaFileId { get; set; }
        //   public DateTime CreateDate { get; set; }
        //   public DateTime UpdateDate { get; set; }
        public string Title { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string NameSuffix { get; set; }
        public string FullName { get; set; }
        //  public string FullNameReversed { get; set; }
        //  public string Uid { get; set; }

    }
}
