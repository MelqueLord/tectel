using AssistenciaTecnica.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssistenciaTecnica.Infrastructure.Data.Configurations;

public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Descricao)
            .HasMaxLength(1000);

        builder.Property(a => a.DataInicio)
            .IsRequired();

        builder.Property(a => a.DataFim)
            .IsRequired();

        builder.Property(a => a.Tipo)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Cor)
            .HasMaxLength(20);

        builder.HasOne(a => a.OrdemServico)
            .WithMany()
            .HasForeignKey(a => a.OrdemServicoId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Cliente)
            .WithMany()
            .HasForeignKey(a => a.ClienteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(a => a.DataInicio);
        builder.HasIndex(a => a.Status);
    }
}
