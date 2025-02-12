using OpenTK.Graphics.OpenGL4;
using System.Linq;

public class ChunkMesh : BaseMesh
{
    private VoxelEngine app;
    private Chunk chunk;
    private int formatSize;

    /// <summary>
    /// Initializes the ChunkMesh class with a reference to the Chunk instance, sets the shader program, VBO format, format size, and attribute names, and creates the VAO.
    /// </summary>
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

    /// <summary>
    /// Builds the chunk mesh and converts the vertex data to a float array.
    /// </summary>
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

    /// <summary>
    /// Creates and initializes the VBO and VAO, sets up the vertex attributes, and returns the VAO.
    /// </summary>
    protected int GetVAO()
    {
        float[] vertexData = GetVertexData();
        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int stride = CalculateStride(vboFormat);
        int offset = 0;

        for (int i = 0; i < attrs.Length; i++)
        {
            int location = GL.GetAttribLocation(program, attrs[i]);
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, GetAttributeSize(vboFormat, i), VertexAttribPointerType.Float, false, stride, offset);
            offset += GetAttributeSize(vboFormat, i) * sizeof(float);
        }

        GL.BindVertexArray(0);
        return vao;
    }
}
