using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

public class World
{
    public VoxelEngine app;
    public Chunk[] chunks { get; private set; }
    public byte[,] voxels { get; private set; }
    public VoxelHandler voxelHandler { get; private set; }

    /// <summary>
    /// Initializes the World class, creating arrays for chunks and voxels, and building the chunks and their meshes. It also initializes the VoxelHandler.
    /// </summary>
    public World(VoxelEngine app)
    {
        this.app = app;
        chunks = new Chunk[Settings.WORLD_VOL];
        voxels = new byte[Settings.WORLD_VOL, Settings.CHUNK_VOL];
        BuildChunks();
        BuildChunkMesh();
        voxelHandler = new VoxelHandler(this);
    }

    public void Update()
    {
        voxelHandler.Update();
    }

    /// <summary>
    /// Creates chunks and assigns them to the Chunks array. It also builds the voxels for each chunk and assigns them to the Voxels array.
    /// </summary>
    private void BuildChunks()
    {
        for (int x = 0; x < Settings.WORLD_W; x++)
        {
            for (int y = 0; y < Settings.WORLD_H; y++)
            {
                for (int z = 0; z < Settings.WORLD_D; z++)
                {
                    var chunk = new Chunk(this, new Vector3i(x, y, z));

                    int chunkIndex = x + Settings.WORLD_W * z + Settings.WORLD_AREA * y;
                    chunks[chunkIndex] = chunk;

                    // Put the chunk voxels in a separate array
                    //voxels[chunkIndex,:] = chunk.BuildVoxels();
                    Array.Copy(chunk.BuildVoxels(), 0, voxels, chunkIndex * Settings.WORLD_VOL, Settings.WORLD_VOL);
                    //for (int i = 0; i < Settings.WORLD_VOL; i++) { voxels[chunkIndex, i] = voxels_chunk[i]; }

                    // Get pointer to voxels
                    chunk.voxels = ref voxels[chunkIndex,0];
                }
            }
        }
    }

    private void BuildChunkMesh()
    {
        foreach (var chunk in chunks)
        {
            chunk.BuildMesh();
        }
    }

    public void Render()
    {
        foreach (var chunk in chunks)
        {
            chunk.Render();
        }
    }
}

