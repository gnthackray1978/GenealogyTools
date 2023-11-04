using System;

namespace AzureContext.Models;

public partial class PlaceCache
{
    public int Id { get; set; }

    public int AltId { get; set; }

    public string Name { get; set; }
    public string NameFormatted { get; set; }
    public string JSONResult { get; set; }
    public string County { get; set; }
    public string Country { get; set; }
    public bool Searched { get; set; }

    public bool BadData { get; set; }

    public string Lat { get; set; }
    public string Long { get; set; }

    public string Src { get; set; }

    public DateTime DateCreated { get; set; }

}