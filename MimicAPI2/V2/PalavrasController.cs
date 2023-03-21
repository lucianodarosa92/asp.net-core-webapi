using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.V2
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        [HttpGet("", Name = "ListarTodas")]
        public string ListarTodas()
        {
            return "Versão 2.0";
        }
    }
}
