using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    [Table("URLTable")]
    public partial class Urltable
    {
        [Column("LinkID")]
        public int LinkId { get; set; }
        public int? OwnerType { get; set; }
        [Column("OwnerID")]
        public int? OwnerId { get; set; }
        public int? LinkType { get; set; }
        public string Name { get; set; }
        [Column("URL")]
        public string Url { get; set; }
        public string Note { get; set; }
    }
}
