using GTYApp.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using GTYApp.Models;

namespace GTYApp.Services
{
    public static class CarouselService
    {
        public static List<CarouselItem> GetCarousel()
        {
            var list = new List<CarouselItem>();
            try
            {
                var data = PureApiClient.Send(HttpMethod.Get, "/carousel/");
                if (data.ValueKind != JsonValueKind.Array) return list;

                foreach (var it in data.EnumerateArray())
                {
                    list.Add(new CarouselItem
                    {
                        Id = it.GetProperty("id").GetInt32(),
                        ItemIndex = it.TryGetProperty("item_index", out var idx) ? idx.GetInt32() : 0,
                        ImageDataUrl = it.TryGetProperty("image_dataurl", out var img) ? (img.GetString() ?? "") : "",
                        Title = it.TryGetProperty("title", out var t) && t.ValueKind != JsonValueKind.Null ? t.GetString() : null,
                        Subtitle = it.TryGetProperty("subtitle", out var st) && st.ValueKind != JsonValueKind.Null ? st.GetString() : null,
                        Description = it.TryGetProperty("description", out var d) && d.ValueKind != JsonValueKind.Null ? d.GetString() : null,
                    });
                }
            }
            catch { }
            return list;
        }

        public static bool Create(CarouselItem item)
        {
            if (!Session.IsAdmin) return false;
            try
            {
                _ = PureApiClient.Send(HttpMethod.Post, "/carousel/", new
                {
                    item_index = item.ItemIndex,
                    image_dataurl = item.ImageDataUrl,
                    title = item.Title,
                    subtitle = item.Subtitle,
                    description = item.Description
                }, withJwt: true);
                return true;
            }
            catch { return false; }
        }

        public static bool Update(int id, CarouselItem item)
        {
            if (!Session.IsAdmin) return false;
            try
            {
                _ = PureApiClient.Send(HttpMethod.Put, $"/carousel/{id}", new
                {
                    item_index = item.ItemIndex,
                    image_dataurl = item.ImageDataUrl,
                    title = item.Title,
                    subtitle = item.Subtitle,
                    description = item.Description
                }, withJwt: true);
                return true;
            }
            catch { return false; }
        }

        public static bool Delete(int id)
        {
            if (!Session.IsAdmin) return false;
            try
            {
                _ = PureApiClient.Send(HttpMethod.Delete, $"/carousel/{id}", null, withJwt: true);
                return true;
            }
            catch { return false; }
        }
    }
}
