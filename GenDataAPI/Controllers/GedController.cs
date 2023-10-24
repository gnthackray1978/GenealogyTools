using System;
using System.Threading;
using System.Threading.Tasks;
using ConfigHelper;
using FTMContextNet.Application.UserServices.GetTreeImportStatus;
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
    public async Task<IActionResult> UploadFiles([FromForm] FilePayload filePayload)
    {
        string size = "0";
        string fileName = "";
        
        try
        {
            size = FilePayload.ExtractFile(filePayload, _iMSGConfigHelper.GedPath, out fileName);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }
        
        if (size == "0")
        {
            return NoContent();
        }

        var r = await _mediator
            .Send(new CreateImportCommand(fileName, size, false),
                new CancellationToken(false));

        return this.ConvertResult(r);
         
    }

    [HttpPut]
    [Route("/ged/select")]
    public async Task<IActionResult> SelectGed([FromBody] int importId)
    {
        var r = await  _mediator
            .Send(new UpdateImportStatusCommand(importId), new CancellationToken(false));
        
        return  this.ConvertResult(r);
    }

    [HttpDelete]
    [Route("/ged/delete")]
    public async Task<IActionResult> DeleteGed([FromBody]int importId)
    {
        if (importId == 42) return Ok();

        var r = await _mediator
            .Send(new DeleteTreeCommand(), new CancellationToken(false));

        return this.ConvertResult(r);
    }

    [HttpGet]
    [Route("/ged/status")]
    public async Task<IActionResult> TreeStatus()
    {
        var r = await _mediator
            .Send(new GetTreeImportStatusQuery(), new CancellationToken(false));

        return Ok(r);
    }
}