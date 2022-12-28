using RyFern.Graphics.Two;
using SharpDX.Direct2D1;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Buffer = System.Buffer;

namespace RyFern.Graphics
{
    public class PixelGL
    {
        private int _width, _height;
        private Pixel[] _buffer;

        public int Width => 
            _width;
        public int Height => 
            _height;
        public int Count =>
            _buffer.Length;
        public Span<Pixel> Pixels => 
            _buffer.AsSpan();

        public Pixel this[int i]
        {
            get => InBounds(i) ? _buffer[i] : Pixel.Transparent;
            set { if (InBounds(i)) _buffer[i] = value; }
        }
        public Pixel this[int x, int y]
        {
            get => this[Index(x, y)];
            set => this[Index(x, y)] = value;
        }
        public Pixel this[float x, float y]
        {
            get => this[SampleIndex(x, y)];
            set => this[SampleIndex(x, y)] = value;
        }

        public PixelGL(int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = new Pixel[_width * _height];
        }

        public void Load(string path)
        {
            using var bitmap = System.Drawing.Image.FromFile(path) as System.Drawing.Bitmap;
            var width = bitmap.Width;
            var height = bitmap.Height;
            var pixels = new uint[width * height];
            var pixelBytes = new byte[(width * height) << 2];
            var data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Marshal.Copy(data.Scan0, pixelBytes, 0, pixelBytes.Length);
            Buffer.BlockCopy(pixelBytes, 0, pixels, 0, pixels.Length * sizeof(uint));
            bitmap.UnlockBits(data);
            ResizeBuffer(width, height);
            var span = _buffer.AsSpan();
            for (var i = 0; i < span.Length; ++i)
                span[i].Packed = pixels[i];
        }

        public void CopyTo(Span<Pixel> buffer)
        {
            var length = buffer.Length < _buffer.Length ? 
                         buffer.Length :
                         _buffer.Length;
            _buffer.AsSpan(0, length)
                   .CopyTo(buffer.Slice(0, length));
        }
        public void CopyTo(Bitmap bitmap) =>
            bitmap.CopyFromMemory(_buffer, _width << 2);
        public void CopyFrom(Span<Pixel> buffer)
        {
            var length = buffer.Length < _buffer.Length ?
                         buffer.Length :
                         _buffer.Length;
            buffer.Slice(0, length)
                  .CopyTo(_buffer.AsSpan(0, length));
        }

        public Bitmap AsBitmap(RenderTarget t)
        {
            var b = new Bitmap(t, new SharpDX.Size2(_width, _height), new BitmapProperties(t.PixelFormat));
            b.CopyFromMemory(_buffer, _width << 2);
            return b;
        }
        public PixelBuffer AsPixelBuffer() =>
            new(_width, _height, _buffer);

        public void Clear() =>
            Clear(Pixel.Transparent);
        public void Clear(Pixel color) =>
            _buffer.AsSpan().Fill(color);

        public void DrawLine(int x0, int y0, int x1, int y1, Pixel color)
        {
            var steep = Math.Abs(x0 - x1) < 
                        Math.Abs(y0 - y1);
            if (steep)
            {
                (x0, y0) = (y0, x0);
                (x1, y1) = (y1, x1);
            }
            if (x0 > x1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }

            int dx = x1 - x0, dy = y1 - y0;
            int x = x0, xInc = dx << 1;
            int y = y0, yInc = y1 > y0 ? 1 : -1;
            int e = 0, eInc = Math.Abs(dy) << 1;
            for (; x <= x1; x++)
            {
                if (steep) this[y, x] = color;
                else this[x, y] = color;

                e += eInc;
                if (e > dx)
                {
                    y += yInc;
                    e -= xInc;
                }
            }
        }
        public void DrawLine(Vector2 start, Vector2 end, Pixel color) =>
            DrawLine((int)start.X, (int)start.Y, (int)end.X, (int)end.Y, color);

