using System.Runtime.CompilerServices;

namespace RyFern.Graphics.Two
{
    public class PixelBuffer
    {
        private readonly int _width;
        private readonly int _height;
        private readonly int _stride;
        private readonly Pixel[] _pixels;

        public int Width => 
            _width;
        public int Height => 
            _height;
        public int Stride => 
            _stride;
        public int Size => 
            _pixels.Length;
        public Pixel[] Buffer =>
            _pixels;
        public Span<Pixel> Pixels =>
            _pixels.AsSpan();

        public Pixel this[int i]
        {
            get => InBounds(i) ? _pixels[i] : Pixel.Transparent;
            set { if(InBounds(i)) _pixels[i] = value; }
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

        public PixelBuffer(int width, int height, uint[] pixels) :
            this(width, height)
        {
            for (var i = 0; i < _pixels.Length; ++i)
                _pixels[i].Packed = pixels[i];
        }
        public PixelBuffer(int width, int height, Pixel[] pixels) :
            this(width, height)
        {
            pixels.AsSpan(0, _pixels.Length).CopyTo(_pixels);
        }
        public PixelBuffer(int width, int height)
        {
            _width = width;
            _height = height;
            _stride = width << 2;
            _pixels = new Pixel[width * height];
        }

        #region Helper Methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InBounds(int i) =>
            i >= 0 && i < _pixels.Length;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool InBounds(int x, int y) =>
            x >= 0 && x < _width &&
            y >= 0 && y < _height;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int Index(int x, int y) =>
            x + y * _width;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SampleIndex(float x, float y) =>
            (int)(x * _width) + ((int)(y * _height) * _width);
        #endregion
    }
}
