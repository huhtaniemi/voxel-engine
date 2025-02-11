using OpenTK.Mathematics;

public class Camera
{
    public Vector3 Position { get; private set; }
    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    public Vector3 Up { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Forward { get; private set; }

    public Matrix4 MProj { get; private set; }
    public Matrix4 MView { get; private set; }

    public Frustum Frustum { get; private set; }

    /// <summary>
    /// Initializes the Camera class, setting the position, yaw, pitch, and other vectors. It also sets up the projection matrix and initializes the frustum.
    /// </summary>
    public Camera(Vector3 position, float yaw, float pitch)
    {
        Position = position;
        Yaw = MathHelper.DegreesToRadians(yaw);
        Pitch = MathHelper.DegreesToRadians(pitch);

        Up = Vector3.UnitY;
        Right = Vector3.UnitX;
        Forward = -Vector3.UnitZ;

        MProj = Matrix4.CreatePerspectiveFieldOfView(Settings.V_FOV, Settings.ASPECT_RATIO, Settings.NEAR, Settings.FAR);
        MView = Matrix4.Identity;

        Frustum = new Frustum(this);
    }

    /// <summary>
    /// Updates the camera vectors and view matrix.
    /// </summary>
    public virtual void Update()
    {
        UpdateVectors();
        UpdateViewMatrix();
    }

    /// <summary>
    /// Updates the view matrix using the LookAt function.
    /// </summary>
    private void UpdateViewMatrix()
    {
        MView = Matrix4.LookAt(Position, Position + Forward, Up);
    }

    /// <summary>
    /// Updates the forward, right, and up vectors based on the current yaw and pitch.
    /// </summary>
    private void UpdateVectors()
    {
        //Forward.X = MathF.Cos(Yaw) * MathF.Cos(Pitch);
        //Forward.Y = MathF.Sin(Pitch);
        //Forward.Z = MathF.Sin(Yaw) * MathF.Cos(Pitch);
        Forward = (MathF.Cos(Yaw) * MathF.Cos(Pitch), MathF.Sin(Pitch), MathF.Sin(Yaw) * MathF.Cos(Pitch));


        Forward = Vector3.Normalize(Forward);
        Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
    }

    /// <summary>
    /// Rotates the camera pitch, clamping it to a maximum value.
    /// </summary>
    public void RotatePitch(float deltaY)
    {
        Pitch -= deltaY;
        Pitch = MathHelper.Clamp(Pitch, -Settings.PITCH_MAX, Settings.PITCH_MAX);
    }

    /// <summary>
    /// Rotates the camera yaw.
    /// </summary>
    public void RotateYaw(float deltaX)
    {
        Yaw += deltaX;
    }

    // Moves the camera in the specified direction based on the given velocity.
    public void MoveLeft(float velocity)
    {
        Position -= Right * velocity;
    }

    public void MoveRight(float velocity)
    {
        Position += Right * velocity;
    }

    public void MoveUp(float velocity)
    {
        Position += Up * velocity;
    }

    public void MoveDown(float velocity)
    {
        Position -= Up * velocity;
    }

    public void MoveForward(float velocity)
    {
        Position += Forward * velocity;
    }

    public void MoveBack(float velocity)
    {
        Position -= Forward * velocity;
    }
}
