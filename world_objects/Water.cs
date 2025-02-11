public class Water
{
    private VoxelEngine app;
    private QuadMesh mesh;

    public Water(VoxelEngine app)
    {
        this.app = app;
        this.mesh = new QuadMesh(app);
    }

    public void Render()
    {
        this.mesh.Render();
    }
}

