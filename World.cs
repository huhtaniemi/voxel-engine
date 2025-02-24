using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

public class World
{
    public VoxelEngine app;
    public Chunk[] chunks { get; private set; }
    public byte[,] voxels { get; private set; }
    public VoxelHandler voxelHandler { get; private set; }

    public World(VoxelEngine app)
    {
        this.app = app;
        chunks = new Chunk[Settings.WORLD_VOL];
        voxels = new byte[Settings.WORLD_VOL, Settings.CHUNK_VOL];
        BuildChunks();
        BuildChunkMesh();
        voxelHandler = new VoxelHandler(this);
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
                    Buffer.BlockCopy(
                        chunk.voxels, 0,
                        this.voxels, chunk_index * this.voxels.GetLength(1),
                        chunk.voxels.Length);

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
            chunk.BuildMesh(ref chunk.mesh);
    }

    public void Update()
    {
        voxelHandler.Update();
    }

    public void Render()
    {
        foreach (var chunk in chunks)
        {
            chunk.Render();
        }
    }
}

