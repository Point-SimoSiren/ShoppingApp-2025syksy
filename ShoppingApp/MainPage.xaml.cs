using ShoppingApp.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace ShoppingApp
{
    public partial class MainPage : ContentPage
    {

        private readonly HttpClient client = new HttpClient
            {
              BaseAddress = new Uri("https://shoppingbackendope.azurewebsites.net")
            };
        
      
        public MainPage()
        {
            InitializeComponent();
          
            LoadDataFromRestAPI();
        }



        // LISTAN HAKEMINEN BACKENDISTÄ
        public async Task LoadDataFromRestAPI()
        {
            // Latausilmoitus näkyviin
            Loading_label.IsVisible = true;
            string json = await client.GetStringAsync("/api/shoplist/");
            // Deserialisoidaan json muodosta C# muotoon
            IEnumerable<Shoplist> ienumShoplist = JsonConvert.DeserializeObject<Shoplist[]>(json);

            ObservableCollection<Shoplist> shoplistCollection = new(ienumShoplist);

            // Data näkyviin listassa
            itemList.ItemsSource = shoplistCollection;

            // Latausilmoitus pois näkyvistä
            Loading_label.IsVisible = false;
        }


        // Lisäyssivulle navigoiminen
        private async void addPageBtn_Clicked(object sender, EventArgs e)
        {
            // This on tämä luokka. Se välitetään AddingPagelle jotta sieltä voidaan kutsua
            // LoadDataFromRestAPI metodia, joka kuuluu MainPagelle.
            AddingPage addingPage = new AddingPage(this);
            await Shell.Current.Navigation.PushModalAsync(addingPage);
        }

        
        private void itemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Shoplist? selectedItem = itemList.SelectedItem as Shoplist;
            kerätty_nappi.Text = "Poimi " + selectedItem?.Item;
        }


        private async void kerätty_nappi_Clicked(object sender, EventArgs e)
        {
            Shoplist? selected = itemList.SelectedItem as Shoplist;

            if (selected == null)
            {
                await DisplayAlert("Valinta puuttuu", "Valitse ensin poimittava tuote", "ok");
                return;
            }

            bool answer = await DisplayAlert("Menikö oikein?", selected.Item + " kerätty?", "Yes! Kyllä meni!", "Ei, Yritän uusiksi");
            if (answer == false)
            {
                return;
            }

            // Jos kaikki on hyvin tuote poistetaan
           
            HttpResponseMessage res = await client.DeleteAsync("/api/shoplist/" + selected.Id);

            if (res.StatusCode == System.Net.HttpStatusCode.OK || 
                res.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                await LoadDataFromRestAPI();
            }
            else
            {
                await DisplayAlert("Tilapäinen virhe", "Joku muu on saattanut poistaa tuotteen sen jälkeen kun listauksesi on viimeeksi päivittynyt?",
                    "Lataa uudelleen");
                await LoadDataFromRestAPI();

            }


        }
    }
}