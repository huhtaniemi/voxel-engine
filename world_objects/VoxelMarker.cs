using ModernGL;
using OpenTK.Mathematics;

public class VoxelMarker
{
    public Camera camera { get; private set; }

    public readonly VoxelEngine app;
    public readonly glContext.Program program;

    private CubeMesh mesh;


    public VoxelMarker(World world)
    {
        this.app = world.app;
        this.camera = app.camera;

        this.program = this.app.ctx.GetProgram("voxel_marker");
        this.program["m_proj"] = this.camera.m_proj;
        this.program["m_model"] = Matrix4.Identity;
        this.program["u_texture_0"] = 0;

        this.mesh = new CubeMesh(this, program);
    }

    public void Update(VoxelHandler voxelHandler)
    {
        this.program["m_view"] = this.camera.m_view;
        this.program["m_model"] = Matrix4.CreateTranslation(voxelHandler.position);
        this.program["mode_id"] = (uint) (voxelHandler.interactionMode ? 1 : 0);
    }

    public void Render(byte voxel_id)
    {
        if (voxel_id != 0)
        {
            mesh.Render();
        }
    }
}

