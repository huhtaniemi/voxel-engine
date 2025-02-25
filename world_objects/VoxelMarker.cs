using OpenTK.Mathematics;

public class VoxelMarker
{
    private VoxelEngine app;
    private VoxelHandler handler;
    private Vector3 position;
    private Matrix4 mModel;
    private CubeMesh mesh;


    public VoxelMarker(VoxelHandler voxelHandler)
    {
        this.app = voxelHandler.app;
        this.handler = voxelHandler;
        this.position = Vector3.Zero;
        this.mModel = GetModelMatrix();
        this.mesh = new CubeMesh(this.app);
    }

    public void Update()
    {
        if (handler.voxelId != 0)
        {
            if (handler.interactionMode)
            {
                position = handler.voxelWorldPos + handler.voxelNormal;
            }
            else
            {
                position = handler.voxelWorldPos;
            }
        }
    }

    private void SetUniform()
    {
        app.shader_program.SetUniform(mesh.program, "mode_id", handler.interactionMode ? 1 : 0);
        app.shader_program.SetUniform(mesh.program, "m_model", GetModelMatrix());
    }

    private Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateTranslation(position);
    }

    public void Render()
    {
        if (handler.voxelId != 0)
        {
            SetUniform();
            mesh.Render();
        }
    }
}

