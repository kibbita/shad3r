using OpenTK.Mathematics;

namespace OpenTK_3D
{
    public class ObjLoader
    {
        public List<Vector3> Vertices { get; private set; }
        public List<int> Indexes { get; private set; }
        public List<Vector3> Normals { get; private set; }
        List<int> FaceIndices { get; set; }


        public ObjLoader(string filePath)
        {
            Vertices = new List<Vector3>();
            Indexes = new List<int>();
            Normals = new List<Vector3>();
            FaceIndices = new List<int>();

            LoadObjFile(filePath);
        }

        private void LoadObjFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Error: File not found.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                if (parts[0] == "v")  // Vertices line
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    Vertices.Add(new Vector3(x, y, z));
                }
                else if (parts[0] == "f")  // Faces line (indexes)
                {

                    FaceIndices = new List<int>();

                    for (int i = 1; i <= 3; i++)
                    {
                        string[] faceParts = parts[i].Split('/');
                        int parsedIndex = int.Parse(faceParts[0]);
                        int vertexIndex = parsedIndex < 0 ? Vertices.Count + parsedIndex : parsedIndex - 1;
                        Indexes.Add(vertexIndex);
                        FaceIndices.Add(vertexIndex);
                    }

                    // AUTO CALCULATE NORMALS.

                    Vector3 v1 = Vertices[FaceIndices[0]];
                    Vector3 v2 = Vertices[FaceIndices[1]];
                    Vector3 v3 = Vertices[FaceIndices[2]];
                    Vector3 normal = CalculateFaceNormal(v1, v2, v3);

                    // Ensure the list has enough space for vertex normals
                    while (Normals.Count < Vertices.Count)
                    {
                        Normals.Add(Vector3.Zero);
                    }

                    // Accumulate the normal for each vertex
                    Normals[FaceIndices[0]] += normal;
                    Normals[FaceIndices[1]] += normal;
                    Normals[FaceIndices[2]] += normal;

                }
            }
        }

        public static Vector3 CalculateFaceNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 edge1 = v2 - v1;
            Vector3 edge2 = v3 - v1;
            return Vector3.Cross(edge1, edge2).Normalized(); // Compute and normalize
        }

    }
}
