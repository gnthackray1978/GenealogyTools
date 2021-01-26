using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncMedia
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string State { get; set; }
        public int Attempts { get; set; }
        public Guid? Amtid { get; set; }
        public DateTime? LastAttemptDate { get; set; }
        public string DownloadUrl { get; set; }
        public Guid? UserDocStoreFileId { get; set; }
        public int? FtmMediaId { get; set; }
        public string EndpointType { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
