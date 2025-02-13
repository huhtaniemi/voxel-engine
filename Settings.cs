using System;
using OpenTK.Mathematics;

public static class Settings
{
    // OpenGL settings
    public const int MAJOR_VER = 3;
    public const int MINOR_VER = 3;
    public const int DEPTH_SIZE = 24;
    public const int NUM_SAMPLES = 1;  // antialiasing

    // Resolution
    public static readonly Vector2i WIN_RES = new Vector2i(1600, 900);

    // World generation
    public const int SEED = 16;

    // Ray casting
    public const int MAX_RAY_DIST = 6;

    // Chunk
    public const int CHUNK_SIZE = 48;
    public const int H_CHUNK_SIZE = CHUNK_SIZE / 2;
    public const int CHUNK_AREA = CHUNK_SIZE * CHUNK_SIZE;
    public const int CHUNK_VOL = CHUNK_AREA * CHUNK_SIZE;
    public static readonly double CHUNK_SPHERE_RADIUS = H_CHUNK_SIZE * Math.Sqrt(3);

    // World
    public const int WORLD_W = 20;
    public const int WORLD_H = 2;
    public const int WORLD_D = WORLD_W;
    public const int WORLD_AREA = WORLD_W * WORLD_D;
    public const int WORLD_VOL = WORLD_AREA * WORLD_H;

    // World center
    public static readonly int CENTER_XZ = WORLD_W * H_CHUNK_SIZE;
    public static readonly int CENTER_Y = WORLD_H * H_CHUNK_SIZE;

    // Camera
    public static readonly float ASPECT_RATIO = WIN_RES.X / WIN_RES.Y;
    public const float FOV_DEG = 50;
    public static readonly float V_FOV = MathHelper.DegreesToRadians(FOV_DEG);  // vertical FOV
    public static readonly float H_FOV = 2 * (float)Math.Atan(Math.Tan(V_FOV * 0.5) * ASPECT_RATIO);  // horizontal FOV
    public const float NEAR = 0.1f;
    public const float FAR = 2000.0f;
    public static readonly float PITCH_MAX = MathHelper.DegreesToRadians(89);

    // Player
    public const float PLAYER_SPEED = 0.005f;
    public const float PLAYER_ROT_SPEED = 0.003f;
    //public static readonly Vector3 PLAYER_POS = new(CENTER_XZ, CHUNK_SIZE, CENTER_XZ);
    public static readonly Vector3 PLAYER_POS = new(0, 0, 1);
    public const float MOUSE_SENSITIVITY = 0.002f;

    // Colors
    public static readonly Vector3 BG_COLOR = new(0.58f, 0.83f, 0.99f);

    // Textures
    public const int SAND = 1;
    public const int GRASS = 2;
    public const int DIRT = 3;
    public const int STONE = 4;
    public const int SNOW = 5;
    public const int LEAVES = 6;
    public const int WOOD = 7;

    // Terrain levels
    public const int SNOW_LVL = 54;
    public const int STONE_LVL = 49;
    public const int DIRT_LVL = 40;
    public const int GRASS_LVL = 8;
    public const int SAND_LVL = 7;

    // Tree settings
    public const double TREE_PROBABILITY = 0.02;
    public const int TREE_WIDTH = 4;
    public const int TREE_HEIGHT = 8;
    public const int TREE_H_WIDTH = TREE_WIDTH / 2;
    public const int TREE_H_HEIGHT = TREE_HEIGHT / 2;

    // Water
    public const float WATER_LINE = 5.6f;
    public static readonly float WATER_AREA = 5 * CHUNK_SIZE * WORLD_W;

    // Cloud
    public const int CLOUD_SCALE = 25;
    public static readonly int CLOUD_HEIGHT = WORLD_H * CHUNK_SIZE * 2;
}

