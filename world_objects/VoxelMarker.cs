using ModernGL;
using OpenTK.Mathematics;

public class VoxelMarker
{
    public readonly VoxelEngine app;
    public readonly glContext.Program program;

    private VoxelHandler handler;
    private Vector3 position;
    private Matrix4 m_model;
    private CubeMesh mesh;


    public VoxelMarker(VoxelHandler voxelHandler)
    {
        this.app = voxelHandler.app;

        this.program = app.GetProgram("voxel_marker");
        this.program["m_proj"] = app.player.m_proj;
        this.program["m_model"] = Matrix4.Identity;
        this.program["u_texture_0"] = 0;

        this.handler = voxelHandler;
        this.position = Vector3.Zero;
        this.m_model = GetModelMatrix();

        this.mesh = new CubeMesh(this);
    }

    public void Update()
    {
        this.program["m_view"] = app.player.m_view;
        if (handler.voxel_id != 0)
        {
            if (handler.interactionMode)
                position = handler.voxel_world_pos + handler.voxel_normal;
            else
                position = handler.voxel_world_pos;
        }
    }

    private Matrix4 GetModelMatrix()
    {
        return (m_model = Matrix4.CreateTranslation(position));
    }

    public void Render()
    {
        if (handler.voxel_id != 0)
        {
            //this.mesh.program["mode_id"] = 1;
            program["mode_id"] = (uint)(handler.interactionMode ? 1 : 0);
            program["m_model"] = GetModelMatrix();
            mesh.Render();
        }
    }
}

