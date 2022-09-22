using System;

namespace FTMContextNet.Domain.Entities.Source
{
    public partial class Fact
    {
        public Fact()
        {
        }

        public int Id { get; set; }
        public int LinkId { get; set; }
        public int LinkTableId { get; set; }
        public int FactTypeId { get; set; }
        public bool Preferred { get; set; }
        public string Date { get; set; }
        public int? PlaceId { get; set; }
        public string Text { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Status { get; set; }
        public string Uid { get; set; }

    }
}
