using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using ModernGL;

public class VoxelEngine : GameWindow
{
    public glContext ctx;

    public Stopwatch clock;
    public double deltaTime;
    public double time_init;
    public double time;
    public static bool is_running = false;

    //public Textures textures;
    //public Player player;
    public ShaderProgram shader_program;
    public Scene scene;

    public VoxelEngine(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        GL.ClearColor(Color4.CornflowerBlue);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);
        GL.Enable(EnableCap.Blend);

        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        this.ctx = moderngl.create_context();

        clock = Stopwatch.StartNew();

        //textures = new Textures(this);
        //player = new Player(this, Settings.PLAYER_POS);
        shader_program = new ShaderProgram(this, ctx);
        scene = new Scene(this);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        clock.Start();
        time_init = DateTime.Now.Ticks;

        is_running = true;
    }


    private void HandleEvents()
    {
        // event.type == pg.QUIT or (event.type == pg.KEYDOWN and event.key == pg.K_ESCAPE)
        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
        {
            is_running = false;
            Close();
        }

        // Handle other events, such as player input
        //player.HandleEvent(MouseState);
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        HandleEvents();

        //player.Update();
        shader_program.Update();
        scene.Update();

        deltaTime = clock.ElapsedMilliseconds;
        clock.Restart();
        time = (DateTime.Now.Ticks - time_init) * 0.001;
        Title = $"{1000.0 / (deltaTime):F0} FPS";
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        scene.Render();

        SwapBuffers();
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
            Flags = ContextFlags.ForwardCompatible,
            DepthBits = 24
        };

        using (var window = new VoxelEngine(GameWindowSettings.Default, nativeWindowSettings))
        {
            window.Run();
        }
    }
}
