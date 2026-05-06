using Microsoft.AspNetCore.Http;
using System.Text.Json;
namespace OrderPoint.Helper
{
    public class CookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetList<T>(string key, List<T> list, int expireMinutes = 60)
        {
            var json = JsonSerializer.Serialize(list);
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(expireMinutes),
                Path = "/"
            };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(key, json, options);
        }

        public List<T>? GetList<T>(string key)
        {
            var json = _httpContextAccessor.HttpContext?.Request.Cookies[key];
            return json is null ? null : JsonSerializer.Deserialize<List<T>>(json);
        }

        public void RemoveCookie(string key)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(key);
        }
    }
}
