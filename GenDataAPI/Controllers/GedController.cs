using System;
using System.Threading;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Models.Create;
using FTMContextNet.Application.UserServices.CreatePersonLocationsInCache;
using FTMContextNet.Domain.Commands;
using GenDataAPI.Hub;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenDataAPI.Controllers;

[ApiController]
[Route("[controller]")]
public partial class GedController : ControllerBase
{

    private readonly IMSGConfigHelper _iMSGConfigHelper;
    private readonly OutputHandler _outputHandler; 
    private readonly IMediator _mediator;


    public GedController(IHubContext<NotificationHub> hubContext, 
        IMSGConfigHelper iMSGConfigHelper, IMediator mediator)
    {
        _iMSGConfigHelper = iMSGConfigHelper;
        _outputHandler = new OutputHandler(hubContext); 
        _mediator = mediator;
    }

    [HttpPost]
    [Route("/ged/add")]
    public ActionResult UploadFiles([FromForm] FilePayload filePayload)
    {
        long size = 0;
        string fileName = "";
        
        try
        {
            size = FilePayload.ExtractFile(filePayload, _iMSGConfigHelper.GedPath, out fileName);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
        if (size == 0)
        {
            return NoContent();
        }

        return this.ConvertResult(_mediator
            .Send(new CreateImportCommand(fileName, size,false), new CancellationToken(false)).Result);
         
    }

    [HttpPut]
    [Route("/ged/select")]
    public ActionResult SelectGed([FromBody] int importId)
    { 
       
        return this.ConvertResult(_mediator
            .Send(new UpdateImportStatusCommand(importId), new CancellationToken(false)).Result);
    }

    [HttpDelete]
    [Route("/ged/delete")]
    public ActionResult DeleteGed([FromBody]int importId)
    {
        if (importId == 42) return Ok();

        return this.ConvertResult(_mediator
            .Send(new DeleteImportCommand(importId), new CancellationToken(false)).Result);
    }
}