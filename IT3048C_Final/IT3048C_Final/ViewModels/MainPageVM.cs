using IT3048C_Final.Models;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.ComponentModel;
using static System.Net.WebRequestMethods;

namespace IT3048C_Final.ViewModels
{
    public class MainPageVM : INotifyPropertyChanged
    {
        // ===== INotifyPropertyChanged Fields =====


        public event PropertyChangedEventHandler PropertyChanged;


        // ===== Fields =====


        // Unbindable Fields
        private ObservableCollection<Card> _hand;
        private int _cardsInDeck;
        private Card _card;

        // Bindable Fields
        public ObservableCollection<Card> Hand
        {
            get => _hand;
            private set
            {
                _hand = value;
                // Update UI
                OnPropertyChanged(nameof(Hand));
            }
        }
        public int CardsInDeck
        {
            get => _cardsInDeck;
            private set
            {
                _cardsInDeck = value;
                // Update UI
                OnPropertyChanged(nameof(CardsInDeck));
            }
        }
        public Card DrawnCard
        {
            get => _card;
            private set
            {
                _card = value;
                // Tell UI to update
                OnPropertyChanged(nameof(DrawnCard));
                OnPropertyChanged(nameof(EnableCardButtons));
                OnPropertyChanged(nameof(DrawnCardImage));
            }
        }
        // For Displaying the Drawn Card Image
        public string DrawnCardImage
        {
            get
            {
                if (DrawnCard != null)
                    return DrawnCard.image;
                else
                    return "https://www.deckofcardsapi.com/static/img/back.png";
            }
        }
        public bool EnableCardButtons { get => DrawnCard != null; }

        // Bindable Commands
        public Command DrawCard { get; private set; }
        public Command AddCardToHand { get; private set; }
        public Command DiscardCard { get; private set; }
        public Command DiscardCardFromHand { get; private set; }


        // ===== Constructor =====


        public MainPageVM()
        {
            // Initialize ObservableCollections
            Hand = new ObservableCollection<Card>();

            // Initialize Commands
            DrawCard = new Command(OnDrawCard);
            AddCardToHand = new Command(OnAddCardToHand);
            DiscardCard = new Command(OnDiscardCard);
            DiscardCardFromHand = new Command(cardCode => OnDiscardCardFromHand(cardCode as string));

            // Get initial API data
            // Use Task.Run(...).Wait() since constructor's not an async function
            Task.Run(InitializeAsync).Wait();
        }


        // ===== Methods =====


        // For getting initial data from API
        private async Task InitializeAsync()
        {
            try
            {
                // Fetch the number of cards in the deck
                CardsInDeck = await App.DeckAPI.GetNumberOfCardsInDeck();
            }
            catch (Exception ex)
            {
                // Handle potential exceptions
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to initialize deck count: {ex.Message}", "OK");
            }
        }

        private async Task UpdateHand()
        {
            Card[] cards = await App.DeckAPI.GetCardsInHand();
            if (cards != null)
            {
                Hand.Clear();
                foreach (Card card in cards)
                    Hand.Add(card);
            }
        }


        // ===== COMMAND CALLBACKS =====


        // Handles pressing the "Draw Card" button
        public async void OnDrawCard()
        {
            try
            {
                // If there are cards in deck AND there's currently no card drawn, draw new card
                if (CardsInDeck > 0 && DrawnCard == null)
                {
                    // Draw new card
                    DrawnCard = await App.DeckAPI.DrawCard();
                    // Update deck count
                    CardsInDeck = await App.DeckAPI.GetNumberOfCardsInDeck();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Warning", "No card was drawn.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to draw card: {ex.Message}", "OK");
            }
        }


        // Handles pressing the "Add to Hand" button
        public async void OnAddCardToHand()
        {
            if (DrawnCard != null)
            {
                // Send request to add card to hand in API
                await App.DeckAPI.AddCardToHand(DrawnCard.code);
                await UpdateHand();
                DrawnCard = null; // Clear the drawn card
                // Update deck count
                CardsInDeck = await App.DeckAPI.GetNumberOfCardsInDeck();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "No card drawn to add to hand.", "OK");
            }
        }


        // Handles pressing the "Discard" button
        public async void OnDiscardCard()
        {
            if (DrawnCard != null)
            {
                await App.DeckAPI.DiscardCard(DrawnCard.code); // Discard the drawn card
                DrawnCard = null; // Clear the drawn card
                // Update deck count
                CardsInDeck = await App.DeckAPI.GetNumberOfCardsInDeck();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "No card to discard.", "OK");
            }
        }


        // Handles pressing the "Discard" buttons in the hand list
        public async void OnDiscardCardFromHand(string cardCode)
        {
            if (cardCode != null)
            {
                // Send request to API to discard card from hand
                await App.DeckAPI.DiscardCardFromHand(cardCode);
                await UpdateHand(); // Update Hand UI
                // Update deck count
                CardsInDeck = await App.DeckAPI.GetNumberOfCardsInDeck();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Card not found in hand.", "OK");
            }
        }


        // ===== INotifyPropertyChanged Methods =====


        // Method to call when updating a value to tell the UI to update
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
