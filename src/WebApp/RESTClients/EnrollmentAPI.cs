using Polly;
using StudentSystem.WebApp.Models;
using System.Net.Http.Headers;

namespace StudentSystem.WebApp.RESTClients;

public class EnrollmentAPI : IEnrollmentAPI
{
    private readonly HttpClient _httpClient;

    public EnrollmentAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AcademicTerm> GetAcademicTerm(string termDate)
    {
        var response = await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1))
            .ExecuteAsync(() => _httpClient.GetAsync($"api/terms/{termDate}"));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AcademicTerm>();
    }

    public async Task RegisterEnrollment(string termDate, Enrollment enrollment)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/terms/{termDate}/enrollments", enrollment);
        response.EnsureSuccessStatusCode();
    }

    public async Task CompleteEnrollment(string termDate, Guid enrollmentId, string grade)
    {
        // The body for this PUT request might just be the grade, depending on the API's command
        var completionData = new { Grade = grade }; 
        var response = await _httpClient.PutAsJsonAsync($"api/terms/{termDate}/enrollments/{enrollmentId}/complete", completionData);
        response.EnsureSuccessStatusCode();
    }
}