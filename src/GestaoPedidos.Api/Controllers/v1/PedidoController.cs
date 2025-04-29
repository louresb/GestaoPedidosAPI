using GestaoPedidos.Api.Controllers.v1.Requests;
using GestaoPedidos.Api.Controllers.v1.Responses;
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
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CriarPedido()
        {
            var pedido = new Pedido();
            await _pedidoRepository.AdicionarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return CreatedAtAction(nameof(ObterPedidoPorId), new { id = pedido.Id }, MapearPedido(pedido));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPedidoPorId(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
                return NotFound();

            return Ok(MapearPedido(pedido));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PedidoResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarPedidos(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? status = null)
        {
            var pedidos = await _pedidoRepository.ListarAsync();

            if (status.HasValue)
            {
                pedidos = pedidos.Where(p => (int)p.Status == status.Value);
            }

            var pedidosPaginados = pedidos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(pedidosPaginados.Select(MapearPedido));
        }

        [HttpPost("{id}/produto")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarProduto(int id, [FromBody] AdicionarProdutoRequest request)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == PedidoStatus.Fechado)
                return BadRequest("Não é possível adicionar produtos a um pedido fechado.");

            var produto = await _produtoRepository.ObterPorIdAsync(request.ProdutoId);
            if (produto == null)
                return NotFound("Produto não encontrado.");

            pedido.AdicionarProduto(produto, request.Quantidade);

            await _pedidoRepository.AtualizarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return Ok(MapearPedido(pedido));
        }

        [HttpDelete("{id}/produto/{produtoId}")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoverProduto(int id, int produtoId)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == PedidoStatus.Fechado)
                return BadRequest("Não é possível remover produtos de um pedido fechado.");

            pedido.RemoverProduto(produtoId);

            await _pedidoRepository.AtualizarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return Ok(MapearPedido(pedido));
        }

        [HttpPut("{id}/fechar")]
        [ProducesResponseType(typeof(PedidoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FecharPedido(int id)
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
                return NotFound("Pedido não encontrado.");

            if (pedido.Status == PedidoStatus.Fechado)
                return BadRequest("O pedido já está fechado.");

            if (pedido.Itens == null || pedido.Itens.Count == 0)
                return BadRequest("Não é possível fechar um pedido sem produtos.");

            pedido.FecharPedido();

            await _pedidoRepository.AtualizarAsync(pedido);
            await _pedidoRepository.SalvarAlteracoesAsync();

            return Ok(MapearPedido(pedido));
        }

        private static PedidoResponse MapearPedido(Pedido pedido)
        {
            return new PedidoResponse
            {
                Id = pedido.Id,
                Status = (int)pedido.Status,
                ValorTotal = pedido.CalcularValorTotal(),
                Itens = pedido.Itens.Select(item => new PedidoItemResponse
                {
                    Id = item.Id,
                    ProdutoId = item.ProdutoId,
                    NomeProduto = item.Produto.Nome,
                    PrecoUnitario = item.Produto.Preco,
                    Quantidade = item.Quantidade
                }).ToList()
            };
        }
    }
}