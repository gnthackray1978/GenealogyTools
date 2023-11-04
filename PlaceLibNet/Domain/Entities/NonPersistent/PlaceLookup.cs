namespace PlaceLibNet.Domain.Entities
{
    public class PlaceLookup
    {
        public int PlaceId { get; set; }
        public string Place { get; set; }

        public string Lat { get; set; }
        public string Lng { get; set; }

        public string PlaceFormatted { get; set; }
        public string Results { get; set; }
    }
}
