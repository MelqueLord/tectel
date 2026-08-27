using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("Pagamentos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.OrdemServicoId)
            .IsRequired();

        builder.Property(p => p.DataPagamento)
            .IsRequired();

        builder.Property(p => p.Valor)
            .IsRequired()
            .HasPrecision(10, 2);

        builder.Property(p => p.FormaPagamento)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Observacao)
            .HasMaxLength(500);

        builder.HasOne(p => p.OrdemServico)
            .WithMany(o => o.Pagamentos)
            .HasForeignKey(p => p.OrdemServicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.OrdemServicoId);
        builder.HasIndex(p => p.DataPagamento);
    }
}
