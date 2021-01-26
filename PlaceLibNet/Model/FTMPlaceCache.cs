namespace PlaceLib.Model
{
    public partial class FTMPlaceCache {

        public int Id { get; set; }

        public int FTMPlaceId { get; set; }

        public string FTMOrginalName { get; set; }
        public string FTMOrginalNameFormatted { get; set; }
        public string JSONResult { get; set; }

    }
}


//CREATE TABLE FTMPlaceCache(
//    Id int,
//    FTMPlaceId  int,
//    FTMOrginalName text,
//FTMOrginalNameFormatted text,
//    JSONResult text,
//    PRIMARY KEY (Id)
//);