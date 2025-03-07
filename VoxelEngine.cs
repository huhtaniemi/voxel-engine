using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using ModernGL;
using OpenTK.Mathematics;
using static ModernGL.glContext;

static class glContextHelper
{
    public static glContext.Program GetProgram(this glContext ctx, string shader_name)
    {
        return ctx.program(
            File.ReadAllText($"shaders/{shader_name}.vert"),
            File.ReadAllText($"shaders/{shader_name}.frag")
        );
    }
}

public class MovingAverage
{
    private Queue<double> _values;
    private int _maxCount = 10;
    private double _valuesSum = 0;

    public MovingAverage() => _values = new Queue<double>();

    public double Add(double value)
    {
        if (_values.Count >= _maxCount)
            _valuesSum -= _values.Dequeue();
        _values.Enqueue(value);
        _valuesSum += value;
        return _valuesSum / _values.Count;
    }
}
public class VoxelEngine : GameWindow
{
    public glContext ctx;

    MovingAverage deltaTimeAvg = new();

    private Textures textures { get; set; }
    private Camera camera { get; set; }
    private Scene scene { get; set; }
    private Player player { get; set; }

    public VoxelEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        this.ctx = moderngl.create_context();
        this.ctx.enable(EnableFlags.DEPTH_TEST | EnableFlags.CULL_FACE | EnableFlags.BLEND);
        this.ctx.set_clearcolor(System.Drawing.Color.CornflowerBlue);

        textures = new Textures(this);
        camera = new(Settings.PLAYER_POS, -90, 0);
        scene = new Scene(ctx, camera);
        player = new Player(camera, scene);
    }

    public static OpenSimplex.Noise _noise;
    static VoxelEngine() =>
        //SimplexNoise.Noise.Seed = Settings.SEED;
        _noise = new OpenSimplex.Noise(Settings.SEED);

    protected override void OnLoad()
    {
        base.OnLoad();
        CursorState = CursorState.Grabbed;
    }


    private void HandleEvents()
    {
        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
        player.HandleEvent(MouseState, KeyboardState);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        // player kayboard and mouse events
        HandleEvents();

        // update vertex data
        scene.Update(DateTime.Now.TimeOfDay.TotalMilliseconds);

        Title = $"{1.0 / deltaTimeAvg.Add(args.Time):F0} FPS";
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);

        // render vertex data
        scene.Render();

        SwapBuffers();
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        camera.UpdatePerspective((float)e.Width / e.Height);
        scene.UpdateProjection();
        GL.Viewport(0, 0, e.Width, e.Height);
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        // Add any necessary cleanup code here
    }

    public static void Main()
    {
        var nativeWindowSettings = new NativeWindowSettings()
        {
            ClientSize = Settings.WIN_RES,
            Title = "OpenGL 3.3 Window",
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible | ContextFlags.Debug,
            DepthBits = 24
        };

        using var window = new VoxelEngine(GameWindowSettings.Default, nativeWindowSettings);
        window.Run();
    }
}
