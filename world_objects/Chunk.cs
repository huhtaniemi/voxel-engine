using System;
using OpenSimplex;
using OpenTK.Mathematics;
using static Settings;


public class Chunk : IDisposable
{
    public VoxelEngine app;
    public World world;
    public Vector3i position { get; private set; }
    public Matrix4 m_model { get; private set; }
    public byte[] voxels;// { get; private set; }

    internal ChunkMesh? mesh;// { get; private set; }
    internal bool IsEmpty;// { get; private set; }
    public Vector3 Center { get; private set; }
    private Func<Chunk, bool> isOnFrustum;

    public void Dispose()
    {
        mesh?.Dispose();
    }

    public Chunk(World world, Vector3i position)
    {
        this.app = world.app;
        this.world = world;
        this.position = position;
        this.m_model = Matrix4.CreateTranslation(new Vector3(position) * Settings.CHUNK_SIZE);
        BuildVoxels(out this.voxels, position, out IsEmpty);
        //BuildMesh(out this.mesh);

        this.Center = (position.ToVector3() + new Vector3(0.5f)) * Settings.CHUNK_SIZE;
        this.isOnFrustum = this.app.player.Frustum.IsOnFrustum;
    }

    internal void BuildMesh(ref ChunkMesh mesh)
    {
        mesh?.Dispose();
        mesh = new ChunkMesh(this);
    }

    public void Render()
    {
        if (IsEmpty) return;
        if (!isOnFrustum(this)) return;

        world.app.shader_program.chunk["m_model"] = m_model;
        //this.mesh?.program["m_model"] = m_model;
        this.mesh?.Render();
    }

    static internal byte[] BuildVoxels(out byte[] voxels, Vector3i position, out bool IsEmpty)
    {
        voxels = new byte[CHUNK_VOL];
        var (cx, cy, cz) = position * Settings.CHUNK_SIZE;
        /*
        GenerateTerrain(ref voxels, cx, cy, cz);

        if (Array.Exists(voxels, v => v != 0))
        {
            IsEmpty = false;
        }
        */
        for (byte x = 0; x < CHUNK_SIZE; x++)
        {
            var wx = x + cx;
            for (byte z = 0; z < CHUNK_SIZE; z++)
            {
                var wz = z + cz;
                var world_height = (int)(VoxelEngine._noise.Evaluate(wx * 0.02, wz * 0.02) * 32 + 32);
                var localHeight = Math.Min(world_height - cy, CHUNK_SIZE);
                for (byte y = 0; y < localHeight; y++)
                {
                    var wy = y + cy;
                    voxels[x + CHUNK_SIZE * z + CHUNK_AREA * y] = 2;// (byte)(wy + 1);
                }
            }
        }
        IsEmpty = !voxels.Any(v => v != 0);
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
