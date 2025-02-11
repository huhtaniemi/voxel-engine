using System;
using OpenTK.Mathematics;

public static class TerrainGen
{
    private static readonly Random random = new Random();

    /// <summary>
    /// This method calculates the height of the terrain at a given (x, z) position using noise functions and an island mask.
    /// </summary>
    public static int GetHeight(int x, int z)
    {
        // island mask
        float island = 1 / (MathF.Pow(0.0025f * MathF.Sqrt(MathF.Pow(x - Settings.CENTER_XZ, 2) + MathF.Pow(z - Settings.CENTER_XZ, 2)), 20) + 0.0001f);
        island = MathF.Min(island, 1);

        // amplitude
        float a1 = Settings.CENTER_Y;
        float a2 = a1 * 0.5f, a4 = a1 * 0.25f, a8 = a1 * 0.125f;

        // frequency
        float f1 = 0.005f;
        float f2 = f1 * 2, f4 = f1 * 4, f8 = f1 * 8;

        if (Noise.Noise2(0.1f * x, 0.1f * z) < 0)
        {
            a1 /= 1.07f;
        }

        float height = 0;
        height += Noise.Noise2(x * f1, z * f1) * a1 + a1;
        height += Noise.Noise2(x * f2, z * f2) * a2 - a2;
        height += Noise.Noise2(x * f4, z * f4) * a4 + a4;
        height += Noise.Noise2(x * f8, z * f8) * a8 - a8;

        height = MathF.Max(height, Noise.Noise2(x * f8, z * f8) + 2);
        height *= island;

        return (int)height;
    }

    /// <summary>
    /// This method calculates the index of a voxel in the voxel array based on its (x, y, z) position.
    /// </summary>
    public static int GetIndex(int x, int y, int z)
    {
        return x + Settings.CHUNK_SIZE * z + Settings.CHUNK_AREA * y;
    }

    /// <summary>
    /// This method sets the voxel ID based on the terrain height and other conditions, and it also calls PlaceTree to place trees if conditions are met.
    /// </summary>
    public static void SetVoxelId(ref byte[] voxels, int x, int y, int z, int wx, int wy, int wz, int worldHeight)
    {
        byte voxelId = 0;

        if (wy < worldHeight - 1)
        {
            // create caves
            if (Noise.Noise3(wx * 0.09f, wy * 0.09f, wz * 0.09f) > 0 &&
                Noise.Noise2(wx * 0.1f, wz * 0.1f) * 3 + 3 < wy && wy < worldHeight - 10)
            {
                voxelId = 0;
            }
            else
            {
                voxelId = Settings.STONE;
            }
        }
        else
        {
            int rng = random.Next(7);
            int ry = wy - rng;
            if (Settings.SNOW_LVL <= ry && ry < worldHeight)
            {
                voxelId = Settings.SNOW;
            }
            else if (Settings.STONE_LVL <= ry && ry < Settings.SNOW_LVL)
            {
                voxelId = Settings.STONE;
            }
            else if (Settings.DIRT_LVL <= ry && ry < Settings.STONE_LVL)
            {
                voxelId = Settings.DIRT;
            }
            else if (Settings.GRASS_LVL <= ry && ry < Settings.DIRT_LVL)
            {
                voxelId = Settings.GRASS;
            }
            else
            {
                voxelId = Settings.SAND;
            }
        }

        // setting ID
        voxels[GetIndex(x, y, z)] = voxelId;

        // place tree
        if (wy < Settings.DIRT_LVL)
        {
            PlaceTree(ref voxels, x, y, z, voxelId);
        }
    }

    /// <summary>
    /// This method places a tree at the specified position if conditions are met, including placing dirt under the tree, leaves, and the tree trunk.
    /// </summary>
    private static void PlaceTree(ref byte[] voxels, int x, int y, int z, byte voxelId)
    {
        if (voxelId != Settings.GRASS || random.NextDouble() > Settings.TREE_PROBABILITY)
        {
            return;
        }
        if (y + Settings.TREE_HEIGHT >= Settings.CHUNK_SIZE)
        {
            return;
        }
        if (x - Settings.TREE_H_WIDTH < 0 || x + Settings.TREE_H_WIDTH >= Settings.CHUNK_SIZE)
        {
            return;
        }
        if (z - Settings.TREE_H_WIDTH < 0 || z + Settings.TREE_H_WIDTH >= Settings.CHUNK_SIZE)
        {
            return;
        }

        // dirt under the tree
        voxels[GetIndex(x, y, z)] = Settings.DIRT;

        // leaves
        int m = 0;
        for (int n = 0, iy = Settings.TREE_H_HEIGHT; iy < Settings.TREE_HEIGHT - 1; iy++, n++)
        {
            int k = iy % 2;
            int rng = random.Next(2);
            for (int ix = -Settings.TREE_H_WIDTH + m; ix < Settings.TREE_H_WIDTH - m * rng; ix++)
            {
                for (int iz = -Settings.TREE_H_WIDTH + m * rng; iz < Settings.TREE_H_WIDTH - m; iz++)
                {
                    if ((ix + iz) % 4 != 0)
                    {
                        voxels[GetIndex(x + ix + k, y + iy, z + iz + k)] = Settings.LEAVES;
                    }
                }
            }
            m += n > 0 ? 1 : n > 1 ? 3 : 0;
        }

        // tree trunk
        for (int iy = 1; iy < Settings.TREE_HEIGHT - 2; iy++)
        {
            voxels[GetIndex(x, y + iy, z)] = Settings.WOOD;
        }

        // top
        voxels[GetIndex(x, y + Settings.TREE_HEIGHT - 2, z)] = Settings.LEAVES;
    }
}
