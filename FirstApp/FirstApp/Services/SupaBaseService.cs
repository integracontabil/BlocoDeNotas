public interface ISupabaseService
{
    Task InitializeAsync();
    Task<IEnumerable<Contact>> GetContactsAsync();
    Task<Contact> AddContactAsync(Contact c);
    Task<Contact> UpdateContactAsync(Contact c);
    Task DeleteContactAsync(long id);
}

public class SupabaseService : ISupabaseService
{
    private readonly Supabase.Client _client;

    public SupabaseService(Supabase.Client client)
    {
        _client = client;
    }

    public async Task InitializeAsync()
    {
        if (!_client.Initialized)
            await _client.InitializeAsync();
    }

    public async Task<IEnumerable<Contact>> GetContactsAsync()
    {
        await InitializeAsync();
        var resp = await _client.From<Contact>().Get();
        return resp.Models;
    }

    public async Task<Contact> AddContactAsync(Contact c)
    {
        await InitializeAsync();
        var resp = await _client.From<Contact>().Insert(c);
        return resp.Models.FirstOrDefault();
    }

    public async Task<Contact> UpdateContactAsync(Contact c)
    {
        await InitializeAsync();
        var resp = await _client.From<Contact>().Update(c);
        return resp.Models.FirstOrDefault();
    }

    public async Task DeleteContactAsync(long id)
    {
        await InitializeAsync();
        await _client.From<Contact>().Delete(new { id });
    }
}
