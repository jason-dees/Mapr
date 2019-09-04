using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace MapR.Functions.Extensions {
    public static class ByteArrayExtensions {

        public static byte[] ResizeImageBytes(this byte[] imageBytes, int width, int height) {
            width = width == 0 ? height : width;
            height = height == 0 ? width : height;
            if (width == 0 && height == 0) {
                return imageBytes;
            }
            using (var outStream = new MemoryStream()) {
                using (var image = Image.Load(imageBytes, out IImageFormat format)) {
                    var resizeOptions = new ResizeOptions() {
                        Mode = ResizeMode.Max,
                        Size = new SixLabors.Primitives.Size(width, height)
                    };
                    image.Mutate(i => i.Resize(resizeOptions));
                    image.Save(outStream, format);
                    return outStream.ToArray();
                }
            }
        }
    }
}
