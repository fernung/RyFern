using Bitmap = SharpDX.Direct2D1.Bitmap;
using SharpDX.Direct2D1;
using RyFern.Graphics.Two;
using RyFern.Graphics;

namespace RyFern.Clients.DX
{
    public class RyFernClient : DX2DClient
    {
        protected static readonly Random _random = new();
        protected static readonly Dictionary<Keys, bool> _keys = new Dictionary<Keys, bool>();

        protected SharpDX.RectangleF _screenRectF;

        protected PixelGL _pixelGL;
        private Bitmap _frontBuffer;

        protected override void Initialize(ClientConfiguration configuration)
        {
            base.Initialize(configuration);
            var keys = Enum.GetValues(typeof(Keys))
                           .Cast<Keys>()
                           .Select(x => x)
                           .ToArray();
            foreach(var key in keys)
            {
                if (_keys.ContainsKey(key))
                    continue;
                _keys.Add(key, false);
            }
            _screenRectF = new(0, 0, configuration.Width, configuration.Height);
            _pixelGL = new(configuration.Width, configuration.Height);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _frontBuffer = _pixelGL.AsBitmap(RenderTarget2D);
        }
        protected override void UnloadContent()
        {
            base.UnloadContent();
            _frontBuffer?.Dispose();
        }

        protected override void EndDraw()
        {
            _pixelGL.CopyTo(_frontBuffer);
            RenderTarget2D.Clear(SharpDX.Color.Black);
            RenderTarget2D.DrawBitmap(_frontBuffer, _screenRectF, 1f, BitmapInterpolationMode.Linear, _screenRectF);
            base.EndDraw();
        }

        protected override void KeyDown(KeyEventArgs e)
        {
            base.KeyDown(e);
            var k = e.KeyCode;
            if (!_keys.ContainsKey(k))
                _keys.Add(k, true);
            _keys[e.KeyCode] = true;
        }
        protected override void KeyUp(KeyEventArgs e)
        {
            base.KeyUp(e);
            var k = e.KeyCode;
            if (!_keys.ContainsKey(k))
                _keys.Add(k, false);
            _keys[e.KeyCode] = false;
        }
    }
}
