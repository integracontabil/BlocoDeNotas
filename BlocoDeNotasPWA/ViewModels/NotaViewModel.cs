using Blazored.LocalStorage;
using BlocoDeNotasPWA.Models;
using BlocoDeNotasPWA.Services;

namespace BlocoDeNotasPWA.ViewModels;

public class NotaViewModel
{
    private readonly SupabaseService _supabase;
    private readonly AuthService _auth;
    private readonly ILocalStorageService _localStorage;
    private const string StorageKey = "notas";

    public List<Nota> Notas { get; private set; } = new();
    public string Texto { get; set; } = string.Empty;

    public NotaViewModel(ILocalStorageService localStorage, SupabaseService supabase, AuthService auth)
    {
        _localStorage = localStorage;
        _supabase = supabase;
        _auth = auth;

        _auth.AuthStateChanged += OnAuthStateChanged;
    }

    private void OnAuthStateChanged()
    {
        if (_auth.IsAuthenticated)
        {
            // dispara carregamento em background
            _ = Task.Run(async () => await CarregarNotas());
        }
        else
        {
            Notas = new List<Nota>();
        }
    }

    public async Task CarregarNotas()
    {
        Notas = await _supabase.GetNotasAsync();
    }

    public async Task AdicionarNota()
    {
        if (string.IsNullOrWhiteSpace(Texto)) return;

        var nota = new Nota
        {
            Texto = Texto,
            CriadoEm = DateTime.UtcNow
        };

        var userId = _auth.GetUserId();
        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var gid))
            nota.UsuarioId = gid;

        Notas.Add(nota);
        Texto = string.Empty;

        await _supabase.AddNotaAsync(nota);
    }

    public async Task DeletarNota(Guid id)
    {
        Notas.RemoveAll(n => n.Id == id);
        await _supabase.DeleteNotaAsync(id);
    }

    public async Task AlterarNota(Guid id, string novoTexto)
    {
        var nota = Notas.FirstOrDefault(n => n.Id == id);
        if (nota != null) nota.Texto = novoTexto;
        await _supabase.UpdateNotaAsync(nota!);
    }
}
