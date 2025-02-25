using OpenTK.Mathematics;
using System;

public class Frustum
{
    private Camera cam;

    private float factorY;
    private float tanY;

    private float factorX;
    private float tanX;

    public Frustum(Camera camera)
    {
        this.cam = camera;

        float halfY = Settings.V_FOV * 0.5f;
        factorY = 1.0f / MathF.Cos(halfY);
        tanY = MathF.Tan(halfY);

        float halfX = Settings.H_FOV * 0.5f;
        factorX = 1.0f / MathF.Cos(halfX);
        tanX = MathF.Tan(halfX);
    }

    public bool IsOnFrustum(Chunk chunk)
    {
        // Vector to sphere center
        Vector3 sphereVec = chunk.Center - cam.Position;

        // Outside the NEAR and FAR planes?
        float sz = Vector3.Dot(sphereVec, cam.Forward);
        if (!(Settings.NEAR - Settings.CHUNK_SPHERE_RADIUS <= sz && sz <= Settings.FAR + Settings.CHUNK_SPHERE_RADIUS))
        {
            return false;
        }

        // Outside the TOP and BOTTOM planes?
        double sy = Vector3.Dot(sphereVec, cam.Up);
        double distY = factorY * Settings.CHUNK_SPHERE_RADIUS + sz * tanY;
        if (!( -distY <= sy && sy <= distY))
        {
            return false;
        }

        // Outside the LEFT and RIGHT planes?
        double sx = Vector3.Dot(sphereVec, cam.Right);
        double distX = factorX * Settings.CHUNK_SPHERE_RADIUS + sz * tanX;
        if (!( -distX <= sx && sx <= distX))
        {
            return false;
        }

        return true;
    }
}

