using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hibzz.Merge
{
	public class MergeManager : Singleton.Simple<MergeManager>
	{
		// The name of the file we are currently working on
		protected FileInfo file;

		// A list of conflicts found in the file
		protected List<Conflict> conflicts = new List<Conflict>();

		protected int _CountConflicts(string fp)
		{
			// variable used to count conflicts
			int count = 0;

			// create the reader for the given filepath
			StreamReader reader = new StreamReader(fp);
			if (reader == null) { return 0; }

			// Loop and read through the entire file
			string line = reader.ReadLine();
			while (line != null)
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

			file = new FileInfo(fp);
			return count;
		}

		protected void _FixConflicts()
		{
			// Save the current version of the conflicted file in a temp file
			SaveCurrentAsTemp();

			// store a list of conflicts by their values "id:<current_string, remote_string>"
			AnalyzeConflicts();

			// open tmp scene in unity editor
			OpenTempScene();
			
		}

		// Get the current version of the conflicted file and save it as scenename.current.tmp.scene
		private void SaveCurrentAsTemp()
		{
			// The below command gets executed in the gitbash
			// git show :2:filename.unity > filename.current.tmp.unity
			Git($"-c \"git show :2:{file.Name} > {file.NameWithoutExtension()}.current.tmp.unity\"");
			AssetDatabase.Refresh();
		}

		// Open the generated temp scene
		private void OpenTempScene()
		{
			EditorSceneManager.OpenScene($"{file.FullNameWithoutExtension()}.current.tmp.unity");
		}

		// Destroy the generated temprary scene and open the (previously) conflicted scene
		private void DestroyTempScene()
		{
			string tmpFile = $"{file.FullNameWithoutExtension()}.current.tmp.unity";
			if(File.Exists(tmpFile))
			{
				// close the temporary scene
				var tmpScene = SceneManager.GetSceneByPath(tmpFile);
				EditorSceneManager.CloseScene(tmpScene, true);

				// open the conflicted scene (now most likely fixed)
				EditorSceneManager.OpenScene(file.FullName);

				// delete the conflicted scene and assosciated meta file
				File.Delete(tmpFile);
				AssetDatabase.Refresh();

				// (if any) clear existing conflicts
				conflicts.Clear();
			}
		}

		// Analyze a conflicted file and populate the conflicts list
		private void AnalyzeConflicts()
		{
			// making sure that the conflict list is empty
			conflicts.Clear();

			FileReader reader = new FileReader(file.FullName);
			while(reader.Reading)
			{
				string line = reader.Read();

				// when a line starts with "<<<<<<<", it represents a conflict
				if(line.StartsWith("<<<<<<<"))
				{
					// Create the variable that would represent the current conflict and
					// store its corresponding values
					Conflict conflict = new Conflict();

					// Get the conflict's object id
					string objectid = GetConflictObjectID(reader);
					AnalyzeConflictBlock(reader, objectid);
				}

				reader.GoToNext();
			}
		}

		// Traverse back to analyze which object the conflict is part of
		private string GetConflictObjectID(FileReader reader)
		{
			// cache the line to go back to after done reading the previous line
			string objectId = null;
			int gobackto = reader.CurrentLine;

			// traverse the file backward till we find a line that starts with "---"
			for (string prev = reader.ReadPrevious(); prev != null; prev = reader.ReadPrevious())
			{
				if (prev.StartsWith("--- "))
				{
					objectId = prev.Substring(prev.IndexOf('&') + 1);
					break;
				}
			}

			reader.GoTo(gobackto);
			return objectId;
		}

		// Populate the conflict details
		private void AnalyzeConflictBlock(FileReader reader, string objectid)
		{
			for(string line = reader.ReadNext(); line != null; line = reader.ReadNext())
			{
				// Don't continue if we have reached the end of current in a conflicted block
				if (line.StartsWith("=======")) { break; }

				// create a new conflict object
				Conflict conflict = new Conflict();
				conflict.ObjectId = objectid;

				// Calculate the conflict details and populate the conflict object
				var details = GetConflictDetails(reader);
				conflict.VariableName = details.varname;
				conflict.Current = details.current;
				conflict.Remote = details.remote;

				// Add this conflict to the list of conflicts
				conflicts.Add(conflict);
			}
		}

		// Get conflict details in the current line
		private (string varname, string current, string remote) GetConflictDetails(FileReader reader)
		{
			// cache the line to go back to
			int gobackto = reader.CurrentLine;

			// The current line will have conflicted data when it's called.
			// So get the variable name and current values.
			string line = reader.Read().Trim();
			string varname = line.Substring(0, line.IndexOf(':'));
			string current = line.Substring(line.IndexOf(':') + 1);


			// now continue reading down till we encounter the same variable name,
			// so that we can get the remote values
			string remote = null;
			for(string next = reader.ReadNext(); next != null; next = reader.ReadNext())
			{
				next = next.Trim();
				if(next.StartsWith($"{varname}:"))
				{
					remote = next.Substring(line.IndexOf(':') + 1);
					break;
				}
			}

			// since we are done processing, we can go back to the previous line
			reader.GoTo(gobackto);

			// return the calculated values
			return (varname, current, remote);
		}

		// REQUIRES git-bash installed for this tool to work
		private void Git(string args)
		{
			using (var process = new System.Diagnostics.Process())
			{
				int exitcode = process.Run(@"C:\Program Files\Git\git-bash.exe", args, file.DirectoryName, out var output, out var errors);
				if(exitcode != 0)
				{
					Debug.LogError($"Error Code: {exitcode}\nDetails: {errors}");

				}
				else if(!string.IsNullOrEmpty(output))
				{
					Debug.Log(output);
				}
			}
		}

		#region Public Singleton Accessors

		/// <summary>
		/// Get the number of conflicts in a given file
		/// </summary>
		/// <param name="fp">Filepath of the file to look into</param>
		/// <returns>The number of the conflicts in the given file</returns>
		public static int CountConflicts(string fp) => GetOrCreateInstance()._CountConflicts(fp);

		/// <summary>
		/// Proceed with the required steps to handle the conflict in the scenes
		/// </summary>
		public static void FixConflicts() => GetOrCreateInstance()._FixConflicts();

		/// <summary>
		/// Get a list of conflicts when actively responding
		/// </summary>
		public static List<Conflict> Conflicts => GetOrCreateInstance().conflicts;

		/// <summary>
		/// Is the system actively resolving conflict now?
		/// </summary>
		public static bool IsResolving 
		{ 
			get { return GetOrCreateInstance().conflicts.Count > 0; } 
		}

		[MenuItem("Hibzz/Test")]
		public static void TempFunc() => GetOrCreateInstance().DestroyTempScene();
		#endregion
	}
}
