using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigHelper;
using FTMContextNet;
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
    public ActionResult UploadFiles([FromForm] Payload payload)
    {
        long size = payload.Files.Sum(f => f.Length);

        // full path to file in temp location
        var filePath = Path.GetTempFileName();

        var f = payload.Files.FirstOrDefault();
        var n = payload.Tags.Split('|').FirstOrDefault();

        if (f != null && n != null)
        {            
            using var stream = new FileStream(Path.Combine(Path.GetTempPath(), n), FileMode.Create);

            f.CopyToAsync(stream);
        }

        return Ok();
    }
}