using System.Numerics;
using System.Runtime.CompilerServices;

namespace RyFern
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float NextFloat(this Random r, float min, float max) =>
            (float)r.NextDouble() * (max - min) + min;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NextVector2(this Random r, float minX, float maxX, float minY, float maxY) =>
            new(r.NextFloat(minX, maxX), r.NextFloat(minY, maxY));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 NextVector2(this Random r, float min, float max) =>
            r.NextVector2(min, max, min, max);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector3 v) =>
            new(v.X, v.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ToVector2(this Vector4 v) =>
            new(v.X, v.Y);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector2 v) =>
            new(v.X, v.Y, 1f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 ToVector3(this Vector4 v) =>
            new(v.X, v.Y, v.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector2 v) =>
            new(v.X, v.Y, 1f, 1f);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 ToVector4(this Vector3 v) =>
            new(v.X, v.Y, v.Z, 1f);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Min(this Vector2 v0, Vector2 v1, Vector2 v2) =>
            new
            (
                MathF.Min(v0.X, MathF.Min(v1.X, v2.X)),
                MathF.Min(v0.Y, MathF.Min(v1.Y, v2.Y))
            );
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Max(this Vector2 v0, Vector2 v1, Vector2 v2) =>
            new
            (
                MathF.Max(v0.X, MathF.Max(v1.X, v2.X)),
                MathF.Max(v0.Y, MathF.Max(v1.Y, v2.Y))
            );


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Barycentric(this Vector2 p, Vector2 v0, Vector2 v1, Vector2 v2)
        {
            float w0 = ((v1.Y - v2.Y) * (p.X - v2.X) + (v2.X - v1.X) * (p.Y - v2.Y)) /
                       ((v1.Y - v2.Y) * (v0.X - v2.X) + (v2.X - v1.X) * (v0.Y - v2.Y));
            float w1 = ((v2.Y - v0.Y) * (p.X - v2.X) + (v0.X - v2.X) * (p.Y - v2.Y)) /
                       ((v1.Y - v2.Y) * (v0.X - v2.X) + (v2.X - v1.X) * (v0.Y - v2.Y));
            float w2 = 1 - w0 - w1;
            return new(w0, w1, w2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Interopolate(this Vector3 v, Vector2 a, Vector2 b, Vector2 c) =>
            v.X * a + v.Y * b + v.Z * c;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InteropolateZ(this Vector3 v, Vector3 a, Vector3 b, Vector3 c) =>
            v.X * a.Z + v.Y * b.Z + v.Z * c.Z;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 SurfaceNormal(this Vector3 a, Vector3 b, Vector3 c) =>
            Vector3.Normalize(Vector3.Cross(c - a, b - a));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 AsHomogeneous(this Vector4 v)
        {
            if (v.W != 0f)
                v /= v.W;
            return new(v.X, v.Y, v.Z);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 ProjectToScreen(this Vector3 v, float width, float height) =>
            new
            (
                (int)((v.X + 1f) * (width * 0.5f)),
                (int)(height - (v.Y + 1f) * (height * 0.5f))
            );


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LightDirection(this Matrix4x4 view, Matrix4x4 world)
        {
            Matrix4x4.Invert(world, out var worldInverse);
            Matrix4x4.Invert(view, out var viewInverse);
            return Vector3.Normalize(Vector3.Transform(view.Translation - world.Translation, worldInverse * viewInverse));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 LightIntensity(this Vector3 light, Vector3 normal0, Vector3 normal1, Vector3 normal2) =>
            new
            (
                MathF.Max(0f, Vector3.Dot(Vector3.Normalize(normal0), light)),
                MathF.Max(0f, Vector3.Dot(Vector3.Normalize(normal1), light)),
                MathF.Max(0f, Vector3.Dot(Vector3.Normalize(normal2), light))
            );


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Transform(this Vector3 v, Matrix4x4 m) =>
            Vector4.Transform(v.ToVector4(), m);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 Transform(this Vector4 v, Matrix4x4 m) =>
            Vector4.Transform(v, m);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(this Vector3 v, Vector3 a) =>
            Vector3.Dot(v, a);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Cross(this Vector3 v, Vector3 a) =>
            Vector3.Cross(v, a);
    }
}
