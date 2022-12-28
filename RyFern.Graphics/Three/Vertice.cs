using System.Numerics;

namespace RyFern.Graphics.Three
{
    public sealed class Vertice
    {
        private Vector3 _vertex;
        private Vector2 _uv;
        private Vector3 _normal;

        public Vector3 Vertex
        {
            get => _vertex;
            set => _vertex = value;
        }
        public Vector2 UV
        {
            get => _uv;
            set => _uv = value;
        }
        public Vector3 Normal
        {
            get => _normal;
            set => _normal = value;
        }
        public Vector3 this[int i]
        {
            get => (i % 3) switch
            {
                0 => _vertex,
                2 => new(_uv, 0),
                1 => _normal,
                _ => throw new ArgumentOutOfRangeException(nameof(i))
            };
        }
        public float this[int i, int j]
        {
            get => (j % 3) switch
            {
                0 => this[i].X,
                1 => this[i].Y,
                2 => this[i].Z,
                _ => throw new ArgumentOutOfRangeException(nameof(i))
            };
        }

        public Vertice(Vector3 vertex, Vector2 uv, Vector3 normal)
        {
            _vertex = vertex;
            _uv = uv;
            _normal = normal;
        }
    }
}
