using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.IO;

public class ShaderProgram
{
    private VoxelEngine app;

    public int chunkProgram;
    public int voxelMarkerProgram;
    public int waterProgram;
    public int cloudsProgram;

    /// <summary>
    ///  Initializes the ShaderProgram class, loads the shaders, and sets the uniforms.
    /// </summary>
    public ShaderProgram(VoxelEngine app)
    {
        this.app = app;

        // Load shaders
        chunkProgram = GetProgram("chunk");
        voxelMarkerProgram = GetProgram("voxel_marker");
        waterProgram = GetProgram("water");
        cloudsProgram = GetProgram("clouds");

        // Set uniforms on initialization
        SetUniformsOnInit();
    }

    /// <summary>
    /// Sets the initial uniforms for each shader program.
    /// </summary>
    private void SetUniformsOnInit()
    {
        // Chunk
        SetUniform(chunkProgram, "m_proj", app.player.MProj);
        SetUniform(chunkProgram, "m_model", Matrix4.Identity);
        SetUniform(chunkProgram, "u_texture_array_0", 1);
        SetUniform(chunkProgram, "bg_color", Settings.BG_COLOR);
        SetUniform(chunkProgram, "water_line", Settings.WATER_LINE);

        // Marker
        SetUniform(voxelMarkerProgram, "m_proj", app.player.MProj);
        SetUniform(voxelMarkerProgram, "m_model", Matrix4.Identity);
        SetUniform(voxelMarkerProgram, "u_texture_0", 0);

        // Water
        SetUniform(waterProgram, "m_proj", app.player.MProj);
        SetUniform(waterProgram, "u_texture_0", 2);
        SetUniform(waterProgram, "water_area", Settings.WATER_AREA);
        SetUniform(waterProgram, "water_line", Settings.WATER_LINE);

        // Clouds
        SetUniform(cloudsProgram, "m_proj", app.player.MProj);
        SetUniform(cloudsProgram, "center", Settings.CENTER_XZ);
        SetUniform(cloudsProgram, "bg_color", Settings.BG_COLOR);
        SetUniform(cloudsProgram, "cloud_scale", Settings.CLOUD_SCALE);
    }

    /// <summary>
    /// Updates the view matrix uniform for each shader program.
    /// </summary>
    public void Update()
    {
        SetUniform(chunkProgram, "m_view", app.player.MView);
        SetUniform(voxelMarkerProgram, "m_view", app.player.MView);
        SetUniform(waterProgram, "m_view", app.player.MView);
        SetUniform(cloudsProgram, "m_view", app.player.MView);
    }

    /// <summary>
    /// Loads and compiles the vertex and fragment shaders, links them into a program, and returns the program ID.
    /// </summary>
    private int GetProgram(string shaderName)
    {
        string vertexShaderSource = File.ReadAllText($"shaders/{shaderName}.vert");
        string fragmentShaderSource = File.ReadAllText($"shaders/{shaderName}.frag");

        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        GL.CompileShader(vertexShader);
        CheckShaderCompile(vertexShader);

        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        GL.CompileShader(fragmentShader);
        CheckShaderCompile(fragmentShader);

        int program = GL.CreateProgram();
        GL.AttachShader(program, vertexShader);
        GL.AttachShader(program, fragmentShader);
        GL.LinkProgram(program);
        CheckProgramLink(program);

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        return program;
    }

    /// <summary>
    /// Sets the uniform values for the shader programs.
    /// </summary>
    public void SetUniform(int program, string name, Matrix4 value)
    {
        int location = GL.GetUniformLocation(program, name);
        GL.UseProgram(program);
        GL.UniformMatrix4(location, false, ref value);
    }

    /// <summary>
    /// Sets the uniform values for the shader programs.
    /// </summary>
    public void SetUniform(int program, string name, int value)
    {
        int location = GL.GetUniformLocation(program, name);
        GL.UseProgram(program);
        GL.Uniform1(location, value);
    }

    /// <summary>
    /// Sets the uniform values for the shader programs.
    /// </summary>
    public void SetUniform(int program, string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(program, name);
        GL.UseProgram(program);
        GL.Uniform3(location, ref value);
    }

    /// <summary>
    /// Sets the uniform values for the shader programs.
    /// </summary>
    public void SetUniform(int program, string name, float value)
    {
        int location = GL.GetUniformLocation(program, name);
        GL.UseProgram(program);
        GL.Uniform1(location, value);
    }

    /// <summary>
    ///  Checks if the shader compilation was successful.
    /// </summary>
    private void CheckShaderCompile(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == (int)All.False)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Shader compilation failed: {infoLog}");
        }
    }

    /// <summary>
    /// Checks if the program linking was successful.
    /// </summary>
    private void CheckProgramLink(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
        if (status == (int)All.False)
        {
            string infoLog = GL.GetProgramInfoLog(program);
            throw new Exception($"Program linking failed: {infoLog}");
        }
    }
}
