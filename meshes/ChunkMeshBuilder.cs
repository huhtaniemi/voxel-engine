using Microsoft.VisualBasic;
using OpenTK.Mathematics;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using static Settings;

public static class VoxelMeshBuilder
{
    public static byte[] GetAO(Vector3i localPos, Vector3i worldPos, byte[,] worldVoxels, char plane)
    {
        int x = localPos.X, y = localPos.Y, z = localPos.Z;
        int wx = worldPos.X, wy = worldPos.Y, wz = worldPos.Z;

        int a, b, c, d, e, f, g, h;

        switch (plane)
        {
            case 'Y':
                a = isVoid((x    , y, z - 1), (wx    , wy, wz - 1), worldVoxels) ? 1:0;
                b = isVoid((x - 1, y, z - 1), (wx - 1, wy, wz - 1), worldVoxels) ? 1:0;
                c = isVoid((x - 1, y, z    ), (wx - 1, wy, wz    ), worldVoxels) ? 1:0;
                d = isVoid((x - 1, y, z + 1), (wx - 1, wy, wz + 1), worldVoxels) ? 1:0;
                e = isVoid((x    , y, z + 1), (wx    , wy, wz + 1), worldVoxels) ? 1:0;
                f = isVoid((x + 1, y, z + 1), (wx + 1, wy, wz + 1), worldVoxels) ? 1:0;
                g = isVoid((x + 1, y, z    ), (wx + 1, wy, wz    ), worldVoxels) ? 1:0;
                h = isVoid((x + 1, y, z - 1), (wx + 1, wy, wz - 1), worldVoxels) ? 1:0;
                break;
            case 'X':
                a = isVoid((x, y    , z - 1), (wx, wy    , wz - 1), worldVoxels) ? 1:0;
                b = isVoid((x, y - 1, z - 1), (wx, wy - 1, wz - 1), worldVoxels) ? 1:0;
                c = isVoid((x, y - 1, z    ), (wx, wy - 1, wz    ), worldVoxels) ? 1:0;
                d = isVoid((x, y - 1, z + 1), (wx, wy - 1, wz + 1), worldVoxels) ? 1:0;
                e = isVoid((x, y    , z + 1), (wx, wy    , wz + 1), worldVoxels) ? 1:0;
                f = isVoid((x, y + 1, z + 1), (wx, wy + 1, wz + 1), worldVoxels) ? 1:0;
                g = isVoid((x, y + 1, z    ), (wx, wy + 1, wz    ), worldVoxels) ? 1:0;
                h = isVoid((x, y + 1, z - 1), (wx, wy + 1, wz - 1), worldVoxels) ? 1:0;
                break;
            default: // 'Z'
                a = isVoid((x - 1, y    , z), (wx - 1, wy    , wz), worldVoxels) ? 1:0;
                b = isVoid((x - 1, y - 1, z), (wx - 1, wy - 1, wz), worldVoxels) ? 1:0;
                c = isVoid((x    , y - 1, z), (wx    , wy - 1, wz), worldVoxels) ? 1:0;
                d = isVoid((x + 1, y - 1, z), (wx + 1, wy - 1, wz), worldVoxels) ? 1:0;
                e = isVoid((x + 1, y    , z), (wx + 1, wy    , wz), worldVoxels) ? 1:0;
                f = isVoid((x + 1, y + 1, z), (wx + 1, wy + 1, wz), worldVoxels) ? 1:0;
                g = isVoid((x    , y + 1, z), (wx    , wy + 1, wz), worldVoxels) ? 1:0;
                h = isVoid((x - 1, y + 1, z), (wx - 1, wy + 1, wz), worldVoxels) ? 1:0;
                break;
        }

        return [(byte)(a + b + c), (byte)(g + h + a), (byte)(e + f + g), (byte)(c + d + e)];
    }

