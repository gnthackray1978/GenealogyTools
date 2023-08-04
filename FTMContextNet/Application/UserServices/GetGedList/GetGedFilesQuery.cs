using System.Collections.Generic;
using FTMContextNet.Application.Models.Read;
using MediatR;

namespace FTMContextNet.Application.UserServices.GetGedList;

public class GetGedFilesQuery : IRequest<List<ImportModel>>
{

}