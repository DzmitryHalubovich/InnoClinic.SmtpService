namespace SmtpAPI.DocumentHttpClient;

public class DocumentsAPIHttpClient
{
    public HttpClient HttpClient { get; set; }

    public DocumentsAPIHttpClient(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri("https://localhost:7208/api/documents/");

        HttpClient = httpClient;
    }

    public async Task<Stream> DownloadAppointmentResultFileAsync(Guid id)
    {
        var response = await HttpClient.GetAsync($"{id}");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStreamAsync();
    }
}
