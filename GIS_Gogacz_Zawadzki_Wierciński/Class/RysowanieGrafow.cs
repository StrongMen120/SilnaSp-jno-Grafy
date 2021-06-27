using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using QuickGraph;
using QuickGraph.Graphviz.Dot;

namespace GIS.KlasyQuickGraph
{
    public class FileDotEngine : QuickGraph.Graphviz.IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            using (StreamWriter writer = new StreamWriter(outputFileName))
            {
                writer.Write(dot);
            }
            return Path.GetFileName(outputFileName);
        }

        public static Bitmap Run(string dot)
        {
            string executable = @".\external\dot.exe";
            string output = @".\external\gprz.dt";
            File.WriteAllText(output, dot);

            System.Diagnostics.Process process = new System.Diagnostics.Process();

            // Stop the process from opening a new window
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // Setup executable and parameters
            process.StartInfo.FileName = executable;
            process.StartInfo.Arguments = string.Format(@"{0} -Tjpg -O", output);

            // Go
            process.Start();
            // and wait dot.exe to complete and exit
            process.WaitForExit();
            Bitmap bitmap = null; ;
            using (Stream bmpStream = System.IO.File.Open(output + ".jpg", System.IO.FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                bitmap = new Bitmap(image);
            }
            //File.Delete(output);
            //File.Delete(output + ".jpg");
            return bitmap;
        }
    }
    public class RysowanieGrafow
    {
        public AdjacencyGraph<char, TaggedEdge<char, int>> convertToGraph(int[,] macierz)
        {
            AdjacencyGraph<char, TaggedEdge<char, int>> graf = new AdjacencyGraph<char, TaggedEdge<char, int>>();

            for (int i = 0; i < macierz.GetLength(0); i++)
            {
                char wierzcholek = Convert.ToChar(i);
                graf.AddVertex(wierzcholek);
            }

            for (int i = 0; i < macierz.GetLength(0); i++)
            {
                for (int j = 0; j < macierz.GetLength(0); j++)
                {
                    if (macierz[i, j] == 1)
                    {
                        var krawedz = new TaggedEdge<char, int>((char)i, (char)j, 1);
                        graf.AddEdge(krawedz);
                    }
                }
            }

            return graf;
        }

        public void wygenerujObrazGrafu(AdjacencyGraph<char, TaggedEdge<char, int>> graf)
        {
            var graphviz = new QuickGraph.Graphviz.GraphvizAlgorithm<char, TaggedEdge<char, int>>(graf);

            FileDotEngine fileDotEngine = new FileDotEngine();
            graphviz.Generate(new FileDotEngine(), "graf.jpg");
            Bitmap bm = FileDotEngine.Run(Convert.ToString(graphviz.Output));
        }
    }
}