namespace NotificationService.API.DocumentHttpClient;

public class DocumentsServiceHttpClient
{
    public HttpClient HttpClient { get; set; }

    public DocumentsServiceHttpClient(HttpClient httpClient, IConfiguration configuration)
    {
        httpClient.BaseAddress = new Uri(configuration["DocumentsServiceUri"]!);

        HttpClient = httpClient;
    }

    public async Task<Stream> DownloadAppointmentResultFileAsync(Guid id)
    {
        var response = await HttpClient.GetAsync($"{id}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }
}
