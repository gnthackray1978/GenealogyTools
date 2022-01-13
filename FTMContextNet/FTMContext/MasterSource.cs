using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class MasterSource
    {
        public MasterSource()
        {
            Source = new HashSet<Source>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string PublisherName { get; set; }
        public string PublisherLocation { get; set; }
        public string PublishDate { get; set; }
        public int? RepositoryId { get; set; }
        public string CallNumber { get; set; }
        public string Comments { get; set; }
        public string Pid { get; set; }
        public int SourceType { get; set; }
        public string TemplateData { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public string Uid { get; set; }

        public virtual Repository Repository { get; set; }
        public virtual ICollection<Source> Source { get; set; }
    }
}
