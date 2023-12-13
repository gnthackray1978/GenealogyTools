using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MSGIdent;
using FTMContextNet.Domain.Caching;
using FTMContextNet.Domain.Commands;
using FTMContextNet.Domain.Entities.NonPersistent.Person;
using LoggingLib;
using MediatR;
using MSG.CommonTypes;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain.Caching;
using PlaceLibNet.Domain.Entities;
using PlaceLibNet.Domain.Entities.Persistent;

namespace FTMContextNet.Application.UserServices.CreatePersonLocationsInCache;

public class CreatePersonLocationsInCache : IRequestHandler<CreatePersonLocationsCommand, CommandResult>
{
    private readonly IPlaceRepository _placeRepository;

    private readonly IPersonPlaceCache _personPlaceCache;

    private readonly IPlaceLookupCache _placeLookupCache;

    private readonly IPlaceLibCoordCache _placeLibCoordCache;

    private readonly Ilog _logger;

    private readonly IAuth _auth;

    public CreatePersonLocationsInCache(IPlaceRepository placeRepository,
        IPlaceLibCoordCache placeLibCoordCache,
        IPersonPlaceCache personPlaceCache,
        IPlaceLookupCache placeLookupCache,
        IAuth auth,
        Ilog logger)
    {
        _placeLibCoordCache = placeLibCoordCache;

        _personPlaceCache = personPlaceCache;

        _placeRepository = placeRepository;

        _placeLookupCache = placeLookupCache;

        _auth = auth;

        _logger = logger;
    }

    public static PlaceCache Convert(PersonPlace pri, PlaceSearchCoordSubset pscs)
    {
        var placeCache = new PlaceCache()
        {
            Name = pri.Place,
            NameFormatted = pri.PlaceFormatted,
            JSONResult = "[]",
            County = pri.County,
            Country = pri.Country ?? ""
        };
        //just take whatever is in the ftmperson table
        if (pscs == null) return placeCache;

        placeCache.Lat = pscs.Lat;
        placeCache.Long = pscs.Long;
        placeCache.County = pscs.Ctyhistnm;// the formatting of this might need checking!
        placeCache.Country = "England";//only english counties work at the moment welsh and scottish counties are in there, the functionality just needs adding to set them
        placeCache.Src = "placelib";
        placeCache.BadData = false;
        placeCache.Searched = true;
        
        
        
        return placeCache;
    }
    
    public async Task<CommandResult> Handle(CreatePersonLocationsCommand request, 
                                            CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }
        
        var timer = new Stopwatch();
        timer.Start();

        var unencodedPlacesCount = _placeRepository.GetUnknownPlacesCount();

        _placeLookupCache.Load();
        _personPlaceCache.Load();

     //   var tp = _placeLookupCache.PlaceLookups.Where(w => w.Place.Contains("Hawks")).ToList();

     //   var t = _personPlaceCache.Items.Where(w => w.Place.Contains("Hawks")).ToList();

        _logger.WriteLine("Unencoded places in cache: " + unencodedPlacesCount);
        _logger.WriteLine("Person table locations count: " + _personPlaceCache.Count);

        //search the persons table place entries to see if they already exist in the place cache
        //if they don't exist then add a new placecache entry. 
        //look in the placelib to see if the new cache entry exists in there. if it does
        //use it to populate the lat and long of the cache entry.

        var newCacheEntries = _personPlaceCache
            .Where(w => !_placeLookupCache.Exists(w.PlaceFormatted))
            .Select(personPlace =>
                Convert(personPlace, _placeLibCoordCache.Search(personPlace.GetComponents(), personPlace.PlaceFormatted))).ToList();
        
        //todo make the repo async
        await Task.Run(() =>_placeRepository.InsertIntoCache(newCacheEntries),cancellationToken);

        timer.Stop();


        _logger.WriteLine("new records added: " + newCacheEntries.Count());

        _logger.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));


        return CommandResult.Success();
    }
}