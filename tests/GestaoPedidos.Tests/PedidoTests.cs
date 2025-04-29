using GestaoPedidos.Domain.Entities;
using GestaoPedidos.Domain.Enums;

namespace GestaoPedidos.Tests
{
    public class PedidoTests
    {
        [Fact]
        public void Nao_Deve_Adicionar_Produto_Em_Pedido_Fechado()
        {
            var pedido = new Pedido();
            var produto = new Produto { Id = 1, Nome = "Produto A", Preco = 10m };

            pedido.AdicionarProduto(produto, 1);
            pedido.FecharPedido();

            pedido.AdicionarProduto(produto, 1);

            Assert.Single(pedido.Itens); 
        }

        [Fact]
        public void Nao_Deve_Remover_Produto_De_Pedido_Fechado()
        {
            var pedido = new Pedido();
            var produto = new Produto { Id = 1, Nome = "Produto A", Preco = 10m };

            pedido.AdicionarProduto(produto, 1);
            pedido.FecharPedido();

            pedido.RemoverProduto(produto.Id);

            Assert.Single(pedido.Itens); 
        }

        [Fact]
        public void Nao_Deve_Fechar_Pedido_Sem_Produtos()
        {
            var pedido = new Pedido();

            pedido.FecharPedido();

            Assert.Equal(PedidoStatus.Aberto, pedido.Status);
        }

        [Fact]
        public void Deve_Fechar_Pedido_Com_Produtos()
        {
            var pedido = new Pedido();
            var produto = new Produto { Id = 1, Nome = "Produto A", Preco = 10m };

            pedido.AdicionarProduto(produto, 2);

            pedido.FecharPedido();

            Assert.Equal(PedidoStatus.Fechado, pedido.Status);
        }

        [Fact]
        public void Deve_Calcular_Valor_Total_Corretamente()
        {
            var pedido = new Pedido();
            var produtoA = new Produto { Id = 1, Nome = "Produto A", Preco = 10m };
            var produtoB = new Produto { Id = 2, Nome = "Produto B", Preco = 20m };

            pedido.AdicionarProduto(produtoA, 2); 
            pedido.AdicionarProduto(produtoB, 1); 

            var total = pedido.CalcularValorTotal();

            Assert.Equal(40m, total);
        }
    }
}
