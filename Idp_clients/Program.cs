using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Idp_clients
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //discovery endpoints
            var client = new HttpClient();
            //Task.Run(async () =>
            //{
            //    var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001/");
            //}).GetAwaiter().GetResult();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5001/");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.ReadKey();
                return;
            }
            //request asses token
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(
                    new ClientCredentialsTokenRequest
                    {
                        Address= disco.TokenEndpoint,
                        ClientId= "m2m.client",
                        ClientSecret= "511536EF-F270-4058-80CA-1C89C192F69A",
                        Scope= "scope1"
                    }
                    
                );
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.ReadKey();
                return;
            }
            Console.WriteLine("this is accessToken:");
            Console.WriteLine(tokenResponse.AccessToken);

            //call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);
            var apiResponse =await apiClient.GetAsync("https://localhost:5003/identity");
            if (!apiResponse.IsSuccessStatusCode)
                Console.WriteLine(apiResponse.StatusCode);
            else
            {
                var content = apiResponse.Content.ReadAsStringAsync();
                Console.WriteLine("this is api result:");
                Console.WriteLine(content.Result);
            }


            Console.ReadKey();
        }
    }
}
