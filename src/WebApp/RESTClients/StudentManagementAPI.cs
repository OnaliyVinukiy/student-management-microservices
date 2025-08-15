using Polly;
using StudentSystem.WebApp.Models;
using System.Net.Http.Headers;

namespace StudentSystem.WebApp.RESTClients;

public class StudentManagementAPI : IStudentManagementAPI
{
    private readonly HttpClient _httpClient;

    public StudentManagementAPI(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Student>> GetStudents()
    {
        var response = await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1))
            .ExecuteAsync(() => _httpClient.GetAsync("api/students"));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<Student>>();
    }

    public async Task<Student> GetStudentById(string studentId)
    {
        var response = await Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(1))
            .ExecuteAsync(() => _httpClient.GetAsync($"api/students/{studentId}"));

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Student>();
    }

    public async Task RegisterStudent(Student student)
    {
        var response = await _httpClient.PostAsJsonAsync("api/students", student);
        response.EnsureSuccessStatusCode();
    }
}