using ModernGL;

public class Water
{
    private Scene scene { get; set; }
    private Camera camera { get; set; }

    private readonly glContext ctx;
    private readonly glContext.Program program;

    private readonly QuadMesh mesh;

    public Water(Scene scene, Camera camera)
    {
        this.scene = scene;
        this.camera = camera;

        this.ctx = this.scene.ctx;

        this.program = this.ctx.GetProgram("water");
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

