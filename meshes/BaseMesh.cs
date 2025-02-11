using OpenTK.Graphics.OpenGL4;
using System;

public abstract class BaseMesh
{
    // OpenGL context
    //private ctx;

    // shader program
    protected int program;

    // vertex buffer data type format: "3f 3f"
    protected string vboFormat;

    // attribute names according to the format: ("in_position", "in_color")
    protected string[] attrs;

    protected int vao; //???
    protected int vbo;


    /// <summary>
    /// Initializes the BaseMesh class with default values for
    ///  the OpenGL context,
    ///  shader program,
    ///  VBO format,
    ///  attribute names.
    ///  vertex array object (VAO),
    ///  vertex buffer object (VBO).
    /// </summary>
    public BaseMesh()
    {
        program = 0;
        vboFormat = string.Empty;
        attrs = Array.Empty<string>();
        vao = 0;
        vbo = 0;
    }

    /// <summary>
    /// Abstract method that must be implemented by derived classes to provide vertex data.
    /// </summary>
    protected abstract float[] GetVertexData();

    /// <summary>
    /// Initializes the VBO and VAO, sets up the vertex attributes, and binds the vertex data.
    /// </summary>
    protected void Initialize()
    {
        float[] vertexData = GetVertexData();
        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int stride = CalculateStride(vboFormat);
        int offset = 0;

        for (int i = 0; i < attrs.Length; i++)
        {
            int location = GL.GetAttribLocation(program, attrs[i]);
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, GetAttributeSize(vboFormat, i), VertexAttribPointerType.Float, false, stride, offset);
            offset += GetAttributeSize(vboFormat, i) * sizeof(float);
        }

        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Uses the shader program, binds the VAO, and renders the mesh.
    /// </summary>
    public void Render()
    {
        GL.UseProgram(program);
        GL.BindVertexArray(vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, GetVertexData().Length / CalculateStride(vboFormat));
        GL.BindVertexArray(0);
    }

    /// <summary>
    /// Calculates the stride (number of bytes between consecutive vertex attributes) based on the VBO format.
    /// </summary>
    protected int CalculateStride(string format)
    {
        int stride = 0;
        string[] elements = format.Split(' ');
        foreach (string element in elements)
        {
            stride += GetAttributeSize(element);
        }
        return stride * sizeof(float);
    }

    /// <summary>
    /// Returns the size of an attribute based on the VBO format.
    /// </summary>
    protected int GetAttributeSize(string format, int index = 0)
    {
        string[] elements = format.Split(' ');
        string element = elements[index];
        return int.Parse(element.Substring(0, element.Length - 1));
    }

    /// <summary>
    /// Creates and initializes the VBO and VAO, sets up the vertex attributes, and returns the VAO.
    /// </summary>
    protected int GetVAO()
    {
        float[] vertexData = GetVertexData();
        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexData.Length * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int stride = CalculateStride(vboFormat);
        int offset = 0;

        for (int i = 0; i < attrs.Length; i++)
        {
            int location = GL.GetAttribLocation(program, attrs[i]);
            GL.EnableVertexAttribArray(location);
            GL.VertexAttribPointer(location, GetAttributeSize(vboFormat, i), VertexAttribPointerType.Float, false, stride, offset);
            offset += GetAttributeSize(vboFormat, i) * sizeof(float);
        }

        GL.BindVertexArray(0);
        return vao;
    }

}

