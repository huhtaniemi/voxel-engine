using ModernGL;

public class Water
{
    public Camera camera { get; private set; }

    public readonly VoxelEngine app;
    public readonly glContext.Program program;

    private readonly QuadMesh mesh;

    public Water(VoxelEngine app)
    {
        this.app = app;
        this.camera = app.camera;

        this.program = this.app.ctx.GetProgram("water");
        this.program["m_proj"] = this.camera.m_proj;
        //this.program["m_model"] = Matrix4.Identity; // quad
        this.program["u_texture_0"] = 2;
        this.program["water_area"] = Settings.WATER_AREA;
        this.program["water_line"] = Settings.WATER_LINE;

        this.mesh = new QuadMesh(this, program);
    }

    public void Update()
    {
        this.program["m_view"] = this.camera.m_view;
    }

    public void Render()
    {
        this.mesh.Render();
    }
}

