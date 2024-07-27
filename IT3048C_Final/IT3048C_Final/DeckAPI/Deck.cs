using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IT3048C_Final.DeckAPI.Responses;
using IT3048C_Final.Models;
using Newtonsoft.Json;

namespace IT3048C_Final.DeckAPI
{
    // Class to handle interactions with the Deck of Cards API
    public class Deck
    {
        // HttpClient object to handle API calls
        private HttpClient _httpClient;
        // Link to API
        public string ApiUrl;
        // ID for deck in API
        public string DeckID;

        public Deck(string apiUrl)
        {
            // Save Link to API
            ApiUrl = apiUrl;
            // Generate new HttpClient
            _httpClient = new HttpClient();
            // Generate new Deck, save Deck ID
            // Use Task.Run(...).Wait() since constructor's not an async function
            Task.Run(GenerateNewDeckID).Wait();
        }

        public async Task<ResponseType> GetApiRequest<ResponseType>(string path) where ResponseType : GenericResponse
        {
            // Define output variable
            ResponseType responseObject = null;
            // Send GET request to API
            HttpResponseMessage response = await _httpClient.GetAsync($"{ApiUrl}{path}");
            if (response != null && response.IsSuccessStatusCode)
            {
                // Convert API response to string
                string responseString = await response.Content.ReadAsStringAsync();
                // Convert response string to object
                responseObject = JsonConvert.DeserializeObject<ResponseType>(responseString);
            }
            return responseObject;
        }

        // Create and shuffle new deck
        public async Task GenerateNewDeckID()
        {
            // Send request to API to create new deck
            DeckResponse response = await GetApiRequest<DeckResponse>("new/shuffle/?deck_count=1");
            // Save deck id
            DeckID = response?.deck_id;
        }

        // Get number of cards remaining in deck
        public async Task<int> GetNumberOfCardsInDeck()
        {
            DeckResponse response = await GetApiRequest<DeckResponse>($"{DeckID}");
            return response?.remaining ?? 0;
        }

        // Draw card from deck
        public async Task<Card> DrawCard()
        {
            // Send request to API to draw 1 card
            DrawCardResponse response = await GetApiRequest<DrawCardResponse>($"{DeckID}/draw/?count=1");
            // Return drawn card
            return response?.cards[0];
        }

        // Shuffle deck
        public async Task ShuffleDeck() => await GetApiRequest<DeckResponse>($"{DeckID}/shuffle/?remaining=true");

        // Add drawn card to hand
        public async Task AddCardToHand(string cardCode) =>
            await GetApiRequest<GenericResponse>($"{DeckID}/pile/userHand/add/?cards={cardCode}");

        public async Task<Card[]> GetCardsInHand()
        {
            // Send request to API to list 
            GetCardsInHandResponse response = await GetApiRequest<GetCardsInHandResponse>($"{DeckID}/pile/userHand/list/");
            // Return list of cards in hand
            return response?.piles.userHand.cards;
        }

        // Discard drawn card
        public async Task DiscardCard(string cardCode)
        {
            // Return card to deck
            await GetApiRequest<DeckResponse>($"{DeckID}/return/?cards={cardCode}");
            // Shuffle deck
            await ShuffleDeck();
        }

        // Discard card from hand
        public async Task DiscardCardFromHand(string cardCode)
        {
            // Return card to deck
            await GetApiRequest<DeckResponse>($"{DeckID}/pile/userHand/return/?cards={cardCode}");
            // Shuffle deck
            await ShuffleDeck();
        }
    }
}