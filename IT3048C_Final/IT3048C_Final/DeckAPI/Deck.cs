using System;

namespace IT3048C_Final.DeckAPI
{
    // Class to handle interactions with the Deck of Cards API
    public class Deck
    {
        // Link to API
        public string ApiUrl;
        // ID for deck in API
        public string DeckID;

        public Deck(string apiUrl)
        {
            // Save Link to API
            ApiUrl = apiUrl;
            // Generate new Deck, save Deck ID
        }
    }
}