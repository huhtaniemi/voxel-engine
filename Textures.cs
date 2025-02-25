using System;
using ModernGL;
using StbImageSharp;


public class Textures
{
    private VoxelEngine app;
    private glContext.Texture texture0;
    /*
    private int texture1;
    private int textureArray0;
    */

    public Textures(VoxelEngine app)
    {
        this.app = app;

        // Load textures
        this.texture0 = Load("frame.png");
        /*
        texture1 = Load("water.png");
        textureArray0 = Load("tex_array_0.png", isTexArray: true);
        */

        // Assign texture unit
        this.texture0.use(location: 0);
        /*
        Use(textureArray0, 1);
        Use(texture1, 2);
        */
    }

    private void flip_horisontal(byte[] pixelbytes, int width, int height, int channels)
    {
        for (int y = 0; y < height; y++)
        {
            int rowStart = y * width * channels;
            // Swap pixels horizontally in the current row
            for (int x = 0; x < width / 2; x++)
            {
                int leftIndex = rowStart + x * channels;
                int rightIndex = rowStart + (width - x - 1) * channels;

                // Swap each channel (R, G, B, A)
                for (int c = 0; c < channels; c++)
                {
                    byte temp = pixelbytes[leftIndex + c];
                    pixelbytes[leftIndex + c] = pixelbytes[rightIndex + c];
                    pixelbytes[rightIndex + c] = temp;
                }
            }
        }
    }

    private glContext.Texture Load(string fileName, bool isTexArray = false)
    {
        //textureimg = pg.image.load(f'assets/{file_name}')
        //textureimg = pg.transform.flip(textureimg, flip_x = True, flip_y = False)

        //StbImage.stbi_set_flip_vertically_on_load(1);
        using Stream stream = File.OpenRead($"assets/{fileName}");
        var textureimg = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        flip_horisontal(textureimg.Data, textureimg.Width, textureimg.Height, (int)textureimg.Comp);

        //texture = self.ctx.texture(
        //    size = texture.get_size(),
        //    components = 4,
        //    data = pg.image.tostring(texture, 'RGBA', False)
        //);
        var texture = app.ctx.texture(
            size: (textureimg.Width, textureimg.Height),
            components: 4,
            data: textureimg.Data
        );

        texture.anisotropy = 32.0f;
        texture.build_mipmaps();
        //texture.filter = (glContext.Texture.fTypes.NEAREST, glContext.Texture.fTypes.NEAREST);
        texture.filter = (glContext.Texture.fTypes.LINEAR_MIPMAP_LINEAR, glContext.Texture.fTypes.NEAREST);
        return texture;
    }

    private void Use(int textureId, int location)
    {
    }
}
