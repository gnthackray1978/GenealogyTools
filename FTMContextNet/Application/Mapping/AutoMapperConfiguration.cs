using AutoMapper;
using FTMContext;
using FTMContextNet.Application.Models.Read;
using FTMContextNet.Domain.Entities.NonPersistent;
using System.Collections.Generic;

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
                    place = item.place,
                    placeformatted = item.placeformatted,
                    placeid = item.placeid,
                    results = item.results
                });
            }

            return tp;
        }
    }
}
