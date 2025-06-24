using System;
using System.IO;
using System.Threading.Tasks;
using PlantUml.Net;

namespace LegoInventoryApi
{
    public static class PlantUmlGenerator
    {
        public static async Task<int> Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Usage: PlantUmlGenerator <inputFile> <outputDirectory>");
                return 1;
            }

            string inputFile = args[0];
            string outputDirectory = args[1];

            try
            {
                Console.WriteLine($"Generating diagram from {inputFile}");
                
                var factory = new RendererFactory();
                var renderer = factory.CreateRenderer();
                
                string plantUmlContent = File.ReadAllText(inputFile);
                string fileName = Path.GetFileNameWithoutExtension(inputFile);
                string outputPath = Path.Combine(outputDirectory, $"{fileName}.png");
                
                var bytes = await renderer.RenderAsync(plantUmlContent, OutputFormat.Png);
                File.WriteAllBytes(outputPath, bytes);
                
                Console.WriteLine($"Generated diagram at {outputPath}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Failed to generate diagram: {ex.Message}");
                return 1;
            }
        }
    }
}