using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class OrdemServicoConfiguration : IEntityTypeConfiguration<OrdemServico>
{
    public void Configure(EntityTypeBuilder<OrdemServico> builder)
    {
        builder.ToTable("OrdensServico");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Numero)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.ClienteId)
            .IsRequired();

        builder.Property(o => o.AparelhoId)
            .IsRequired();

        builder.Property(o => o.DefeitoRelatado)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(o => o.Diagnostico)
            .HasMaxLength(1000);

        builder.Property(o => o.ServicoRealizado)
            .HasMaxLength(2000);

        builder.Property(o => o.DataEntrada)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(o => o.ValorServico)
            .HasPrecision(10, 2);

        builder.Property(o => o.ValorPecas)
            .HasPrecision(10, 2);

        builder.Property(o => o.Desconto)
            .HasPrecision(10, 2);

        builder.Property(o => o.ValorTotal)
            .HasPrecision(10, 2);

        builder.Property(o => o.ValorPago)
            .HasPrecision(10, 2);

        builder.Property(o => o.Observacoes)
            .HasMaxLength(2000);

        builder.Property(o => o.PrazoGarantiaDias)
            .HasDefaultValue(90);

        builder.HasOne(o => o.Cliente)
            .WithMany()
            .HasForeignKey(o => o.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Aparelho)
            .WithMany()
            .HasForeignKey(o => o.AparelhoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(o => o.Numero)
            .IsUnique();

        builder.HasIndex(o => o.ClienteId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.DataEntrada);
    }
}
