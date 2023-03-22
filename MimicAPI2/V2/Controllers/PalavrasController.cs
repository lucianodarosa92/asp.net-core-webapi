using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.V2.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    [ApiVersion("2.0")]
    public class PalavrasController : ControllerBase
    {
        /// <summary>
        /// teste V2
        /// </summary>
        /// <returns></returns>
        [HttpGet("", Name = "ListarTodas")]
        public string ListarTodas()
        {
            return "Versão 2.0";
        }
    }
}
