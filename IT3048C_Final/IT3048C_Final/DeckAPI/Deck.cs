using System;
using System.Net.Http;
using System.Threading.Tasks;
using IT3048C_Final.DeckAPI.Responses;
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
            GenerateNewDeckID();
        }

        public async Task<ResponseType?> GetApiRequest<ResponseType>(string path) where ResponseType : struct
        {
            // Define output variable
            ResponseType? responseObject = null;
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

        public async Task GenerateNewDeckID()
        {
            NewDeckResponse? response = await GetApiRequest<NewDeckResponse>("new/shuffle/?deck_count=1");
            DeckID = response?.deck_id;
        }
    }
}