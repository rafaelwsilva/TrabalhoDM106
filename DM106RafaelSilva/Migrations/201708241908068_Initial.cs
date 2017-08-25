namespace DM106RafaelSilva.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        DateOrder = c.DateTime(nullable: false),
                        DateDelivery = c.DateTime(),
                        Status = c.String(),
                        TotalPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalWeight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        nome = c.String(nullable: false),
                        descricao = c.String(),
                        cor = c.String(),
                        modelo = c.String(nullable: false, maxLength: 8),
                        codigo = c.String(nullable: false, maxLength: 8),
                        preco = c.Decimal(nullable: false, precision: 18, scale: 2),
                        peso = c.Decimal(nullable: false, precision: 18, scale: 2),
                        altura = c.Decimal(nullable: false, precision: 18, scale: 2),
                        largura = c.Decimal(nullable: false, precision: 18, scale: 2),
                        comprimento = c.Decimal(nullable: false, precision: 18, scale: 2),
                        diametro = c.Decimal(nullable: false, precision: 18, scale: 2),
                        url = c.String(maxLength: 80),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "ProductId", "dbo.Products");
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.OrderItems", new[] { "ProductId" });
            DropTable("dbo.Products");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
        }
    }
}
