using System;

namespace IT3048C_Final.Models
{
    // Model for Card Data Fetched from Deck of Cards API
    public class Card
    {
        // Shorthand for both suit/value
        public string code { get; set; }
        // Link for card sprite
        // Either use default Deck of Cards API sprite or
        // Convert to relative link to custom card asset
        public string image { get; set; }
        // Pip value of card, or name of face card
        public string value { get; set; }
        // Card's suit
        public string suit { get; set; }

        public override string ToString() =>
            "{\n" +
            $"\tcode: \"{code}\"\n" +
            $"\timage: \"{image}\"\n" +
            $"\tvalue: \"{value}\"\n" +
            $"\tsuit: \"{suit}\"\n" +
            "}";
    }
}