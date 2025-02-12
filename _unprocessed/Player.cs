using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;
using System.Net;
using System.Xml.Linq;

//using Camera;

public class Player : Camera
{
    private VoxelEngine app;

    /// <summary>
    /// Initializes the Player class, inheriting from Camera.
    /// </summary>
    public Player(VoxelEngine app, Vector3 position, float yaw = -90, float pitch = 0)
        : base(position, yaw, pitch)
    {
        this.app = app;
    }

    /// <summary>
    /// Calls KeyboardControl and MouseControl methods, then updates the base class.
    /// </summary>
    public override void Update()
    {
        KeyboardControl();
        MouseControl();
        base.Update();
    }

    /// <summary>
    /// Handles mouse button events to add or remove voxels.
    /// </summary>
    /// <param name="e"></param>
    public void HandleEvent(MouseState e)
    {
        // Adding and removing voxels with clicks
        if (e.IsAnyButtonDown)
        {
            var voxelHandler = app.scene.world.voxelHandler;

            if (e.IsButtonDown(MouseButton.Button1))
            {
                voxelHandler.SetVoxel();
            }
            if (e.IsButtonDown(MouseButton.Button3))
            {
                voxelHandler.SwitchMode();
            }
        }
    }

    /// <summary>
    /// Handles mouse movement to rotate the camera.
    /// </summary>
    private void MouseControl()
    {
        var (mouse_dx, mouse_dy) = app.MouseState.Delta;
        if (mouse_dx != 0)
        {
            RotateYaw(mouse_dx * Settings.MOUSE_SENSITIVITY);
        }
        if (mouse_dy != 0)
        {
            RotatePitch(mouse_dy * Settings.MOUSE_SENSITIVITY);
        }
    }

    /// <summary>
    ///  Handles keyboard input to move the player.
    /// </summary>
    private void KeyboardControl()
    {
        var keyState = app.KeyboardState;
        float velocity = Settings.PLAYER_SPEED * (float)app.deltaTime;
        if (keyState.IsKeyDown(Keys.W))
        {
            MoveForward(velocity);
        }
        if (keyState.IsKeyDown(Keys.S))
        {
            MoveBack(velocity);
        }
        if (keyState.IsKeyDown(Keys.D))
        {
            MoveRight(velocity);
        }
        if (keyState.IsKeyDown(Keys.A))
        {
            MoveLeft(velocity);
        }
        if (keyState.IsKeyDown(Keys.Q))
        {
            MoveUp(velocity);
        }
        if (keyState.IsKeyDown(Keys.E))
        {
            MoveDown(velocity);
        }
    }
}
