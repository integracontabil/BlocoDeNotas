using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlocoDeNotasPWA.Models;

namespace BlocoDeNotasPWA.Services;

public class SupabaseService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly string _url;
    private readonly string _key;
    private const string StorageKey = "notas";

    public SupabaseService(IConfiguration config, HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;

        // Monta a URL da API REST
        _url = $"{config["SupabaseUrl"]}/rest/v1/notas";
        _key = config["SupabaseKey"]!;

        // Cabeçalhos obrigatórios para Supabase
        _http.DefaultRequestHeaders.Add("apikey", _key);
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_key}");
        _http.DefaultRequestHeaders.Add("Prefer", "return=representation");
    }

    // 🔹 Buscar notas
    public async Task<List<Nota>> GetNotasAsync()
    {
        try
        {
            var notas = await _http.GetFromJsonAsync<List<Nota>>($"{_url}?select=*");

            if (notas is not null)
                await _localStorage.SetItemAsync(StorageKey, notas);

            return notas ?? new();
        }
        catch
        {
            var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey);
            return stored ?? new();
        }
    }

    // 🔹 Adicionar nota
    public async Task AddNotaAsync(Nota nota)
    {
        // Monta payload compatível com a tabela
        var payload = new
        {
            id = nota.Id,              // uuid (gerado pelo C#)
            texto = nota.Texto,        // campo obrigatório
            criado_em = nota.CriadoEm // opcional, Supabase já tem default
        };

        // Atualiza cache local
        var stored = await GetNotasLocal();
        stored.Add(nota);
        await _localStorage.SetItemAsync(StorageKey, stored);

        try
        {
            var response = await _http.PostAsJsonAsync(_url, payload);
            response.EnsureSuccessStatusCode();
        }
        catch
        {
            // offline → mantém apenas no LocalStorage
        }
    }

    // 🔹 Atualizar nota
    public async Task UpdateNotaAsync(Nota nota)
    {
        var stored = await GetNotasLocal();
        var idx = stored.FindIndex(n => n.Id == nota.Id);
        if (idx >= 0) stored[idx] = nota;
        await _localStorage.SetItemAsync(StorageKey, stored);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"{_url}?id=eq.{nota.Id}")
            {
                Content = JsonContent.Create(new { texto = nota.Texto })
            };
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch { }
    }

    // 🔹 Deletar nota
    public async Task DeleteNotaAsync(Guid id)
    {
        var stored = await GetNotasLocal();
        stored.RemoveAll(n => n.Id == id);
        await _localStorage.SetItemAsync(StorageKey, stored);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_url}?id=eq.{id}");
            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch { }
    }

    // 🔹 Utilitário: pega cache local
    private async Task<List<Nota>> GetNotasLocal()
    {
        var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey);
        return stored ?? new List<Nota>();
    }
}
