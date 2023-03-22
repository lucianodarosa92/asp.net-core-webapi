using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.V1.Models;
using MimicAPI.V1.Models.DTO;
using MimicAPI.V1.Repositories.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.V1.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Route("api/[controller]")]
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("1.1")]
    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// teste V1
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
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

        /// <summary>
        /// teste comentáriosafnfksfnsfsakdfbskjfhbksjfbhkhf
        /// </summary>
        /// <param name="id">sfnklsjnflksjfnklsjfnklsdjfnlksjfnlskajfdn</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
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

        /// <summary>
        /// teste comentáriosafnfksfnsfsakdfbskjfhbksjfbhkhf
        /// </summary>
        /// <param name="palavra">sfnklsjnflksjfnklsjfnklsdjfnlksjfnlskajfdn</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
        [HttpPost("", Name = "Cadastrar")]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            if (palavra == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            palavra.Ativo = true;
            DateTime dateTime = DateTime.Now;
            palavra.Criado = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

            _repository.Cadastrar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = palavraDTO.Id }), "GET"));

            return Created($"/api/palavras/{palavra.Id}", palavraDTO);
        }

        /// <summary>
        /// teste comentáriosafnfksfnsfsakdfbskjfhbksjfbhkhf
        /// </summary>
        /// <param name="id">sfnklsjnflksjfnklsjfnklsdjfnlksjfnlskajfdn</param>
        /// <param name="palavra">sfnklsjnflksjfnklsjfnklsdjfnlksjfnlskajfdn</param>
        /// <returns></returns>
        [MapToApiVersion("1.0")]
        [MapToApiVersion("1.1")]
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
            DateTime dateTime = DateTime.Now;
            palavra.Atualizado = dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.TicksPerSecond));

            _repository.Atualizar(palavra);

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(palavra);

            palavraDTO.Links = new List<LinkDTO>();

            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("Listar", new { Id = palavraDTO.Id }), "GET"));

            return Ok(palavraDTO);
        }

        /// <summary>
        /// teste comentáriosafnfksfnsfsakdfbskjfhbksjfbhkhf
        /// </summary>
        /// <param name="id">sfnklsjnflksjfnklsjfnklsdjfnlksjfnlskajfdn</param>
        /// <returns></returns>
        [MapToApiVersion("1.1")]
        [HttpDelete("{id}", Name = "Deletar")]
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