using System;
using Microsoft.AspNetCore.Mvc;

namespace EVisaTicketSystem.Core.Controllers;

// [ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{

}
