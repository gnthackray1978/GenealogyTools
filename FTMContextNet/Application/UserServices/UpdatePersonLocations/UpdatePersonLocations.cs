using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FTMContextNet.Data.Repositories;
using MSGIdent;
using FTMContextNet.Domain.Commands;
using LoggingLib;
using MediatR;
using PlaceLibNet.Data.Repositories;
using PlaceLibNet.Domain;
using PlaceLibNet.Domain.Caching;
using MSG.CommonTypes;

namespace FTMContextNet.Application.UserServices.UpdatePersonLocations;

public class UpdatePersonLocations : IRequestHandler<UpdatePersonLocationsCommand, CommandResult>
{
    private readonly IPlaceRepository _placeRepository;

    private readonly IPersistedCacheRepository _persistedCacheRepository;

    private readonly IAuth _auth;

    private readonly Ilog _ilog;

    public UpdatePersonLocations(IPlaceRepository placeRepository, 
        IPersistedCacheRepository persistedCacheRepository, Ilog logger, IAuth auth)
    {
        _persistedCacheRepository = persistedCacheRepository;

        _placeRepository = placeRepository;

        _auth = auth;

        _ilog = logger;
    }
    /// <summary>
    /// Update lat and long fields in the persons table.
    /// </summary>
    public void Execute()
    {
        var locations = _persistedCacheRepository.GetPersonMapLocations();

        var placeCache = new PlaceLookupCache(_placeRepository, new PlaceNameFormatter());

        placeCache.Load();

        _ilog.WriteLine(locations.Count + " locations");

        int count = 0;

        foreach (var location in locations)
        {
            var match = placeCache.Search(location.Location);

            if (match != null)
            {
                location.Lat = match.Lat;
                location.Lng = match.Lng;
            }

            match = placeCache.Search(location.AltLocation);

            if (match != null)
            {
                location.AltLat = match.Lat;
                location.AltLng = match.Lng;
            }

            if (location.Lat != "" || location.AltLat != "")
            {
                count++;
                _persistedCacheRepository.UpdatePersons(location.Id, location.Lat, location.Lng, location.AltLat,
                    location.AltLng);
            }
        }

        _ilog.WriteLine("found " + count + "locations");
    }

    public async Task<CommandResult> Handle(UpdatePersonLocationsCommand request, CancellationToken cancellationToken)
    {
        if (_auth.GetUser() == -1)
        {
            return CommandResult.Fail(CommandResultType.Unauthorized);
        }
         
        _ilog.WriteLine("UpdatePersonLocations started");

        var timer = new Stopwatch();
        timer.Start();

        await Task.Run(Execute, cancellationToken);

        timer.Stop();
        _ilog.WriteLine("Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff"));

        return CommandResult.Success();
    }
}