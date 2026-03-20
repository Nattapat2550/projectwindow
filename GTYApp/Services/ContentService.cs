using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GTYApp.Services
{
    public static class ContentService
    {
        public static Dictionary<string, string> GetHomeSections()
        {
            var map = new Dictionary<string, string>
            {
                {"hero", "Welcome"},
                {"features", "Pure API running"},
                {"cta", "Get Started"}
            };

            try
            {
                // ⭐ ดึงข้อมูลแบบไม่บล็อคคิวงาน UI
                var hero = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Get, "/homepage/hero")).GetAwaiter().GetResult();

                // ⭐ แกะเปลือกข้อมูล JSON
                if (hero.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Object) hero = d;

                map["hero"] = hero.TryGetProperty("title", out var t) && t.ValueKind == JsonValueKind.String ? (t.GetString() ?? map["hero"]) : map["hero"];
                map["features"] = hero.TryGetProperty("subtitle", out var st) && st.ValueKind == JsonValueKind.String ? (st.GetString() ?? map["features"]) : map["features"];
                map["cta"] = hero.TryGetProperty("cta_text", out var ct) && ct.ValueKind == JsonValueKind.String ? (ct.GetString() ?? map["cta"]) : map["cta"];
            }
            catch { }

            return map;
        }

        public static void UpsertSection(string section, string content)
        {
            try
            {
                var hero = GetHeroObjectSafe();

                switch ((section ?? "").Trim().ToLowerInvariant())
                {
                    case "hero":
                        hero.Title = content ?? "";
                        break;
                    case "features":
                        hero.Subtitle = content ?? "";
                        break;
                    case "cta":
                        hero.CtaText = content ?? "";
                        break;
                    default:
                        _ = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Put, $"/homepage/{section}", new { content = content ?? "" }, true)).GetAwaiter().GetResult();
                        return;
                }

                _ = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Put, "/homepage/hero", new
                {
                    title = hero.Title,
                    subtitle = hero.Subtitle,
                    cta_text = hero.CtaText,
                    cta_link = hero.CtaLink
                }, true)).GetAwaiter().GetResult();
            }
            catch { }
        }

        private static HeroObj GetHeroObjectSafe()
        {
            try
            {
                var hero = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Get, "/homepage/hero")).GetAwaiter().GetResult();

                if (hero.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Object) hero = d;

                return new HeroObj
                {
                    Title = hero.TryGetProperty("title", out var t) ? (t.GetString() ?? "") : "",
                    Subtitle = hero.TryGetProperty("subtitle", out var st) ? (st.GetString() ?? "") : "",
                    CtaText = hero.TryGetProperty("cta_text", out var ct) ? (ct.GetString() ?? "") : "",
                    CtaLink = hero.TryGetProperty("cta_link", out var cl) ? (cl.GetString() ?? "/") : "/"
                };
            }
            catch
            {
                return new HeroObj { Title = "", Subtitle = "", CtaText = "", CtaLink = "/" };
            }
        }

        private sealed class HeroObj
        {
            public string Title { get; set; } = "";
            public string Subtitle { get; set; } = "";
            public string CtaText { get; set; } = "";
            public string CtaLink { get; set; } = "/";
        }
    }
}