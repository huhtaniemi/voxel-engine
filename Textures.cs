using OpenTK.Graphics.OpenGL4;
using System;
using System.Drawing;
//using System.Drawing.Common;
using System.Drawing.Imaging;

public class Textures
{
    private VoxelEngine app;
    private int texture0;
    private int texture1;
    private int textureArray0;

    /// <summary>
    /// Takes an VoxelEngine instance and a method to load a texture from a file.
    /// Initializes the Textures class, loads the textures, and assigns them to texture units.
    /// </summary>
    public Textures(VoxelEngine app)
    {
        this.app = app;

        // Load textures
        texture0 = Load("frame.png");
        texture1 = Load("water.png");
        textureArray0 = Load("tex_array_0.png", isTexArray: true);

        // Assign texture unit
        Use(texture0, 0);
        Use(textureArray0, 1);
        Use(texture1, 2);
    }

    /// <summary>
    /// Generates a texture ID, loads the image, uploads it to the GPU, and sets the texture parameters.
    /// Loads a texture from a file. If isTexArray is true, it creates a texture array.
    /// </summary>
    private int Load(string fileName, bool isTexArray = false)
    {
#pragma warning disable CA1416
        Bitmap bitmap = new($"assets/{fileName}");
        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
#pragma warning restore CA1416

        int textureId = GL.GenTexture();
        GL.BindTexture(isTexArray ? TextureTarget.Texture2DArray : TextureTarget.Texture2D, textureId);

#pragma warning disable CA1416
        if (isTexArray)
        {
            int numLayers = 3 * bitmap.Height / bitmap.Width;
            GL.TexImage3D(TextureTarget.Texture2DArray, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height / numLayers, numLayers, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
        }
        else
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);
        }
#pragma warning restore CA1416

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        GL.BindTexture(TextureTarget.Texture2D, 0);

        return textureId;
    }

    /// <summary>
    /// Binds the texture to a specified texture unit.
    /// </summary>
    private void Use(int textureId, int location)
    {
        GL.ActiveTexture(TextureUnit.Texture0 + location);
        GL.BindTexture(TextureTarget.Texture2D, textureId);
    }
}
