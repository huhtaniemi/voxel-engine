using Microsoft.VisualBasic;
using OpenTK.Mathematics;
using System;
using System.Linq;

public static class VoxelMeshBuilder
{
    /*

    /// <summary>
    /// Calculates the ambient occlusion (AO) values for a voxel based on its neighbors.
    /// </summary>
    public static Vector4h GetAO(Vector3i localPos, Vector3i worldPos, byte[,,] worldVoxels, char plane)
    {
        int x = localPos.X, y = localPos.Y, z = localPos.Z;
        int wx = worldPos.X, wy = worldPos.Y, wz = worldPos.Z;

        bool a, b, c, d, e, f, g, h;

        switch (plane)
        {
            case 'Y':
                a = IsVoid(new Vector3i(x, y, z - 1), new Vector3i(wx, wy, wz - 1), worldVoxels);
                b = IsVoid(new Vector3i(x - 1, y, z - 1), new Vector3i(wx - 1, wy, wz - 1), worldVoxels);
                c = IsVoid(new Vector3i(x - 1, y, z), new Vector3i(wx - 1, wy, wz), worldVoxels);
                d = IsVoid(new Vector3i(x - 1, y, z + 1), new Vector3i(wx - 1, wy, wz + 1), worldVoxels);
                e = IsVoid(new Vector3i(x, y, z + 1), new Vector3i(wx, wy, wz + 1), worldVoxels);
                f = IsVoid(new Vector3i(x + 1, y, z + 1), new Vector3i(wx + 1, wy, wz + 1), worldVoxels);
                g = IsVoid(new Vector3i(x + 1, y, z), new Vector3i(wx + 1, wy, wz), worldVoxels);
                h = IsVoid(new Vector3i(x + 1, y, z - 1), new Vector3i(wx + 1, wy, wz - 1), worldVoxels);
                break;
            case 'X':
                a = IsVoid(new Vector3i(x, y, z - 1), new Vector3i(wx, wy, wz - 1), worldVoxels);
                b = IsVoid(new Vector3i(x, y - 1, z - 1), new Vector3i(wx, wy - 1, wz - 1), worldVoxels);
                c = IsVoid(new Vector3i(x, y - 1, z), new Vector3i(wx, wy - 1, wz), worldVoxels);
                d = IsVoid(new Vector3i(x, y - 1, z + 1), new Vector3i(wx, wy - 1, wz + 1), worldVoxels);
                e = IsVoid(new Vector3i(x, y, z + 1), new Vector3i(wx, wy, wz + 1), worldVoxels);
                f = IsVoid(new Vector3i(x, y + 1, z + 1), new Vector3i(wx, wy + 1, wz + 1), worldVoxels);
                g = IsVoid(new Vector3i(x, y + 1, z), new Vector3i(wx, wy + 1, wz), worldVoxels);
                h = IsVoid(new Vector3i(x, y + 1, z - 1), new Vector3i(wx, wy + 1, wz - 1), worldVoxels);
                break;
            default: // 'Z'
                a = IsVoid(new Vector3i(x - 1, y, z), new Vector3i(wx - 1, wy, wz), worldVoxels);
                b = IsVoid(new Vector3i(x - 1, y - 1, z), new Vector3i(wx - 1, wy - 1, wz), worldVoxels);
                c = IsVoid(new Vector3i(x, y - 1, z), new Vector3i(wx, wy - 1, wz), worldVoxels);
                d = IsVoid(new Vector3i(x + 1, y - 1, z), new Vector3i(wx + 1, wy - 1, wz), worldVoxels);
                e = IsVoid(new Vector3i(x + 1, y, z), new Vector3i(wx + 1, wy, wz), worldVoxels);
                f = IsVoid(new Vector3i(x + 1, y + 1, z), new Vector3i(wx + 1, wy + 1, wz), worldVoxels);
                g = IsVoid(new Vector3i(x, y + 1, z), new Vector3i(wx, wy + 1, wz), worldVoxels);
                h = IsVoid(new Vector3i(x - 1, y + 1, z), new Vector3i(wx - 1, wy + 1, wz), worldVoxels);
                break;
        }

        Vector4h ao;// = new((a + b + c), (g + h + a), (e + f + g), (c + d + e)));
        //uint ao = (uint)((a ? 1 : 0) + (b ? 1 : 0) + (c ? 1 : 0)), (g ? 1 : 0) + (h ? 1 : 0) + (a ? 1 : 0), (e ? 1 : 0) + (f ? 1 : 0) + (g ? 1 : 0), (c ? 1 : 0) + (d ? 1 : 0) + (e ? 1 : 0);
        return ao;
    }

    public static uint PackData(int x, int y, int z, byte voxelId, byte faceId, int aoId, byte flipId)
    {
        uint a = (uint)x, b = (uint)y, c = (uint)z, d = voxelId, e = faceId, f = (uint)aoId, g = flipId;

        int bBit = 6, cBit = 6, dBit = 8, eBit = 3, fBit = 2, gBit = 1;
        int fgBit = fBit + gBit;
        int efgBit = eBit + fgBit;
        int defgBit = dBit + efgBit;
        int cdefgBit = cBit + defgBit;
        int bcdefgBit = bBit + cdefgBit;

        uint packedData
            = (a << bcdefgBit)
            | (b << cdefgBit)
            | (c << defgBit)
            | (d << efgBit)
            | (e << fgBit)
            | (f << gBit)
            | g;
        return packedData;
    }

    /// <summary>
    /// Calculates the chunk index based on the world voxel position.
    /// </summary>
    public static int GetChunkIndex(Vector3i worldVoxelPos)
    {
        int wx = worldVoxelPos.X,
            wy = worldVoxelPos.Y,
            wz = worldVoxelPos.Z;
        int cx = wx / Settings.CHUNK_SIZE;
        int cy = wy / Settings.CHUNK_SIZE;
        int cz = wz / Settings.CHUNK_SIZE;
        if (!(0 <= cx && cx < Settings.WORLD_W && 0 <= cy && cy < Settings.WORLD_H && 0 <= cz && cz < Settings.WORLD_D))
        {
            return -1;
        }

        int index = cx + Settings.WORLD_W * cz + Settings.WORLD_AREA * cy;
        return index;
    }
    */
    /*
    public static bool IsVoid(Vector3i localVoxelPos, Vector3i worldVoxelPos, byte[,,] worldVoxels)
    {
        int chunkIndex = GetChunkIndex(worldVoxelPos);
        if (chunkIndex == -1)
            return false;
        byte[] chunkVoxels = worldVoxels[chunkIndex];

        int x = localVoxelPos.X, y = localVoxelPos.Y, z = localVoxelPos.Z;
        int voxelIndex =
            x % Settings.CHUNK_SIZE +
            z % Settings.CHUNK_SIZE * Settings.CHUNK_SIZE +
            y % Settings.CHUNK_SIZE * Settings.CHUNK_AREA;

        return chunkVoxels[voxelIndex] == 0;
    }
    */


