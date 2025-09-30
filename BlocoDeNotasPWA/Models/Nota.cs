namespace BlocoDeNotasPWA.Models;

public class Nota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Texto { get; set; } = string.Empty;

    // mapeia para "criado_em" no Supabase
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    // mapeia para "usuario_id" (auth.uid())
    public Guid UsuarioId { get; set; }
}
