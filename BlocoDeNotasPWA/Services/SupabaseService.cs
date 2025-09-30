using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using BlocoDeNotasPWA.Models;

namespace BlocoDeNotasPWA.Services;

public class SupabaseService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthService _auth;
    private readonly string _urlNotas;
    private readonly string _anonKey;
    private const string StorageKey = "notas";

    public SupabaseService(IConfiguration config, HttpClient http, ILocalStorageService localStorage, AuthService auth)
    {
        _http = http;
        _localStorage = localStorage;
        _auth = auth;
        _urlNotas = $"{config["SupabaseUrl"]}/rest/v1/notas";
        _anonKey = config["SupabaseKey"]!;
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string url, object? content = null)
    {
        var req = new HttpRequestMessage(method, url);
        req.Headers.Add("apikey", _anonKey);
        var token = _auth.GetAccessToken();
        if (!string.IsNullOrEmpty(token))
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _anonKey);

        if (content != null) req.Content = JsonContent.Create(content);
        return req;
    }

    public async Task<List<Nota>> GetNotasAsync()
    {
        try
        {
            var req = CreateRequest(HttpMethod.Get, $"{_urlNotas}?select=*");
            var resp = await _http.SendAsync(req);
            if (resp.IsSuccessStatusCode)
            {
                var notas = await resp.Content.ReadFromJsonAsync<List<Nota>>();
                if (notas is not null) await _localStorage.SetItemAsync(StorageKey, notas);
                return notas ?? new();
            }
            else
            {
                var local = await _localStorage.GetItemAsync<List<Nota>>(StorageKey);
                return local ?? new();
            }
        }
        catch
        {
            var local = await _localStorage.GetItemAsync<List<Nota>>(StorageKey);
            return local ?? new();
        }
    }

    public async Task AddNotaAsync(Nota nota)
    {
        var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey) ?? new List<Nota>();
        stored.Add(nota);
        await _localStorage.SetItemAsync(StorageKey, stored);

        try
        {
            var payload = new
            {
                id = nota.Id,
                texto = nota.Texto,
                criado_em = nota.CriadoEm,
                usuario_id = nota.UsuarioId   // 👈 incluído
            };

            var req = CreateRequest(HttpMethod.Post, _urlNotas, payload);
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }
        catch
        {
            // offline, nota fica só no localStorage
        }
    }


    public async Task UpdateNotaAsync(Nota nota)
    {
        var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey) ?? new List<Nota>();
        var idx = stored.FindIndex(n => n.Id == nota.Id);
        if (idx >= 0) stored[idx] = nota;
        await _localStorage.SetItemAsync(StorageKey, stored);

        try
        {
            var req = CreateRequest(new HttpMethod("PATCH"), $"{_urlNotas}?id=eq.{nota.Id}", new { texto = nota.Texto });
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }
        catch { }
    }

    public async Task DeleteNotaAsync(Guid id)
    {
        var stored = await _localStorage.GetItemAsync<List<Nota>>(StorageKey) ?? new List<Nota>();
        stored.RemoveAll(n => n.Id == id);
        await _localStorage.SetItemAsync(StorageKey, stored);

        try
        {
            var req = CreateRequest(HttpMethod.Delete, $"{_urlNotas}?id=eq.{id}");
            var resp = await _http.SendAsync(req);
            resp.EnsureSuccessStatusCode();
        }
        catch { }
    }
}
