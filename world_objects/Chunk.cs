using OpenTK.Mathematics;
using System;
using static Settings;


public class Chunk
{
    public VoxelEngine app;
    /*
    public World world;
    public Vector3i Position { get; private set; }
    public Matrix4 MModel { get; private set; }
    */
    public byte[] voxels { get; private set; }

    internal ChunkMesh? mesh { get; private set; }
    /*
    public bool IsEmpty { get; private set; }

    public Vector3 Center { get; private set; }
    private Func<Chunk, bool> isOnFrustum;

    /// <summary>
    /// Initializes the Chunk class with a reference to the World instance, its position, and other properties.
    /// </summary>
    public Chunk(World world, Vector3i position)
    {
        this.app = world.app;
        //this.world = world;
        this.Position = position;
        this.MModel = GetModelMatrix();
        this.voxels = new byte[Settings.CHUNK_VOL];
        this.IsEmpty = true;

        this.Center = (position.ToVector3() + new Vector3(0.5f)) * Settings.CHUNK_SIZE;
        this.isOnFrustum = this.app.player.Frustum.IsOnFrustum;
    }
    */

    public Chunk(VoxelEngine app)
    {
        this.app = app;
        this.voxels = BuildVoxels();

        BuildMesh();
    }

    /*
    /// <summary>
    /// Sets the uniform for the chunk's model matrix.
    /// </summary>
    private Matrix4 GetModelMatrix()
    {
        return Matrix4.CreateTranslation(new Vector3(Position) * Settings.CHUNK_SIZE);
    }

    private void SetUniform()
    {
        app.shader_program.SetUniform(mesh.program, "m_model", MModel);
    }
    */

    /// <summary>
    /// Builds the mesh for the chunk.
    /// </summary>
    private void BuildMesh() =>
        this.mesh = new ChunkMesh(this);

    /// <summary>
    /// Renders the chunk if it is not empty and is within the frustum.
    /// </summary>
    public void Render()
    {
        /*
        if (!IsEmpty && isOnFrustum(this))
        {
            SetUniform();
            mesh.Render();
        }
        */
        this.mesh?.Render();
    }

    /// <summary>
    ///  Builds the voxels for the chunk and checks if the chunk is empty.
    /// </summary>
    static private byte[] BuildVoxels()
    {
        var voxels = new byte[CHUNK_VOL];
        /*
        var (cx, cy, cz) = Position * Settings.CHUNK_SIZE;
        GenerateTerrain(ref voxels, cx, cy, cz);

        if (Array.Exists(voxels, v => v != 0))
        {
            IsEmpty = false;
        }
        */
        for (byte x = 0; x < CHUNK_SIZE; x++)
        {
            for (byte z = 0; z < CHUNK_SIZE; z++)
            {
                 for (byte y = 0; y < CHUNK_SIZE; y++)
                {
                    // todo: noise
                    voxels[x + CHUNK_SIZE * z + CHUNK_AREA * y] =
                        (byte)(x + y + z);
                }
            }
        }
        return voxels;
    }


    /*
    /// <summary>
    ///  Generates the terrain for the chunk.
    /// </summary>
    private static void GenerateTerrain(ref byte[] voxels, int cx, int cy, int cz)
    {
        for (int x = 0; x < Settings.CHUNK_SIZE; x++)
        {
            int wx = x + cx;
            for (int z = 0; z < Settings.CHUNK_SIZE; z++)
            {
                int wz = z + cz;
                int worldHeight = TerrainGen.GetHeight(wx, wz);
                int localHeight = Math.Min(worldHeight - cy, Settings.CHUNK_SIZE);

                for (int y = 0; y < localHeight; y++)
                {
                    int wy = y + cy;
                    TerrainGen.SetVoxelId(ref voxels, x, y, z, wx, wy, wz, worldHeight);
                }
            }
        }
    }
    */
}

