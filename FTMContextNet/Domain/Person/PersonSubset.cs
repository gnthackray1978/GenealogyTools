namespace FTMContext
{
    public class PersonSubset
    {
        
        public int Id { get; set; }

        public string Sex { get; set; }
   
        public string BirthDate { get; set; }
    //    public string BirthPlace { get; set; }
        public int? BirthPlaceId { get; set; }
       // public int? BirthFactId { get; set; }
        public string DeathDate { get; set; }
      //  public string DeathPlace { get; set; }
        public int? DeathPlaceId { get; set; }

        public string GivenName { get; set; }
        public string FamilyName { get; set; }
  

     
    }
}
