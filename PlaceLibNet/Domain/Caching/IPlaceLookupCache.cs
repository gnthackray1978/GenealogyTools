using System.Collections.Generic;
using PlaceLibNet.Domain.Entities;

namespace PlaceLibNet.Domain.Caching;

public interface IPlaceLookupCache
{
    List<PlaceLookup> PlaceLookups { get; set; }
    bool Exists(string place);
    PlaceLookup Search(string place);
}