using System.Collections.Generic;

namespace MimicAPI.Helpers
{
    public class PaginacaoList<T> : List<T>
    {
        public Paginacao Paginacao { get; set; }
    }
}