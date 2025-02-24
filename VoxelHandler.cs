using OpenTK.Mathematics;
using System;
using System.Diagnostics.CodeAnalysis;

public class VoxelHandler
{
    public VoxelEngine app;
    private Chunk[] chunks;

    // Ray casting result
    private Chunk chunk;
    public byte voxelId;
    private int voxelIndex;
    private Vector3i voxelLocalPos;
    public Vector3i voxelWorldPos;
    public Vector3i voxelNormal;

    public bool interactionMode = false;  // false: remove voxel, true: add voxel
    private const byte newVoxelId = Settings.DIRT;

    public VoxelHandler(World world)
    //public VoxelHandler(VoxelEngine app, Chunk[] chunks)
    {
        this.app = world.app;
        this.chunks = world.chunks;
    }

    public void Update()
    {
        RayCast();
    }

    /*
    public void SetVoxel()
    {
        if (interactionMode)
        {
            AddVoxel();
        }
        else
        {
            RemoveVoxel();
        }
    }

    public void SwitchMode()
    {
        interactionMode = !interactionMode;
    }

    private void AddVoxel()
    {
        if (voxelId != 0)
        {
            var result = GetVoxelId(voxelWorldPos + voxelNormal);

            if (result.voxelId == 0)
            {
                result.chunk.voxels[result.voxelIndex] = newVoxelId;
                result.chunk.BuildMesh();

                if (result.chunk.IsEmpty)
                {
                    result.chunk.IsEmpty = false;
                }
            }
        }
    }

    private void RemoveVoxel()
    {
        if (voxelId != 0)
        {
            chunk.voxels[voxelIndex] = 0;
            chunk.BuildMesh();
            RebuildAdjacentChunks();
        }
    }

    private void RebuildAdjacentChunks()
    {
        int lx = voxelLocalPos.X, ly = voxelLocalPos.Y, lz = voxelLocalPos.Z;
        int wx = voxelWorldPos.X, wy = voxelWorldPos.Y, wz = voxelWorldPos.Z;

        if (lx == 0) RebuildAdjChunk(new Vector3i(wx - 1, wy, wz));
        else if (lx == Settings.CHUNK_SIZE - 1) RebuildAdjChunk(new Vector3i(wx + 1, wy, wz));

        if (ly == 0) RebuildAdjChunk(new Vector3i(wx, wy - 1, wz));
        else if (ly == Settings.CHUNK_SIZE - 1) RebuildAdjChunk(new Vector3i(wx, wy + 1, wz));

        if (lz == 0) RebuildAdjChunk(new Vector3i(wx, wy, wz - 1));
        else if (lz == Settings.CHUNK_SIZE - 1) RebuildAdjChunk(new Vector3i(wx, wy, wz + 1));
    }

    private void RebuildAdjChunk(Vector3i adjVoxelPos)
    {
        int index = GetChunkIndex(adjVoxelPos);
        if (index != -1)
        {
            chunks[index].BuildMesh();
        }
    }

    private int GetChunkIndex(Vector3i position)
    {
        int x = position.X / Settings.CHUNK_SIZE;
        int y = position.Y / Settings.CHUNK_SIZE;
        int z = position.Z / Settings.CHUNK_SIZE;
        return x + Settings.WORLD_W * z + Settings.WORLD_AREA * y;
    }
    */

    private (byte voxelId, int voxelIndex, Vector3i voxelLocalPos, Chunk chunk) GetVoxelId(Vector3i voxelWorldPos)
    {
        int cx = voxelWorldPos.X / Settings.CHUNK_SIZE;
        int cy = voxelWorldPos.Y / Settings.CHUNK_SIZE;
        int cz = voxelWorldPos.Z / Settings.CHUNK_SIZE;

        if (cx >= 0 && cx < Settings.WORLD_W && cy >= 0 && cy < Settings.WORLD_H && cz >= 0 && cz < Settings.WORLD_D)
        {
            int chunkIndex = cx + Settings.WORLD_W * cz + Settings.WORLD_AREA * cy;
            var chunk = chunks[chunkIndex];

            int lx = voxelWorldPos.X % Settings.CHUNK_SIZE;
            int ly = voxelWorldPos.Y % Settings.CHUNK_SIZE;
            int lz = voxelWorldPos.Z % Settings.CHUNK_SIZE;

            int voxelIndex = lx + Settings.CHUNK_SIZE * lz + Settings.CHUNK_AREA * ly;
            byte voxelId = chunk.voxels[voxelIndex];

            return (voxelId, voxelIndex, new Vector3i(lx, ly, lz), chunk);
        }
        return (0, 0, new Vector3i(0), null);
    }

    private void RayCast()
    {
        Vector3 start = app.player.Position;
        Vector3 end = app.player.Position + app.player.Forward * Settings.MAX_RAY_DIST;

        Vector3i currentVoxelPos = new Vector3i((int)start.X, (int)start.Y, (int)start.Z);
        voxelId = 0;
        voxelNormal = new Vector3i(0);
        int stepDir = -1;

        float dx = MathF.Sign(end.X - start.X);
        float deltaX = dx != 0 ? MathF.Min(dx / (end.X - start.X), 10000000.0f) : 10000000.0f;
        float maxX = dx > 0 ? deltaX * (1.0f - MathF.Fract(start.X)) : deltaX * MathF.Fract(start.X);

        float dy = MathF.Sign(end.Y - start.Y);
        float deltaY = dy != 0 ? MathF.Min(dy / (end.Y - start.Y), 10000000.0f) : 10000000.0f;
        float maxY = dy > 0 ? deltaY * (1.0f - MathF.Fract(start.Y)) : deltaY * MathF.Fract(start.Y);

        float dz = MathF.Sign(end.Z - start.Z);
        float deltaZ = dz != 0 ? MathF.Min(dz / (end.Z - start.Z), 10000000.0f) : 10000000.0f;
        float maxZ = dz > 0 ? deltaZ * (1.0f - MathF.Fract(start.Z)) : deltaZ * MathF.Fract(start.Z);

        while (!(maxX > 1.0f && maxY > 1.0f && maxZ > 1.0f))
        {
            var result = GetVoxelId(currentVoxelPos);
            if (result.voxelId != 0)
            {
                voxelId = result.voxelId;
                voxelIndex = result.voxelIndex;
                voxelLocalPos = result.voxelLocalPos;
                chunk = result.chunk;
                voxelWorldPos = currentVoxelPos;

                if (stepDir == 0) voxelNormal.X = -dx;
                else if (stepDir == 1) voxelNormal.Y = -dy;
                else voxelNormal.Z = -dz;
                return;
            }

            if (maxX < maxY)
            {
                if (maxX < maxZ)
                {
                    currentVoxelPos.X += (int)dx;
                    maxX += deltaX;
                    stepDir = 0;
                }
                else
                {
                    currentVoxelPos.Z += (int)dz;
                    maxZ += deltaZ;
                    stepDir = 2;
                }
            }
            else
            {
                if (maxY < maxZ)
                {
                    currentVoxelPos.Y += (int)dy;
                    maxY += deltaY;
                    stepDir = 1;
                }
                else
                {
                    currentVoxelPos.Z += (int)dz;
                    maxZ += deltaZ;
                    stepDir = 2;
                }
            }
        }
    }
}

