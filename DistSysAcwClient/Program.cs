using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;

#region Task 10 and beyond

class Program
{
    static async Task Main()
    {
        Console.WriteLine("Hello. What would you like to do?");

        using HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:44394/");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        while (true)
        {
            string input = Console.ReadLine().Trim();

            if (string.IsNullOrEmpty(input))
            {
                continue;
            }

            if (input == "Exit")
            {
                break;
            }

            if (input == "TalkBack Hello")
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("api/talkback/hello");
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response {responseBody}");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error {e.Message}");
                }
            }

            //if(input == "TalkBack Sort")
            //{
            //    int[] numbers;
            //    try
            //    {
            //        string number = input.Substring("TalkBack Sort".Length).Trim();
            //        number = number.Trim('[', ']');
            //        numbers = Array.ConvertAll(number.Split(','), int.Parse);
            //    }
            //    catch
            //    {
            //        Console.WriteLine("Invalid input. Please enter integers like: [6,1,8,4,3]");
            //        continue;
            //    }


            //}

            if (input.StartsWith("User Get", StringComparison.OrdinalIgnoreCase))
            {
                string username = input.Substring("User Get".Length).Trim();

                try
                {
                    HttpResponseMessage response = await client.GetAsync("api/user/new?username=" + username);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine(responseBody);
                    }
                    else
                    {
                        Console.WriteLine(response.StatusCode);
                    }
                }

                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
    }
}


#endregion