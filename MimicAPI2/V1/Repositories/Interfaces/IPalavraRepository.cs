using MimicAPI.Helpers;
using MimicAPI.V1.Models;

namespace MimicAPI.V1.Repositories.Interfaces
{
    public interface IPalavraRepository
    {
        PaginationList<Palavra> ListarTodas(PalavraUrlQuery query);
        Palavra Listar(int id);
        void Cadastrar(Palavra palavra);
        void Atualizar(Palavra palavra);
        void Deletar(int id);

    }
}