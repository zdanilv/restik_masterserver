using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.IO;

namespace Client
{
    internal class Program
    {
        private static HttpClient client = new HttpClient();
        private static string Url = "http://localhost:1337/sign/user=test;password=B75808BAF85D4ECBB3D5717A2CB7FF1710767B09867E252E9E9FFA2C4A0CF689";
        static void Main(string[] args)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Hello, World!");
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri(Url);
            request.Method = HttpMethod.Post;

            var response = client.SendAsync(request);
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                Console.WriteLine(response.Result);
                Console.WriteLine(response.Result.RequestMessage);
                Console.WriteLine(response.Result.Content.Headers.ContentEncoding);
                HttpContent responseContent = response.Result.Content;
                var result = responseContent.ReadAsStringAsync();
                Console.WriteLine(result.Result.ToString());
            }
            else Console.WriteLine("Invalid");
            Console.ReadLine();
        }
    }
}