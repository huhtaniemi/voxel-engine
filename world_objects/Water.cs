using ModernGL;

public class Water
{
    public readonly VoxelEngine app;
    public readonly glContext.Program program;

    private readonly QuadMesh mesh;

    public Water(VoxelEngine app)
    {
        this.app = app;

        this.program = app.GetProgram("water");
        this.program["m_proj"] = app.player.m_proj;
        //this.program["m_model"] = Matrix4.Identity; // quad
        this.program["u_texture_0"] = 2;
        this.program["water_area"] = Settings.WATER_AREA;
        this.program["water_line"] = Settings.WATER_LINE;

        this.mesh = new QuadMesh(this);
    }

    public void Update()
    {
        this.program["m_view"] = app.player.m_view;
    }

    public void Render()
    {
        this.mesh.Render();
    }
}

