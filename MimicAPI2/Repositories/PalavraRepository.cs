using Microsoft.EntityFrameworkCore;
using MimicAPI.DataBase;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
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
        public PaginationList<Palavra> ListarTodas(PalavraUrlQuery query)
        {
            var paginacaoList = new PaginationList<Palavra>();

            var itens = _banco.Palavras.AsNoTracking().AsQueryable();

            itens = itens.Where(a => a.Ativo == true);

            if (query.Data.HasValue) itens = itens.Where(a => a.Criado >= query.Data || a.Atualizado >= query.Data);

            if (query.NumeroPagina.HasValue)
            {
                var totalDeRegistros = itens.Count();

                itens = itens.Skip((query.NumeroPagina.Value - 1) * query.RegistrosPorPagina.Value).Take(query.RegistrosPorPagina.Value);

                var paginacao = new Paginacao();
                paginacao.NumeroPagina = query.NumeroPagina.Value;
                paginacao.RegistrosPorPagina = query.RegistrosPorPagina.Value;
                paginacao.TotalDeRegistros = totalDeRegistros;
                paginacao.TotalDePaginas = (int)Math.Ceiling((double)totalDeRegistros / query.RegistrosPorPagina.Value);
                paginacaoList.Paginacao = paginacao;
            }

            paginacaoList.Results.AddRange(itens.ToList());

            return paginacaoList;
        }
        public Palavra Listar(int id)
        {
            var palavra = _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id || a.Ativo == true);

            return palavra;
        }
        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }
        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }
        public void Deletar(int id)
        {
            var palavra = Listar(id);

            if (palavra != null)
            {
                palavra.Ativo = false;
                _banco.Palavras.Update(palavra);
                _banco.SaveChanges();
            }
        }
    }
}