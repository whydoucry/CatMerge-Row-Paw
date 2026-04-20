using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DrawingColor = System.Drawing.Color;
using DrawingRectangle = System.Drawing.Rectangle;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace CatMergeRowPaw.Views
{
    public class TextRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<string, Texture2D> _cache = new();

        public TextRenderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public Texture2D GetTextTexture(string text, XnaColor color)
        {
            var key = text + "|" + color.PackedValue;
            if (_cache.TryGetValue(key, out var existing))
            {
                return existing;
            }

            var texture = CreateTextTexture(text, color);
            _cache[key] = texture;
            return texture;
        }

        private Texture2D CreateTextTexture(string text, XnaColor color)
        {
            using var measureBitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            using var measureGraphics = Graphics.FromImage(measureBitmap);
            var font = new Font("Arial", 18, FontStyle.Regular, GraphicsUnit.Pixel);
            var sizeF = measureGraphics.MeasureString(text, font);
            var width = Math.Max(1, (int)Math.Ceiling(sizeF.Width));
            var height = Math.Max(1, (int)Math.Ceiling(sizeF.Height));

            using var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(DrawingColor.Transparent);
            graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            using var brush = new SolidBrush(DrawingColor.FromArgb(color.A, color.R, color.G, color.B));
            graphics.DrawString(text, font, brush, new PointF(0, 0));

            var data = new XnaColor[width * height];
            var bitmapData = bitmap.LockBits(new DrawingRectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            try
            {
                var bytes = new byte[bitmapData.Stride * height];
                Marshal.Copy(bitmapData.Scan0, bytes, 0, bytes.Length);
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        var offset = y * bitmapData.Stride + x * 4;
                        var b = bytes[offset];
                        var g = bytes[offset + 1];
                        var r = bytes[offset + 2];
                        var a = bytes[offset + 3];
                        data[y * width + x] = new XnaColor(r, g, b, a);
                    }
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            var texture = new Texture2D(_graphicsDevice, width, height);
            texture.SetData(data);
            return texture;
        }
    }
}
