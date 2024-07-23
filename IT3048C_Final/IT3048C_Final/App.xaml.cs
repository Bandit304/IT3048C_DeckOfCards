using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using IT3048C_Final.Views;
using IT3048C_Final.DeckAPI;

namespace IT3048C_Final
{
    public partial class App : Application
    {
        public static Deck DeckAPI { get; set; }

        public App()
        {
            InitializeComponent();

            DeckAPI = new Deck("https://www.deckofcardsapi.com/api/deck/");

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
