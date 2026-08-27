using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class AparelhoConfiguration : IEntityTypeConfiguration<Aparelho>
{
    public void Configure(EntityTypeBuilder<Aparelho> builder)
    {
        builder.ToTable("Aparelhos");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .ValueGeneratedOnAdd();

        builder.Property(a => a.ClienteId)
            .IsRequired();

        builder.Property(a => a.Tipo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.Marca)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Modelo)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.Cor)
            .HasMaxLength(50);

        builder.Property(a => a.Imei)
            .HasMaxLength(20);

        builder.Property(a => a.NumeroSerie)
            .HasMaxLength(50);

        builder.Property(a => a.SenhaDesbloqueio)
            .HasMaxLength(100);

        builder.Property(a => a.EstadoAparelho)
            .HasMaxLength(500);

        builder.Property(a => a.ItensEntregues)
            .HasMaxLength(500);

        builder.Property(a => a.Observacoes)
            .HasMaxLength(1000);

        builder.Property(a => a.DataCadastro)
            .IsRequired();

        builder.HasOne(a => a.Cliente)
            .WithMany()
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.ClienteId);
        builder.HasIndex(a => a.Imei);
    }
}
