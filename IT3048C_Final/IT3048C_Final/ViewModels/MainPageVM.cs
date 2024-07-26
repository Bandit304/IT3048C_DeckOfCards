using IT3048C_Final.Models;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

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
            InitializeAsync();
        }

        // For getting initial data from API
        private async void InitializeAsync()
        {
            int? deckCount = await App.DeckAPI.GetNumberOfCardsInDeck();
            if (deckCount != null)
                CardsInDeck = (int)deckCount;
        }

        // ===== COMMAND CALLBACKS =====

        // Handles pressing the "Draw Card" button
        public async void OnDrawCard()
        {

        }

        // Handles pressing the "Add to Hand" button
        public async void OnAddCardToHand()
        {

        }

        // Handles pressing the "Discard" button
        public async void OnDiscardCard()
        {

        }

        // Handles pressing the "Discard" buttons in the hand list
        public async void OnDiscardCardFromHand(string cardCode)
        {

        }
    }
}