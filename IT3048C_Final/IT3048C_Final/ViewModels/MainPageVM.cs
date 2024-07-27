using IT3048C_Final.Models;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.ComponentModel;

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
                // If card is not null, enable buttons
                EnableCardButtons = value != null;
                // Tell UI to update
                OnPropertyChanged(nameof(DrawnCard));
                OnPropertyChanged(nameof(EnableCardButtons));
            }
        }
        public bool EnableCardButtons { get; private set; }

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
                await UpdateDeckCount();
            }
            catch (Exception ex)
            {
                // Handle potential exceptions
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to initialize deck count: {ex.Message}", "OK");
            }
        }

        private async Task UpdateDeckCount()
        {
            // Fetch the number of cards in the deck
            int? deckCount = await App.DeckAPI.GetNumberOfCardsInDeck();
            CardsInDeck = deckCount ?? 0; // Use 0 if deckCount is null
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
                    await UpdateDeckCount();
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
                await UpdateDeckCount();
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
                await UpdateDeckCount();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "No card to discard.", "OK");
            }
        }


        // Handles pressing the "Discard" buttons in the hand list
        public async void OnDiscardCardFromHand(string cardCode)
        {
            if (string.IsNullOrEmpty(cardCode)) return;

            Card cardToDiscard = Hand.FirstOrDefault(c => c.code == cardCode);
            if (cardToDiscard != null)
            {
                // Send request to API to discard card from hand
                await App.DeckAPI.DiscardCardFromHand(cardCode);
                Hand.Remove(cardToDiscard); // Remove the card from hand
                // Update deck count
                await UpdateDeckCount();
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
