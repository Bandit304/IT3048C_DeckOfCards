using IT3048C_Final.Models;
using System;

namespace IT3048C_Final.DeckAPI.Responses
{
    public struct GetCardsInHandResponse
    {
        public bool success;
        public string deck_id;
        public int remaining;
        public PileList piles;

        public struct Pile
        {
            public Card[] cards;
            public int remaining;
        }

        public struct PileList
        {
            public Pile userHand;
        }
    }
}