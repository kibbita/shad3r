using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTK_3D
{
    public class Camera
    {
        
        private Matrix4 modelMatrix; //transform the object in the world
        public Matrix4 viewMatrix; // The view matrix moves the camera in the world
        private Matrix4 projectionMatrix; // The projection matrix defines how the 3D world is projected into 2D (the screen).

        public Vector3 position = new Vector3(0.0f, 0.0f, 10.0f); // Camera starting position
        public Vector3 target = Vector3.Zero; // Look at the origin
        public Vector3 cameraUp = Vector3.UnitY; // Up direction

        public float angle = 0.0f;

        public void Initialize(int sizeX, int sizeY)
        {
            modelMatrix = Matrix4.Identity; // No transformation initially
            modelMatrix = Matrix4.CreateScale(0.5f); // Reduce el tamaño a la mitad

            viewMatrix = Matrix4.LookAt(position, target, cameraUp); // Camera setup

            var fov = 60f;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(fov), // Field of View
                (float)sizeX / sizeY, // Aspect Ratio
                0.2f, // Near Plane
                100.0f // Far Plane
            );
        }

        public void SendMatricesToShader(int shaderProgram)
        {
            int modelLoc = GL.GetUniformLocation(shaderProgram, "model");
            int viewLoc = GL.GetUniformLocation(shaderProgram, "view");
            int projLoc = GL.GetUniformLocation(shaderProgram, "projection");

            GL.UniformMatrix4(modelLoc, false, ref modelMatrix);
            GL.UniformMatrix4(viewLoc, false, ref viewMatrix);
            GL.UniformMatrix4(projLoc, false, ref projectionMatrix);
        }
    }


}
