using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#region Task 10 and beyond

class Program
{
    static async Task Main()
    {
        string storedUsername = null;
        string storedApiKey = null;

        Console.WriteLine("Hello. What would you like to do?");

        using HttpClient client = new HttpClient();
        client.BaseAddress = new Uri("https://localhost:44394/");
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        while (true)
        {
            string input = Console.ReadLine().Trim();

            //if (string.IsNullOrEmpty(input))
            //{
            //    continue;
            //}

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
                    Console.WriteLine(responseBody);
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

            if (input.StartsWith("TalkBack Sort", StringComparison.OrdinalIgnoreCase))
            {
                int[] numbers;
                try
                {
                    string numbersPart = input.Substring("TalkBack Sort".Length).Trim();
                    numbersPart = numbersPart.Trim('[', ']');
                    numbers = Array.ConvertAll(numbersPart.Split(',', StringSplitOptions.RemoveEmptyEntries), int.Parse);
                }
                catch
                {
                    continue;
                }

                string query = string.Join("&", numbers.Select(n => $"_integers={n}"));

                try
                {
                    HttpResponseMessage response = await client.GetAsync("api/talkback/sort?" + query);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error" + e.Message);
                }
            }

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


            if (input.StartsWith("User Post", StringComparison.OrdinalIgnoreCase))
            {
                string username = input.Substring("User Post".Length).Trim();
                if (string.IsNullOrEmpty(username))
                {
                    continue;
                }
                try
                {
                    string jsonBody = $"\"{username}\"";
                    var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("api/user/new", content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        storedUsername = username;
                        storedApiKey = responseBody.Trim('"');
                        Console.WriteLine("Got API Key");
                    }
                    else
                    {
                        Console.WriteLine(responseBody);
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("Error" + e.Message);
                }
            }

            if (input.StartsWith("User Set", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = input.Split(' ', 3);

                if (parts.Length < 3)
                {
                    Console.WriteLine("Invalid input. Use: User Set <username> <apikey>");
                    continue;
                }

                storedUsername = parts[1].Trim();
                storedApiKey = parts[2].Trim();

                Console.WriteLine("Stored"); 
            }

            if (input.StartsWith("User Delete", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrEmpty(storedUsername) || string.IsNullOrEmpty(storedApiKey))
                {
                    Console.WriteLine("You need to do a User Post or User Set first");
                    continue;
                }


                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Delete, $"api/user/removeuser?username={storedUsername}");
                    request.Headers.Add("ApiKey", storedApiKey);

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("True"); 
                                                   
                        storedUsername = null;
                        storedApiKey = null;
                    }
                    else
                    {
                        Console.WriteLine("False");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }

            }
        }
    }
}


#endregion