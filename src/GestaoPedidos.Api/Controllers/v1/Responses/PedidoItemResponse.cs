namespace GestaoPedidos.Api.Controllers.v1.Responses
{
    public class PedidoItemResponse
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = null!;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
    }
}
