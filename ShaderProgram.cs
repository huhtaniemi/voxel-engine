using System;
using OpenTK.Mathematics;
using ModernGL;


public class ShaderProgram
{
    private VoxelEngine app;
    private glContext ctx;

    //public glContext.Program chunk;
    //public glContext.Program voxel_marker;
    //public glContext.Program water; // quad
    //public glContext.Program clouds;

    public glContext.Program quad;

    /// <summary>
    ///  Initializes the ShaderProgram class, loads the shaders, and sets the uniforms.
    /// </summary>
    public ShaderProgram(VoxelEngine app, glContext ctx)
    {
        this.app = app;
        this.ctx = ctx;

        // Load shaders
        /*
        chunk = GetProgram("chunk");
        voxel_marker = GetProgram("voxel_marker");
        water = GetProgram("water");
        clouds = GetProgram("clouds");
        */

        quad = GetProgram("quad");

        // Set uniforms on initialization
        SetUniformsOnInit();
    }

    private void SetUniformsOnInit()
    {

        // Chunk
        /*
        //this.chunk["m_proj"] = app.player.MProj; //self.player.m_proj;
        this.chunk["m_model"] = Matrix4.Identity;
        this.chunk["u_texture_array_0"] = 1;
        this.chunk["bg_color"] = Settings.BG_COLOR;
        this.chunk["water_line"] = Settings.WATER_LINE;
        */

        // Marker
        /*
        //this.voxel_marker["m_proj"] = app.player.MProj;
        this.voxel_marker["m_model"] = Matrix4.Identity;
        this.voxel_marker["u_texture_0"] = 0;

        // Water
        /*
        //this.water["m_proj"] = app.player.MProj;
        this.water["u_texture_0"] = 2;
        this.water["water_area"] = Settings.WATER_AREA;
        this.water["water_line"] = Settings.WATER_LINE;
        */

        // Clouds
        /*
        //this.clouds["m_proj"] = app.player.MProj;
        this.clouds["center"] = Settings.CENTER_XZ;
        this.clouds["bg_color"] = Settings.BG_COLOR;
        this.clouds["cloud_scale"] = Settings.CLOUD_SCALE;
        */
    }

    /// <summary>
    /// Updates the view matrix uniform for each shader program.
    /// </summary>
    public void Update()
    {
        /*
        this.chunk["m_view"] = app.player.MView; // m_view
        this.voxel_marker["m_view"] = app.player.MView;
        this.water["m_view"] = app.player.MView;
        this.clouds["m_view"] = app.player.MView;
        */
    }

    /// <summary>
    /// Loads and compiles the vertex and fragment shaders, links them into a program, and returns the program ID.
    /// </summary>
    private glContext.Program GetProgram(string shader_name)
    {
        var vertex_shader = File.ReadAllText($"shaders/{shader_name}.vert");
        var fragment_shader = File.ReadAllText($"shaders/{shader_name}.frag");

        var program = this.ctx.program(vertex_shader, fragment_shader);
        return program;
    }
}
