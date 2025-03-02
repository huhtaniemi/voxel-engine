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
    public double deltaTime;
    public double time_init;
    public double time;

    public Textures textures;
    public Player player;
    public Scene scene;

    public VoxelEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        this.ctx = moderngl.create_context();
        this.ctx.enable(EnableFlags.DEPTH_TEST | EnableFlags.CULL_FACE | EnableFlags.BLEND);
        this.ctx.set_clearcolor(System.Drawing.Color.CornflowerBlue);

        textures = new Textures(this);
        player = new Player(this, Settings.PLAYER_POS);
        scene = new Scene(this);
    }

    public static OpenSimplex.Noise _noise;
    static VoxelEngine() =>
        //SimplexNoise.Noise.Seed = Settings.SEED;
        _noise = new OpenSimplex.Noise(Settings.SEED);

    protected override void OnLoad()
    {
        base.OnLoad();
        CursorState = CursorState.Grabbed;

        time_init = DateTime.Now.Ticks;
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
        scene.Update();

        time = DateTime.Now.TimeOfDay.TotalMilliseconds * 0.001;
        deltaTime = args.Time * 1000;

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
        player.UpdateViewport(e.Size);
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
