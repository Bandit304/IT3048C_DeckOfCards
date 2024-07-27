using IT3048C_Final.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IT3048C_Final.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MainPageVM ViewModel { get; set; }

        public MainPage()
        {
            InitializeComponent();

            BindingContext = ViewModel = new MainPageVM();
        }
    }
}