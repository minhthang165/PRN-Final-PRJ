using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PRN_Final_Project.Service.Dto;
using PRN_Final_Project.Service.Interface;

namespace PRN_Final_Project.Service
{
    public class AIExtractorService : IAIExtractor
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public AIExtractorService(IConfiguration config)
        {
            _apiKey = config["Gemini:ApiKey"] ?? throw new Exception("Missing Gemini API key");
            _httpClient = new HttpClient();
        }

        public async Task<CVExtractedInfo> ExtractData(string fileUrl)
        {
            var instructionPrompt = """
        Dựa vào nội dung file CV được cung cấp, hãy trích xuất các thông tin sau dưới dạng JSON:
        - GPA (dạng số thập phân, ví dụ 3.5, **Nếu GPA nhận được là hệ 10 thì hãy chuyển sang hệ 4 bằng cách lấy GPA nhân 4 sau đó chia 10 (làm tròn xuống)**, nếu không đọc được GPA hãy để 0.0)
        - Education (tên trường đại học, chuyên ngành,..., **Hãy trả về một mảng string, không cần tạo object**)
        - Skill (các kỹ năng như lập trình, teamwork,...)
        
        Chỉ trả về đối tượng JSON, không giải thích gì thêm.
        """;


            // 1. Tải nội dung file từ URL về dưới dạng byte array
            var fileBytes = await _httpClient.GetByteArrayAsync(fileUrl);

            // 2. Chuyển byte array thành chuỗi Base64
            var fileBase64 = Convert.ToBase64String(fileBytes);

            // 3. Cập nhật lại body request để gửi dữ liệu inline thay vì fileUri
            var body = new
            {
                contents = new[]
                {
            new {
                parts = new object[]
                {
                    new { text = instructionPrompt },
                    new {
                        inlineData = new {
                            mimeType = "application/pdf",
                            data = fileBase64
                        }
                    }
                }
            }
        }
            };

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={_apiKey}"),
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseString);
            var textContent = jsonDoc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            var cleanJson = textContent.Trim().Replace("```json", "").Replace("```", "").Trim();
            Console.WriteLine(cleanJson);
            var extracted = JsonSerializer.Deserialize<CVExtractedInfo>(cleanJson);
            return extracted!;
        }
    }
}