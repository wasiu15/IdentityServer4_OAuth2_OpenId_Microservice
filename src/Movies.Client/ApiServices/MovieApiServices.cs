using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movies.Client.Models;
using Newtonsoft.Json;

namespace Movies.Client.ApiServices
{
    public class MovieApiServices : IMovieApiServices
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor; // used to access the stored token

        public MovieApiServices(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserInfoViewModel> GetUserInfo()
        {
            var idpClient = _httpClientFactory.CreateClient("IDPClient"); // creating this httpclient which we registered in the Program class

            var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();
            if(metaDataResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while requesting the access token");
            }

            var accessToken = await _httpContextAccessor
                .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var userInfoResponse = await idpClient.GetUserInfoAsync(
                new UserInfoRequest
                {
                    Address = metaDataResponse.UserInfoEndpoint,
                    Token = accessToken
                });
            if(userInfoResponse.IsError)
            {
                throw new HttpRequestException("Something went wrong while getting user info");
            }

            var userInfoDictionary = new Dictionary<string, string>();

            foreach (var claim in userInfoResponse.Claims)
            {
                userInfoDictionary.Add(claim.Type, claim.Value);
            }
            return new UserInfoViewModel(userInfoDictionary);
        }

        public Task<Movie> CreateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMovie(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Movie> GetMovie(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/movies/"+id);

            var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync();
            var movie = JsonConvert.DeserializeObject<Movie>(content);
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetMovies()
        {
            //  WAY 1

            var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

            var request = new HttpRequestMessage(
                HttpMethod.Get,
                "/movies");

            var response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

            var content = await response.Content.ReadAsStringAsync();
            var movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            return movieList;



            //  WAY 2

            //  1 - Get Token from Idendtity Server
            //  1 - Send request to Protected API
            //  3 - Deserialize Object to MovieList


            //  1. "retrieve" our api credentials. This must be registered on Identity Server
            //var apiClientCredentials = new ClientCredentialsTokenRequest
            //{
            //    Address = "https://localhost:5005/connect/token",

            //    ClientId = "movieClient",
            //    ClientSecret = "secret",

            //    // This is the scope our Protected API requires
            //    Scope = "movieAPI"
            //};

            //// creates a new HttpClient to talk to our IdentityServer (localhost:5005)
            //var client = new HttpClient();

            //// just checks if we can reach the Discovery document. Not 100% needed but..
            //var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            //if(disco.IsError)
            //{
            //    return null; // throw 500 error
            //}

            ////  2. Authenticates and get an access token from Identity Server
            //var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
            //if(tokenResponse.IsError)
            //{
            //    return null;
            //}



            ////  2 - Send Request to Protected API

            ////  Another HttpClient for talking with our Protected API
            //var apiClient = new HttpClient();

            ////  3 - Set the access_token in the request Authorization: Bearer <token>
            //apiClient.SetBearerToken(tokenResponse.AccessToken);

            ////  4 - Send a request to our Protected API
            //var response = await apiClient.GetAsync("https://localhost:5001/api/movies");
            //response.EnsureSuccessStatusCode(); // for exception handling

            //var content = await response.Content.ReadAsStringAsync();

            ////  Deserialize Object to MovieList
            //List<Movie> movieList = JsonConvert.DeserializeObject<List<Movie>>(content);
            //return movieList;





            //var movieList = new List<Movie>();
            //movieList.Add(
            //    new Movie
            //    {
            //        Id = 1,
            //        Genre = "Comics",
            //        Title = "asd",
            //        Rating = "9.2",
            //        ImageUrl = "image/src",
            //        ReleaseDate = DateTime.Now,
            //        Owner = "swn"
            //    });

            //return await Task.FromResult(movieList);
        }


        public Task<Movie> UpdateMovie(Movie movie)
        {
            throw new NotImplementedException();
        }
    }
}
