namespace PlaceLibNet.Domain.Entities;

public class PlaceLocationDto
{
    public int Id { get; set; }
    public string BirthLat { get; set; }
    public string BirthLong { get; set; }
    public string AltLat { get; set; }
    public string AltLong { get; set; }
}