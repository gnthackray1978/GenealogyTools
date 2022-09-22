using System.Collections.Generic;

namespace FTMContextNet.Application.Models.Read
{
    public class InfoModel
    {
        public int Unsearched { get; set; }

        public int NotFound { get; set; }

        public int BadLocationsCount { get; set; }

        public int PlacesCount { get; set; }

        public int DupeEntryCount { get; set; }

        public int TreeRecordCount { get; set; }

        public int PersonViewCount { get; set; }

        public int OriginMappingCount { get; set; }

        public int MarriagesCount { get; set; }

        public List<string> Results { get; set; }
    }
}
