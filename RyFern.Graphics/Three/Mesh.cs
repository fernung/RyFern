using RyFern.Graphics.Two;
using System.Globalization;
using System.Numerics;

namespace RyFern.Graphics.Three
{
    public sealed class Mesh
    {
        private Triangle[] _triangles;
        private PixelBuffer _diffuse;

        public Span<Triangle> Triangles =>
            _triangles.AsSpan();
        public PixelBuffer Diffuse
        {
            get => _diffuse;
            set => _diffuse = value;
        }

        public Triangle this[int i] => 
            _triangles[i % _triangles.Length];
        public Vertice this[int i, int j] => 
            this[i][j];

        public Mesh(string path) :
            this(Loader.LoadModel(path), PixelGL.FromFile(path.Replace(".obj", "_diffuse.png")))
        { }
        public Mesh(Triangle[] triangles) :
            this(triangles, null)
        { }
        public Mesh(Triangle[] triangles, PixelBuffer diffuse)
        {
            _triangles = triangles;
            _diffuse = diffuse;
        }

        private static class Loader
        {
            private const string VerticeTag = "v";
            private const string NormalTag = "vn";
            private const string UVTag = "vt";
            private const string TrinagleTag = "f";

            private static readonly char[] Separators =
            {
            '/', ' '
        };


            public static Triangle[] LoadModel(string path)
            {
                var triangles = new List<Triangle>();

                var vertices = new List<Vector3>();
                var normals = new List<Vector3>();
                var uv = new List<Vector2>();

                using (var stream = File.OpenRead(path))
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var items = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);

                        switch (items.FirstOrDefault())
                        {
                            case VerticeTag:
                                vertices.Add(new Vector3(
                                    Convert.ToSingle(items[1], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToSingle(items[2], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToSingle(items[3], CultureInfo.InvariantCulture.NumberFormat)));
                                break;
                            case NormalTag:
                                normals.Add(new Vector3(
                                    Convert.ToSingle(items[1], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToSingle(items[2], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToSingle(items[3], CultureInfo.InvariantCulture.NumberFormat)));
                                break;
                            case UVTag:
                                uv.Add(new Vector2(
                                    Convert.ToSingle(items[1], CultureInfo.InvariantCulture.NumberFormat),
                                    Convert.ToSingle(items[2], CultureInfo.InvariantCulture.NumberFormat)));
                                break;
                            case TrinagleTag:
                                var triangle = new Triangle(
                                    new Vertice(
                                        vertices[Convert.ToInt32(items[1]) - 1],
                                        uv[Convert.ToInt32(items[2]) - 1],
                                        normals[Convert.ToInt32(items[3]) - 1]),
                                    new Vertice(
                                        vertices[Convert.ToInt32(items[4]) - 1],
                                        uv[Convert.ToInt32(items[5]) - 1],
                                        normals[Convert.ToInt32(items[6]) - 1]),
                                    new Vertice(
                                        vertices[Convert.ToInt32(items[7]) - 1],
                                        uv[Convert.ToInt32(items[8]) - 1],
                                        normals[Convert.ToInt32(items[9]) - 1])
                                );

                                triangles.Add(triangle);
                                break;
                        }
                    }
                }

                return triangles.ToArray();
            }
        }
    }
}
