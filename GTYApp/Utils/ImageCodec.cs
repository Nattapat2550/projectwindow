using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

namespace GTYApp.Utils
{
    public static class ImageCodec
    {
        public static string ToDataUrl(Image img)
        {
            using var ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);
            var b64 = Convert.ToBase64String(ms.ToArray());
            return "data:image/png;base64," + b64;
        }

        public static Image FromDataUrl(string dataUrl)
        {
            var idx = dataUrl.IndexOf("base64,");
            var b64 = dataUrl[(idx + 7)..];
            var bytes = Convert.FromBase64String(b64);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }
    }
}
