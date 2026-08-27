using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

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

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Endereco)
            .HasMaxLength(300);

        builder.Property(c => c.Bairro)
            .HasMaxLength(100);

        builder.Property(c => c.Cidade)
            .HasMaxLength(100);

        builder.Property(c => c.Estado)
            .HasMaxLength(2);

        builder.Property(c => c.Cep)
            .HasMaxLength(8);

        builder.Property(c => c.Observacoes)
            .HasMaxLength(1000);

        builder.Property(c => c.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.DataCadastro)
            .IsRequired();

        builder.Property(c => c.DataAtualizacao);

        builder.HasIndex(c => c.Nome);

        builder.HasIndex(c => c.CpfCnpj)
            .IsUnique()
            .HasFilter("[CpfCnpj] IS NOT NULL");
    }
}
