using GestaoPedidos.Domain.Entities;

namespace GestaoPedidos.Domain.Interfaces.Repositories
{
    public interface IProdutoRepository
    {
        Task<Produto?> ObterPorIdAsync(int id);
        Task AdicionarAsync(Produto produto);
    }
}
