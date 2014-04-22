using System;
using System.Collections.Generic;
using System.Linq;

namespace BailerMethod
{
    class Program
    {
        static void Main()
        {
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
            Console.WriteLine();
            Console.WriteLine("{0} Resulting Complexes ({1} Eantiomer Pairs):", results.Count, results.Count(i => i.Chiral) / 2);

            foreach (var complex in results)
            {
                Console.WriteLine(complex);
            }
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        } 
    }
}
