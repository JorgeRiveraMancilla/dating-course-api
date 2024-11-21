using dating_course_api.Src.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase;
}
