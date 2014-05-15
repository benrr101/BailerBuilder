using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BailerBuilder
{
    class Program
    {
        static void Main()
        {
            // Generate an output file
            DateTime now = DateTime.Now;
            string fileName = String.Format("{0}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}_Compounds.txt", now.Year, now.Month,
                now.Day, now.Hour, now.Minute, now.Second);
            Stream outputFile;
            StreamWriter outputWriter;
            try
            {
                outputFile = File.Create(fileName);
                outputWriter = new StreamWriter(outputFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("*** Failed to create output file: {0}", e.Message);
                Console.WriteLine("Press any key to exit");
                Console.ReadLine();
                return;
            }
            Console.WriteLine("Compounds generated during this session will be stored in: {0}", fileName);

            // Generate the header for the file
            outputWriter.Write(OctahedralComplex.GetFormatString());

            // Get the metal center and ligand list
            Console.WriteLine("6 Ligands, comma separated: ");
            string ligands = Console.ReadLine();
            if (String.IsNullOrWhiteSpace(ligands))
            {
                Console.WriteLine("*** 6 comma separated ligands must be provided");
                return;
            }
            List<string> ligandList = new List<string>(ligands.Trim().Split(','));
            if (ligandList.Count != 6)
            {
                Console.WriteLine("*** 6 comma separated ligands must be provided");
                return;
            }

            // Do the magic! Generate the complexes
            var results = OctahedralComplex.GenerateComplexes(ligandList);

            // Output the results
            outputWriter.WriteLine("\n\nLigands Given: {0}", ligands);
            string resultingString = String.Format("{0} Resulting Complexes ({1} Eantiomer Pairs):", results.Count, results.Count(i => i.Chiral) / 2);
            Console.WriteLine();
            Console.WriteLine(resultingString);
            outputWriter.WriteLine(resultingString);

            foreach (var complex in results)
            {
                Console.WriteLine(complex);
                outputWriter.WriteLine(complex);
            }
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();

            outputWriter.Close();
            outputFile.Close();
        } 
    }
}
