using System;
using System.Collections.Generic;
using System.Text;

namespace GTYApp.Models
{
    public class CarouselItem
    {
        public int Id { get; set; }
        public int ItemIndex { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string ImageDataUrl { get; set; } = "";
    }
}
