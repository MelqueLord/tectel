using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descricao)
            .HasMaxLength(1000);

        builder.Property(p => p.Categoria)
            .HasMaxLength(100);

        builder.Property(p => p.Marca)
            .HasMaxLength(100);

        builder.Property(p => p.ModeloCompativel)
            .HasMaxLength(200);

        builder.Property(p => p.PrecoCusto)
            .HasPrecision(10, 2);

        builder.Property(p => p.PrecoVenda)
            .HasPrecision(10, 2);

        builder.Property(p => p.QuantidadeEstoque)
            .HasDefaultValue(0);

        builder.Property(p => p.EstoqueMinimo)
            .HasDefaultValue(0);

        builder.Property(p => p.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.DataCadastro)
            .IsRequired();

        builder.Ignore(p => p.EstoqueBaixo);

        builder.HasIndex(p => p.Codigo)
            .IsUnique();

        builder.HasIndex(p => p.Nome);
        builder.HasIndex(p => p.Categoria);
    }
}
