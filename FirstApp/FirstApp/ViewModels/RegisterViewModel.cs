public class RegisterViewModel : INotifyPropertyChanged
{
    private readonly ISupabaseService _service;

    public string Name { get; set; }
    public string Phone { get; set; }

    public ICommand SaveCommand { get; }
    public ICommand ViewRecordsCommand { get; }

    public RegisterViewModel(ISupabaseService service)
    {
        _service = service;
        SaveCommand = new Command(async () => await Save());
        ViewRecordsCommand = new Command(async () => await Shell.Current.GoToAsync("records"));
    }

    private async Task Save()
    {
        var contact = new Contact { Name = Name, Phone = Phone };
        await _service.AddContactAsync(contact);
        // limpar campos e opcionalmente mostrar mensagem
        Name = Phone = string.Empty;
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Phone));
    }

    // INotifyPropertyChanged implementation omitted for brevidade
}