    /*
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
    */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetChunkIndex(Vector3i worldVoxelPos)
    {
        int wx = worldVoxelPos.X,
            wy = worldVoxelPos.Y,
            wz = worldVoxelPos.Z;
        int cx = wx / Settings.CHUNK_SIZE;
        int cy = wy / Settings.CHUNK_SIZE;
        int cz = wz / Settings.CHUNK_SIZE;
        if (!(0 <= cx && cx < Settings.WORLD_W && 0 <= cy && cy < Settings.WORLD_H && 0 <= cz && cz < Settings.WORLD_D))
            return -1;

        int index = cx + Settings.WORLD_W * cz + Settings.WORLD_AREA * cy;
        return index;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int MathMod(int a, int b)
    {
        return (Math.Abs(a * b) + a) % b;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int PythonModulo(int a, int b)
    {
        int mod = a % b;
        if ((mod < 0 && b > 0) || (mod > 0 && b < 0))
            mod += b;
        return mod;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool isVoid(Vector3i localVoxelPos, Vector3i worldVoxelPos, byte[,] worldVoxels)
    {
        int chunkIndex = GetChunkIndex(worldVoxelPos);
        if (chunkIndex == -1)
            return false;

        var (x, y, z) = localVoxelPos;
        int voxelIndex =
            MathMod(x, Settings.CHUNK_SIZE) +
            MathMod(z, Settings.CHUNK_SIZE) * Settings.CHUNK_SIZE +
            MathMod(y, Settings.CHUNK_SIZE) * Settings.CHUNK_AREA;

        return worldVoxels[chunkIndex, voxelIndex] == 0;
    }

    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool isVoid(Vector3i localVoxelPos, Vector3i worldVoxelPos, byte[,] chunkVoxels)
    {
        (var x, var y, var z) = localVoxelPos;
        if ((x is >= 0 and < CHUNK_SIZE) && (y is >= 0 and < CHUNK_SIZE) && (z is >= 0 and < CHUNK_SIZE))
            return chunkVoxels[x + CHUNK_SIZE * z + CHUNK_AREA * y] == 0;
        return true;
    }
    */

    record struct packedVertex(int x, int y, int z, byte voxelId, byte face, byte ao_id, bool flip_id)
    {
        public int x { get; init; } = x;
        public int y { get; init; } = y;
        public int z { get; init; } = z;
        public byte voxelId { get; init; } = voxelId;
        public byte face { get; init; } = face;
        public byte ao_id { get; init; } = ao_id;
        public bool flip_id { get; init; } = flip_id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int addData(byte[] vertexData, int index, packedVertex[] vertices)
    {
        foreach (var vertex in vertices)
        {
            vertexData[index + 0] = (byte)vertex.x;
            vertexData[index + 1] = (byte)vertex.y;
            vertexData[index + 2] = (byte)vertex.z;
            vertexData[index + 3] = vertex.voxelId;
            vertexData[index + 4] = vertex.face;
            vertexData[index + 5] = vertex.ao_id;
            vertexData[index + 6] = (byte)(vertex.flip_id ? 1 : 0);
            index += 7;
        }
        return index;
    }


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


    public static object BuildChunkMesh(byte[] chunkVoxels, int formatSize,Vector3i chunkPos, byte[,] worldVoxels)
    {
        byte[] vertexData = new byte[CHUNK_VOL * 18 * formatSize * sizeof(byte)]; // asuming bytesize for vbo_data
        int index = 0;

        for (byte x = 0; x < CHUNK_SIZE; x++)
        {
            for (byte y = 0; y < CHUNK_SIZE; y++)
            {
                for (byte z = 0; z < CHUNK_SIZE; z++)
                {
                    byte voxelId = chunkVoxels[x + CHUNK_SIZE * z + CHUNK_AREA * y];
                    if (voxelId == 0)
                        continue;

                    // Voxel world position
                    var (cx, cy, cz) = chunkPos;
                    int wx = x + cx * CHUNK_SIZE;
                    int wy = y + cy * CHUNK_SIZE;
                    int wz = z + cz * CHUNK_SIZE;

                    // Top face
                    if (isVoid((x, y + 1, z), (wx, wy + 1, wz), worldVoxels))
                    {
                        var ao = GetAO((x, y + 1, z), (wx, wy + 1, wz), worldVoxels, 'Y');
                        var flip_id = ao[1] + ao[3] > ao[0] + ao[2];
                        packedVertex[] v =
                        [
                            new(x    , y + 1, z    , voxelId, 0, ao[0], flip_id),
                            new(x + 1, y + 1, z    , voxelId, 0, ao[1], flip_id),
                            new(x + 1, y + 1, z + 1, voxelId, 0, ao[2], flip_id),
                            new(x    , y + 1, z + 1, voxelId, 0, ao[3], flip_id)
                        ];
                        index = addData(vertexData, index,
                            flip_id
                            ? [v[1], v[0], v[3], v[1], v[3], v[2]]
                            : [v[0], v[3], v[2], v[0], v[2], v[1]]);
                    }

                    // Bottom face
                    if (isVoid((x, y - 1, z), (wx, wy - 1, wz), worldVoxels))
                    {
                        var ao = GetAO((x, y - 1, z), (wx, wy - 1, wz), worldVoxels, 'Y');
                        var flip_id = ao[1] + ao[3] > ao[0] + ao[2];
                        packedVertex[] v =
                        [
                            new(x    , y, z    , voxelId, 1, ao[0], flip_id),
                            new(x + 1, y, z    , voxelId, 1, ao[1], flip_id),
                            new(x + 1, y, z + 1, voxelId, 1, ao[2], flip_id),
                            new(x    , y, z + 1, voxelId, 1, ao[3], flip_id)
                        ];
                        index = addData(vertexData, index,
                            flip_id
                            ? [v[1], v[3], v[0], v[1], v[2], v[3]]
                            : [v[0], v[2], v[3], v[0], v[1], v[2]]);
                    }

                    // Right face
                    if (isVoid((x + 1, y, z), (wx + 1, wy, wz), worldVoxels))
                    {
                        var ao = GetAO((x + 1, y, z), (wx + 1, wy, wz), worldVoxels, 'X');
                        var flip_id = ao[1] + ao[3] > ao[0] + ao[2];
                        packedVertex[] v =
                        [
                            new(x + 1, y    , z    , voxelId, 2, ao[0], flip_id),
                            new(x + 1, y + 1, z    , voxelId, 2, ao[1], flip_id),
                            new(x + 1, y + 1, z + 1, voxelId, 2, ao[2], flip_id),
                            new(x + 1, y    , z + 1, voxelId, 2, ao[3], flip_id)
                        ];
                        index = addData(vertexData, index,
                            flip_id
                            ? [v[3], v[0], v[1], v[3], v[1], v[2]]
                            : [v[0], v[1], v[2], v[0], v[2], v[3]]);
                    }

                    // Left face
                    if (isVoid((x - 1, y, z), (wx - 1, wy, wz), worldVoxels))
                    {
                        var ao = GetAO((x - 1, y, z), (wx - 1, wy, wz), worldVoxels, 'X');
                        var flip_id = ao[1] + ao[3] > ao[0] + ao[2];
                        packedVertex[] v =
                        [
                            new(x, y    , z    , voxelId, 3, ao[0], flip_id),
                            new(x, y + 1, z    , voxelId, 3, ao[1], flip_id),
                            new(x, y + 1, z + 1, voxelId, 3, ao[2], flip_id),
                            new(x, y    , z + 1, voxelId, 3, ao[3], flip_id)
                        ];
                        index = addData(vertexData, index,
                            flip_id
                            ? [v[3], v[1], v[0], v[3], v[2], v[1]]
                            : [v[0], v[2], v[1], v[0], v[3], v[2]]);
                    }

                    // Back face
                    if (isVoid((x, y, z - 1), (wx, wy, wz - 1), worldVoxels))
                    {
                        var ao = GetAO((x, y, z - 1), (wx, wy, wz - 1), worldVoxels, 'Z');
                        var flip_id = ao[1] + ao[3] > ao[0] + ao[2];
                        packedVertex[] v =
                        [
                            new(x    , y    , z, voxelId, 4, ao[0], flip_id),
                            new(x    , y + 1, z, voxelId, 4, ao[1], flip_id),
                            new(x + 1, y + 1, z, voxelId, 4, ao[2], flip_id),
                            new(x + 1, y    , z, voxelId, 4, ao[3], flip_id)
                        ];
                        index = addData(vertexData, index,
                            flip_id
                            ? [v[3], v[0], v[1], v[3], v[1], v[2]]
                            : [v[0], v[1], v[2], v[0], v[2], v[3]]);
                    }

                    // Front face
                    if (isVoid((x, y, z + 1), (wx, wy, wz + 1), worldVoxels))
                    {
                        var ao = GetAO((x, y, z + 1), (wx, wy, wz + 1), worldVoxels, 'Z');
                        var flip_id = ao[1] + ao[3] > ao[0] + ao[2];
                        packedVertex[] v =
                        [
                            new(x    , y    , z + 1, voxelId, 5, ao[0], flip_id),
                            new(x    , y + 1, z + 1, voxelId, 5, ao[1], flip_id),
                            new(x + 1, y + 1, z + 1, voxelId, 5, ao[2], flip_id),
                            new(x + 1, y    , z + 1, voxelId, 5, ao[3], flip_id)
                        ];
                        index = addData(vertexData, index,
                            flip_id
                            ? [v[3], v[1], v[0], v[3], v[2], v[1]]
                            : [v[0], v[2], v[1], v[0], v[3], v[2]]);
                    }
                }
            }
        }

        return vertexData.Take(index).ToArray();
        //return vertexData.Take(index).Select(v => (float)v).ToArray();
    }

}