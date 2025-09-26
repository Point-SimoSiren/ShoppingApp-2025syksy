using ShoppingApp.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace ShoppingApp
{
    public partial class MainPage : ContentPage
    {
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingbackendope.azurewebsites.net");
            string json = await client.GetStringAsync("/api/shoplist/");

            // Deserialisoidaan json muodosta C# muotoon
            IEnumerable<Shoplist> ienumShoplist = JsonConvert.DeserializeObject<Shoplist[]>(json);

         
            ObservableCollection<Shoplist> shoplistCollection = new(ienumShoplist);

            // Data näkyviin listassa
            itemList.ItemsSource = shoplistCollection;

            // Latausilmoitus pois näkyvistä
            Loading_label.IsVisible = false;
        }
    }
}