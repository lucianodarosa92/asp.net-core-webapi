using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI2.DataBase;
using MimicAPI2.Models;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace MimicAPI2.Controllers
{
    [Route("api/palavras")]
    public class PalavrasController : ControllerBase
    {
        private readonly MimicContext _banco;

        public PalavrasController(MimicContext banco)
        {
            _banco = banco;
        }

        [HttpGet]
        [Route("")]
        public ActionResult ListarPalavras([FromQuery] palavraUrlQuery query)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Ativo == true);
            if (obj is null) return NotFound();

            var itens = _banco.Palavras.AsQueryable();
            if (query.Data.HasValue)
            {
                itens = itens.Where(a => (a.Criado >= query.Data || a.Atualizado >= query.Data) && a.Ativo == true);
            }

            if (query.PagNumero.HasValue)
            {
                var totalDeRegistros = itens.Count();

                itens = itens.Skip((query.PagNumero.Value - 1) * query.PagRegistros.Value).Take(query.PagRegistros.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPaginas = query.PagNumero.Value;
                paginacao.RegistrosPorPagina = query.PagRegistros.Value;
                paginacao.TotalDeRegistros = totalDeRegistros;
                paginacao.TotalDePaginas = (int)Math.Ceiling((double)totalDeRegistros / query.PagRegistros.Value);

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginacao));

                if (query.PagNumero > paginacao.TotalDePaginas)
                {
                    return NotFound();
                }
            }

            if (itens.Count() > 0) return Ok(itens);

            return NotFound();
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult ListarPalavras(int id)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id && a.Ativo == true);
            if (obj is null) return NotFound();

            return Ok(obj);
        }

        [HttpPost]
        [Route("")]
        public ActionResult CadastrarPalavra([FromBody] Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();

            return Created($"/api/palavras/{palavra.id}", palavra);
        }

        [HttpPut]
        [Route("{id}")]
        public ActionResult AtualizarPalavra(int id, [FromBody] Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id);
            if (obj is null) return NotFound();

            palavra.id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok(palavra);
        }

        [HttpDelete]
        [Route("{id}")]
        public ActionResult DeletarPalavra(int id)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id && a.Ativo == true);
            if (obj is null) return NotFound();

            if (obj.Ativo)
            {
                obj.Ativo = false;
                _banco.Palavras.Update(obj);
                _banco.SaveChanges();
                return Ok();
            }

            return NoContent();
        }
    }
}