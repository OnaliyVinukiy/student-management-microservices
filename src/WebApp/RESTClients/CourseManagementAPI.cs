using Polly;
using StudentSystem.WebApp.Models;
using System.Net.Http.Headers;

namespace StudentSystem.WebApp.RESTClients;

public class CourseManagementAPI : ICourseManagementAPI
{
    private readonly HttpClient _httpClient;

    public CourseManagementAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Course>> GetCourses()
    {
        var response = await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1))
            .ExecuteAsync(() => _httpClient.GetAsync("api/courses"));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Course>>();
    }

    public async Task<Course> GetCourseById(string courseCode)
    {
        var response = await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1))
            .ExecuteAsync(() => _httpClient.GetAsync($"api/courses/{courseCode}"));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Course>();
    }

    public async Task RegisterCourse(Course course)
    {
        var response = await _httpClient.PostAsJsonAsync("api/courses", course);
        response.EnsureSuccessStatusCode();
    }
}