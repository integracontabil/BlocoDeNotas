using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace BlocoDeNotasPWA.Services;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly string _urlAuth;
    private readonly string _anonKey;

    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _expiresAt;

    private const string TOKEN_KEY = "auth_token";
    private const string REFRESH_KEY = "refresh_token";
    private const string EXPIRES_KEY = "token_expires";

    public event Action? AuthStateChanged;

    public AuthService(IConfiguration config, HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
        _urlAuth = $"{config["SupabaseUrl"]}/auth/v1";
        _anonKey = config["SupabaseKey"]!;
    }

    public bool IsAuthenticated => !string.IsNullOrEmpty(_accessToken) && _expiresAt > DateTime.UtcNow;

    public string? GetAccessToken() => _accessToken;

    public string? GetUserId()
    {
        if (string.IsNullOrEmpty(_accessToken)) return null;
        var parts = _accessToken.Split('.');
        if (parts.Length < 2) return null;
        var payload = Base64UrlDecode(parts[1]);
        using var doc = JsonDocument.Parse(payload);
        if (doc.RootElement.TryGetProperty("sub", out var sub)) return sub.GetString();
        if (doc.RootElement.TryGetProperty("user_id", out var uid)) return uid.GetString();
        return null;
    }

    public string? GetUserEmail()
    {
        if (string.IsNullOrEmpty(_accessToken)) return null;
        var parts = _accessToken.Split('.');
        if (parts.Length < 2) return null;
        var payload = Base64UrlDecode(parts[1]);
        using var doc = JsonDocument.Parse(payload);
        if (doc.RootElement.TryGetProperty("email", out var email)) return email.GetString();
        return null;
    }

    public async Task InitializeAsync()
    {
        _accessToken = await _localStorage.GetItemAsync<string>(TOKEN_KEY);
        _refreshToken = await _localStorage.GetItemAsync<string>(REFRESH_KEY);
        var expires = await _localStorage.GetItemAsync<string>(EXPIRES_KEY);
        if (!string.IsNullOrEmpty(expires) && DateTime.TryParse(expires, out var dt))
            _expiresAt = dt;
        else
            _expiresAt = DateTime.MinValue;

        if (!IsAuthenticated && !string.IsNullOrEmpty(_refreshToken))
        {
            var ok = await RefreshTokenAsync();
            if (!ok)
            {
                _accessToken = null;
                _refreshToken = null;
                _expiresAt = DateTime.MinValue;
            }
        }

        AuthStateChanged?.Invoke();
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync($"{_urlAuth}/signup", new { email, password });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var req = new HttpRequestMessage(HttpMethod.Post, $"{_urlAuth}/token?grant_type=password");
        req.Headers.Add("apikey", _anonKey);
        req.Content = JsonContent.Create(new { email, password });

        var response = await _http.SendAsync(req);
        if (!response.IsSuccessStatusCode)
            return false;

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);

        _accessToken = doc.RootElement.GetProperty("access_token").GetString();
        _refreshToken = doc.RootElement.GetProperty("refresh_token").GetString();
        var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
        _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

        await _localStorage.SetItemAsync(TOKEN_KEY, _accessToken);
        await _localStorage.SetItemAsync(REFRESH_KEY, _refreshToken);
        await _localStorage.SetItemAsync(EXPIRES_KEY, _expiresAt.ToString("o"));

        AuthStateChanged?.Invoke();
        return true;
    }

    public async Task LogoutAsync()
    {
        _accessToken = null;
        _refreshToken = null;
        _expiresAt = DateTime.MinValue;

        await _localStorage.RemoveItemAsync(TOKEN_KEY);
        await _localStorage.RemoveItemAsync(REFRESH_KEY);
        await _localStorage.RemoveItemAsync(EXPIRES_KEY);

        AuthStateChanged?.Invoke();
    }

    public async Task<bool> RefreshTokenAsync()
    {
        if (string.IsNullOrEmpty(_refreshToken)) return false;

        var response = await _http.PostAsJsonAsync($"{_urlAuth}/token?grant_type=refresh_token", new { refresh_token = _refreshToken });
        if (!response.IsSuccessStatusCode) return false;

        var content = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(content);

        _accessToken = doc.RootElement.GetProperty("access_token").GetString();
        _refreshToken = doc.RootElement.GetProperty("refresh_token").GetString();
        var expiresIn = doc.RootElement.GetProperty("expires_in").GetInt32();
        _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);

        await _localStorage.SetItemAsync(TOKEN_KEY, _accessToken);
        await _localStorage.SetItemAsync(REFRESH_KEY, _refreshToken);
        await _localStorage.SetItemAsync(EXPIRES_KEY, _expiresAt.ToString("o"));

        AuthStateChanged?.Invoke();
        return true;
    }

    private static string Base64UrlDecode(string input)
    {
        string s = input.Replace('-', '+').Replace('_', '/');
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }
        var bytes = Convert.FromBase64String(s);
        return Encoding.UTF8.GetString(bytes);
    }
}
