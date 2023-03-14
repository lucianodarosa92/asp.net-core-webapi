using Microsoft.EntityFrameworkCore;
using MimicAPI.Helpers;
using MimicAPI.Repositories.Contracts;
using MimicAPI2.DataBase;
using MimicAPI2.Models;
using System;
using System.Linq;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;
        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }
        public PaginacaoList<Palavra> ListarPalavras(palavraUrlQuery query)
        {
            var itens = _banco.Palavras.AsNoTracking().AsQueryable();
            if (query.Data.HasValue)
            {
                itens = itens.Where(a => (a.Criado >= query.Data || a.Atualizado >= query.Data) && a.Ativo == true);
            }

            var paginacaoList = new PaginacaoList<Palavra>();

            if (query.PagNumero.HasValue)
            {
                var totalDeRegistros = itens.Count();

                itens = itens.Skip((query.PagNumero.Value - 1) * query.PagRegistros.Value).Take(query.PagRegistros.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPaginas = query.PagNumero.Value;
                paginacao.RegistrosPorPagina = query.PagRegistros.Value;
                paginacao.TotalDeRegistros = totalDeRegistros;
                paginacao.TotalDePaginas = (int)Math.Ceiling((double)totalDeRegistros / query.PagRegistros.Value);

                paginacaoList.Paginacao = paginacao;
            }

            paginacaoList.AddRange(itens.ToList());

            return paginacaoList;
        }
        public Palavra ListarPalavras(int id)
        {
            var palavra = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.id == id && a.Ativo == true);

            return palavra;
        }
        public void CadastrarPalavra(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }
        public void AtualizarPalavra(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
        public void DeletarPalavra(int id)
        {
            var palavra = ListarPalavras(id);

            if (palavra != null)
            {
                palavra.Ativo = false;
                _banco.Palavras.Update(palavra);
                _banco.SaveChanges();
            }
        }
    }
}