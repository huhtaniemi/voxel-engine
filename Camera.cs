using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;


public class Camera
{
    public Vector3 Position { get; private set; }

    protected float Yaw { get; private set; }
    protected float Pitch { get; private set; }

    public Vector3 Up { get; private set; } = Vector3.UnitY;
    public Vector3 Right { get; private set; } = Vector3.UnitX;
    public Vector3 Forward { get; private set; } = -Vector3.UnitZ;


    public Matrix4 m_proj { get; private set; }
    public Matrix4 m_view { get; private set; }

    protected struct Frustum(float hFOV, float vFOV)
    {
        public readonly float factorX = 1.0f / MathF.Cos(hFOV * 0.5f);
        public readonly float tanX = MathF.Tan(hFOV * 0.5f);
        public readonly float factorY = 1.0f / MathF.Cos(vFOV * 0.5f);
        public readonly float tanY = MathF.Tan(vFOV * 0.5f);
    };
    protected Frustum viewFrustum { get; private set; }


    public Camera(Vector3 position, float yaw, float pitch)
    {
        Position = position;

        Yaw = MathHelper.DegreesToRadians(yaw);
        Pitch = MathHelper.DegreesToRadians(pitch);

        m_proj = Matrix4.CreatePerspectiveFieldOfView(Settings.V_FOV, Settings.ASPECT_RATIO, Settings.NEAR, Settings.FAR);
        m_view = Matrix4.Identity;

        viewFrustum = new(Settings.H_FOV, Settings.V_FOV);
    }

    public bool isInView(Vector3 center)
    {
        Vector3 sphereVec = center - Position;

        // Outside the NEAR and FAR planes?
        float sz = Vector3.Dot(sphereVec, Forward);
        if (!(Settings.NEAR - Settings.CHUNK_SPHERE_RADIUS <= sz && sz <= Settings.FAR + Settings.CHUNK_SPHERE_RADIUS))
        {
            return false;
        }

        // Outside the LEFT and RIGHT planes?
        double sx = Vector3.Dot(sphereVec, Right);
        double distX = viewFrustum.factorX * Settings.CHUNK_SPHERE_RADIUS + sz * viewFrustum.tanX;
        if (!(-distX <= sx && sx <= distX))
        {
            return false;
        }

        // Outside the TOP and BOTTOM planes?
        double sy = Vector3.Dot(sphereVec, Up);
        double distY = viewFrustum.factorY * Settings.CHUNK_SPHERE_RADIUS + sz * viewFrustum.tanY;
        if (!(-distY <= sy && sy <= distY))
        {
            return false;
        }

        return true;
    }

    public void UpdatePerspective(float aspect)
    {
        m_proj = Matrix4.CreatePerspectiveFieldOfView(Settings.V_FOV, aspect, Settings.NEAR, Settings.FAR);
    }
    public void UpdateView()
    {
        // the front matrix is calculated
        Forward = (MathF.Cos(Yaw) * MathF.Cos(Pitch), MathF.Sin(Pitch), MathF.Sin(Yaw) * MathF.Cos(Pitch));

        // We need to make sure the vectors are all normalized, as otherwise we would get some funky results.
        Forward = Vector3.Normalize(Forward);

        // Calculate both the right and the up vector using cross product.
        // Note that we are calculating the right from the global up; this behaviour might
        // not be what you need for all cameras so keep this in mind if you do not want a FPS camera.
        Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Forward));

        // "eye"    - position of camera in world space.
        // "target" - position of target in world space (thast camera looks at).
        // "up"     - normalized 'Up' vector  of camera orientation in world space (should not be parallel to the camera direction, that is target - eye).
        m_view = Matrix4.LookAt(Position, Position + Forward, Up);
    }

    public void RotatePitch(float deltaY)
    {
        Pitch -= deltaY;
        Pitch = MathHelper.Clamp(Pitch, -Settings.PITCH_MAX, Settings.PITCH_MAX);
    }

    public void RotateYaw(float deltaX) =>
        Yaw += deltaX;

    // Moves the camera in the specified direction based on the given velocity.
    public void MoveLeft(float velocity) =>
        Position -= Right * velocity;

    public void MoveRight(float velocity) =>
        Position += Right * velocity;

    public void MoveUp(float velocity) =>
        Position += Up * velocity;

    public void MoveDown(float velocity) =>
        Position -= Up * velocity;

    public void MoveForward(float velocity) =>
        Position += Forward * velocity;

    public void MoveBack(float velocity) =>
        Position -= Forward * velocity;
}
