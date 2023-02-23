using Microsoft.AspNetCore.Mvc;
using MimicAPI2.DataBase;
using MimicAPI2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public ActionResult ListarPalavras()
        {
            return Ok(_banco.Palavras);
        }

        [HttpGet, Route("{id}")]
        public ActionResult ListarPalavras(int id)
        {
            return Ok(_banco.Palavras.Find(id));
        }

        [HttpPost, Route("")]
        public ActionResult CadastrarPalavra([FromBody] Palavra palavra)
        {
            _banco.Palavras.Add(palavra);

            return Ok();
        }

        [HttpPut, Route("{id}")]
        public ActionResult AtualizarPalavra(int id, Palavra palavra)
        {
            palavra.id = id;
            _banco.Palavras.Update(palavra);

            return Ok();
        }

        [HttpDelete, Route("{id}")]
        public ActionResult DeletarPalavra(int id)
        {
            _banco.Palavras.Remove(_banco.Palavras.Find(id));

            return Ok();
        }
    }
}