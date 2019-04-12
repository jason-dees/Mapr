using Microsoft.AspNetCore.Http;

namespace MapR.Features.AddMap.Models {
    public class AddMap {
        public string Name { get; set; }
        public IFormFile ImageData { get; set; }
    }
}