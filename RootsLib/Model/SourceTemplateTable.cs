using System.ComponentModel.DataAnnotations.Schema;

namespace RootsLib.Model
{
    public partial class SourceTemplateTable
    {
        [Column("TemplateID")]
        public int TemplateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? Favorite { get; set; }
        public string Category { get; set; }
        public string Footnote { get; set; }
        public string ShortFootnote { get; set; }
        public string Bibliography { get; set; }
        public string FieldDefs { get; set; }
    }
}
