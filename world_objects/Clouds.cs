using ModernGL;
using OpenTK.Graphics.OpenGL4;

public class Clouds
{
    private Scene scene { get; set; }
    private Camera camera { get; set; }

    private readonly glContext ctx;
    private readonly glContext.Program program;

    private readonly CloudMesh mesh;

    public Clouds(Scene scene, Camera camera)
    {
        this.scene = scene;
        this.camera = camera;

        this.ctx = this.scene.ctx;

        this.program = this.ctx.GetProgram("clouds");
        this.program["m_proj"] = this.camera.m_proj;
        this.program["center"] = Settings.CENTER_XZ;
        this.program["bg_color"] = Settings.BG_COLOR;
        this.program["cloud_scale"] = Settings.CLOUD_SCALE;

        this.mesh = new CloudMesh(this, program);
    }

    public void Update()
    {
        this.program["m_view"] = this.camera.m_view;
        this.program["u_time"] = (float)(this.scene.time * 0.001);
    }

    public void Render()
    {
        mesh.Render();
    }
}
