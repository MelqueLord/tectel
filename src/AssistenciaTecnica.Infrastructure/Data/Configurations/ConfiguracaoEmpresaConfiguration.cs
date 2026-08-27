using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class ConfiguracaoEmpresaConfiguration : IEntityTypeConfiguration<ConfiguracaoEmpresa>
{
    public void Configure(EntityTypeBuilder<ConfiguracaoEmpresa> builder)
    {
        builder.ToTable("ConfiguracaoEmpresa");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CpfCnpj)
            .HasMaxLength(14);

        builder.Property(c => c.Telefone)
            .HasMaxLength(20);

        builder.Property(c => c.WhatsApp)
            .HasMaxLength(20);

        builder.Property(c => c.Endereco)
            .HasMaxLength(300);

        builder.Property(c => c.LogoCaminho)
            .HasMaxLength(500);

        builder.Property(c => c.TextoPadraoOrdem)
            .HasMaxLength(5000);

        builder.Property(c => c.PrazoPadraoGarantiaDias)
            .HasDefaultValue(90);
    }
}
