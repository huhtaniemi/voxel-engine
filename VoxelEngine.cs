using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using ModernGL;
using OpenTK.Mathematics;

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
    public ShaderProgram shader_program;
    public Scene scene;

    public VoxelEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        this.ctx = moderngl.create_context();
        this.ctx.enable();// ctx.DepthTest | ctx.CullFace | ctx.Blend);
        this.ctx.set_clearcolor(System.Drawing.Color.CornflowerBlue);

        textures = new Textures(this);
        player = new Player(this, Settings.PLAYER_POS);
        shader_program = new ShaderProgram(this, ctx);
        scene = new Scene(this);
    }

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

        // update shader uniforms
        shader_program.Update();

        // update vertex data
        scene.Update();

        time = (DateTime.Now.Ticks - time_init) * 0.001;
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

        player.m_proj = Matrix4.CreatePerspectiveFieldOfView(Settings.V_FOV, (float)e.Width / e.Height, Settings.NEAR, Settings.FAR);
        shader_program.chunk["m_proj"] = player.m_proj;
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
