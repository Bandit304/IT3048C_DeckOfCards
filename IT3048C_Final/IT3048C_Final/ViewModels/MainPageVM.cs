using IT3048C_Final.Models;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace IT3048C_Final.ViewModels
{
    public class MainPageVM
    {
        // Unbindable Fields
        private Card _card;

        // Bindable Fields
        public ObservableCollection<Card> Hand { get; set; }
        public int CardsInDeck { get; set; }
        public Card DrawnCard
        {
            get => _card;
            set
            {
                _card = value;
                // If card is not null, enable buttons
                EnableCardButtons = value != null;
            }
        }
        public bool EnableCardButtons { get; private set; }

        // Bindable Commands
        public Command DrawCard { get; private set; }
        public Command AddCardToHand { get; private set; }
        public Command DiscardCard { get; private set; }
        public Command DiscardCardFromHand { get; private set; }

        public MainPageVM()
        {
            // Initialize Commands
            DrawCard = new Command(OnDrawCard);
            AddCardToHand = new Command(OnAddCardToHand);
            DiscardCard = new Command(OnDiscardCard);
            DiscardCardFromHand = new Command(cardCode => OnDiscardCardFromHand(cardCode as string));

            // Get initial API data
            // Use Task.Run(...).Wait() since constructor's not an async function
            Task.Run(InitializeAsync).Wait();
        }

        // For getting initial data from API
        private async Task InitializeAsync()
        {
            try
            {
                // Fetch the number of cards in the deck
                int? deckCount = await App.DeckAPI.GetNumberOfCardsInDeck();
                CardsInDeck = deckCount ?? 0; // Use 0 if deckCount is null
            }
            catch (Exception ex)
            {
                // Handle potential exceptions
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to initialize deck count: {ex.Message}", "OK");
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
                    int? deckCount = await App.DeckAPI.GetNumberOfCardsInDeck();
                    CardsInDeck = deckCount ?? 0;
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
                Hand.Add(DrawnCard); // Add the drawn card to the hand
                DrawnCard = null; // Clear the drawn card
                EnableCardButtons = false; // Disable buttons if needed
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
                EnableCardButtons = false; // Disable buttons if needed
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
                int? deckCount = await App.DeckAPI.GetNumberOfCardsInDeck();
                CardsInDeck = deckCount ?? 0; // Use 0 if deckCount is null
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Card not found in hand.", "OK");
            }
        }

    }
}
