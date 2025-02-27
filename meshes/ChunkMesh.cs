using System;
using ModernGL;

public class ChunkMesh : BaseMesh
{
    private Chunk chunk;

    public ChunkMesh(Chunk chunk)
        //: base(ref chunk.app.ctx, chunk.world.program, "3u1 1u1 1u1", ["in_position", "voxel_id", "face_id"])
        //: base(ref chunk.app.ctx, chunk.world.program, "3u1 1u1 1u1 1u1 1u1", ["in_position", "voxel_id", "face_id", "ao_id", "flip_id"])
        : base(chunk.app.ctx, chunk.world.program, "1u4", ["packed_data"])
    {
        this.chunk = chunk;
        Rebuild();
    }

    protected override object GetVertexData()
    {
        object mesh = VoxelMeshBuilder.BuildChunkMesh(
            chunk.voxels,
            vbo_format_size,
            chunk.position,
            chunk.world.voxels
        );
        //Console.WriteLine($"mesh: len {mesh.Length}, data {string.Join(", ", mesh)}");
        return mesh;
    }

}
