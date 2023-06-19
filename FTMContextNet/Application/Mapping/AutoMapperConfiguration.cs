using AutoMapper;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Domain.Entities.NonPersistent;
using System.Collections.Generic;
using PlaceLibNet.Application.Models.Read;
using PlaceLibNet.Domain.Entities;

namespace FTMContextNet.Application.Mapping
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            this.CreateMap<Info, InfoModel>();
            this.CreateMap<IEnumerable<PlaceLookup>, IEnumerable<PlaceModel>>().ConvertUsing(new Converter());
        }
    }

    class Converter: ITypeConverter<IEnumerable<PlaceLookup>, IEnumerable<PlaceModel>>
    {
        
        public IEnumerable<PlaceModel> Convert(IEnumerable<PlaceLookup> source, IEnumerable<PlaceModel> destination, ResolutionContext context)
        {
            var tp = new List<PlaceModel>();

            foreach(var item in source)
            {
                tp.Add(new PlaceModel()
                {
                    place = item.Place,
                    placeformatted = item.PlaceFormatted,
                    placeid = item.PlaceId,
                    results = item.Results
                });
            }

            return tp;
        }
    }
}
