using OpenTK.Graphics.OpenGL4;
using System.Linq;

public class ChunkMesh : BaseMesh
{
    private Chunk chunk;

    public ChunkMesh(Chunk chunk)
        : base(ref chunk.app.ctx, ref chunk.app.shader_program.chunk, "3u1 1u1 1u1", ["in_position", "voxel_id", "face_id"])
    {
        this.chunk = chunk;
        Rebuild();
    }

    protected override object GetVertexData()
    {
        byte[] mesh = VoxelMeshBuilder.BuildChunkMesh(
            chunk.voxels,
            vbo_format_size//,
            //chunk.Position,
            //chunk.world.voxels
        );
        return mesh;
    }

}
