using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTK_3D
{
    public class MyGameWindow : GameWindow
    {
        // Buffers and shader program
        private int vao, vbo, ebo, shaderProgram, vboNormals;
        private ObjLoader model;
        Camera Camera = new Camera();


        public MyGameWindow() : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            this.CenterWindow(new Vector2i(1280, 720)); // Center the window
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            // Load 3D model from an .obj file
            model = new ObjLoader("../../../Models/teddy.obj");

            // Generate OpenGL Buffers
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            vboNormals = GL.GenBuffer();
            ebo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            // Upload vertex data to GPU
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, model.Vertices.Count * Vector3.SizeInBytes, model.Vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            // Upload normal data to GPU (as a separate VBO)
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
            GL.BufferData(BufferTarget.ArrayBuffer, model.Normals.Count * Vector3.SizeInBytes, model.Normals.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.EnableVertexAttribArray(1);

            // Upload index data to GPU
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, model.Indexes.Count * sizeof(int), model.Indexes.ToArray(), BufferUsageHint.StaticDraw);

            // Unbind VAO
            GL.BindVertexArray(0);

            // Compile and use shaders
            shaderProgram = BuildShaders();
            GL.UseProgram(shaderProgram);

            Camera.Initialize(Size.X, Size.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(shaderProgram);
            GL.BindVertexArray(vao);

            int objectColorLoc = GL.GetUniformLocation(shaderProgram, "objectColor");
            GL.Uniform3(objectColorLoc, new Vector3(0.5f, 0.8f, 1.0f)); // Light blue

            // Light position and color
            int lightPosLoc = GL.GetUniformLocation(shaderProgram, "lightPos");
            GL.Uniform3(lightPosLoc, new Vector3(20.0f, 20.0f, 20.0f)); // Light position

            int lightColorLoc = GL.GetUniformLocation(shaderProgram, "lightColor");
            GL.Uniform3(lightColorLoc, new Vector3(1.0f, 1.0f, 1.0f)); // White light

            // Camera position for specular lighting
            int viewPosLoc = GL.GetUniformLocation(shaderProgram, "viewPos");
            GL.Uniform3(viewPosLoc, Camera.position);

            Camera.SendMatricesToShader(shaderProgram);

            // Draw the model using indices
            GL.DrawElements(PrimitiveType.Triangles, model.Indexes.Count, DrawElementsType.UnsignedInt, 0);

            SwapBuffers(); // Display the frame
        }

        protected override void OnUnload()
        {
            // Cleanup GPU resources
            GL.DeleteVertexArray(vao);
            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(ebo);
            GL.DeleteProgram(shaderProgram);
            base.OnUnload();
        }

        private int BuildShaders()
        {
            // Load shader source code from files
            string vertexShaderCode = Utils.LoadShaderCode("../../../Shaders/vertexShader.glsl");
            string fragmentShaderCode = Utils.LoadShaderCode("../../../Shaders/fragmentShader.glsl");

            // Compile vertex shader
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderCode);
            GL.CompileShader(vertexShader);


            // Compile fragment shader
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderCode);
            GL.CompileShader(fragmentShader);

            // Link shaders into a program
            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            // Cleanup individual shaders (they are now linked)
            GL.DetachShader(program, vertexShader);
            GL.DetachShader(program, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height); // Adjust viewport on window resize
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            Camera.angle += 2f * (float)args.Time; // Rotate speed

            float radiusFromCenter = 25.0f;
            float camX = MathF.Sin(Camera.angle) * radiusFromCenter;
            float camZ = MathF.Cos(Camera.angle) * radiusFromCenter;

            Camera.position = new Vector3(camX, 0.0f, camZ);
            Camera.viewMatrix = Matrix4.LookAt(Camera.position, Camera.target, Camera.cameraUp);
        }
    }
}
