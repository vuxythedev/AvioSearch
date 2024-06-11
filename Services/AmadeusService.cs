using Entities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RepositoryContracts;
using ServiceContracts;
using ServiceContracts.DTO;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Services
{
    public class AmadeusService : IProviderService
    {
        private TokenResponse? _accessToken;
        private readonly AmadeusConfigOptions _amadeusConfigOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFlightSearchRepository _flightSearchRepository;
        

        public AmadeusService(IHttpClientFactory httpClientFactory, IOptions<AmadeusConfigOptions> amadeusConfigOptions, IFlightSearchRepository flightSearchRepository)
        {
            _httpClientFactory = httpClientFactory;
            _amadeusConfigOptions = amadeusConfigOptions.Value;          
            _flightSearchRepository = flightSearchRepository;
        }

        private async Task<TokenResponse?> GetTokenAsync()
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var url = $"{_amadeusConfigOptions.AuthorizationURL}";

                var content = new StringContent(
                    $"grant_type=client_credentials&client_id={_amadeusConfigOptions.APIKey}&client_secret={_amadeusConfigOptions.APISecret}",
                    Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    TokenResponse? tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseString);

                    if(tokenResponse != null)
                        tokenResponse.LastUpdate = DateTime.Now;

                    return tokenResponse;
                }
                else
                {
                    // ISerilog nuget package!!!
                    throw new Exception($"Error getting token: {response.StatusCode}");
                }
            }
        }

        public async Task<List<FlightResponse>?> GetFlightOfferList(FlightRequest flightRequest) 
        {
            List<FlightResponse> flightOfferResponseList = new List<FlightResponse>();
            List<FlightOffer> flightOfferList = new List<FlightOffer>();

            string searchKey = flightRequest.OriginLocationCode + flightRequest.DestinationLocationCode + flightRequest.DepartureDate + flightRequest.Adults +
                        flightRequest.Children + flightRequest.Currency;

            var resultDB = await _flightSearchRepository.GetAsync(searchKey);

            if(resultDB != null && resultDB.FlightOffers.Count > 0)
            {
                foreach(FlightOffer flightOffer in resultDB.FlightOffers)
                {
                    flightOfferResponseList.Add(flightOffer.ToFlightResponse());
                }
                List<FlightResponse> flightOfferResponseOrderedList = flightOfferResponseList.OrderBy(item => item.Price).ToList();

                return flightOfferResponseOrderedList; 
            }

            if (_accessToken == null || (_accessToken != null && _accessToken.LastUpdate.AddSeconds(_accessToken.ExpiresIn) > DateTime.Now))
            {
                _accessToken = await GetTokenAsync();
            }

            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken?.AccessToken);

                var parameters = new Dictionary<string, string>
                {
                    { "originLocationCode", flightRequest.OriginLocationCode},
                    { "destinationLocationCode", flightRequest.DestinationLocationCode},
                    { "departureDate", flightRequest.DepartureDate },
                    { "adults", flightRequest.Adults},
                    { "children", flightRequest.Children},
                    { "currencyCode", flightRequest.Currency}
                };

                var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
                var requestUrl = $"{_amadeusConfigOptions.BaseURL}?{queryString}";

                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseString);

                    if (apiResponse != null && apiResponse.Data != null)
                    {
                        foreach (var filghtDataAPI in apiResponse.Data)
                        {
                            FlightOffer flightOffer = TransformFlightOfferData(filghtDataAPI);
                            flightOfferList.Add(flightOffer);

                            FlightResponse flightResponse = flightOffer.ToFlightResponse();

                            flightOfferResponseList.Add(flightResponse);                      
                            
                        }
                    }

                    FlightSearch flightSearch = new FlightSearch();

                    flightSearch.SearchKey = searchKey;
                    flightSearch.FlightOffers = flightOfferList;
                    flightSearch.LastUpdate = DateTime.UtcNow;


                   await _flightSearchRepository.AddAsync(flightSearch);

                   return flightOfferResponseList;
                }
                else
                {
                    throw new Exception($"Error getting flight offers: {response.StatusCode}");
                }
            }
        }

        private FlightOffer TransformFlightOfferData(FlightOfferAPI flightOfferAPI)
        {
            FlightOffer flightOfferData = new FlightOffer();
            var itineraries = flightOfferAPI?.Itineraries?.FirstOrDefault();
            var flightPrice = flightOfferAPI?.Price;

            flightOfferData.OriginCode = itineraries?.Segments?.FirstOrDefault()?.Departure?.IataCode;
            flightOfferData.DestinationCode = itineraries?.Segments?.LastOrDefault()?.Arrival?.IataCode;
            flightOfferData.DepartureDate = itineraries?.Segments?.FirstOrDefault()?.Departure?.At ?? DateTime.MinValue;
            flightOfferData.Itineraries = itineraries?.Segments?.Count ?? 0;
            flightOfferData.NumberOfBookableSeats = flightOfferAPI?.NumberOfBookableSeats;
            flightOfferData.CurrencyCode = flightPrice?.Currency;
            flightOfferData.Price = decimal.TryParse(flightPrice?.Total, out decimal price) ? price : 0m;

            return flightOfferData;
        }
           
    }

    #region BindingClasses
   
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }
        [JsonPropertyName("state")]
        public string? State { get; set; }
        [JsonPropertyName("expires_in")]
        public double ExpiresIn { get; set; }
        public DateTime LastUpdate { get; set; }

    }

    public class AmadeusConfigOptions
    {
        public string? APIKey { get; set; }
        public string? APISecret { get; set; }
        public string? AuthorizationURL { get; set; }
        public string? BaseURL { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("data")]
        public List<FlightOfferAPI>? Data { get; set; }
    }

    public class FlightOfferAPI
    {
        [JsonPropertyName("key")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("numberOfBookableSeats")]
        public int NumberOfBookableSeats {get; set;}

        [JsonPropertyName("itineraries")]
        public List<Itinerary>? Itineraries { get; set; }

        [JsonPropertyName("price")]
        public Price? Price { get; set; }

        [JsonPropertyName("travelerPricings")]
        public List<TravelerPricing>? TravelerPricings { get; set; }
    }

    public class Itinerary
    {
        [JsonPropertyName("key")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("segments")]
        public List<Segment>? Segments { get; set; }

        [JsonPropertyName("flightOfferId")]
        public int FlightOfferId { get; set; }
        //[ForeignKey("FlightOfferId")]
        [JsonPropertyName("flightOffer")]
        public FlightOfferAPI? FlightOffer { get; set; }
    }

    public class Segment
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("departure")]
        public Departure? Departure { get; set; }

        [JsonPropertyName("arrival")]
        public Arrival? Arrival { get; set; }
        [JsonPropertyName("itineraryId")]
        public int ItineraryId { get; set; }
        // [ForeignKey("ItineraryId")]
        [JsonPropertyName("itineraryId")]
        public Itinerary? Itinerary { get; set; }
    }

    public class Departure
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("iataCode")]
        public string? IataCode { get; set; }

        [JsonPropertyName("at")]
        public DateTime At { get; set; }
    }

    public class Arrival
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("iataCode")]
        public string? IataCode { get; set; }

        [JsonPropertyName("at")]
        public DateTime At { get; set; }
    }

    public class Price
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }

        [JsonProperty("currency")]
        public string? Currency { get; set; }

        [JsonProperty("total")]
        public string? Total { get; set; }
    }

    public class TravelerPricing
    {
        [JsonPropertyName("id")]
        [Key]
        public int Id { get; set; }

        [JsonPropertyName("travelerId")]
        public string? TravelerId { get; set; }

        [JsonPropertyName("price")]
        public Price? Price { get; set; }

        [JsonPropertyName("flightOfferId")]

        public int FlightOfferId { get; set; }
        //[ForeignKey("FlightOfferId")]
        [JsonPropertyName("flightOffer")]
        public FlightOfferAPI? FlightOffer { get; set; }
    }

    #endregion
}
