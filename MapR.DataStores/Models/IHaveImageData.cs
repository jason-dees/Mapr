using System;
namespace MapR.DataStores.Models {
    public interface IHaveImageData {
        byte[] ImageBytes { get; set; }
        string ImageUri { get; set; }
        string ImageBlobName { get; }
    }
}
