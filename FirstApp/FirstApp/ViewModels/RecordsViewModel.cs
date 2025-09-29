< ContentPage xmlns = "http://schemas.microsoft.com/dotnet/2021/maui"
             x: Class = "FirstApp.Views.RecordsPage"
             Title = "Registros Gravados" >
  < VerticalStackLayout Padding = "10" >
    < Label Text = "Registros Gravados" FontSize = "22" HorizontalOptions = "Center" />
    < CollectionView ItemsSource = "{Binding Items}" >
      < CollectionView.ItemTemplate >
        < DataTemplate >
          < Grid Padding = "8" ColumnDefinitions = "*,Auto,Auto" >
            < StackLayout >
              < Label Text = "{Binding Name}" FontAttributes = "Bold" />
              < Label Text = "{Binding Phone}" FontSize = "Small" />
            </ StackLayout >
            < Button Text = "Alterar" Command = "{Binding Source={RelativeSource AncestorType={x:Type vm:RecordsViewModel}}, Path=EditCommand}" CommandParameter = "{Binding .}" Grid.Column = "1" />
            < Button Text = "Excluir" Command = "{Binding Source={RelativeSource AncestorType={x:Type vm:RecordsViewModel}}, Path=DeleteCommand}" CommandParameter = "{Binding .}" Grid.Column = "2" />
          </ Grid >
        </ DataTemplate >
      </ CollectionView.ItemTemplate >
    </ CollectionView >

    < Button Text = "Cadastrar Novo" Command = "{Binding NewCommand}" />
  </ VerticalStackLayout >
</ ContentPage >
