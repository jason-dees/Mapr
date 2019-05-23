using System;
using ImageMagick;

namespace MapR.Extensions {
    public static class ByteArrayExtensions {

        internal static byte[] Resize(this byte[] imageBytes, int width, int height) {
            using (var image = new MagickImage(imageBytes)) {
                width = width == 0 ? image.Width : width;
                height = height == 0 ? image.Height : height;

                var ratio = (double)image.Width / image.Height;
                var scalePercentage = new Percentage(100);

                scalePercentage = width / height > ratio
                    ? new Percentage((double)height / image.Height * 100)
                    : new Percentage((double)width / image.Width * 100);

                image.Resize(scalePercentage);
                return image.ToByteArray();
            }
        }
    }
}
