using IT3048C_Final.Models;
using System;

namespace IT3048C_Final.DeckAPI.Responses
{
    public struct DrawCardResponse
    {
        public bool success;
        public string deck_id;
        public Card[] cards;
        public int remaining;
    }
}