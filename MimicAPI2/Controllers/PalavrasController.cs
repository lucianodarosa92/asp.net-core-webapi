using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using MimicAPI.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
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
            var Palavras = _repository.ListarTodas(query);

            if (Palavras is null || Palavras.Results.Count() <= 0)
                return NotFound();

            PaginationList<PalavraDTO> palavrasDTO = CriarLinksListaPalavrasDTO(query, Palavras);

            return Ok(palavrasDTO);
        }

        private PaginationList<PalavraDTO> CriarLinksListaPalavrasDTO(PalavraUrlQuery query, PaginationList<Palavra> palavras)
        {
            var palavrasDTO = _mapper.Map<PaginationList<Palavra>, PaginationList<PalavraDTO>>(palavras);

            foreach (var palavraDTO in palavrasDTO.Results)
            {
                palavraDTO.Links = new List<LinkDTO>();
                palavraDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = palavraDTO.Id }), "GET"));
            }

            palavrasDTO.Links.Add(new LinkDTO("self", Url.Link("ListarTodas", query), "GET"));

            if (palavras.Paginacao != null)
            {
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(palavras.Paginacao));

                if (query.NumeroPagina + 1 <= palavras.Paginacao.TotalDePaginas)
                {
                    var queryStr = new PalavraUrlQuery() { NumeroPagina = query.NumeroPagina + 1, RegistrosPorPagina = query.RegistrosPorPagina, Data = query.Data };
                    palavrasDTO.Links.Add(new LinkDTO("next", Url.Link("ListarTodas", queryStr), "GET"));
                }

                if (query.NumeroPagina - 1 > 0)
                {
                    var queryStr = new PalavraUrlQuery() { NumeroPagina = query.NumeroPagina - 1, RegistrosPorPagina = query.RegistrosPorPagina, Data = query.Data };

                    palavrasDTO.Links.Add(new LinkDTO("prev", Url.Link("ListarTodas", queryStr), "GET"));
                }
            }

            return palavrasDTO;
        }

        [HttpGet("{id}", Name = "Listar")]
        public ActionResult Listar(int id)
        {
            var palavra = _repository.Listar(id);

            if (palavra is null) 
                return NotFound();

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = palavraDTO.Id }), "GET"));
            palavraDTO.Links.Add(new LinkDTO("update", Url.Link("Listar", new { Id = palavraDTO.Id }), "PUT"));
            palavraDTO.Links.Add(new LinkDTO("delete", Url.Link("Listar", new { Id = palavraDTO.Id }), "DELETE"));

            return Ok(palavraDTO);
        }

        [HttpPost("", Name = "Cadastrar")]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            if (palavra == null) 
                return BadRequest();

            if (!ModelState.IsValid) 
                return UnprocessableEntity(ModelState);

            palavra.Ativo = true;
            palavra.Criado = DateTime.Now;

            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);
            
            palavraDTO.Links = new List<LinkDTO>();

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = palavraDTO.Id }), "GET"));

            return Created($"/api/palavras/{palavra.Id}", palavraDTO);
        }

        [HttpPut("{id}", Name = "Atualizar")]
        public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
        {
            var item = _repository.Listar(id);
            if (item is null) 
                return NotFound();

            if (palavra == null) 
                return BadRequest();

            if (!ModelState.IsValid) 
                return UnprocessableEntity(ModelState);

            palavra.Id = id;
            palavra.Ativo = item.Ativo;
            palavra.Criado = item.Criado;
            palavra.Atualizado = DateTime.Now;
            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = palavraDTO.Id }), "GET"));

            return Ok(palavraDTO);
        }

        [HttpDelete("", Name = "Deletar")]
        public ActionResult Deletar(int id)
        {
            var iten = _repository.Listar(id);
            if (iten is null) 
                return NotFound();

            _repository.Deletar(id);

            return NoContent();
        }
    }
}