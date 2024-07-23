using System;

namespace IT3048C_Final.Models
{
    // Model for Card Data Fetched from Deck of Cards API
    public class Card
    {
        // Shorthand for both suit/value
        public string Code { get; set; }
        // Link for card sprite
        // Either use default Deck of Cards API sprite or
        // Convert to relative link to custom card asset
        public string Image { get; set; }
        // Pip value of card, or name of face card
        public string Value { get; set; }
        // Card's suit
        public string Suit { get; set; }
    }
}