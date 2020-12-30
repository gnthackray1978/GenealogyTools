using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncState
    {
        public int Id { get; set; }
        public Guid FileId { get; set; }
        public Guid FtmId { get; set; }
        public int TreeId { get; set; }
        public string TreeName { get; set; }
        public int TreeRights { get; set; }
        public int LocaleId { get; set; }
        public DateTime TreeModified { get; set; }
        public int SyncStatus { get; set; }
        public int SyncType { get; set; }
        public DateTime SyncStart { get; set; }
        public DateTime SyncEnd { get; set; }
        public DateTime LastSync { get; set; }
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSyncing { get; set; }
        public int HintsSyncState { get; set; }
    }
}
