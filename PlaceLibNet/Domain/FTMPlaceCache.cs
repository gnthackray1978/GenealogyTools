namespace PlaceLib.Model
{
    public partial class FtmPlaceCache {

        public int Id { get; set; }

        public int FTMPlaceId { get; set; }

        public string FTMOrginalName { get; set; }
        public string FTMOrginalNameFormatted { get; set; }
        public string JSONResult { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public bool Searched { get; set; }

        public bool BadData { get; set; }

        public string Lat { get; set; }
        public string Long { get; set; }

        public string Src { get; set; }
    }
}

 