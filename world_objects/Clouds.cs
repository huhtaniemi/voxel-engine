using ModernGL;
using OpenTK.Graphics.OpenGL4;

public class Clouds
{
    public readonly VoxelEngine app;
    public readonly glContext.Program program;

    private CloudMesh mesh;

    public Clouds(VoxelEngine app)
    {
        this.app = app;

        this.program = this.app.ctx.GetProgram("clouds");
        this.program["m_proj"] = app.player.m_proj;
        this.program["center"] = Settings.CENTER_XZ;
        this.program["bg_color"] = Settings.BG_COLOR;
        this.program["cloud_scale"] = Settings.CLOUD_SCALE;

        this.mesh = new CloudMesh(this, program);
    }

    public void Update()
    {
        this.program["m_view"] = app.player.m_view;
        this.program["u_time"] = (float)app.time;
    }

    public void Render()
    {
        mesh.Render();
    }
}
