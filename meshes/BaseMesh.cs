using System;
using ModernGL;

public abstract class BaseMesh
{
    protected readonly glContext ctx;
    protected glContext.Program program;
    protected glContext.VertexArray? vao;

    // vertex buffer data type format: [count]type[size] [[count]type[size]...] [/usage]
    protected string vbo_format;
    protected int vbo_format_size;

    // attribute names according to the format, like: ("in_position", "in_color")
    // names are defined by the user in the vertex shader program stage.
    protected string[] attrs;


    public BaseMesh(ref glContext ctx, ref glContext.Program program, string vbo_format, string[] attrs)
    {
        this.ctx = ctx;
        this.program = program;
        this.vbo_format = vbo_format;
        // todo: 'size' is ignored, assumed byte
        this.vbo_format_size = vbo_format.Split(' ').Sum(fmt => int.Parse(fmt[..1]));
        this.attrs = attrs;
    }

    protected abstract object GetVertexData();

    /// <summary>
    /// Creates and initializes the VBO and VAO, sets up the vertex attributes, and returns the VAO.
    /// </summary>
    protected glContext.VertexArray GetVAO()
    {
        var vertex_data = GetVertexData();
        using var vbo = ctx.buffer(vertex_data);
        var vao = ctx.vertex_array(
            program, [(vbo, this.vbo_format, this.attrs)], skip_errors: true
        );
        return vao;
    }

    protected void Rebuild() =>
        this.vao = GetVAO();

    public void Render() =>
        this.vao?.render();
}

