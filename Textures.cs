using OpenTK.Graphics.OpenGL4;
using System;


public class Textures
{
    private VoxelEngine app;
    private int texture0;
    private int texture1;
    private int textureArray0;

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

    private int Load(string fileName, bool isTexArray = false)
    {
        //textureimg = pg.image.load(f'assets/{file_name}')
        //textureimg = pg.transform.flip(textureimg, flip_x = True, flip_y = False)

        int textureId = GL.GenTexture();
        //texture = self.ctx.texture(
        //    size = texture.get_size(),
        //    components = 4,
        //    data = pg.image.tostring(texture, 'RGBA', False)
        //);

        return textureId;
    }

    private void Use(int textureId, int location)
    {
    }
}
