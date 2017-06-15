using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsoSurfaceToObj {
    /// <summary>
    /// TODO this is an optimised .obj writer using LINQ, it will compress the .obj be recycling normals between faces
    /// </summary>
    class CompressedObjWriter {
     /*   public static Tuple<string, List<Vertex>, List<Vertex>, List<Vertex>> WriteObjFile(IEnumerable<Triangle> triangles, bool includeNormals, string groupname, string material,
            List<Vertex> vertexes, List<Vertex> normals, List<Vertex> textures) {
            StringBuilder file = new StringBuilder();

            int vertCount = vertexes.Count;
            int normalCount = normals.Count;
            int texCount = textures.Count;

            // header
            file.AppendLine("# object generated from shape file");
            file.AppendLine();
            file.AppendLine("g " + groupname);// todo should we set each building as a separate grouping?
            file.AppendLine("usemtl " + material);
            List<String> faces = new List<String>();

            foreach (Triangle triangle in triangles) {
                string face = "f ";

                face += vertexes.IndexOfOrInsertFrom1(triangle.Pt1) + "/" + textures.IndexOfOrInsertFrom1(triangle.Pt1T) +
                        (includeNormals ? "/" + normals.IndexOfOrInsertFrom1(triangle.Pt1N) + " " : " ");
                face += vertexes.IndexOfOrInsertFrom1(triangle.Pt2) + "/" + textures.IndexOfOrInsertFrom1(triangle.Pt2T) +
                        (includeNormals ? "/" + normals.IndexOfOrInsertFrom1(triangle.Pt2N) + " " : " ");
                face += vertexes.IndexOfOrInsertFrom1(triangle.Pt3) + "/" + textures.IndexOfOrInsertFrom1(triangle.Pt3T) +
                        (includeNormals ? "/" + normals.IndexOfOrInsertFrom1(triangle.Pt3N) + " " : " ");
                faces.Add(face);
            }

            file.AppendLine();
            foreach (Vertex vertex in vertexes.Skip(vertCount)) {
                file.AppendLine("v " + vertex.X + " " + vertex.Y + " " + vertex.Z);
            }

            if (includeNormals) {
                file.AppendLine();
                foreach (Vertex vertex in normals.Skip(normalCount)) {
                    file.AppendLine("vn " + vertex.X + " " + vertex.Y + " " + vertex.Z);
                }
            }

            file.AppendLine();
            foreach (Vertex vertex in textures.Skip(texCount)) {
                file.AppendLine("vt " + vertex.X + " " + vertex.Y + " " + vertex.Z);
            }

            file.AppendLine();
            foreach (string face in faces) {
                file.AppendLine(face);
            }

            return new Tuple<string, List<Vertex>, List<Vertex>, List<Vertex>>(file.ToString(), vertexes, normals, textures);
        }*/

    }

    public static class ListExtentions {

        public static int IndexOfOrInsertFrom1<T>(this List<T> list, T obj2Find) {
            int index = list.IndexOf(obj2Find);
            if (index != -1) {
                return index + 1;
            }
            list.Add(obj2Find);
            return list.IndexOf(obj2Find) + 1;

        }
    }
}