    /*
    public static int AddData(uint[] vertexData, int index, params uint[] vertices)
    {
        foreach (uint vertex in vertices)
        {
            vertexData[index] = vertex;
            index++;
        }
        return index;
    }
    */



    /*
    public static uint[] BuildChunkMesh(byte[] chunkVoxels, int formatSize, Vector3i chunkPos, byte[,,] worldVoxels)
    {
        uint[] vertexData = new uint[Settings.CHUNK_VOL * 18 * formatSize];
        int index = 0;

        for (int x = 0; x < Settings.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Settings.CHUNK_SIZE; y++)
            {
                for (int z = 0; z < Settings.CHUNK_SIZE; z++)
                {
                    byte voxelId = chunkVoxels[x + Settings.CHUNK_SIZE * z + Settings.CHUNK_AREA * y];

                    if (voxelId == 0)
                    {
                        continue;
                    }

                    // Voxel world position
                    int cx = chunkPos.X, cy = chunkPos.Y, cz = chunkPos.Z;
                    int wx = x + cx * Settings.CHUNK_SIZE;
                    int wy = y + cy * Settings.CHUNK_SIZE;
                    int wz = z + cz * Settings.CHUNK_SIZE;

                    // Top face
                    if (IsVoid(new Vector3i(x, y + 1, z), new Vector3i(wx, wy + 1, wz), worldVoxels))
                    {
                        Vector4h ao = GetAO(new Vector3i(x, y + 1, z), new Vector3i(wx, wy + 1, wz), worldVoxels, 'Y');
                        bool flipId = ao[1] + ao[3] > ao[0] + ao[2];

                        // Format: x, y, z, voxel_id, face_id, ao_id, flip_id
                        uint v0 = PackData(x, y + 1, z, voxelId, 0, ao[0], flipId ? (byte)1 : (byte)0);
                        uint v1 = PackData(x + 1, y + 1, z, voxelId, 0, ao[1], flipId ? (byte)1 : (byte)0);
                        uint v2 = PackData(x + 1, y + 1, z + 1, voxelId, 0, ao[2], flipId ? (byte)1 : (byte)0);
                        uint v3 = PackData(x, y + 1, z + 1, voxelId, 0, ao[3], flipId ? (byte)1 : (byte)0);

                        if (flipId)
                        {
                            index = AddData(vertexData, index, v1, v0, v3, v1, v3, v2);
                        }
                        else
                        {
                            index = AddData(vertexData, index, v0, v3, v2, v0, v2, v1);
                        }
                    }

                    // Bottom face
                    if (IsVoid(new Vector3i(x, y - 1, z), new Vector3i(wx, wy - 1, wz), worldVoxels))
                    {
                        Vector4h ao = GetAO(new Vector3i(x, y - 1, z), new Vector3i(wx, wy - 1, wz), worldVoxels, 'Y');
                        bool flipId = ao[1] + ao[3] > ao[0] + ao[2];

                        uint v0 = PackData(x, y, z, voxelId, 1, ao[0], flipId ? (byte)1 : (byte)0);
                        uint v1 = PackData(x + 1, y, z, voxelId, 1, ao[1], flipId ? (byte)1 : (byte)0);
                        uint v2 = PackData(x + 1, y, z + 1, voxelId, 1, ao[2], flipId ? (byte)1 : (byte)0);
                        uint v3 = PackData(x, y, z + 1, voxelId, 1, ao[3], flipId ? (byte)1 : (byte)0);

                        if (flipId)
                        {
                            index = AddData(vertexData, index, v1, v3, v0, v1, v2, v3);
                        }
                        else
                        {
                            index = AddData(vertexData, index, v0, v2, v3, v0, v1, v2);
                        }
                    }

                    // Right face
                    if (IsVoid(new Vector3i(x + 1, y, z), new Vector3i(wx + 1, wy, wz), worldVoxels))
                    {
                        Vector4h ao = GetAO(new Vector3i(x + 1, y, z), new Vector3i(wx + 1, wy, wz), worldVoxels, 'X');
                        bool flipId = ao[1] + ao[3] > ao[0] + ao[2];

                        uint v0 = PackData(x + 1, y, z, voxelId, 2, ao[0], flipId ? (byte)1 : (byte)0);
                        uint v1 = PackData(x + 1, y + 1, z, voxelId, 2, ao[1], flipId ? (byte)1 : (byte)0);
                        uint v2 = PackData(x + 1, y + 1, z + 1, voxelId, 2, ao[2], flipId ? (byte)1 : (byte)0);
                        uint v3 = PackData(x + 1, y, z + 1, voxelId, 2, ao[3], flipId ? (byte)1 : (byte)0);

                        if (flipId)
                        {
                            index = AddData(vertexData, index, v3, v0, v1, v3, v1, v2);
                        }
                        else
                        {
                            index = AddData(vertexData, index, v0, v1, v2, v0, v2, v3);
                        }
                    }

                    // Left face
                    if (IsVoid(new Vector3i(x - 1, y, z), new Vector3i(wx - 1, wy, wz), worldVoxels))
                    {
                        Vector4h ao = GetAO(new Vector3i(x - 1, y, z), new Vector3i(wx - 1, wy, wz), worldVoxels, 'X');
                        bool flipId = ao[1] + ao[3] > ao[0] + ao[2];

                        uint v0 = PackData(x, y, z, voxelId, 3, ao[0], flipId ? (byte)1 : (byte)0);
                        uint v1 = PackData(x, y + 1, z, voxelId, 3, ao[1], flipId ? (byte)1 : (byte)0);
                        uint v2 = PackData(x, y + 1, z + 1, voxelId, 3, ao[2], flipId ? (byte)1 : (byte)0);
                        uint v3 = PackData(x, y, z + 1, voxelId, 3, ao[3], flipId ? (byte)1 : (byte)0);

                        if (flipId)
                        {
                            index = AddData(vertexData, index, v3, v1, v0, v3, v2, v1);
                        }
                        else
                        {
                            index = AddData(vertexData, index, v0, v2, v1, v0, v3, v2);
                        }
                    }

                    // Back face
                    if (IsVoid(new Vector3i(x, y, z - 1), new Vector3i(wx, wy, wz - 1), worldVoxels))
                    {
                        Vector4h ao = GetAO(new Vector3i(x, y, z - 1), new Vector3i(wx, wy, wz - 1), worldVoxels, 'Z');
                        bool flipId = ao[1] + ao[3] > ao[0] + ao[2];

                        uint v0 = PackData(x, y, z, voxelId, 4, ao[0], flipId ? (byte)1 : (byte)0);
                        uint v1 = PackData(x, y + 1, z, voxelId, 4, ao[1], flipId ? (byte)1 : (byte)0);
                        uint v2 = PackData(x + 1, y + 1, z, voxelId, 4, ao[2], flipId ? (byte)1 : (byte)0);
                        uint v3 = PackData(x + 1, y, z, voxelId, 4, ao[3], flipId ? (byte)1 : (byte)0);

                        if (flipId)
                        {
                            index = AddData(vertexData, index, v3, v0, v1, v3, v1, v2);
                        }
                        else
                        {
                            index = AddData(vertexData, index, v0, v1, v2, v0, v2, v3);
                        }
                    }

                    // Front face
                    if (IsVoid(new Vector3i(x, y, z + 1), new Vector3i(wx, wy, wz + 1), worldVoxels))
                    {
                        Vector4h ao = GetAO(new Vector3i(x, y, z + 1), new Vector3i(wx, wy, wz + 1), worldVoxels, 'Z');
                        bool flipId = ao.Y + ao.W > ao.X + ao.Z;

                        uint v0 = PackData(x, y, z + 1, voxelId, 5, ao.X, flipId ? (byte)1 : (byte)0);
                        uint v1 = PackData(x, y + 1, z + 1, voxelId, 5, ao.Y, flipId ? (byte)1 : (byte)0);
                        uint v2 = PackData(x + 1, y + 1, z + 1, voxelId, 5, ao.Z, flipId ? (byte)1 : (byte)0);
                        uint v3 = PackData(x + 1, y, z + 1, voxelId, 5, ao.W, flipId ? (byte)1 : (byte)0);

                        if (flipId)
                        {
                            index = AddData(vertexData, index, v3, v1, v0, v3, v2, v1);
                        }
                        else
                        {
                            index = AddData(vertexData, index, v0, v2, v1, v0, v3, v2);
                        }
                    }
                }
            }
        }

        return vertexData.Take(index + 1).ToArray();
        //return vertexData.[:index + 1];
    }
    */


    public static byte[] BuildChunkMesh(byte[] chunkVoxels, int formatSize/*,Vector3i chunkPos, byte[,,] worldVoxels*/)
    {
        byte[] vertexData = new byte[CHUNK_VOL * 18 * formatSize * sizeof(byte)]; // asuming bytesize for vbo_data
        int index = 0;

        //return vertexData.[:index + 1];
        return vertexData.Take(index + 1).ToArray();
    }

}