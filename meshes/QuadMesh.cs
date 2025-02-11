using OpenTK.Graphics.OpenGL4;
using System.Linq;

public class QuadMesh : BaseMesh
{
    private VoxelEngine app;

    public QuadMesh(VoxelEngine app) : base()
    {
        this.app = app;
        this.program = app.shader_program.waterProgram;

        this.vboFormat = "2u1 3u1";
        this.attrs = ["in_tex_coord", "in_position"];
        this.vao = this.GetVAO();
    }

    /// <summary>
    /// two triangles with counterclockwise vertex traversal,
    /// joined into one array with float32 type.
    /// </summary>
    protected override float[] GetVertexData()
    {
        var vertices = new[] // uint8
        {
            (0f, 0f, 0f), (1f, 0f, 1f), (1f, 0f, 0f),
            (0f, 0f, 0f), (0f, 0f, 1f), (1f, 0f, 1f)
        };

        var tex_coords = new[]  // uint8
        {
            (0f, 0f), (1f, 1f), (1f, 0f),
            (0f, 0f), (0f, 1f), (1f, 1f)
        };
        
        var vertex_data = // np.hstack([tex_coords, vertices])
            tex_coords
            .SelectMany(tc => new[] { tc.Item1, tc.Item2 })
            .Concat(vertices.SelectMany(v => new[] { v.Item1, v.Item2, v.Item3 }))
            .ToArray();

        return vertex_data;
    }
}


