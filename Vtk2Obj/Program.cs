using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IsoSurfaceToObj {
    class Program {
        private static bool swapYZ = true;
        static void Main(string[] args) {
            //settings - precision - set in significatn figutres Digits
            // Swap YZ 

            if (args.Length != 1) {
                Console.WriteLine("requires a .vtk file or a directory as an argument");
                return;
            }

            string vtkfile = args[0];
            if (File.Exists(vtkfile)) {
                ConvertVtkFile(vtkfile);
            }
            else {
                if (Directory.Exists(vtkfile)) {
                    foreach (var file in Directory.GetFiles(vtkfile)) {
                        ConvertVtkFile(file);
                    }
                }
            }            

        }

        private static void ConvertVtkFile(string vtkfile) {
            using (var file = File.ReadLines(vtkfile).GetEnumerator()) {
                Console.WriteLine("Starting " + vtkfile);

                if (!ReadUntilLine(file, "# vtk DataFile Version 4.2")) return;
                if (!ReadUntilLine(file, "ASCII")) return;
                if (!ReadUntilLine(file, "DATASET POLYDATA")) return;

                if (!ReadUntilLineStartsWith(file, "POINTS")) return;

                double[,] points = ReadPoints(file);

                if (!ReadUntilLineStartsWith(file, "POLYGONS")) return;

                int[,] triangles = ReadTriangles(file);

                if (!ReadUntilLineStartsWith(file, "NORMALS")) return;
                file.MoveNext();
                double[,] normals = ReadDoublePoints(file, triangles.GetLength(0));

                Console.WriteLine("Generating Obj");
                var objfile = WriteAnObjFile(points, normals, null, triangles);

                string objFilePath = Path.ChangeExtension(vtkfile, ".obj");
                Console.WriteLine("Writing Obj File");
                File.WriteAllText(objFilePath, objfile.ToString());
                Console.WriteLine("Finished " + objFilePath);
            }
        }

        private static StringBuilder WriteAnObjFile(double[,] points, double[,] normals, double[,] textures, int[,] triangles,
            string groupname = "mesh", string material = "mesh") {

            // todo note that we are not checking uniqueness of the normals or the texture points, we assume this wont make a difference... see the CompressedObjWriter for something that will 

            StringBuilder file = new StringBuilder();

            // header
            file.AppendLine("# object generated from DAvid Birch vtk to obj");
            
            file.AppendLine();
            for (int v = 0; v < points.GetLength(0); v++) {
                file.AppendLine("v " + points[v, 0].ToPreciseString() + " " + points[v, swapYZ? 2 : 1].ToPreciseString() + " " + points[v, swapYZ ? 1 : 2].ToPreciseString());
            }


            if (normals != null) {
                file.AppendLine();
                for (int v = 0; v < normals.GetLength(0); v++) {
                    file.AppendLine("vn " + normals[v, 0].ToPreciseString() + " " + normals[v, swapYZ ? 2 : 1].ToPreciseString() + " " + normals[v, swapYZ ? 1 : 2].ToPreciseString());
                }
            }

            if (textures != null) {
                file.AppendLine();
                for (int v = 0; v < textures.GetLength(0); v++) {
                    file.AppendLine("vt " + textures[v, 0].ToPreciseString() + " " + textures[v, swapYZ ? 2 : 1].ToPreciseString() + " " + textures[v, swapYZ ? 1 : 2].ToPreciseString());
                }
            }


            file.AppendLine();
            file.AppendLine("g " + groupname);
            file.AppendLine("usemtl " + material);


            for (int t = 0; t < triangles.GetLength(0); t++) {
                // note indexes are 1 based
                file.AppendLine("f " 
                        + (triangles[t, 0]+1) + "/" + (textures == null ? "" : (t+1).ToString()) + (normals == null ? "" : "/" + (t+1)) + " "
                        + (triangles[t, 1]+1) + "/" + (textures == null ? "" : (t+1).ToString()) + (normals == null ? "" : "/" + (t+1)) + " "
                        + (triangles[t, 2]+1) + "/" + (textures == null ? "" : (t+1).ToString()) + (normals == null ? "" : "/" + (t+1)) );
            }

            return file;
        }
    

        private static int[,] ReadTriangles(IEnumerator<string> file) {
            Console.WriteLine("Parsing Triangles");
            var header = file.Current;
            int numberofTriangles = int.Parse(header.Split(new[] { " " }, StringSplitOptions.None)[1]);// todo error handling 
            // POLYGONS 1504 6016
            int[,] points = new int[numberofTriangles, 3];

            file.MoveNext();

            int curr = 0;

            while (file.Current != string.Empty && curr<numberofTriangles) {// read until a new line as the number of doubles per line is undefined

                var ints = file.Current.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(); //todo error handling

                    if (ints[0]!= 3) Console.WriteLine("found a Polygon which is not a triangle");

                    points[curr, 0] = ints[1];
                    points[curr, 1] = ints[2];
                    points[curr, 2] = ints[3];
                    curr++;

                if (!file.MoveNext()) {
                    Console.WriteLine("could not find all " + numberofTriangles + " triangles before the file ended");
                    return null;
                }
            }

            if (curr != numberofTriangles) {
                Console.WriteLine("did not find all " + numberofTriangles+" triangles");
                return null;
            }

            return points;
        }

        private static double[,] ReadPoints(IEnumerator<string> file) {
            Console.WriteLine("Parsing Points");
            var header = file.Current;
            int numberofPoints = int.Parse(header.Split(new[] {" "}, StringSplitOptions.None)[1]);// todo error handling 
            // POINTS 1629 double
            

            file.MoveNext();

            return ReadDoublePoints(file, numberofPoints);
        }

        private static double[,] ReadDoublePoints(IEnumerator<string> file, int numberofPoints) {
            Console.WriteLine("Parsing "+numberofPoints+" 3d points as doubles");
            double[,] points = new double[numberofPoints, 3];
            int curr = 0;

            while (file.Current != string.Empty && curr<numberofPoints) {
// read until a new line as the number of doubles per line is undefined

                var doubles = file.Current.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(double.Parse)
                    .ToList(); //todo error handling

                for (int i = 0; i < doubles.Count; i += 3) {
                    points[curr, 0] = doubles[i];
                    points[curr, 1] = doubles[i + 1];
                    points[curr, 2] = doubles[i + 2];
                    curr++;
                }


                if (!file.MoveNext()) {
                    Console.WriteLine("could not find all "+numberofPoints+" before the file ended");
                    return null;
                }
            }

            if (curr != numberofPoints ) {
                Console.WriteLine("did not find all "+numberofPoints);
                return null;
            }

            return points;
        }

        private static bool ReadUntilLineStartsWith(IEnumerator<string> file, string line) {
            Console.WriteLine("Searching for line starting with "+line);
            while (!file.Current.StartsWith(line)) {
                if (!file.MoveNext()) {
                    Console.WriteLine("could not find line starting with " + line);
                    return false;
                }

            }

            return true;
        }

        private static bool ReadUntilLine(IEnumerator<string> file, string line) {
            Console.WriteLine("Searching for line " + line);

            while (file.Current != line) {
                if (!file.MoveNext()) {
                    Console.WriteLine("could not find line "+line);
                    return false;
                }

            }

            return true;
        }
    }
}
