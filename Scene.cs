using OpenTK.Graphics.OpenGL4;

public class Scene
{
    private VoxelEngine app;
    //public World world;
    //private VoxelMarker voxelMarker;
    private Water water;
    //private Clouds clouds;

    /// <summary>
    ///  Initializes the Scene class, creating instances of World, VoxelMarker, Water, and Clouds.
    /// </summary>
    public Scene(VoxelEngine app)
    {
        this.app = app;
        //this.world = new World(this.app);
        //this.voxelMarker = new VoxelMarker(this.world.voxelHandler);
        this.water = new Water(app);
        //this.clouds = new Clouds(app);
    }

    /// <summary>
    /// Updates the world, voxel marker, and clouds.
    /// </summary>
    public void Update()
    {
        //this.world.Update();
        //this.voxelMarker.Update();
        //this.clouds.Update();
    }

    /// <summary>
    /// Renders the world, clouds, water, and voxel marker, disabling and enabling cull face as needed.
    /// </summary>
    public void Render()
    {
        // Chunks rendering
        //this.world.Render();

        // Rendering without cull face
        GL.Disable(EnableCap.CullFace);
        //this.clouds.Render();
        this.water.Render();
        GL.Enable(EnableCap.CullFace);

        // Voxel selection
        //this.voxelMarker.Render();
    }
}
