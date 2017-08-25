namespace DM106RafaelSilva.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Models;

    internal sealed class Configuration : DbMigrationsConfiguration<DM106RafaelSilva.Models.DM106RafaelSilvaContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;

        }

        protected override void Seed(DM106RafaelSilva.Models.DM106RafaelSilvaContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            context.Database.CreateIfNotExists();
            context.Products.AddOrUpdate(
                p => p.Id,
                new Product { Id = 1, nome = "produto 1", descricao = "descrição produto 1", cor = "preto", modelo = "1P", codigo = "COD1", preco = 10, peso = 10, altura = 10, largura = 10, comprimento = 10, diametro = 10, url = "hostname.com/produto1" },
                new Product { Id = 2, nome = "produto 2", descricao = "descrição produto 2", cor = "branco", modelo = "2P", codigo = "COD2", preco = 20, peso = 20, altura = 20, largura = 20, comprimento = 20, diametro = 20, url = "hostname.com/produto2" },
                new Product { Id = 3, nome = "produto 3", descricao = "descrição produto 3", cor = "azul", modelo = "3P", codigo = "COD3", preco = 30, peso = 30, altura = 30, largura = 30, comprimento = 30, diametro = 30, url = "hostname.com/produto3" },
                new Product { Id = 4, nome = "produto 4", descricao = "descrição produto 4", cor = "amarelo", modelo = "4P", codigo = "COD3", preco = 40, peso = 40, altura = 40, largura = 40, comprimento = 40, diametro = 40, url = "hostname.com/produto4" },
                new Product { Id = 5, nome = "produto 5", descricao = "descrição produto 5", cor = "verde", modelo = "5P", codigo = "COD5", preco = 50, peso = 50, altura = 50, largura = 50, comprimento = 50, diametro = 50, url = "hostname.com/produto5" },
                new Product { Id = 6, nome = "produto 6", descricao = "descrição produto 6", cor = "roxo", modelo = "6P", codigo = "COD6", preco = 60, peso = 60, altura = 60, largura = 60, comprimento = 60, diametro = 60, url = "hostname.com/produto6" },
                new Product { Id = 7, nome = "produto 7", descricao = "descrição produto 7", cor = "marrom", modelo = "7P", codigo = "COD7", preco = 70, peso = 70, altura = 70, largura = 70, comprimento = 70, diametro = 70, url = "hostname.com/produto7" },
                new Product { Id = 8, nome = "produto 8", descricao = "descrição produto 8", cor = "rosa", modelo = "8P", codigo = "COD8", preco = 80, peso = 80, altura = 80, largura = 80, comprimento = 80, diametro = 80, url = "hostname.com/produto8" },
                new Product { Id = 9, nome = "produto 9", descricao = "descrição produto 9", cor = "laranja", modelo = "9P", codigo = "COD9", preco = 90, peso = 90, altura = 90, largura = 90, comprimento = 90, diametro = 90, url = "hostname.com/produto9" }
            );
        }
    }
}
