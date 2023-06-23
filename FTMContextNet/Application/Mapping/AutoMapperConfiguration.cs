using System;
using AutoMapper;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Domain.Entities.NonPersistent;
using System.Collections.Generic;
using System.Linq;
using FTMContextNet.Domain.Entities.Persistent.Cache;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Domain.Entities;

namespace FTMContextNet.Application.Mapping
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            this.CreateMap<Info, InfoModel>();
            this.CreateMap<IEnumerable<PlaceLookup>, IEnumerable<PlaceModel>>().ConvertUsing(new PlaceConverter());
            this.CreateMap<IEnumerable<FTMImport>, IEnumerable<GedFileModel>>().ConvertUsing(new GedFileInfoConverter());
        }
    }

    class PlaceConverter: ITypeConverter<IEnumerable<PlaceLookup>, IEnumerable<PlaceModel>>
    {
        public IEnumerable<PlaceModel> Convert(IEnumerable<PlaceLookup> source, IEnumerable<PlaceModel> destination, ResolutionContext context)
        {
            return source
                .Select(item => 
                    new PlaceModel()
                    {
                        place = item.Place, 
                        placeformatted = item.PlaceFormatted, 
                        placeid = item.PlaceId, 
                        results = item.Results
                    }).ToList();
        }
    }

    class GedFileInfoConverter : ITypeConverter<IEnumerable<FTMImport>, IEnumerable<GedFileModel>>
    {
        public IEnumerable<GedFileModel> Convert(IEnumerable<FTMImport> source, IEnumerable<GedFileModel> destination, ResolutionContext context)
        {
            return source
                .Select(item =>
                {
                    DateTime.TryParse(item.DateImported, out DateTime dt);

                    return new GedFileModel
                    {
                        Id = item.Id,
                        FileName = item.FileName,
                        FileSize = item.FileSize,
                        DateImported = dt
                    };
                }).ToList();
        }
    }
}
