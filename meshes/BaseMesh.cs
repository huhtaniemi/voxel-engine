using System;
using ModernGL;

public abstract class BaseMesh
{
    protected readonly glContext ctx;
    protected glContext.Program program;
    protected glContext.VertexArray? vao;

    // vertex buffer data type format: [count]type[size] [[count]type[size]...] [/usage]
    protected string vbo_format;

    // attribute names according to the format, like: ("in_position", "in_color")
    // names are defined by the user in the vertex shader program stage.
    protected string[] attrs;


    public BaseMesh(ref glContext ctx, ref glContext.Program program, string vbo_format, string[] attrs)
    {
        this.ctx = ctx;
        this.program = program;
        //this.vbo = null;
        //this.vao = null;
        this.vbo_format = vbo_format;
        this.attrs = attrs;
    }

    protected abstract object GetVertexData();


    /// <summary>
    /// Creates and initializes the VBO and VAO, sets up the vertex attributes, and returns the VAO.
    /// </summary>
    protected glContext.VertexArray GetVAO()
    {
        var vertex_data = GetVertexData();
        var vbo = ctx.buffer(ref vertex_data);
        var vao = ctx.vertex_array(
            ref program, [(vbo, this.vbo_format, this.attrs)], true
        );
        //vao = self.ctx.vertex_array(
        //   self.program, [(vbo, self.vbo_format, *self.attrs)], skip_errors = True
        //)
        return vao;
    }


    public void Render()
    {
        this.vao?.render();
    }
}

