using System;
using Microsoft.AspNetCore.Http;

namespace MapR.Features.AddMarker.Models {
    public class AddMarker {
        public string Name { get; set; }
        public string Description { get; set; }
        public string CustomCss { get; set; }
        public IFormFile ImageData { get; set; }
    }
}