        public void DrawTriangle(int x0, int y0, int x1, int y1, int x2, int y2, Pixel color)
        {
            if (y0 == y1 && y0 == y2) 
                return;
            if (y0 > y1)
            {
                (x0, x1) = (x1, x0);
                (y0, y1) = (y1, y0);
            }
            if (y0 > y2)
            {
                (x0, x2) = (x2, x0);
                (y0, y2) = (y2, y0);
            }
            if (y1 > y2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }

            DrawLine(x0, y0, x1, y1, color);
            DrawLine(x1, y1, x2, y2, color);
            DrawLine(x2, y2, x0, y0, color);
        }
        public void DrawTriangle(Vector2 v0, Vector2 v1, Vector2 v2, Pixel color) =>
            DrawTriangle((int)v0.X, (int)v0.Y, (int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y, color);

        public void DrawRectangle(int x, int y, int width, int height, Pixel color)
        {
            DrawLine(x,         y,          x + width,  y,          color);
            DrawLine(x + width, y,          x + width,  y + height, color);
            DrawLine(x + width, y + height, x,          y + height, color);
            DrawLine(x,         y + height, x,          y,          color);
        }
        public void DrawRectangle(Vector2 start, Vector2 end, Pixel color)
        {
            var size = end - start;
            DrawRectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y, color);
        }

        public void DrawCircle(int x, int y, int radius, Pixel color)
        {
            int d = 3 - 2 * radius;
            int i = 0;
            int j = radius;

            while (i <= j)
            {
                this[x + i, y + j] = color;
                this[x + j, y + i] = color;
                this[x + j, y - i] = color;
                this[x + i, y - j] = color;
                this[x - i, y - j] = color;
                this[x - j, y - i] = color;
                this[x - j, y + i] = color;
                this[x - i, y + j] = color;

                if (d < 0)
                {
                    d += 4 * i + 6;
                }
                else
                {
                    d += 4 * (i - j) + 10;
                    j--;
                }

                i++;
            }
        }
        public void DrawCircle(Vector2 center, int radius, Pixel color) =>
            DrawCircle((int)center.X, (int)center.Y, radius, color);

        public void FillTriangle(int x0, int y0, int x1, int y1, int x2, int y2, Pixel color)
        {
            var minX = Math.Min(x0, Math.Min(x1, x2));
            var minY = Math.Min(y0, Math.Min(y1, y2));
            var maxX = Math.Max(x0, Math.Max(x1, x2));
            var maxY = Math.Max(y0, Math.Max(y1, y2));

            Vector2 point = Vector2.Zero;
            Vector3 baryCoord;
            for (var y = minY; y <= maxY; y++)
                for (var x = minX; x <= maxX; x++)
                {
                    point.X = x; point.Y = y;
                    baryCoord = point.Barycentric(new(x0, y0), new(x1, y1), new(x2, y2));
                    if (baryCoord.X < 0 ||
                        baryCoord.Y < 0 ||
                        baryCoord.Z < 0)
                        continue;
                    this[x, y] = color;
                }
        }
        public void FillTriangle(Vector2 v0, Vector2 v1, Vector2 v2, Pixel color) =>
            FillTriangle((int)v0.X, (int)v0.Y, (int)v1.X, (int)v1.Y, (int)v2.X, (int)v2.Y, color);

        public void FillRectangle(int x, int y, int width, int height, Pixel color)
        {
            var xw = x + width;
            var yh = y + height;
            for (var ys = y; ys <= yh; ys++)
                for (var xs = y; xs <= xw; xs++)
                    this[xs, ys] = color;
        }
        public void FillRectangle(Vector2 start, Vector2 end, Pixel color)
        {
            var size = end - start;
            FillRectangle((int)start.X, (int)start.Y, (int)size.X, (int)size.Y, color);
        }

        public void FillCircle(int x, int y, int radius, Pixel color)
        {
            for (var ys = -radius; ys <= radius; ys++)
                for (var xs = -radius; xs <= radius; xs++)
                    if (xs * xs + ys * ys <= radius * radius)
                        this[x + xs, y + ys] = color;
        }
        public void FillCircle(Vector2 center, int radius, Pixel color) =>
            FillCircle((int)center.X, (int)center.Y, radius, color);

        public void FlipV()
        {
            Pixel temp;
            int x = 0, y = 0, idx1, idx2;
            for (; y < _height / 2; y++)
                for (; x < _width; x++)
                {
                    idx1 = x + y * _width;
                    idx2 = x + (_height - y - 1) * _width;
                    temp = this[idx1];
                    this[idx1] = this[idx2];
                    this[idx2] = temp;
                }
        }
        public void FlipH()
        {
            FlipBoth();
            FlipV();
        }
        public void FlipBoth() =>
            _buffer.AsSpan().Reverse();

        #region Helper Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResizeBuffer(int width, int height)
        {
            _width = width;
            _height = height;
            _buffer = new Pixel[_width * _height];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InBounds(int i) =>
            i >= 0 && i < _buffer.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Index(int x, int y) =>
            x + y * _width;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SampleIndex(float x, float y) =>
            (int)(x * _width) + ((int)(y * _height) * _width);
        #endregion
    }
}
