using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimicAPI2.DataBase;
using MimicAPI2.Models;
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

        [HttpGet, Route("")]
        public ActionResult ListarPalavras(DateTime? data)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Ativo == true);
            if (obj is null) return NotFound();

            var item = _banco.Palavras.AsQueryable();
            if (data.HasValue)
            {
                item = item.Where(a => (a.Criado >= data || a.Atualizado >= data) && a.Ativo == true);
            }

            return Ok(item);
        }

        [HttpGet, Route("{id}")]
        public ActionResult ListarPalavras(int id)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id && a.Ativo == true);
            if (obj is null) return NotFound();

            return Ok(obj);
        }

        [HttpPost, Route("")]
        public ActionResult CadastrarPalavra([FromBody] Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
            return Created($"/api/palavras/{palavra.id}", palavra);
        }

        [HttpPut, Route("{id}")]
        public ActionResult AtualizarPalavra(int id, [FromBody] Palavra palavra)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id);
            if (obj is null) return NotFound();

            palavra.id = id;
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();

            return Ok(palavra);
        }

        [HttpDelete, Route("{id}")]
        public ActionResult DeletarPalavra(int id)
        {
            var obj = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id && a.Ativo == true);
            if (obj is null) return NotFound();

            if (obj.Ativo)
            {
                obj.Ativo = false;
                _banco.Palavras.Update(obj);
                _banco.SaveChanges();
                return NoContent();
            }

            return NotFound();
        }
    }
}