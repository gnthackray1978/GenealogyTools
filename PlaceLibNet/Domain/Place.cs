namespace PlaceLibNet.Model
{
    public partial class Place
    {
        public Place()
        {
            //Fact = new HashSet<Fact>();
            //PersonBirthPlaceNavigation = new HashSet<Person>();
            //PersonDeathPlaceNavigation = new HashSet<Person>();
            //PersonMarriagePlaceNavigation = new HashSet<Person>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        // public DateTime CreateDate { get; set; }
        //  public DateTime UpdateDate { get; set; }
        //   public string Uid { get; set; }

        //public virtual ICollection<Fact> Fact { get; set; }
        //public virtual ICollection<Person> PersonBirthPlaceNavigation { get; set; }
        //public virtual ICollection<Person> PersonDeathPlaceNavigation { get; set; }
        //public virtual ICollection<Person> PersonMarriagePlaceNavigation { get; set; }
    }
}
