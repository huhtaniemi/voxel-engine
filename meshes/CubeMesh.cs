using System;
using ModernGL;

public class CubeMesh : BaseMesh
{
    public CubeMesh(VoxelMarker voxel_marker, glContext.Program program)
        : base(program.ctx, program, "2f2 3f2", ["in_tex_coord_0", "in_position"])
    {
        Rebuild();
    }

    private static (float x, float y, float z)[] GetData((float x, float y, float z)[] vertices, (int a, int b, int c)[] indices)
    {
        return [.. indices
            .SelectMany(idx => new[] { vertices[idx.a], vertices[idx.b], vertices[idx.c] })
            //.Select(vtx => ((Half)vtx.x, (Half)vtx.y, (Half)vtx.z ))
        ];
    }
    private static (float x, float y)[] GetData((float x, float y)[] vertices, (int a, int b, int c)[] indices)
    {
        return [.. indices
            .SelectMany(idx => new[] { vertices[idx.a], vertices[idx.b], vertices[idx.c] })
        ];
    }

    protected override object GetVertexData()
    {
        (float, float, float)[] vertices =
        [
            (0, 0, 1), (1, 0, 1), (1, 1, 1), (0, 1, 1),
            (0, 1, 0), (0, 0, 0), (1, 0, 0), (1, 1, 0)
        ];
        (int, int, int)[] indices =
        [
            (0, 2, 3), (0, 1, 2),
            (1, 7, 2), (1, 6, 7),
            (6, 5, 4), (4, 7, 6),
            (3, 4, 5), (3, 5, 0),
            (3, 7, 4), (3, 2, 7),
            (0, 6, 1), (0, 5, 6)
        ];
        var vertex_data = GetData(vertices, indices);

        (float, float)[] texCoordVertices =
        [
            (0, 0), (1, 0), (1, 1), (0, 1)
        ];
        (int, int, int)[] texCoordIndices =
        [
            (0, 2, 3), (0, 1, 2),
            (0, 2, 3), (0, 1, 2),
            (0, 1, 2), (2, 3, 0),
            (2, 3, 0), (2, 0, 1),
            (0, 2, 3), (0, 1, 2),
            (3, 1, 2), (3, 0, 1),
        ];
        var tex_coord_data = GetData(texCoordVertices, texCoordIndices);

        return (Half[])[.. tex_coord_data
            .Zip(vertex_data, (tcrd, vtx) => new[] { tcrd.x, tcrd.y, vtx.x, vtx.y, vtx.z})
            .SelectMany(data => data.Select(v => (Half)v))
        ];
    }
}

