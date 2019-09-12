using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class SourceTable
    {
        [Column("SourceID")]
        public int SourceId { get; set; }
        public string Name { get; set; }
        public string RefNumber { get; set; }
        public string ActualText { get; set; }
        public string Comments { get; set; }
        public int? IsPrivate { get; set; }
        [Column("TemplateID")]
        public int? TemplateId { get; set; }
        public string Fields { get; set; }
    }
}
