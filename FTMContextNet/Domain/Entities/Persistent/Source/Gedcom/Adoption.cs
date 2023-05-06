namespace FTMContextNet.Domain.Entities.Persistent.Source.Gedcom
{
    public class Adoption
    {
        public DatePlace DatePlace { get; set; }
        public string Type { get; set; }
        public string AdoptingParents { get; set; }
        public string Note { get; set; }
    }
}
