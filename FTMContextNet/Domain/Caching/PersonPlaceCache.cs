﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using PlaceLibNet.Domain;

namespace FTMContextNet.Domain.Caching;

public interface IPersonPlaceCache : IEnumerable<PersonPlace>
{
    List<PersonPlace> Items { get; set; }
    int Count { get; }
    int InvalidLocationsCount { get; }
    int DuplicateLocationsCount { get; }
    void InsertRange(List<string> places);

    /// <summary>
    /// Add place if valid and not already added
    /// </summary>
    /// <param name="place">Place entry from the persons table</param>
    void Insert(string place);

    IEnumerator<PersonPlace> GetEnumerator();
}

public class PersonPlaceCache :  IPersonPlaceCache
{
    private int _duplicateCount = 0;
    private int _invalidPlaces = 0;
    private IPlaceNameFormatter _placeNameFormatter;
    public List<PersonPlace> Items { get; set; }

    public int Count => Items.Count;

    public int InvalidLocationsCount => _invalidPlaces;

    public int DuplicateLocationsCount => _duplicateCount;

    /// <summary>
    /// Load with list of places from the persons table.
    /// Then format places and remove duplicates.
    /// </summary>
    /// <param name="places">Unformatted places from the persons table</param>
    /// <param name="iNameFormatter"></param>
    public PersonPlaceCache(List<string> places, IPlaceNameFormatter iNameFormatter)
    {
        _placeNameFormatter= iNameFormatter;
        InsertRange(places);
    }

    public void InsertRange(List<string> places)
    {
        foreach (var place in places)
        {
            Insert(place);
        }
    }

    /// <summary>
    /// Add place if valid and not already added
    /// </summary>
    /// <param name="place">Place entry from the persons table</param>
    public void Insert(string place)
    {
        Items ??= new List<PersonPlace>();

        string formattedPlace = this._placeNameFormatter.Format(place);
        string nation = this._placeNameFormatter.ValidateSupportedNation(formattedPlace);

        if (nation!=null)
        {
            var placeLh = new PersonPlace()
            {
                Place = place,
                PlaceFormatted = formattedPlace,
                Country = nation
            };

            if (Items.All(a => a.PlaceFormatted != placeLh.PlaceFormatted))
                Items.Add(placeLh);
            else
                _duplicateCount++;
        }
        else
        {
            _invalidPlaces++;
        }
    }
    
   

    public IEnumerator<PersonPlace> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}