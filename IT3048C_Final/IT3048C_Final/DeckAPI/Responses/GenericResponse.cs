using System;

namespace IT3048C_Final.DeckAPI.Responses
{
    public struct GenericResponse
    {
        public bool success;
        public string deck_id;
        public int remaining;
    }
}