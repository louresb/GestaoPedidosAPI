using GestaoPedidos.Domain.Enums;

namespace GestaoPedidos.Domain.Entities
{
    public class Pedido
    {
        public int Id { get; set; }
        public PedidoStatus Status { get; private set; } = PedidoStatus.Aberto;
        public List<PedidoProduto> Itens { get; private set; } = new();

        public void AdicionarProduto(Produto produto, int quantidade)
        {
            if (Status == PedidoStatus.Fechado) return;

            var itemExistente = Itens.FirstOrDefault(i => i.ProdutoId == produto.Id);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                Itens.Add(new PedidoProduto
                {
                    Pedido = this,
                    ProdutoId = produto.Id,
                    Produto = produto,
                    Quantidade = quantidade
                });
            }
        }

        public void RemoverProduto(int produtoId)
        {
            if (Status == PedidoStatus.Fechado) return;

            var item = Itens.FirstOrDefault(i => i.ProdutoId == produtoId);

            if (item != null)
            {
                Itens.Remove(item);
            }
        }

        public void FecharPedido()
        {
            if (Itens.Count == 0) return;
            Status = PedidoStatus.Fechado;
        }

        public decimal CalcularValorTotal()
        {
            return Itens.Sum(item => item.Produto.Preco * item.Quantidade);
        }
    }
}
