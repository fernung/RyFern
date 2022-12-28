using System.Numerics;

namespace RyFern.Graphics.Three
{
    public sealed class Triangle
    {
        private Vertice _v0;
        private Vertice _v1;
        private Vertice _v2;

        public Vertice V0
        {
            get => _v0;
            set => _v0 = value;
        }
        public Vertice V1
        {
            get => _v1;
            set => _v1 = value;
        }
        public Vertice V2
        {
            get => _v2;
            set => _v2 = value;
        }
        public Vertice this[int i] =>
            (i % 3) switch
            {
                0 => _v0,
                1 => _v1,
                2 => _v2,
                _ => throw new ArgumentOutOfRangeException(nameof(i))
            };
        public Vector3 this[int i, int j] =>
            this[i][j];

        public Triangle(Vertice v0, Vertice v1, Vertice v2)
        {
            _v0 = v0;
            _v1 = v1;
            _v2 = v2;
        }
    }
}
