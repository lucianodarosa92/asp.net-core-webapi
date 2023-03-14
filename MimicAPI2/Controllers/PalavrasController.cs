using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Repositories;
using MimicAPI2.Models;
using Newtonsoft.Json;
using System.Linq;

namespace MimicAPI2.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly PalavraRepository _repositoryPalavra;

        public PalavrasController(PalavraRepository repositoryPalavra)
        {
            _repositoryPalavra = repositoryPalavra;
        }

        [Route("")]
        [HttpGet]        
        public ActionResult ListarPalavras([FromQuery] palavraUrlQuery query)
        {
            var itens = _repositoryPalavra.ListarPalavras(query);
            if (itens is null) return NotFound();

            if (query.PagNumero > itens.Paginacao.NumeroPaginas || itens.Count() <= 0)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(itens.Paginacao));

            return Ok(itens.ToList());
        }

        [Route("{id}")]
        [HttpGet]        
        public ActionResult ListarPalavras(int id)
        {
            var palavra = _repositoryPalavra.ListarPalavras(id);
            if (palavra is null) return NotFound();
            return Ok(palavra);
        }

        [Route("")]
        [HttpPost]        
        public ActionResult CadastrarPalavra([FromBody] Palavra palavra)
        {
            _repositoryPalavra.CadastrarPalavra(palavra);

            return Created($"/api/palavras/{palavra.id}", palavra);
        }

        [Route("{id}")]
        [HttpPut]        
        public ActionResult AtualizarPalavra(int id, [FromBody] Palavra palavra)
        {
            var obj = _repositoryPalavra.ListarPalavras(id);
            if (obj is null) return NotFound();

            palavra.id = id;
            _repositoryPalavra.AtualizarPalavra(palavra);

            return Ok(palavra);
        }

        [Route("{id}")]
        [HttpDelete]        
        public ActionResult DeletarPalavra(int id)
        {
            var obj = _repositoryPalavra.ListarPalavras(id);
            if (obj is null) return NotFound();

            _repositoryPalavra.DeletarPalavra(id);

            return NoContent();
        }
    }
}