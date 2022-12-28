using RyFern.Clients;
using RyFern.Clients.DX;
using RyFern.Graphics;
using RyFern.Graphics.Two;
using System.Numerics;

namespace RyFern.Runner.Demos
{
    
    public class RandomShapesClient :
        RyFernClient
    {
        private ShapeMode _mode;
        private int _randomRadius;
        private Pixel _randomPixel;
        private Vector2 _randomStart, _randomEnd;
        private Vector2 _randomV0, _randomV1, _randomV2;

        protected override void Initialize(ClientConfiguration configuration)
        {
            base.Initialize(configuration);
            var width = Width;
            var height = Height;
            
            _randomPixel = Pixel.Transparent;

            _randomStart = Vector2.Zero;
            _randomEnd = Vector2.Zero;

            _randomV0 = Vector2.Zero;
            _randomV1 = Vector2.Zero;
            _randomV2 = Vector2.Zero;
            
            SetMode(ShapeMode.Blank);
        }
        protected override void Update(Time time)
        {
            base.Update(time);
            CheckInput();
            UpdateMode();
        }
        protected override void Draw(Time time)
        {
            base.Draw(time);
        }

        private void CheckInput()
        {
            if (_keys[Keys.D0]) SetMode(ShapeMode.Blank);
            else if (_keys[Keys.D1]) SetMode(ShapeMode.Fill);
            else if (_keys[Keys.D2]) SetMode(ShapeMode.Pixel);
            else if (_keys[Keys.D3]) SetMode(ShapeMode.Line);
            else if (_keys[Keys.D4]) SetMode(ShapeMode.Triangle);
            else if (_keys[Keys.D5]) SetMode(ShapeMode.Rectangle);
            else if (_keys[Keys.D6]) SetMode(ShapeMode.Circle);
            else if (_keys[Keys.D7]) SetMode(ShapeMode.TriangleFill);
            else if (_keys[Keys.D8]) SetMode(ShapeMode.RectangleFill);
            else if (_keys[Keys.D9]) SetMode(ShapeMode.CircleFill);
        }
        private void UpdateMode()
        {
            switch(_mode)
            {
                default:
                case ShapeMode.Blank: return;
                case ShapeMode.Fill: UpdateRandomFill(); break;
                case ShapeMode.Pixel: UpdateRandomPixel(); break;
                case ShapeMode.Line: UpdateRandomLines(); break;
                case ShapeMode.Triangle: UpdateRandomTriangles(); break;
                case ShapeMode.Rectangle: UpdateRandomRectangles(); break;
                case ShapeMode.Circle: UpdateRandomCircles(); break;
                case ShapeMode.TriangleFill: UpdateRandomTriangleFills(); break;
                case ShapeMode.RectangleFill: UpdateRandomRectangleFills(); break;
                case ShapeMode.CircleFill: UpdateRandomCircleFills(); break;
            }
        }
        private void SetMode(ShapeMode mode)
        {
            _mode = mode;
            _pixelGL.Clear();
            Title = $"[{((int)_mode)}]: {_mode} Mode";
        }

        private void UpdateRandomFill()
        {
            GetRandomPixel();
            _pixelGL.Pixels.Fill(_randomPixel);
        }
        private void UpdateRandomPixel()
        {
            for (var i = 0; i < _pixelGL.Count; ++i)
                GetRandomPixel(i);
        }
        private void UpdateRandomLines()
        {
            GetRandomPixel();
            GetRandomLine();
            _pixelGL.DrawLine(_randomStart, _randomEnd, _randomPixel);
        }
        private void UpdateRandomTriangles()
        {
            GetRandomPixel();
            GetRandomTriangle();
            _pixelGL.DrawTriangle(_randomV0, _randomV1, _randomV2, _randomPixel);
        }
        private void UpdateRandomRectangles()
        {
            GetRandomPixel();
            GetRandomLine();
            _pixelGL.DrawRectangle(_randomStart, _randomEnd, _randomPixel);
        }
        private void UpdateRandomCircles()
        {
            GetRandomCircle();
            _pixelGL.DrawCircle(_randomStart, _randomRadius, _randomPixel);
        }
        private void UpdateRandomTriangleFills()
        {
            GetRandomPixel();
            GetRandomTriangle();
            _pixelGL.FillTriangle(_randomV0, _randomV1, _randomV2, _randomPixel);
        }
        private void UpdateRandomRectangleFills()
        {
            GetRandomPixel();
            GetRandomLine();
            _pixelGL.FillRectangle(_randomStart, _randomEnd, _randomPixel);
        }
        private void UpdateRandomCircleFills()
        {
            GetRandomCircle();
            _pixelGL.FillCircle(_randomStart, _randomRadius, _randomPixel);
        }

        private void GetRandomPixel() =>
            _randomPixel = _random.NextPixel();
        private void GetRandomPixel(int i)
        {
            GetRandomPixel();
            var span = _pixelGL.Pixels;
            span[i].B = _randomPixel.B;
            span[i].G = _randomPixel.G;
            span[i].R = _randomPixel.R;
        }
        private void GetRandomLine()
        {
            var width = Width;
            var height = Height;
            _randomStart = _random.NextVector2(0, width, 0, height);
            _randomEnd = _random.NextVector2(0, width, 0, height);
        }
        private void GetRandomTriangle()
        {
            var width = Width;
            var height = Height;
            _randomV0 = _random.NextVector2(0, width, 0, height);
            _randomV1 = _random.NextVector2(0, width, 0, height);
            _randomV2 = _random.NextVector2(0, width, 0, height);
        }
        private void GetRandomCircle()
        {
            GetRandomPixel();
            GetRandomLine();
            _randomRadius = _random.Next(0, Width / 4);
        }

        protected enum ShapeMode
        {
            Blank,
            Fill,
            Pixel,
            Line,
            Triangle,
            Rectangle,
            Circle,
            TriangleFill,
            RectangleFill,
            CircleFill
        }
    }
}
