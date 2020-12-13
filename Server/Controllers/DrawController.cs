using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ZoomersClient.Shared.Services;
using ZoomersClient.Shared.Models;
using System.Linq;
using Net.Codecrete.QrCodeGenerator;
using System.Drawing.Imaging;
using System.IO;
using ZoomersClient.Shared.Models.DTOs;
using ZoomersClient.Server.Services;

namespace ZoomersClient.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DrawController : ControllerBase
    {
        private readonly ILogger<DrawController> _logger;

        public DrawController(ILogger<DrawController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void Post([FromBody] string dataUrl)
        {
            _logger.LogInformation(dataUrl);
        }
    }
}