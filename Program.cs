using System;
using System.ComponentModel.DataAnnotations;
 using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MyApp
{
    class Workitem
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? AssignedTo { get; set; }
        public string? State { get; set; }

        public string? BugUrl { get; set; }
    }
    internal class Program
    {
        const string AccessToken = "AZDO_PAT_SC2020";
        static async Task Main(string[] args)
        {
            string azdoPat = Environment.GetEnvironmentVariable(AccessToken) ?? string.Empty;
            if (string.IsNullOrEmpty(azdoPat))
            {
                Console.WriteLine("{AccessToken} environment variable is not set.");
                return;
            }

            if (args.Length == 0)
            {
                Console.WriteLine("Please provide the workitem numbers as command-line arguments.");
                return;
            }

            try
            {
                var workitems = args.Select(arg => int.Parse(arg)).ToList();
                Console.WriteLine("Workitems:");

                var workitemList = new List<Workitem>();
                foreach (var num in workitems)
                {
                    var workitem = await GetWorkitemDetailsAsync(num);
                    if (workitem != null)
                    {
                        workitemList.Add(workitem);
                    }
                }

                foreach (var workitem in workitemList)
                {
                    Console.WriteLine($"ID: {workitem.Id}, Title: {workitem.Title}, Assigned To: {workitem.AssignedTo}, State: {workitem.State}, Bug URL: {workitem.BugUrl}");
                }
                var htmlContent = "<html><head><title>Workitems</title></head><body>";
                htmlContent += "<h1>Workitems</h1>";
                htmlContent += "<table border='1'><tr><th>ID</th><th>Title</th><th>State</th><th>Assigned To</th><th>Notes</th></tr>";

                foreach (var workitem in workitemList)
                {
                    htmlContent += $"<tr><td><a href='{workitem.BugUrl}'>{workitem.Id}</a></td><td>{workitem.Title}</td><td>{workitem.State}</td><td>{workitem.AssignedTo}</td><td></td></tr>";
                }

                htmlContent += "</table></body></html>";

                var filePath = "workitems.html";
                await File.WriteAllTextAsync(filePath, htmlContent);
                Console.WriteLine($"HTML file generated: {filePath}");

                Console.WriteLine("Press any key to exit.");
            }
            catch (FormatException)
            {
                Console.WriteLine("One or more arguments are not valid workitem Ids.");
            }
        }

        async static Task<Workitem> GetWorkitemDetailsAsync(int workitem)
        {
            var bugUrl = string.Format("https://dev.azure.com/supercomputing2020/Project%20D%20-%20MASS/_workitems/edit/{0}/", workitem);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", 
                Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{Environment.GetEnvironmentVariable(AccessToken)}")));

            var response = await client.GetAsync($"https://dev.azure.com/supercomputing2020/Project%20D%20-%20MASS/_apis/wit/workitems/{workitem}?api-version=6.0");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(content);
                var fields = jsonDoc.RootElement.GetProperty("fields");
                var title = fields.GetProperty("System.Title").GetString();
                var assignedTo = fields.GetProperty("System.AssignedTo").GetProperty("displayName").GetString();
                var state = fields.GetProperty("System.State").GetString();
                return new Workitem
                {
                    Id = workitem,
                    Title = title ?? "Untitled",
                    AssignedTo = assignedTo ?? "Unassigned",
                    State = state ?? "Unknown",
                    BugUrl = bugUrl?? "Unknown"
                };
            }
            else
            {
                Console.WriteLine($"Failed to fetch workitem {workitem}. Status Code: {response.StatusCode}");
                return null;
            }
        }
    }
}