﻿using GestaoPedidos.Domain.Entities;

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
                new Produto { Nome = "Produto A", Preco = 10.10m },
                new Produto { Nome = "Produto B", Preco = 20.25m },
                new Produto { Nome = "Produto C", Preco = 30.50m }
            };

            context.Produtos.AddRange(produtos);
            context.SaveChanges();
        }
    }
}
