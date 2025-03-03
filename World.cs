using System;
using OpenTK.Mathematics;
using ModernGL;

public class World
{
    private Scene scene { get; set; }
    private Camera camera { get; set; }

    public glContext ctx { get; private set; }
    public glContext.Program program { get; private set; }

    public Chunk[] chunks { get; private set; }
    public byte[][] voxels { get; private set; }

    public World(Scene scene, Camera camera)
    {
        this.scene = scene;
        this.camera = camera;

        this.ctx = this.scene.ctx;

        this.program = this.ctx.GetProgram("chunk");
        this.program["m_proj"] = this.camera.m_proj;
        this.program["m_model"] = Matrix4.Identity;
        //this.program["u_texture_0"] = 0;
        this.program["u_texture_array_0"] = 1;
        this.program["bg_color"] = Settings.BG_COLOR;
        this.program["water_line"] = Settings.WATER_LINE;

        //
        chunks = new Chunk[Settings.WORLD_VOL];
        // voxels are actually list of refs to byte[]-arrays,
        // therefore this array will point to same (original) byte-array.
        voxels = new byte[Settings.WORLD_VOL][];
        //for (int i = 0; i < Settings.WORLD_VOL; i++)
        //    voxels[i] = new byte[Settings.CHUNK_VOL];
        BuildChunks();
        BuildChunkMesh();
    }

    private void BuildChunks()
    {
        for (int x = 0; x < Settings.WORLD_W; x++)
        {
            for (int y = 0; y < Settings.WORLD_H; y++)
            {
                for (int z = 0; z < Settings.WORLD_D; z++)
                {
                    int chunk_index = x + Settings.WORLD_W * z + Settings.WORLD_AREA * y;

                    var chunk = new Chunk(this, new(x, y, z));

                    // note:
                    // it okey to build voxels for each chunk beforehand
                    // build besh (of each chunk) uses world voxels to test for adjecent spaces?

                    chunks[chunk_index] = chunk;
                    //chunk.BuildVoxels(out this.voxels[chunk_index]);
                    this.voxels[chunk_index] = chunk.voxels;

                    // Put the chunk voxels in a separate array
                    //voxels[chunkIndex,:] = chunk.BuildVoxels();

                    // Get pointer to voxels
                    //chunk.voxels = ref voxels[chunkIndex,0];
                }
            }
        }
    }

    private void BuildChunkMesh()
    {
        foreach (var chunk in chunks)
            chunk.BuildMesh();
    }

    public void UpdateProjection()
    {
        this.program["m_proj"] = this.camera.m_proj;
    }
    public void Update()
    {
        this.program["m_view"] = this.camera.m_view;
    }

    public void Render()
    {
        foreach (var chunk in chunks)
        {
            if (!camera.isInView(chunk.Center))
                continue;
            // update each frame for each chunk
            // ... but the app is the same, do we need to? should world obj update thies ?
            // ahh.. this is to position individual chunk to relative position! render specific...?
            this.program["m_model"] = chunk.m_model;
            chunk.Render();
        }
    }
}

