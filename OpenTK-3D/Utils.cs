using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTK_3D
{
    public static class Utils
    {
        public static string LoadShaderCode(string path)
        {
            if (!File.Exists(path)) {
                throw new FileNotFoundException("File shader couldn't be loaded");
            }
            return File.ReadAllText(path);
        }



    }
}
