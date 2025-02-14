using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace ModernGL
{
    /// <summary>
    /// Modern OpenGL context
    /// </summary>
    public class glContext
    {
        /// <summary>
        /// shader program
        /// </summary>
        public class Program : IDisposable
        {
            public class Shader : IDisposable
            {
                internal readonly int id;

                public Shader(ShaderType type) =>
                    this.id = GL.CreateShader(type);

                public Shader(ShaderType type, string source)
                    : this(type)
                {
                    GL.ShaderSource(id, source);
                    GL.CompileShader(id);
                    CheckShaderCompile(id);
                }

                public void Dispose() =>
                    GL.DeleteShader(id);

                private void CheckShaderCompile(int shader)
                {
                    GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
                    if (status == (int)All.False)
                    {
                        string infoLog = GL.GetShaderInfoLog(shader);
                        throw new Exception($"Shader compilation failed: {infoLog}");
                    }
                }
            }

            internal readonly int id;

            public Program() =>
                this.id = GL.CreateProgram();

            public Program(Shader[] shaders) : this()
            {
                foreach (var shader in shaders)
                {
                    attach(shader);
                }
                link();
            }

            public void Dispose() =>
                GL.DeleteProgram(id);

            internal void attach(Shader shader) =>
                GL.AttachShader(id, shader.id);

            internal void link()
            {
                GL.LinkProgram(id);
                CheckProgramLink(id);
            }

            private void CheckProgramLink(int program)
            {
                GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int status);
                if (status == (int)All.False)
                {
                    string infoLog = GL.GetProgramInfoLog(program);
                    throw new Exception($"Program linking failed: {infoLog}");
                }
            }

            /// Sets the uniform values for the shader programs.
            public object this[string key]
            {
                set
                {
                    int location = GL.GetUniformLocation(id, key);
                    GL.UseProgram(id);

                    switch (value)
                    {
                        case int ival:
                            GL.Uniform1(location, ival);
                            break;
                        case float fval:
                            GL.Uniform1(location, fval);
                            break;
                        case Vector3 v3val:
                            GL.Uniform3(location, v3val);
                            break;
                        case Matrix4 m4val:
                            GL.UniformMatrix4(location, false, ref m4val);
                            break;
                        default:
                            throw new NotSupportedException($"Type '{value.GetType()}' not supported.");
                    }
                }
            }
        }

        /// <summary>
        /// A buffer format is a short string describing the layout of data in a vertex buffer object (VBO).
        /// A VBO often contains a homogeneous array of C-like structures.The buffer format describes what 
        /// each element of the array looks like.For example, a buffer containing an array of high-precision 2D vertex
        /// positions might have the format "2f8" - each element of the array consists of two floats, each 
        /// float being 8 bytes wide, ie.a double.
        /// </summary>
        /// <remarks
        /// Buffer formats are used in the Context.vertex_array() constructor, as the 2nd component of the content arg.
        /// </remarks>
        public class Buffer : IDisposable
        {
            internal class BufferFormat
            {
                // each token represents one [count]type[size] triple.
                internal record ElementToken()
                {
                    public enum ElementType
                    {
                        f, i, u, x
                    }

                    internal readonly ElementType type;
                    internal readonly int size; // = 4;
                    internal readonly int count; // = 1;
                    internal readonly string? attr;

                    internal readonly int offset;

                    // parse a token: [count]type[size]
                    public ElementToken(string tokenstr, string attr) : this()
                    {
                        int index = 0;
                        // Parse optional count (if any digits appear at the start).
                        while (index < tokenstr.Length && char.IsDigit(tokenstr[index]))
                        {
                            this.count = this.count * 10 + (tokenstr[index] - '0');
                            index++;
                        }
                        if (this.count == 0)
                            this.count = 1; // Default count is 1.

                        // Ensure that a type character is present.
                        if (index >= tokenstr.Length)
                            throw new Exception("Missing type specifier in token: " + tokenstr);

                        // The next character must be one of: f, i, u, x.
                        this.type = tokenstr[index] switch
                        {
                            'f' => ElementType.f,
                            'i' => ElementType.i,
                            'u' => ElementType.u,
                            'x' => ElementType.x,
                            _ => throw new Exception($"Unknown type specifier: {tokenstr[index]}"),
                        };

                        index++;

                        // Parse optional size digits.
                        while (index < tokenstr.Length && char.IsDigit(tokenstr[index]))
                        {
                            this.size = this.size * 10 + (tokenstr[index] - '0');
                            index++;
                        }
                        // Default sizes: numeric types default to 4; padding (x) defaults to 1.
                        if (this.size == 0)
                        {
                            this.size = (this.type == ElementType.x) ? 1 : 4;
                        }

                        // Validate the size for the given type using the allowed combinations.

                        var validSize = this.type switch
                        {
                            ElementType.f => (size == 1 || size == 2 || size == 4 || size == 8),
                            ElementType.i => (size == 1 || size == 2 || size == 4),
                            ElementType.u => (size == 1 || size == 2 || size == 4),
                            ElementType.x => (size == 1 || size == 2 || size == 4 || size == 8),
                            _ => throw new Exception($"Unknown type specifier: {this.type}"),
                        };

                        //bool validSize = false;
                        //switch (this.type)
                        //{
                        //    case ElementType.f:
                        //        // Valid sizes: 1 (unsigned byte normalized), 2 (half float), 4 (float), 8 (double)
                        //        validSize = (size == 1 || size == 2 || size == 4 || size == 8);
                        //        break;
                        //    case ElementType.i:
                        //        // Valid sizes: 1 (byte), 2 (short), 4 (int)
                        //        validSize = (size == 1 || size == 2 || size == 4);
                        //        break;
                        //    case ElementType.u:
                        //        // Valid sizes: 1 (unsigned byte), 2 (unsigned short), 4 (unsigned int)
                        //        validSize = (size == 1 || size == 2 || size == 4);
                        //        break;
                        //    case ElementType.x:
                        //        // Valid sizes: 1, 2, 4, 8 (padding bytes)
                        //        validSize = (size == 1 || size == 2 || size == 4 || size == 8);
                        //        break;
                        //}
                        if (!validSize)
                            throw new Exception($"Invalid size {this.size} for type '{this.type}' in token '{tokenstr}'.");

                        this.attr = attr;
                        this.offset = 0; // TODO: calculate offset
                    }

                    public override string ToString()
                    {
                        return $"{count}{type}{size}";
                    }
                }
                internal List<ElementToken> tokens { get; } = [];

                public enum BufferUsage
                {
                    vertex,    // /v (default)
                    instance,  // /i
                    render     // /r
                }
                public BufferUsage usage { get; private set; } = BufferUsage.vertex;

                // specifies the byte offset between consecutive vertex attributes.
                // is the total size in bytes of a single vertex entry.
                // note: If your vertex attributes are tightly packed(meaning one attribute immediately
                //       follows the previous one with no extra data in between), you can set stride to 0. 
                internal readonly int stride = 0;

                // [count]type[size] [[count]type[size]...] [/usage]
                public BufferFormat(string format, string[] attrs)
                {
                    if (string.IsNullOrWhiteSpace(format))
                        throw new ArgumentException("Format string cannot be null or empty.");

                    int attrpos = 0;
                    foreach (string elementfmt in format.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (elementfmt[0] == '/')
                        {
                            if (elementfmt.Length != 2)
                                throw new Exception($"Invalid usage specifier: {elementfmt}");

                            // note: usage specifier must be the last element in the format string?
                            // note: usage specifier must be unique?

                            this.usage = elementfmt[1] switch
                            {
                                'v' => BufferUsage.vertex,
                                'i' => BufferUsage.instance,
                                'r' => BufferUsage.render,
                                _ => throw new Exception($"Unknown usage specifier: {elementfmt[1]}"),
                            };
                        }
                        else this.tokens.Add(new ElementToken(elementfmt, attrs[attrpos++]));
                    }

                    stride = tokens.Sum(t => t.size * t.count);
                }

                public override string ToString()
                {
                    var tokensfmt = string.Join(" ", tokens);
                    var usagefmt = usage switch
                    {
                        BufferUsage.vertex => "/v",
                        BufferUsage.instance => "/i",
                        BufferUsage.render => "/r",
                        _ => ""
                    };
                    return $"{tokensfmt} {usagefmt}";
                }

                public IEnumerator<ElementToken> GetEnumerator() => tokens.GetEnumerator();
            }


            internal readonly int id;

            public VertexAttribPointerType ptype;
            public int length;
            public bool normalized { get
                => ptype != VertexAttribPointerType.Float
                && ptype != VertexAttribPointerType.Double;
            }

            public Buffer() =>
                this.id = GL.GenBuffer();

            public Buffer(ref object data, bool dynamic = false) : this()
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, id);
                var DrawType = dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw;
                length = ((Array)data).Length;
                switch (data)
                {
                    case byte[] byteData:
                        length *= sizeof(byte);
                        ptype = VertexAttribPointerType.UnsignedByte;
                        GL.BufferData(BufferTarget.ArrayBuffer, length, byteData, DrawType);
                        break;
                    case int[] integerData:
                        length *= sizeof(int);
                        ptype = VertexAttribPointerType.Int;
                        GL.BufferData(BufferTarget.ArrayBuffer, length, integerData, DrawType);
                        break;
                    case float[] floatData:
                        length *= sizeof(float);
                        ptype = VertexAttribPointerType.Float;
                        GL.BufferData(BufferTarget.ArrayBuffer, length, floatData, DrawType);
                        break;
                    case double[] doubleData:
                        length *= sizeof(double);
                        ptype = VertexAttribPointerType.Double;
                        GL.BufferData(BufferTarget.ArrayBuffer, length, doubleData, DrawType);
                        break;
                    default:
                        throw new NotSupportedException($"Type '{data.GetType()}' not supported.");
                }
            }
            public Buffer(int reserve = 0, bool dynamic = false) : this()
            {
                throw new NotImplementedException();
            }

            public void Dispose()
            {
                // Attachments to unbound container objects, such as deletion of a buffer attached to
                // a vertex array object which is not bound to the context, are not affected and continue to
                // act as references on the deleted object, as described in the following section.
                GL.DeleteBuffer(id);
            }
        }

        /// <summary>
        /// A VertexArray object is an OpenGL object that stores all of the state needed to supply vertex data.
        /// It stores the format of the vertex data as well as the Buffer objects providing the vertex data arrays.
        /// In ModernGL, the VertexArray object also stores a reference for a Program object.
        /// Compared to OpenGL, VertexArray also stores a Program object.
        /// </summary>
        public class VertexArray : IDisposable
        {
            protected readonly int id;
            private readonly int program_id;

            private int vertices;

            protected VertexArray() =>
                this.id = GL.GenVertexArray();

            protected VertexArray(Program program) : this() =>
                this.program_id = program.id;

            public VertexArray(Program program, (Buffer vbo, string vbo_format, string[] attrs)[] content, bool skip_errors = false)
                : this(program)
            {
                foreach (var item in content)
                    bind_content(item);
            }

            private void bind_content((Buffer vbo, string vbo_format, string[] attrs) content)
            {
                var vbo_tokens = new Buffer.BufferFormat(content.vbo_format, content.attrs);
                this.vertices = content.vbo.length / vbo_tokens.stride;

                var offset = 0;
                GL.BindVertexArray(id);
                GL.BindBuffer(BufferTarget.ArrayBuffer, content.vbo.id);
                foreach (var token in vbo_tokens)
                {
                    // https://stackoverflow.com/a/39684775/1820584
                    var index = GL.GetAttribLocation(program_id, token.attr);
                    GL.EnableVertexAttribArray(index);
                    GL.VertexAttribPointer(index, token.count, content.vbo.ptype,
                        content.vbo.normalized, vbo_tokens.stride, offset);
                    offset += token.size * token.count;
                }
                GL.BindVertexArray(0);
            }


            public void Dispose() =>
                GL.DeleteVertexArray(id);

            public void render(PrimitiveType mode = PrimitiveType.Triangles, int vertices = -1, int first = 0, int instances = -1)
            {
                GL.UseProgram(program_id);
                GL.BindVertexArray(id);
                if (instances == -1)
                    // Specifies the number of indices to be rendered. (6 in water example)
                    GL.DrawArrays(mode, first, vertices == -1 ? this.vertices : vertices);
                else
                    GL.DrawArraysInstanced(mode, first, vertices == -1 ? this.vertices : vertices, instances);
                GL.BindVertexArray(0);
                //GL.UseProgram(0);
            }
        }


        public Program program(string vertex_shader, string fragment_shader)
        {
            //return = new Program([
            //    new (ShaderType.VertexShader, vertex_shader),
            //    new (ShaderType.FragmentShader, fragment_shader),
            //]);

            using (Program.Shader vertex = new(ShaderType.VertexShader, vertex_shader))
            using (Program.Shader fragment = new(ShaderType.FragmentShader, fragment_shader))
            {
                return new Program([vertex, fragment]);
            }
        }

        /// <summary>
        /// Returns a new Buffer object.
        /// The data can be anything supporting the buffer interface.
        /// The data and reserve parameters are mutually exclusive.
        /// </summary>
        public Buffer buffer(ref object data, bool dynamic = false)
        {
            return new Buffer(ref data, dynamic);
        }


        /// <summary>
        /// Returns a new VertexArray object.
        /// A VertexArray describes how buffers are read by a shader program.
        /// The content is a list of tuples containing a buffer, a format string and
        /// any number of attribute names.Attribute names are defined by the user in
        /// the Vertex Shader program stage.
        /// The default mode is TRIANGLES.
        public VertexArray vertex_array(Program program, (Buffer vbo, string vbo_format, string[] attrs)[] content, bool skip_errors = false)
        {
            return new VertexArray(program, content, skip_errors);
        }

    };


#pragma warning disable CS8981
    public class moderngl
#pragma warning restore CS8981
    {
        private static glContext? instance;

        public static glContext create_context()
        {
            if (instance == null)
            {
                instance = new glContext();
            }
            return instance;
        }
    }
}
