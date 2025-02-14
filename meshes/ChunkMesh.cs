using OpenTK.Graphics.OpenGL4;
using System.Linq;

public class ChunkMesh : BaseMesh
{
    private VoxelEngine app;
    private Chunk chunk;
    private int formatSize;

    public ChunkMesh(Chunk chunk) : base()
    {
        this.app = chunk.app;
        this.chunk = chunk;
        this.program = app.shader_program.chunkProgram;

        this.vboFormat = "1u4";
        this.formatSize = vboFormat.Split(' ').Sum(fmt => int.Parse(fmt.Substring(0, 1)));
        this.attrs = new[] { "packed_data" };
        this.vao = GetVAO();
    }

    public void Rebuild()
    {
        this.vao = GetVAO();
    }

    protected override float[] GetVertexData()
    {
        uint[] mesh = VoxelMeshBuilder.BuildChunkMesh(
            chunk.voxels,
            formatSize,
            chunk.Position,
            chunk.world.voxels
        );
        return mesh.Select(v => (float)v).ToArray();
    }

}
