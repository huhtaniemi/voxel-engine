using System;
using OpenTK.Graphics.OpenGL4;

public class Scene
{
    private World world { get; set; }
    public VoxelHandler voxel_handler { get; private set; }
    private VoxelMarker voxel_marker { get; set; }
    private Water water { get; set; }
    private Clouds clouds { get; set; }

    public double time;
    public double deltaTime;

    //private QuadMesh quad; // demo
    //private Chunk chunk; // demo2

    public Scene(VoxelEngine app)
    {
        this.world = new World(app);
        this.voxel_handler = new VoxelHandler(this.world);
        this.voxel_marker = new VoxelMarker(this.world);
        this.water = new Water(app);
        this.clouds = new Clouds(app);

        //this.quad = new QuadMesh(app); // demo
        //this.chunk = new Chunk(app); // demo2
    }

    public void UpdateProjection()
    {
        this.world.UpdateProjection();
    }

    public void Update(double time)
    {
        this.deltaTime = time - this.time;
        this.time = time;

        this.world.Update();
        this.voxel_handler.Update();
        this.voxel_marker.Update(this.voxel_handler);
        this.water.Update();
        this.clouds.Update();
    }

    public void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Chunks rendering
        this.world.Render();

        // Rendering without cull face
        GL.Disable(EnableCap.CullFace);
        this.clouds.Render();
        this.water.Render();
        GL.Enable(EnableCap.CullFace);

        // Voxel selection
        this.voxel_marker.Render(this.voxel_handler.voxel_id);

        //this.quad.Render(); // demo
        //this.chunk.Render(); // demo2
    }
}
