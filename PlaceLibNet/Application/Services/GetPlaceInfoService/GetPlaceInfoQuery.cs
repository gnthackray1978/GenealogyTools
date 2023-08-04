using MediatR;
using PlaceLibNet.Application.Models.Read;

namespace PlaceLibNet.Application.Services.GetPlaceInfoService;

public class GetPlaceInfoQuery : IRequest<PlaceInfoModel>
{

}