using System;

namespace IT3048C_Final.DeckAPI.Responses
{
    public struct NewDeckResponse
    {
        public bool success;
        public string deck_id;
        public bool shuffled;
        public int remaining;
    }
}