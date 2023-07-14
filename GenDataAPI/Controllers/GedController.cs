using System;
using ConfigHelper;
using FTMContextNet;
using FTMContextNet.Application.Models.Create;
using GenDataAPI.Hub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace GenDataAPI.Controllers;

[ApiController]
[Route("[controller]")]
public partial class GedController : ControllerBase
{

    private readonly IMSGConfigHelper _iMSGConfigHelper;
    private readonly OutputHandler _outputHandler;
    private readonly FTMFacade _facade;

    public GedController(IHubContext<NotificationHub> hubContext, IMSGConfigHelper iMSGConfigHelper)
    {
        _iMSGConfigHelper = iMSGConfigHelper;
        _outputHandler = new OutputHandler(hubContext);
        _facade = new FTMFacade(_iMSGConfigHelper, _outputHandler);
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
 
        return this.ConvertResult(_facade.CreateImport(new CreateImportModel
        {
            Selected = false,
            FileName = fileName,
            FileSize = size,
        }));
        
    }

    [HttpPut]
    [Route("/ged/select")]
    public ActionResult SelectGed([FromBody] int importId)
    { 
        return this.ConvertResult(_facade.SelectImport(importId));
    }

    [HttpDelete]
    [Route("/ged/delete")]
    public ActionResult DeleteGed([FromBody]int importId)
    {
        if (importId == 42) return Ok();

        return this.ConvertResult(_facade.DeleteImport(importId));
    }
}