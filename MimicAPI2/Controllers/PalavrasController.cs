using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

        [HttpGet("{id}", Name = "Listar")]
        public ActionResult Listar(int id)
        {
            var palavra = _repository.Listar(id);
            if (palavra is null) return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();

            palavraDTO.Links.Add(
                new LinkDTO("self", Url.Link("", new { Id = palavraDTO.Id }), "GET")
            );

            palavraDTO.Links.Add(
                new LinkDTO("update", Url.Link("", new { Id = palavraDTO.Id }), "PUT")
            );

            palavraDTO.Links.Add(
                new LinkDTO("delete", Url.Link("", new { Id = palavraDTO.Id }), "DELETE")
            );

            return Ok(palavraDTO);
        }

        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        [HttpPut("{id}", Name = "")]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var obj = _repository.Listar(id);
            if (obj is null) return NotFound();

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok(palavra);
        }

        [HttpDelete("{id}", Name = "")]
        public ActionResult Deletar(int id)
        {
            var obj = _repository.Listar(id);
            if (obj is null) return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}