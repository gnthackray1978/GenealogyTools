namespace FTMContext
{
    public class ChildRelationshipSubset
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int RelationshipId { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}
