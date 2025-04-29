namespace GestaoPedidos.Api.Controllers.v1.Responses
{
    public class PedidoResponse
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public decimal ValorTotal { get; set; }
        public List<PedidoItemResponse> Itens { get; set; } = new();
    }
}
