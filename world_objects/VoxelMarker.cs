using OpenTK.Mathematics;

public class VoxelMarker
{
    private VoxelEngine app;
    private VoxelHandler handler;
    private Vector3 position;
    private Matrix4 mModel;
    private CubeMesh mesh;

    /// <summary>
    ///  Initializes the VoxelMarker class with a reference to the VoxelHandler instance, sets the initial position, model matrix, and creates a new CubeMesh instance.
    /// </summary>
    public VoxelMarker(VoxelHandler voxelHandler)
    {
        this.app = voxelHandler.app;
        this.handler = voxelHandler;
        this.position = Vector3.Zero;
        this.mModel = GetModelMatrix();
        this.mesh = new CubeMesh(this.app);
    }

    /// <summary>
    /// Updates the position of the voxel marker based on the voxel handler's interaction mode and voxel position.
    /// </summary>
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

    /// <summary>
    /// Sets the uniforms for the shader program, including the interaction mode and model matrix.
    /// </summary>
    private void SetUniform()
    {
        app.shader_program.SetUniform(mesh.program, "mode_id", handler.interactionMode ? 1 : 0);
        app.shader_program.SetUniform(mesh.program, "m_model", GetModelMatrix());
    }

    /// <summary>
    /// Creates the model matrix for the voxel marker based on its position.
    /// </summary>
    private Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateTranslation(position);
    }

    /// <summary>
    /// Renders the voxel marker if a voxel is selected.
    /// </summary>
    public void Render()
    {
        if (handler.voxelId != 0)
        {
            SetUniform();
            mesh.Render();
        }
    }
}

