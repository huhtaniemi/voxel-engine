using System;
using OpenTK.Graphics.OpenGL4;

public class Scene
{
    //private World world;
    //private VoxelMarker voxelMarker;
    //private Water water;
    //private Clouds clouds;

    //private QuadMesh quad; // demo
    private Chunk chunk; // demo2

    public Scene(VoxelEngine app)
    {
        //this.world = new World(this.app);
        //this.voxelMarker = new VoxelMarker(this.world.voxelHandler);
        //this.water = new Water(app);
        //this.clouds = new Clouds(app);

        //this.quad = new QuadMesh(app); // demo
        this.chunk = new Chunk(app); // demo2
    }

    public void Update()
    {
        //this.world.Update();
        //this.voxelMarker.Update();
        //this.clouds.Update();
    }

    public void Render()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        // Chunks rendering
        //this.world.Render();

        // Rendering without cull face
        //GL.Disable(EnableCap.CullFace);
        //this.clouds.Render();
        //this.water.Render();
        //GL.Enable(EnableCap.CullFace);

        // Voxel selection
        //this.voxelMarker.Render();

        //this.quad.Render(); // demo
        this.chunk.Render(); // demo2
    }
}
