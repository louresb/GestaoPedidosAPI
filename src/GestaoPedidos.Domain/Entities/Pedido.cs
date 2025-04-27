using GestaoPedidos.Domain.Enums;

namespace GestaoPedidos.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public PedidoStatus Status { get; private set; } = PedidoStatus.Aberto;
        public List<Produto> Produtos { get; private set; } = new();

        public void AdicionarProduto(Produto produto)
        {
            if (Status == PedidoStatus.Fechado) return;
            Produtos.Add(produto);
        }

        public void RemoverProduto(Produto produto)
        {
            if (Status == PedidoStatus.Fechado) return;
            Produtos.Remove(produto);
        }

        public void FecharPedido()
        {
            if (Produtos.Count == 0) return;
            Status = PedidoStatus.Fechado;
        }
    }
}
