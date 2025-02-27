using System;
using ModernGL;

public class CloudMesh : BaseMesh
{
    public CloudMesh(Clouds cloud)
        : base(cloud.app.ctx, cloud.program, "3u2", ["in_position"])
    {
        Rebuild();
    }

    protected override object GetVertexData()
    {
        byte[] cloud_data = new byte[Settings.WORLD_AREA * Settings.CHUNK_SIZE * Settings.CHUNK_SIZE];
        GenClouds(cloud_data);

        return BuildMesh(cloud_data);
    }

    private static void GenClouds(byte[] cloud_data)
    {
        for (int x = 0; x < Settings.WORLD_W * Settings.CHUNK_SIZE; x++)
        {
            for (int z = 0; z < Settings.WORLD_D * Settings.CHUNK_SIZE; z++)
            {
                if (VoxelEngine._noise.Evaluate(0.13 * x, 0.13 * z) < 0.2)
                {
                    continue;
                }
                cloud_data[x + Settings.WORLD_W * Settings.CHUNK_SIZE * z] = 1;
            }
        }
    }

    private static ushort[] BuildMesh(byte[] cloud_data)
    {
        var mesh = new ushort[Settings.WORLD_AREA * Settings.CHUNK_AREA * 6 * 3];
        var index = 0;

        int width = Settings.WORLD_W * Settings.CHUNK_SIZE;
        int depth = Settings.WORLD_D * Settings.CHUNK_SIZE;

        int y = Settings.CLOUD_HEIGHT;
        HashSet<int> visited = new HashSet<int>();

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = x + width * z;
                if (cloud_data[idx] == 0 || visited.Contains(idx))
                {
                    continue;
                }

                int x_count = 1;
                idx = (x + x_count) + width * z;
                while (x + x_count < width && cloud_data[idx] == 1 && !visited.Contains(idx))
                {
                    x_count++;
                    idx = (x + x_count) + width * z;
                }

                List<int> z_count_list = new List<int>();
                for (int ix = 0; ix < x_count; ix++)
                {
                    int z_count = 1;
                    idx = (x + ix) + width * (z + z_count);
                    while ((z + z_count) < depth && cloud_data[idx] == 1 && !visited.Contains(idx))
                    {
                        z_count++;
                        idx = (x + ix) + width * (z + z_count);
                    }
                    z_count_list.Add(z_count);
                }

                int z_count_min = z_count_list.Any() ? z_count_list.Min() : 1;

                for (int ix = 0; ix < x_count; ix++)
                {
                    for (int iz = 0; iz < z_count_min; iz++)
                    {
                        visited.Add((x + ix) + width * (z + iz));
                    }
                }

                var v0 = (x          , y, z              );
                var v1 = (x + x_count, y, z + z_count_min);
                var v2 = (x + x_count, y, z              );
                var v3 = (x          , y, z + z_count_min);

                foreach (var vertex in new[] { v0, v1, v2, v0, v3, v1 })
                {
                    mesh[index + 0] = (ushort)vertex.Item1;
                    mesh[index + 1] = (ushort)vertex.Item2;
                    mesh[index + 2] = (ushort)vertex.Item3;
                    index += 3;
                }
            }
        }

        return mesh.Take(index).ToArray();
    }
}

