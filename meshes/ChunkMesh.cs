using OpenTK.Graphics.OpenGL4;
using System.Linq;

public class ChunkMesh : BaseMesh
{
    private Chunk chunk;

    public ChunkMesh(Chunk chunk)
        : base(ref chunk.app.ctx, ref chunk.app.shader_program.chunk, "3u1 1u1 1u1 1u1 1u1", ["in_position", "voxel_id", "face_id", "ao_id", "flip_id"])
        //: base(ref chunk.app.ctx, ref chunk.app.shader_program.chunk, "3f4", ["in_position"])
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
