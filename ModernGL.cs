using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace ModernGL
{
    /// <summary>
    /// Modern OpenGL context
    /// </summary>
    public class glContext : IDisposable
    {
        private bool _disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                //GL.DeleteProgram(Handle);
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal glContext()
        {
            // DEFAULT_BLENDING
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.TextureCubeMapSeamless);

            /*
            this.max_samples = 0;
            GL.GetInteger(GetPName.MaxSamples, out this.max_samples);

            this.max_integer_samples = 0;
            GL.GetInteger(GetPName.MaxIntegerSamples, out this.max_integer_samples);

            this.max_color_attachments = 0;
            GL.GetInteger(GetPName.MaxColorAttachments, out this.max_color_attachments);

            this.max_texture_units = 0;
            GL.GetInteger(GetPName.MaxTextureImageUnits, out this.max_texture_units);
            this.default_texture_unit = this.max_texture_units - 1;

            this.max_label_length = 0;
            GL.GetInteger(GetPName.MaxLabelLength, out this.max_label_length);

            this.max_debug_message_length = 0;
            GL.GetInteger(GetPName.MaxDebugMessageLength, out this.max_debug_message_length);

            this.max_debug_group_stack_depth = 0;
            GL.GetInteger(GetPName.MaxDebugGroupStackDepth, out this.max_debug_group_stack_depth);

            this.max_anisotropy = 0.0f;
            GL.GetFloatv(GetPName.MaxTextureMaxAnisotropy, out this.max_anisotropy);

            int bound_framebuffer = 0;
            GL.GetInteger(GetPName.DrawFramebufferBinding, out bound_framebuffer);
            //*/
        }

        /// <summary>
        /// shader program
        /// </summary>
        public class Program : IDisposable
        {
            public class Shader : IDisposable
            {
                internal readonly int id;
                internal int programm_id;

                public Shader(ShaderType type) =>
                    this.id = GL.CreateShader(type);

                public Shader(ShaderType type, string source)
                    : this(type)
                {
                    GL.ShaderSource(id, source);
                    GL.CompileShader(id);
                    GL.GetShader(id, ShaderParameter.CompileStatus, out int status);
                    if (status == (int)All.False)
                    {
                        string infoLog = GL.GetShaderInfoLog(id);
                        throw new Exception($"Error compiling Shader({id}).\n\n{infoLog}");
                    }
                }

                public void Dispose()
                {
                    if (programm_id != 0)
                        GL.DetachShader(programm_id, id);
                    GL.DeleteShader(id);
                }

                internal void attach(Program program) =>
                    GL.AttachShader((this.programm_id = program.id), id);

            }

            internal readonly int id;

            internal record struct AttributesInfo
            {
                public AttributesInfo(int i, int location, int size, ActiveAttribType type) =>
                    (index, this.location, this.size, this.type) = (i, location, size, type);
                public int index { get; }
                public int location { get; }
                public int size { get; }
                public ActiveAttribType type { get; }

                public bool IsFloat
                {
                    get => type switch
                    {
                        ActiveAttribType.Float or
                        ActiveAttribType.FloatVec2 or ActiveAttribType.FloatVec3 or ActiveAttribType.FloatVec4 or
                        ActiveAttribType.FloatMat2 or ActiveAttribType.FloatMat3 or ActiveAttribType.FloatMat4 or
                        ActiveAttribType.FloatMat2x3 or ActiveAttribType.FloatMat2x4 or
                        ActiveAttribType.FloatMat3x2 or ActiveAttribType.FloatMat3x4 or
                        ActiveAttribType.FloatMat4x2 or ActiveAttribType.FloatMat4x3 => true,
                        _ => false
                    };
                }
                public bool IsDouble
                {
                    get => type switch
                    {
                        ActiveAttribType.Double or
                        ActiveAttribType.DoubleVec2 or ActiveAttribType.DoubleVec3 or
                        ActiveAttribType.DoubleVec4 or ActiveAttribType.DoubleMat2 or
                        ActiveAttribType.DoubleMat3 or ActiveAttribType.DoubleMat4 or
                        ActiveAttribType.DoubleMat2x3 or ActiveAttribType.DoubleMat2x4 or
                        ActiveAttribType.DoubleMat3x2 or ActiveAttribType.DoubleMat3x4 or
                        ActiveAttribType.DoubleMat4x2 or ActiveAttribType.DoubleMat4x3 => true,
                        _ => false
                    };
                }
                public bool IsInt
                {
                    get => type switch
                    {
                        ActiveAttribType.Int or
                        ActiveAttribType.IntVec2 or ActiveAttribType.IntVec3 or ActiveAttribType.IntVec4 => true,
                        _ => false
                    };
                }
                public bool IsUInt
                {
                    get => type switch
                    {
                        ActiveAttribType.UnsignedInt or
                        ActiveAttribType.UnsignedIntVec2 or ActiveAttribType.UnsignedIntVec3 or ActiveAttribType.UnsignedIntVec4 => true,
                        _ => false
                    };
                }
            }
            internal readonly Dictionary<string, AttributesInfo> _attributes = [];

            internal record struct UniformInfo
            {
                public UniformInfo(int i, int location, int size, ActiveUniformType type) =>
                    (index, this.location, this.size, this.type) = (i, location, size, type);
                public int index { get; }
                public int location { get; }
                public int size { get; }
                public ActiveUniformType type { get; }
            }
            internal readonly Dictionary<string, UniformInfo> _uniforms = [];

            public Program() =>
                this.id = GL.CreateProgram();

            public Program(List<Shader> shaders) : this()
            {
                try
                {
                    foreach (var shader in shaders)
                        shader.attach(this);

                    link();

                    GL.GetProgram(id, GetProgramParameterName.ActiveAttributes, out var attributesCount);
                    for (var i = 0; i < attributesCount; i++)
                    {
                        var key = GL.GetActiveAttrib(id, i, out int size, out ActiveAttribType type);
                        var location = GL.GetAttribLocation(id, key);
                        _attributes[key] = new(i, location, size, type);
                    }
                    GL.GetProgram(id, GetProgramParameterName.ActiveUniforms, out var uniformsCount);
                    for (var i = 0; i < uniformsCount; i++)
                    {
                        var key = GL.GetActiveUniform(id, i, out int size, out ActiveUniformType type);
                        var location = GL.GetUniformLocation(id, key);
                        _uniforms[key] = new(i, location, size, type);
                    }
                }
                finally
                {
                    shaders.ForEach(s => s.Dispose());
                }
            }

            public void Dispose() =>
                GL.DeleteProgram(id);

            internal void link()
            {
                GL.LinkProgram(id);
                GL.GetProgram(id, GetProgramParameterName.LinkStatus, out int status);
                if (status == (int)All.False)
                {
                    string infoLog = GL.GetProgramInfoLog(id);
                    throw new Exception($"Program linking failed: {infoLog}");
                }
            }

            public object this[string key]
            {
                set
                {
                    var info = _uniforms[key];
                    GL.UseProgram(id);
                    switch (value)
                    {
                        case int ival:
                            GL.Uniform1(info.location, ival);
                            break;
                        case float fval:
                            GL.Uniform1(info.location, fval);
                            break;
                        case Vector3i iv3val:
                            GL.Uniform3(info.location, iv3val);
                            break;
                        case Vector3 v3val:
                            GL.Uniform3(info.location, v3val);
                            break;
                        case Matrix4 m4val:
                            GL.UniformMatrix4(info.location, false, ref m4val);
                            break;
                        default:
                            throw new NotSupportedException($"Type '{value.GetType()}' not supported.");
                    }
                }
                get
                {
                    var info = _uniforms[key];
                    //Console.WriteLine($"Uniform '{key}' size: {info.size}, type: {info.type}");
                    switch (info.type)
                    {
                        case ActiveUniformType.Int:
                            int[] ival = new int[info.size];
                            GL.GetUniform(id, info.location, ival);
                            return info.size == 1 ? ival[0] : ival;

                        case ActiveUniformType.Float:
                            float[] fval = new float[info.size];
                            GL.GetUniform(id, info.location, fval);
                            return info.size == 1 ? fval[0] : fval;

                        case ActiveUniformType.IntVec3:
                            Vector3i[] iv3val = new Vector3i[1];
                            GL.GetUniform(id, info.location, out iv3val[0].X);
                            return info.size == 1 ? iv3val[0] : iv3val;

                        case ActiveUniformType.FloatVec3:
                            Vector3[] v3val = new Vector3[info.size];
                            GL.GetUniform(id, info.location, out v3val[0].X);
                            return info.size == 1 ? v3val[0] : v3val;

                        case ActiveUniformType.FloatMat4:
                            Matrix4[] m4val = new Matrix4[info.size];
                            GL.GetUniform(id, info.location, out m4val[0].Row0.X);
                            return info.size == 1 ? m4val[0] : m4val;

                        default:
                            throw new NotSupportedException($"Type '{info.type}' not supported.");
                    }
                }
            }
        }

        public Program program(string vertex_shader, string fragment_shader)
        {
            return new Program([
                new(ShaderType.VertexShader, vertex_shader),
                new(ShaderType.FragmentShader, fragment_shader)
            ]);
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
                internal record ElementToken
                {
                    public enum ElementType
                    {
                        f, i, u, x
                    }

                    internal readonly ElementType type;
                    internal readonly int size; // = 4;
                    internal readonly int count; // = 1;
                    internal readonly string attr;

                    // parse a token: [count]type[size]
                    public ElementToken(string tokenstr, string attr)
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
                            this.size = (this.type == ElementType.x) ? 1 : 4;

                        // Validate the size for the given type using the allowed combinations.

                        var validSize = this.type switch
                        {
                            // valid: 1 (unsigned byte normalized), 2 (half float), 4 (float), 8 (double)
                            ElementType.f => (size == 1 || size == 2 || size == 4 || size == 8),

                            // valid: 1 (byte), 2 (short), 4 (int)
                            ElementType.i => (size == 1 || size == 2 || size == 4),

                            // valid: 1 (unsigned byte), 2 (unsigned short), 4 (unsigned int)
                            ElementType.u => (size == 1 || size == 2 || size == 4),

                            // valid: 1, 2, 4, 8 (padding bytes)
                            ElementType.x => (size == 1 || size == 2 || size == 4 || size == 8),

                            _ => throw new Exception($"Unknown type specifier: {this.type}"),
                        };

                        if (!validSize)
                            throw new Exception($"Invalid size {this.size} for type '{this.type}' in token '{tokenstr}'.");

                        this.attr = attr;
                    }

                    public VertexAttribPointerType ptype
                    {
                        get {
                            return type switch
                            {
                                ElementType.f => size switch
                                {
                                    1 => VertexAttribPointerType.Float,
                                    2 => VertexAttribPointerType.HalfFloat,
                                    4 => VertexAttribPointerType.Float,
                                    8 => VertexAttribPointerType.Double,
                                    _ => VertexAttribPointerType.Float
                                },
                                ElementType.i => size switch
                                {
                                    1 => VertexAttribPointerType.Byte,
                                    2 => VertexAttribPointerType.Short,
                                    _ => VertexAttribPointerType.Int
                                },
                                ElementType.u => size switch
                                {
                                    1 => VertexAttribPointerType.UnsignedByte,
                                    2 => VertexAttribPointerType.UnsignedShort,
                                    _ => VertexAttribPointerType.UnsignedInt
                                },
                                ElementType.x => size switch
                                {
                                    1 => VertexAttribPointerType.Byte,
                                    2 => VertexAttribPointerType.Short,
                                    4 => VertexAttribPointerType.Float,
                                    8 => VertexAttribPointerType.Double,
                                    _ => VertexAttribPointerType.Byte
                                },
                                _ => VertexAttribPointerType.Float
                            };
                        }
                    }

                    public bool normalized
                    {
                        get => type == ElementType.f && size == 1; // vbo_format is "[n]f[1]"
                    }

                    public int bytes { get => size * count; }

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

                internal readonly int divisor; // = 0?;

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

                        this.divisor = this.usage switch
                        {
                            BufferUsage.vertex => 0,
                            BufferUsage.instance => 1,
                            BufferUsage.render => 0x7fffffff,
                            _ => throw new Exception($"Unknown usage specifier: {this.usage}"),
                        };
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
            // always count of bytes in the buffer
            public int length;

            public Buffer() =>
                this.id = GL.GenBuffer();

            public Buffer(object data, bool dynamic = false) : this()
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, id);
                var DrawType = dynamic ? BufferUsageHint.DynamicDraw : BufferUsageHint.StaticDraw;
                length = ((Array)data).Length;
                switch (data)
                {
                    case byte[] byteData:
                        length *= sizeof(byte);
                        GL.BufferData(BufferTarget.ArrayBuffer, length, byteData, DrawType);
                        break;
                    case int[] integerData:
                        length *= sizeof(int);
                        GL.BufferData(BufferTarget.ArrayBuffer, length, integerData, DrawType);
                        break;
                    case float[] floatData:
                        length *= sizeof(float);
                        GL.BufferData(BufferTarget.ArrayBuffer, length, floatData, DrawType);
                        break;
                    case double[] doubleData:
                        length *= sizeof(double);
                        GL.BufferData(BufferTarget.ArrayBuffer, length, doubleData, DrawType);
                        break;
                    default:
                        throw new NotSupportedException($"Type '{data.GetType()}' not supported.");
                }
                Console.WriteLine($"buffer id {id} length {length} type '{data.GetType()}' error:" + GL.GetError());
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
        /// Returns a new Buffer object.
        /// The data can be anything supporting the buffer interface.
        /// The data and reserve parameters are mutually exclusive.
        /// </summary>
        public Buffer buffer(object data, bool dynamic = false)
        {
            return new Buffer(data, dynamic);
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
            private readonly Program program;

            private int num_vertices;
            private int num_instances;

            protected VertexArray(Program program) =>
                (this.id, this.program) = (GL.GenVertexArray(), program);

            public VertexArray(Program program, (Buffer vbo, string vbo_format, string[] attrs)[] content, bool skip_errors = false)
                : this(program)
            {
                GL.BindVertexArray(id);
                foreach (var item in content)
                    bind_content(item, skip_errors);
                GL.BindVertexArray(0);
            }


            private void bind_content((Buffer vbo, string vbo_format, string[] attrs) content, bool skip_errors)
            {
                var vbo_tokens = new Buffer.BufferFormat(content.vbo_format, content.attrs);

                this.num_vertices = content.vbo.length / vbo_tokens.stride;
                this.num_instances = 1;

                GL.BindBuffer(BufferTarget.ArrayBuffer, content.vbo.id);

                var offset = 0;
                foreach (var token in vbo_tokens)
                {
                    if (token.type == Buffer.BufferFormat.ElementToken.ElementType.x)
                    {
                        Console.WriteLine($"padding bytes vbo {content.vbo.id} token '{token}' stride {vbo_tokens.stride} offset {offset}  ");
                        offset += token.bytes;
                        continue;
                    }
                    if (!program._attributes.ContainsKey(token.attr))
                    {
                        offset += token.bytes;
                        if (!skip_errors)
                            throw new ArgumentException($"attribute {token.attr} not found in shader program.");
                        else continue;
                    }

                    var attr = program._attributes[token.attr];
                    if (attr.IsFloat)
                    {
                        GL.VertexAttribPointer(attr.location, token.count, token.ptype, token.normalized, vbo_tokens.stride, offset);
                    }
                    else if (attr.IsInt || attr.IsUInt)
                    {
                        GL.VertexAttribIPointer(attr.location, token.count, (VertexAttribIntegerType)token.ptype, vbo_tokens.stride, offset);
                    }
                    else if (attr.IsDouble)
                    {
                        GL.VertexAttribLPointer(attr.location, token.count, (VertexAttribDoubleType)token.ptype, vbo_tokens.stride, offset);
                    }
                    else throw new NotSupportedException($"attrib ype '{attr.type}' not supported.");

                    Console.WriteLine("error:" + GL.GetError());

                    GL.VertexAttribDivisor(attr.location, vbo_tokens.divisor);
                    GL.EnableVertexAttribArray(attr.location);

                    offset += token.bytes;
                }
            }


            public void Dispose() =>
                GL.DeleteVertexArray(id);

            public void render(PrimitiveType mode = PrimitiveType.Triangles, int vertices = -1, int first = 0, int instances = -1)
            {
                GL.UseProgram(program.id);
                GL.BindVertexArray(id);

                if (vertices < 0)
                    vertices = this.num_vertices;
                if (instances < 0)
                    instances = this.num_instances;

                //if (self->index_buffer != None)
                //    GL.DrawElementsInstanced(mode, vertices, self->index_element_type, ptr, instances);
                //else
                //GL.DrawArrays(mode, 0, vertices);
                GL.DrawArraysInstanced(mode, first, vertices, instances);
            }
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


        public class Texture : IDisposable
        {
            private readonly (int Width, int Height, int layers) size;
            private readonly int components;
            private readonly int samples;
            private readonly int id;

            private const int max_texture_units = 32;
            private readonly int default_texture_unit = max_texture_units - 1;

            private TextureTarget texture_target =>
                this.samples > 0 ? TextureTarget.Texture2DMultisample: TextureTarget.Texture2D;

            private PixelType pixel_type = PixelType.UnsignedByte; // data_type->gl_type;
            private PixelFormat base_format = PixelFormat.Rgba; // data_type->base_format[components];
            private PixelInternalFormat internal_format = PixelInternalFormat.Rgba; // data_type->internal_format[components];

            public void Dispose() =>
                GL.DeleteTexture(this.id);

            public Texture() =>
                this.id = GL.GenTexture();

            public Texture((int Width, int Height, int layers) size, int components, int samples) : this() =>
                (this.size, this.components, this.samples) = (size, components, samples);

            public Texture((int Width, int Height, int layers) size, int components, byte[] data,
                int samples = 0, int alignment = 1, bool is_array=false) : this(size, components, 0)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + default_texture_unit);
                GL.BindTexture(this.texture_target, this.id);

                //texture->data_type = data_type;
                if (this.samples > 0)
                {
                    GL.TexImage2DMultisample((TextureTargetMultisample)this.texture_target,
                        this.samples, internal_format, size.Width, size.Height, true);
                }
                else
                {
                    GL.PixelStore(PixelStoreParameter.PackAlignment, alignment);
                    GL.PixelStore(PixelStoreParameter.UnpackAlignment, alignment);

                    GL.TexImage2D(this.texture_target, 0, internal_format, size.Width, size.Height,0, base_format, pixel_type, data);
                    // !float_type
                    filter = (fTypes.NEAREST, fTypes.NEAREST);
                }
            }

            private int max_level = 0;
            public void build_mipmaps(int _base = 0, int max_level = 1000)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + default_texture_unit);
                GL.BindTexture(this.texture_target, this.id);

                if (_base > max_level)
                {
                    Console.WriteLine($"invalid base {_base} > max_level {max_level}");
                    return;
                }
                GL.TexParameter(this.texture_target, TextureParameterName.TextureBaseLevel, _base);
                GL.TexParameter(this.texture_target, TextureParameterName.TextureMaxLevel, (this.max_level = max_level));

                // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
                // Generated mipmaps go all the way down to just one pixel.
                // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
                GL.GenerateMipmap((GenerateMipmapTarget)this.texture_target);

                //  not necessary?
                //filter = (fTypes.LINEAR_MIPMAP_LINEAR, fTypes.LINEAR);
            }

            // The location is the texture unit we want to bind the texture.
            // This should correspond with the value of the sampler2D uniform in the shader because
            // samplers read from the texture unit we assign to them:
            //   location(int) – The texture location/unit.
            public void use(int location=0)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + location);
                GL.BindTexture(this.texture_target, this.id);
            }

            public enum fTypes
            {
                LINEAR = (int)TextureMinFilter.Linear,
                NEAREST = (int)TextureMinFilter.Nearest,
                LINEAR_MIPMAP_LINEAR = (int)TextureMinFilter.LinearMipmapLinear,
                NEAREST_MIPMAP_LINEAR = (int)TextureMinFilter.NearestMipmapLinear,
                LINEAR_MIPMAP_NEAREST = (int)TextureMinFilter.LinearMipmapNearest,
            };

            private (fTypes, fTypes) _filter = (fTypes.NEAREST_MIPMAP_LINEAR, fTypes.LINEAR);
            public (fTypes, fTypes) filter
            {
                get => _filter;
                internal set
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + default_texture_unit);
                    GL.BindTexture(this.texture_target, this.id);

                    var (min_filter, mag_filter) = (_filter = value);
                    GL.TexParameter(this.texture_target, TextureParameterName.TextureMinFilter, (int)min_filter);
                    GL.TexParameter(this.texture_target, TextureParameterName.TextureMagFilter, (int)mag_filter);
                }
            }

            private bool _repeat_x = true;
            public bool repeat_x
            {
                get => _repeat_x;
                internal set
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + default_texture_unit);
                    GL.BindTexture(this.texture_target, this.id);
                    GL.TexParameter(this.texture_target, TextureParameterName.TextureWrapS,
                        (_repeat_x = value) ? (int)TextureWrapMode.Repeat : (int)TextureWrapMode.ClampToEdge);
                }
            }

            private bool _repeat_y = true;
            public bool repeat_y
            {
                get => _repeat_y;
                internal set
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + default_texture_unit);
                    GL.BindTexture(this.texture_target, this.id);
                    GL.TexParameter(this.texture_target, TextureParameterName.TextureWrapT,
                        (_repeat_y = value) ? (int)TextureWrapMode.Repeat : (int)TextureWrapMode.ClampToEdge);
                }
            }

            private float _anisotropy = 0.0f;
            public float anisotropy
            {
                get => _anisotropy;
                internal set
                {
                    GL.ActiveTexture(TextureUnit.Texture0 + default_texture_unit);

                    GL.BindTexture(this.texture_target, this.id);
                    GL.TexParameter(this.texture_target, TextureParameterName.TextureMaxAnisotropy, _anisotropy = value);
                    Console.WriteLine("TextureMaxAnisotropy error:" + GL.GetError());
                }
            }
        }


        public Texture texture((int Width, int Height) size, int components, byte[] data,
            int samples = 0, int alignment = 1, string dtype = "f1")
        {
            return new Texture((size.Width, size.Height, 0), components, data, samples, alignment);
        }

        public Texture texture_array((int Width, int Height, int layers) size, int components, byte[] data,
            int samples = 0, int alignment = 1, string dtype = "f1")
        {
            return new Texture(size, components, data, samples, alignment, is_array: true);
        }


        [Flags]
        // these are for ctx.Enable() function
        public enum EnableFlags
        {
            NOTHING = 0,
            BLEND = 1,
            DEPTH_TEST = 2,
            CULL_FACE = 4,
            RASTERIZER_DISCARD = 8,
            PROGRAM_POINT_SIZE = 16,
            INVALID = 0x40000000,
        }
        public void enable(EnableFlags flags = EnableFlags.NOTHING)
        {
            if (flags.HasFlag(EnableFlags.DEPTH_TEST))
                GL.Enable(EnableCap.DepthTest);
            if (flags.HasFlag(EnableFlags.CULL_FACE))
                GL.Enable(EnableCap.CullFace);
            if (flags.HasFlag(EnableFlags.BLEND))
                GL.Enable(EnableCap.Blend);
        }

        internal void set_clearcolor(System.Drawing.Color color)
        {
            GL.ClearColor(color);// Color4.CornflowerBlue);
        }
    };


#pragma warning disable CS8981
    public static class moderngl
#pragma warning restore CS8981
    {
        private static glContext? instance;

        public static glContext create_context()
        {
            if (instance == null)
            {
                instance = new glContext();
                GL.Enable(EnableCap.DebugOutput);
                GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);
            }
            return instance;
        }
        private static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
        {
            var messageString = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(message, length);
            Console.WriteLine($"OpenGL Debug:\n{messageString}");
        }
    }
}
