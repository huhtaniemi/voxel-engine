using OpenTK.Mathematics;

public class VoxelMarker
{
    private VoxelEngine app;
    private VoxelHandler handler;
    private Vector3 position;
    private Matrix4 m_model;
    private CubeMesh mesh;


    public VoxelMarker(VoxelHandler voxelHandler)
    {
        this.app = voxelHandler.app;
        this.handler = voxelHandler;
        this.position = Vector3.Zero;
        this.m_model = GetModelMatrix();
        this.mesh = new CubeMesh(this.app);
    }

    public void Update()
    {
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
            app.shader_program.voxel_marker["mode_id"] = (uint)(handler.interactionMode ? 1 : 0);
            app.shader_program.voxel_marker["m_model"] = GetModelMatrix();
            mesh.Render();
        }
    }
}

