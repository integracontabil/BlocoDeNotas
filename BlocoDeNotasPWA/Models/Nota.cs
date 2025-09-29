namespace BlocoDeNotasPWA.Models;

public class Nota
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Texto { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
