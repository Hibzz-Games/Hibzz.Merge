using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hibzz.Merge
{
	public class MergeManager : Singleton.Simple<MergeManager>
	{
		protected int p_CountConflicts(string fp)
		{
			// variable used to count conflicts
			int count = 0;

			// create the reader for the given filepath
			StreamReader reader = new StreamReader(fp);
			if(reader == null) { return 0; }

			// Loop and read through the entire file
			string line = reader.ReadLine();
			while(line != null)
			{
				// A conflict in a conflicted git file starts with <<<<<<<
				// so, we increment the conflict count
				if (line.StartsWith("<<<<<<<")) 
				{ 
					count++; 
				}

				// go to the next line
				line = reader.ReadLine();
			}

			return count;
		}

		protected void p_FixConflicts()
		{
		}

		#region Public Singleton Accessors
		
		/// <summary>
		/// Get the number of conflicts in a given file
		/// </summary>
		/// <param name="fp">Filepath of the file to look into</param>
		/// <returns>The number of the conflicts in the given file</returns>
		public static int CountConflicts(string fp) => GetOrCreateInstance().p_CountConflicts(fp);

		/// <summary>
		/// Proceed with the required steps to handle the conflict in the scenes
		/// </summary>
		public static void FixConflicts() => GetOrCreateInstance().p_FixConflicts();

		#endregion
	}
}
