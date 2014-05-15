using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BailerBuilder
{
    class OctahedralComplex : ICloneable, IEquatable<OctahedralComplex>
    {

        #region Properties
        
        /// <summary>
        /// Internal representation of the ligands.
        /// </summary>
        /// <remarks>
        /// Despite a list being relatively expensive compared to native
        /// arrays, this type is used to enable arbitrary removal and insertion
        /// which is necessary for generating all the isomers.
        /// </remarks>
        List<string> _ligands = new List<string> {null, null, null, null, null, null};

        public string A
        {
            get { return _ligands[0]; }
            set { _ligands[0] = value; }
        }

        public string B
        {
            get { return _ligands[1]; }
            set { _ligands[1] = value; }
        }
        public string C
        {
            get { return _ligands[2]; }
            set { _ligands[2] = value; }
        }
        public string D
        {
            get { return _ligands[3]; }
            set { _ligands[3] = value; }
        }
        public string E
        {
            get { return _ligands[4]; }
            set { _ligands[4] = value; }
        }
        public string F
        {
            get { return _ligands[5]; }
            set { _ligands[5] = value; }
        }

        /// <summary>
        /// Whether or not the complex is chiral. This is checked by looking
        /// for trans ligand pairs (which imply axial mirror planes) and
        /// dihedral mirror planes.
        /// </summary>
        public bool Chiral
        {
            get
            {
                return
                    !(A == B || C == D || E == F || (A == C && B == D) || (C == E && D == F) || (A == E && B == F) ||
                      (A == D && B == C) || (C == F && D == E) || (A == F && B == E));
            }
        }

        #endregion

        #region Static Members

        /// <summary>
        /// Performs the Bailer method to find all the unique permutations of
        /// the complex. Duplicates are checked before inserting. Eantiomer
        /// pairs are also generated and added AFTER the chiral complex that
        /// it was generated from.
        /// </summary>
        /// <param name="ligands">The list of ligands </param>
        /// <returns>A list of all unique complexes</returns>
        public static List<OctahedralComplex> GenerateComplexes(List<string> ligands)
        {
            // Sorting the ligands is very important
            ligands.Sort();

            // Create a result set
            List<OctahedralComplex> results = new List<OctahedralComplex>();

            // Create the initial complex
            OctahedralComplex initialComplex = new OctahedralComplex {_ligands = ligands};
            results.Add(initialComplex);
            var initPermutations = initialComplex.PermutateComplex();
            if (!results.Contains(initPermutations[0]))
                results.Add(initPermutations[0]);
            if (!results.Contains(initPermutations[1]))
                results.Add(initPermutations[1]);

            // Generate the unique ligands that will need a chance to be 'king'
            var remainingLigands = new List<string>(ligands);
            remainingLigands.Remove(initialComplex.A);
            var uniqueLigands = remainingLigands.Distinct().ToList();
            uniqueLigands.Remove(initialComplex.B);

            // Create the variations of the complex 
            foreach (string kingLigand in uniqueLigands)
            {
                // Insert the first instance of the new king ligand into its throne
                var newComplex = (OctahedralComplex) initialComplex.Clone();
                newComplex._ligands.RemoveAt(newComplex._ligands.IndexOf(kingLigand));
                newComplex._ligands.Insert(1, kingLigand);

                // Add the new complex and its permutations to the result set
                // Avoid adding duplicates
                if(!results.Contains(newComplex))
                    results.Add(newComplex);

                var permutations = newComplex.PermutateComplex();
                if (!results.Contains(permutations[0]))
                    results.Add(permutations[0]);
                if(!results.Contains(permutations[1]))
                    results.Add(permutations[1]);
            }

            // Find the chiral compounds and generate the eantiomers
            foreach (var chiralComplex in results.Where(c => c.Chiral).ToList())
            {
                results.Insert(results.IndexOf(chiralComplex) + 1, chiralComplex.GetEantiomer());
            }

            return results;
        }

        public static string GetFormatString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Ligand Assignment Format:");
            sb.AppendLine(@"  F  A  C");
            sb.AppendLine(@"   \ | / ");
            sb.AppendLine(@"     M   ");
            sb.AppendLine(@"   / | \ ");
            sb.AppendLine(@"  D  B  E");

            return sb.ToString();
        }

        #endregion

        #region Instance Members

        /// <summary>
        /// Get a list of tuples representing the trans pairs of the complex.
        /// This is only provided to make finding duplicates marginally easier.
        /// </summary>
        /// <returns>List of tuples representing the trans pairs of the complex</returns>
        private IEnumerable<Tuple<string, string>> GetTransPairTuples()
        {
            return new[]
            {
                new Tuple<string, string>(A, B),
                new Tuple<string, string>(C, D),
                new Tuple<string, string>(E, F)
            };
        }

        /// <summary>
        /// Generates an enatiomer of this complex using the modified Kelly Method.
        /// (Basically we cross the non-axial ligands.
        /// </summary>
        /// <returns>An eantiomer of this complex</returns>
        public OctahedralComplex GetEantiomer()
        {
            // Duplicate this complex and mirror the existing complex
            var clone = (OctahedralComplex)Clone();
            
            // Swap F & C
            var temp = clone.F;
            clone.F = clone.C;
            clone.C = temp;

            // Swap E & D
            temp = clone.E;
            clone.E = clone.D;
            clone.D = temp;

            return clone;
        }

        /// <summary>
        /// Performs the Bailer method transformations to the current complex.
        /// This also involves cloning the existing complexes.
        /// </summary>
        /// <returns>
        /// An array of two OctahedralComplexes that are permutations of the
        /// original complex.
        /// </returns>
        private OctahedralComplex[] PermutateComplex()
        {
            OctahedralComplex[] results = new OctahedralComplex[2];
            // Permutation 1: Swap D and E
            OctahedralComplex perm1 = (OctahedralComplex)Clone();
            string temp = perm1.D;
            perm1.D = perm1.E;
            perm1.E = temp;
            results[0] = perm1;

            // Permutation 2: Swap E and C
            OctahedralComplex perm2 = (OctahedralComplex)perm1.Clone();
            temp = perm2.C;
            perm2.C = perm2.E;
            perm2.E = temp;
            results[1] = perm2;

            return results;
        } 

        #endregion

        #region IClonable Members

        /// <summary>
        /// Generates a new OctahedralComplex with the same ligand assignments
        /// as the original.
        /// </summary>
        /// <returns>A new OctahedralComplex object that </returns>
        public object Clone()
        {
            return new OctahedralComplex()
            {
                A = A,
                B = B,
                C = C,
                D = D,
                E = E,
                F = F
            };
        }

        #endregion

        #region IEquatable Members
        
        /// <summary>
        /// Performs the proper comparison of octahedral complexes as per 
        /// Bailer's method: if both complexes have the same trans pairs, then
        /// the complexes are the same.
        /// </summary>
        /// <remarks>
        /// This will only work with complexes generated with the static method.
        /// </remarks>
        /// <param name="a">The first complex to compare</param>
        /// <param name="b">The second complex to compare</param>
        /// <returns>True if the complexes have the same trans pairs, false otherwise.</returns>
        public static bool operator ==(OctahedralComplex a, OctahedralComplex b)
        {
            // Create a list of trans pairs for a and b
            var aTuples = a.GetTransPairTuples();
            var bTuples = b.GetTransPairTuples();

            return aTuples.OrderBy(i => i).SequenceEqual(bTuples.OrderBy(j => j));
        }

        /// <summary>
        /// Determines whether or not the current octahedral complex is equal
        /// to the other octahedral complex. This actually just falls through
        /// to the == operator
        /// </summary>
        /// <seealso cref="operator=="/>
        /// <param name="other">The other octahedral complex to compare with</param>
        /// <returns>Whether the complexes are the same or not</returns>
        public bool Equals(OctahedralComplex other)
        {
            return this == other;
        }

        /// <summary>
        /// Does object-level comparison of the two octahedral complexes.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((OctahedralComplex)obj);
        }

        public static bool operator !=(OctahedralComplex a, OctahedralComplex b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return (_ligands != null ? _ligands.GetHashCode() : 0);
        }

        #endregion

        /// <summary>
        /// Generates a string that lists the complex's ligands and whether or
        /// not the complex is chiral
        /// </summary>
        /// <returns>A string as per the summary's description</returns>
        public override string ToString()
        {
            return String.Format("A:{0}, B:{1}, C:{2}, D:{3}, E:{4}, F:{5} {6}", A, B, C, D, E, F, Chiral ? "CHIRAL" : "");
        }
    }
}
