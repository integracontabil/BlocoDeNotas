using Blazored.LocalStorage;
using BlocoDeNotasPWA.Models;
using BlocoDeNotasPWA.Services;

namespace BlocoDeNotasPWA.ViewModels;

public class NotaViewModel
{
    private readonly ILocalStorageService _localStorage;
    private readonly SupabaseService _supabase;
    private const string StorageKey = "notas";

    public List<Nota> Notas { get; private set; } = new();
    public string Texto { get; set; } = string.Empty;

    public NotaViewModel(ILocalStorageService localStorage, SupabaseService supabase)
    {
        _localStorage = localStorage;
        _supabase = supabase;
    }

    // 🔹 Carregar notas (Supabase → fallback LocalStorage)
    public async Task CarregarNotas()
    {
        try
        {
            var notas = await _supabase.GetNotasAsync();

            if (notas is not null && notas.Any())
            {
                Notas = notas;
                await _localStorage.SetItemAsync(StorageKey, Notas);
                Console.WriteLine("Carregado do Supabase.");
            }
            else
            {
                var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey);
                Notas = stored ?? new List<Nota>();
                Console.WriteLine("Carregado do LocalStorage (Supabase vazio).");
            }
        }
        catch
        {
            var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey);
            Notas = stored ?? new List<Nota>();
            Console.WriteLine("Carregado do LocalStorage (erro Supabase).");
        }
    }

    // 🔹 Adicionar nota
    public async Task AdicionarNota()
    {
        if (string.IsNullOrWhiteSpace(Texto)) return;

        var nota = new Nota { Texto = Texto };
        Notas.Add(nota);
        Texto = string.Empty;

        // Local
        await _localStorage.SetItemAsync(StorageKey, Notas);

        // Supabase
        try
        {
            await _supabase.AddNotaAsync(nota);
            Console.WriteLine($"Nota adicionada ao Supabase: {nota.Texto}");
        }
        catch
        {
            Console.WriteLine("Falha ao salvar no Supabase, mantida apenas no LocalStorage.");
        }
    }

    // 🔹 Deletar nota
    public async Task DeletarNota(Guid id)
    {
        Notas.RemoveAll(n => n.Id == id);
        await _localStorage.SetItemAsync(StorageKey, Notas);

        try
        {
            await _supabase.DeleteNotaAsync(id);
            Console.WriteLine($"Nota {id} deletada do Supabase.");
        }
        catch
        {
            Console.WriteLine("Falha ao deletar no Supabase, removida apenas do LocalStorage.");
        }
    }

    // 🔹 Alterar nota
    public async Task AlterarNota(Guid id, string novoTexto)
    {
        var nota = Notas.FirstOrDefault(n => n.Id == id);
        if (nota != null)
        {
            nota.Texto = novoTexto;
        }

        await _localStorage.SetItemAsync(StorageKey, Notas);

        try
        {
            await _supabase.UpdateNotaAsync(nota!);
            Console.WriteLine($"Nota {id} atualizada no Supabase.");
        }
        catch
        {
            Console.WriteLine("Falha ao atualizar no Supabase, alterada apenas no LocalStorage.");
        }
    }
}
