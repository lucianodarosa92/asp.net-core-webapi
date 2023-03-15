using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;

        public PalavrasController(IPalavraRepository repository)
        {
            _repository = repository;
        }

        [Route("")]
        [HttpGet]
        public ActionResult ListarPalavras([FromQuery] PalavraUrlQuery query)
        {
            var itens = _repository.ListarPalavras(query);
            if (itens is null) return NotFound();

            if (query.NumeroPagina > itens.Paginacao.TotalDePaginas || itens.Count() <= 0)
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
            var palavra = _repository.Listar(id);
            if (palavra is null) return NotFound();
            return Ok(palavra);
        }

        [Route("")]
        [HttpPost]
        public ActionResult CadastrarPalavra([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        [Route("{id}")]
        [HttpPut]
        public ActionResult AtualizarPalavra(int id, [FromBody] Palavra palavra)
        {
            var obj = _repository.Listar(id);
            if (obj is null) return NotFound();

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok(palavra);
        }

        [Route("{id}")]
        [HttpDelete]
        public ActionResult DeletarPalavra(int id)
        {
            var obj = _repository.Listar(id);
            if (obj is null) return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}