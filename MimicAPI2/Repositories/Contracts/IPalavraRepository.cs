using MimicAPI.Helpers;
using MimicAPI2.Models;

namespace MimicAPI.Repositories.Contracts
{
    public interface IPalavraRepository
    {
        PaginacaoList<Palavra> ListarPalavras(palavraUrlQuery query);
        Palavra ListarPalavras(int id);
        void CadastrarPalavra(Palavra palavra);
        void AtualizarPalavra(Palavra palavra);
        void DeletarPalavra(int id);
    }
}