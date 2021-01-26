using System;
using System.Collections.Generic;

namespace FTMContext.Models
{
    public partial class SyncPerson
    {
        public SyncPerson()
        {
            SyncCitation = new HashSet<SyncCitation>();
            SyncFact = new HashSet<SyncFact>();
            SyncFactDelete = new HashSet<SyncFactDelete>();
            SyncRelationship = new HashSet<SyncRelationship>();
            SyncWeblink = new HashSet<SyncWeblink>();
        }

        public long FtmId { get; set; }
        public string AmtId { get; set; }
        public string FamilySearchId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }

        public virtual ICollection<SyncCitation> SyncCitation { get; set; }
        public virtual ICollection<SyncFact> SyncFact { get; set; }
        public virtual ICollection<SyncFactDelete> SyncFactDelete { get; set; }
        public virtual ICollection<SyncRelationship> SyncRelationship { get; set; }
        public virtual ICollection<SyncWeblink> SyncWeblink { get; set; }
    }
}
