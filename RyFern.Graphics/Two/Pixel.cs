using System.Runtime.InteropServices;

namespace RyFern.Graphics.Two
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Pixel
    {
        [FieldOffset(0)] public uint Packed;
        [FieldOffset(0)] public byte R;
        [FieldOffset(1)] public byte G;
        [FieldOffset(2)] public byte B;
        [FieldOffset(3)] public byte A;

        public Pixel() :
            this(0x00000000)
        { }
        public Pixel(uint packed)
        {
            Packed = packed;
        }
        public Pixel(float r, float g, float b, float a = 1f)
        {
            R = (byte)Math.Clamp(r * byte.MaxValue, 0, byte.MaxValue);
            G = (byte)Math.Clamp(g * byte.MaxValue, 0, byte.MaxValue);
            B = (byte)Math.Clamp(b * byte.MaxValue, 0, byte.MaxValue);
            A = (byte)Math.Clamp(a * byte.MaxValue, 0, byte.MaxValue);
        }
        public Pixel(int r, int g, int b, int a = byte.MaxValue)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }
        public Pixel(byte r, byte g, byte b, byte a = byte.MaxValue)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static implicit operator Pixel(uint v) => new(v);
        public static implicit operator uint(Pixel p) => p.Packed;
        public static Pixel operator *(Pixel p, float v)
        {
            var c = new Pixel(p.Packed);
            c.R = (byte)Math.Min(byte.MaxValue, Math.Max(0, (v * c.R)));
            c.G = (byte)Math.Min(byte.MaxValue, Math.Max(0, (v * c.G)));
            c.B = (byte)Math.Min(byte.MaxValue, Math.Max(0, (v * c.B)));
            return c;
        }
        public static Pixel operator *(float v, Pixel p) =>
            p * v;

        public static readonly Pixel Transparent = new(0);
        public static readonly Pixel White = new(0xFFFFFFFF);
        public static readonly Pixel Black = new(0xFF000000);
        public static readonly Pixel Red = new(0xFFFF0000);
        public static readonly Pixel Green = new(0xFF00FF00);
        public static readonly Pixel Blue = new(0xFF0000FF);
        public static readonly Pixel Cyan = new(0xFF00FFFF);
        public static readonly Pixel Yellow = new(0xFFFFFF00);
        public static readonly Pixel Magenta = new(0xFFFF00FF);
    }

    public static class PixelExtensions
    {
        public static Pixel NextPixel(this Random r)
        {
            var bytes = new byte[3];
            r.NextBytes(bytes);
            return new Pixel(bytes[0], bytes[1], bytes[2]);
        }
    }
}
