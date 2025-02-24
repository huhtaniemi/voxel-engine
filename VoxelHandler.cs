using System;
using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

public class VoxelHandler
{
    public VoxelEngine app;
    private Chunk[] chunks;

    // Ray casting result
    private Chunk? chunk;
    public byte voxel_id;
    private int voxel_index;
    private Vector3i voxel_local_pos;
    public Vector3i voxel_world_pos;
    public Vector3i voxel_normal;

    public bool interactionMode = false;  // false: remove voxel, true: add voxel
    private const byte new_voxel_id = Settings.DIRT;

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


    public void SetVoxel()
    {
        if (interactionMode)
            AddVoxel();
        else
            RemoveVoxel();
    }

    public void SwitchMode()
    {
        interactionMode = !interactionMode;
    }

    private void AddVoxel()
    {
        if (voxel_id != 0)
        {
            var  result = GetVoxelId(voxel_world_pos + voxel_normal);

            if (result.voxel_id == 0 && result.chunk != null)
            {
                result.chunk.voxels[result.voxel_index] = new_voxel_id;
                //result.chunk.BuildMesh();
                result.chunk.BuildMesh(out result.chunk.mesh);

                if (result.chunk.IsEmpty)
                    result.chunk.IsEmpty = false;
            }
        }
    }

    private void RemoveVoxel()
    {
        if (voxel_id != 0 && chunk != null)
        {
            chunk.voxels[voxel_index] = 0;
            //chunk.BuildMesh();
            chunk.BuildMesh(out chunk.mesh);
            RebuildAdjacentChunks();
        }
    }

    private void RebuildAdjacentChunks()
    {
        var (lx, ly, lz) = voxel_local_pos;
        var (wx, wy, wz) = voxel_world_pos;

        if (lx == 0) RebuildAdjChunk(new(wx - 1, wy, wz));
        else if (lx == Settings.CHUNK_SIZE - 1) RebuildAdjChunk(new(wx + 1, wy, wz));

        if (ly == 0) RebuildAdjChunk(new(wx, wy - 1, wz));
        else if (ly == Settings.CHUNK_SIZE - 1) RebuildAdjChunk(new(wx, wy + 1, wz));

        if (lz == 0) RebuildAdjChunk(new(wx, wy, wz - 1));
        else if (lz == Settings.CHUNK_SIZE - 1) RebuildAdjChunk(new(wx, wy, wz + 1));
    }

    private void RebuildAdjChunk(Vector3i adjVoxelPos)
    {
        int index = VoxelMeshBuilder.GetChunkIndex(adjVoxelPos);
        if (index != -1)
        {
            //chunks[index].BuildMesh();
            chunks[index].BuildMesh(out chunks[index].mesh);
        }
    }


    private (byte voxel_id, int voxel_index, Vector3i voxel_pos, Chunk? chunk) GetVoxelId(Vector3i voxel_world_pos)
    {
        var chunk_pos = voxel_world_pos / Settings.CHUNK_SIZE;
        var (cx, cy, cz) = chunk_pos;

        if (cx >= 0 && cx < Settings.WORLD_W && cy >= 0 && cy < Settings.WORLD_H && cz >= 0 && cz < Settings.WORLD_D)
        {
            var chunk_index = cx + Settings.WORLD_W * cz + Settings.WORLD_AREA * cy;
            var chunk = chunks[chunk_index];

            var (lx, ly, lz) = (voxel_local_pos = (voxel_world_pos - chunk_pos * Settings.CHUNK_SIZE));

            var voxel_index = lx + Settings.CHUNK_SIZE * lz + Settings.CHUNK_AREA * ly;
            var voxel_id = chunk.voxels[voxel_index];

            return (voxel_id, voxel_index, new(lx, ly, lz), chunk);
        }
        return (0, 0, new(0), null);
    }

    /*
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int FastFloor(double x) =>
        x < (int)x ? (int)x - 1 : (int)x;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public decimal FastFract(decimal value) =>
        value - Math.Floor(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double FastFract(double x) => (x % 1 + 1) % 1;
    */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float FastFract(float x) => (x % 1 + 1) % 1;

    private bool RayCast()
    {
        var (x1, y1, z1) = app.player.Position;
        var (x2, y2, z2) = app.player.Position + app.player.Forward * Settings.MAX_RAY_DIST;

        var current_voxel_pos = ((Vector3i)app.player.Position);

        var dx = MathF.Sign(x2 - x1);
        float deltaX = dx != 0 ? Math.Min(dx / (x2 - x1), 10000000.0f) : 10000000.0f;
        float maxX = dx > 0 ? deltaX * (1.0f - FastFract(x1)) : deltaX * FastFract(x1);

        var dy = MathF.Sign(y2 - y1);
        float deltaY = dy != 0 ? MathF.Min(dy / (y2 - y1), 10000000.0f) : 10000000.0f;
        float maxY = dy > 0 ? deltaY * (1.0f - FastFract(y1)) : deltaY * FastFract(y1);

        var dz = MathF.Sign(z2 - z1);
        float deltaZ = dz != 0 ? MathF.Min(dz / (z2 - z1), 10000000.0f) : 10000000.0f;
        float maxZ = dz > 0 ? deltaZ * (1.0f - FastFract(z1)) : deltaZ * FastFract(z1);

        voxel_id = 0;
        voxel_normal = new(0);
        int stepDir = -1;

        while (!(maxX > 1.0f && maxY > 1.0f && maxZ > 1.0f))
        {
            var result = GetVoxelId(current_voxel_pos);
            if (result.voxel_id != 0)
            {
                (voxel_id, voxel_index, voxel_local_pos, chunk) = result;
                voxel_world_pos = current_voxel_pos;

                if (stepDir == 0)
                    voxel_normal.X = -dx;
                else if (stepDir == 1)
                    voxel_normal.Y = -dy;
                else
                    voxel_normal.Z = -dz;
                return true;
            }

            if (maxX < maxY)
            {
                if (maxX < maxZ)
                {
                    current_voxel_pos.X += (int)dx;
                    maxX += deltaX;
                    stepDir = 0;
                }
                else
                {
                    current_voxel_pos.Z += (int)dz;
                    maxZ += deltaZ;
                    stepDir = 2;
                }
            }
            else
            {
                if (maxY < maxZ)
                {
                    current_voxel_pos.Y += (int)dy;
                    maxY += deltaY;
                    stepDir = 1;
                }
                else
                {
                    current_voxel_pos.Z += (int)dz;
                    maxZ += deltaZ;
                    stepDir = 2;
                }
            }
        }
        return false;
    }
}

