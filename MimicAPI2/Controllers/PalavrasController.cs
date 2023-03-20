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

        [HttpGet("", Name = "ListarTodas")]
        public ActionResult ListarTodas([FromQuery] PalavraUrlQuery query)
        {
            var itens = _repository.ListarTodas(query);

            if (itens is null || itens.Results.Count() <= 0)
                return NotFound();

            var itensDTO = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(itens);

            foreach (var itemDTO in itensDTO.Results)
            {
                itemDTO.Links = new List<LinkDTO>();
                itemDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = itemDTO.Id }), "GET"));
            }

            itensDTO.Links.Add(new LinkDTO("self", Url.Link("ListarTodas", query), "GET"));

            if (itens.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(itens.Paginacao));

                if (query.NumeroPagina + 1 <= itens.Paginacao.TotalDePaginas)
                {
                    var queryStr = new PalavraUrlQuery() { NumeroPagina = query.NumeroPagina + 1, RegistrosPorPagina = query.RegistrosPorPagina, Data = query.Data };
                    itens.Links.Add(new LinkDTO("next", Url.Link("ListarTodas", queryStr), "GET"));
                }

                if (query.NumeroPagina - 1 > 0)
                {
                    var queryStr = new PalavraUrlQuery() { NumeroPagina = query.NumeroPagina - 1, RegistrosPorPagina = query.RegistrosPorPagina, Data = query.Data };

                    itens.Links.Add(new LinkDTO("prev", Url.Link("ListarTodas", queryStr), "GET"));
                }
            }

            return Ok(itensDTO);
        }

        [HttpGet("{id}", Name = "Listar")]
        public ActionResult Listar(int id)
        {
            var item = _repository.Listar(id);

            if (item is null) return NotFound();

            PalavraDTO itemDTO = _mapper.Map<Palavra, PalavraDTO>(item);

            itemDTO.Links = new List<LinkDTO>();

            itemDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = itemDTO.Id }), "GET"));
            itemDTO.Links.Add(new LinkDTO("update", Url.Link("Listar", new { Id = itemDTO.Id }), "PUT"));
            itemDTO.Links.Add(new LinkDTO("delete", Url.Link("Listar", new { Id = itemDTO.Id }), "DELETE"));

            return Ok(itemDTO);
        }

        [HttpPost("", Name = "Cadastrar")]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavras/{palavra.Id}", palavra);
        }

        [HttpPut("", Name = "Atualizar")]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var iten = _repository.Listar(id);
            if (iten is null) return NotFound();

            palavra.Id = id;
            _repository.Atualizar(palavra);

            return Ok(palavra);
        }

        [HttpDelete("", Name = "Deletar")]
        public ActionResult Deletar(int id)
        {
            var iten = _repository.Listar(id);
            if (iten is null) return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}