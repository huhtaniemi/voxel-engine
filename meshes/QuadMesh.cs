using System;
using ModernGL;

public class QuadMesh : BaseMesh
{
    public QuadMesh(Water water)
        : base(water.app.ctx, water.program, "2u1 3u1", ["in_tex_coord", "in_position"])
        //: base(quad.ctx, quad.program, "3f 3f", ["in_position", "in_color"])
    {
        Rebuild();
    }

    protected override object GetVertexData()
    {
        (byte x, byte y, byte z)[] vertices =
        [
            (0, 0, 0), (1, 0, 1), (1, 0, 0),
            (0, 0, 0), (0, 0, 1), (1, 0, 1)
        ];
        (byte x, byte y)[] tex_coords =
        [
            (0, 0), (1, 1), (1, 0),
            (0, 0), (0, 1), (1, 1)
        ];

        return (byte[])[.. tex_coords
            .Zip(vertices, (tcrd, vtx) => new[] { tcrd.x, tcrd.y, vtx.x, vtx.y, vtx.z})
            .SelectMany(v => v)
        ];

        /*
        var vertices = new (double, double, double)[]
        {
            (0.5, 0.5, 0.0), (-0.5, 0.5, 0.0), (-0.5, -0.5, 0.0),
            (0.5, 0.5, 0.0), (-0.5, -0.5, 0.0), (0.5, -0.5, 0.0)
        };
        var colors = new (double, double, double)[]
        {
            (0, 1, 0), (1, 0, 0), (1, 1, 0),
            (0, 1, 0), (1, 1, 0), (0, 0, 1)
        };

        var vertex_data = // np.hstack([vertices, colors], dtype='float32')
            vertices.Zip(colors, (v, c) => new float[] {
                (float)v.Item1, (float)v.Item2, (float)v.Item3, (float)c.Item1, (float)c.Item2, (float)c.Item3 })
            .SelectMany(x => x)
            .ToArray();

        return vertex_data;
        */
    }
}


