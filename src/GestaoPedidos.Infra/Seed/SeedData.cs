using GestaoPedidos.Domain.Entities;

namespace GestaoPedidos.Infra.Seed
{
    public static class SeedData
    {
        public static void Inicializar(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Produtos.Any())
                return;

            var produtos = new List<Produto>
            {
                new Produto { Nome = "Produto A", Preco = 10 },
                new Produto { Nome = "Produto B", Preco = 20 },
                new Produto { Nome = "Produto C", Preco = 30 }
            };

            context.Produtos.AddRange(produtos);
            context.SaveChanges();
        }
    }
}
