using GestaoPedidos.Domain.Entities;
using GestaoPedidos.Domain.Enums;
using GestaoPedidos.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidos.Api.Controllers.v1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IProdutoRepository _produtoRepository;

        public PedidoController(IPedidoRepository pedidoRepository, IProdutoRepository produtoRepository)
        {
            _pedidoRepository = pedidoRepository;
            _produtoRepository = produtoRepository;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarPedido()
        {
            var pedido = new Pedido();

            await _pedidoRepository.AdicionarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return CreatedAtAction(nameof(ObterPedidoPorId), new { id = pedido.Id }, pedido);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPedidoPorId(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound();

            return Ok(pedido);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Pedido>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarPedidos()
        {
            var pedidos = await _pedidoRepository.ListarAsync();
            return Ok(pedidos);
        }

        [HttpPost("{id}/produto/{produtoId}")]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarProduto(int id, int produtoId)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == PedidoStatus.Fechado)
                return BadRequest("Não é possível adicionar produtos a um pedido fechado.");

            var produto = await _produtoRepository.ObterPorIdAsync(produtoId);

            var produtoOriginal = await _produtoRepository.ObterPorIdAsync(produtoId);

            if (produtoOriginal == null)
                return NotFound("Produto não encontrado.");

            // Cópia do produto para evitar tracking
            var novoProduto = new Produto
            {
                Nome = produtoOriginal.Nome,
                Preco = produtoOriginal.Preco
            };

            pedido.AdicionarProduto(novoProduto);

            await _pedidoRepository.AtualizarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return Ok(pedido);
        }

        [HttpDelete("{id}/produto/{produtoId}")]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverProduto(int id, int produtoId)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == PedidoStatus.Fechado)
                return BadRequest("Não é possível remover produtos de um pedido fechado.");

            var produtoParaRemover = pedido.Produtos.FirstOrDefault(p => p.Id == produtoId);

            if (produtoParaRemover == null)
                return NotFound("Produto não encontrado no pedido.");

            pedido.RemoverProduto(produtoParaRemover);

            await _pedidoRepository.AtualizarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return Ok(pedido);
        }

        [HttpPut("{id}/fechar")]
        [ProducesResponseType(typeof(Pedido), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FecharPedido(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);

            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == PedidoStatus.Fechado)
                return BadRequest("O pedido já está fechado.");

            if (pedido.Produtos == null || pedido.Produtos.Count == 0)
                return BadRequest("Não é possível fechar um pedido sem produtos.");

            pedido.FecharPedido();

            await _pedidoRepository.AtualizarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return Ok(pedido);
        }
    }
}