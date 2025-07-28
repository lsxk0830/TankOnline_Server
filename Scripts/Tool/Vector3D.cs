public struct Vector3D
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vector3D(float x, float y, float z) => (X, Y, Z) = (x, y, z);

    // 向量减法
    public static Vector3D operator -(Vector3D a, Vector3D b) =>
        new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    // 向量叉积
    public static Vector3D Cross(Vector3D a, Vector3D b) =>
        new Vector3D(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );

    // 向量点积
    public static double Dot(Vector3D a, Vector3D b) =>
        a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    // 向量模长
    public double Magnitude() =>
        Math.Sqrt(X * X + Y * Y + Z * Z);

    // 归一化（单位向量）
    public Vector3D Normalize()
    {
        float mag = (float)Magnitude();
        if (mag < 1e-9) throw new ArgumentException("零向量无法归一化");
        return new Vector3D(X / mag, Y / mag, Z / mag);
    }
}