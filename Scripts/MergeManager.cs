using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Hibzz.Merge
{
	public class MergeManager : Singleton.Simple<MergeManager>
	{
		// The name of the file we are currently working on
		protected FileInfo file;

		protected int p_CountConflicts(string fp)
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

		protected void p_FixConflicts()
		{
			// Save the current version of the conflicted file in a temp file
			SaveCurrentAsTemp();

			// store a list of conflicts by their values "id:<current_string, remote_string>"

			// open tmp scene in unity editor
			OpenTempScene();
			
		}

		/// <summary>
		/// Get the current version of the conflicted file and save it as scenename.current.tmp.scene
		/// </summary>
		private void SaveCurrentAsTemp()
		{
			// The below command gets executed in the gitbash
			// git show :2:filename.unity > filename.current.tmp.unity
			Git($"-c \"git show :2:{file.Name} > {file.NameWithoutExtension()}.current.tmp.unity\"");
		}

		// Open the generated temp scene
		private void OpenTempScene()
		{
			EditorSceneManager.OpenScene($"{file.FullNameWithoutExtension()}.current.tmp.unity");
		}

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

				// delete the conflicted scene
				File.Delete(tmpFile);
			}
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
		public static int CountConflicts(string fp) => GetOrCreateInstance().p_CountConflicts(fp);

		/// <summary>
		/// Proceed with the required steps to handle the conflict in the scenes
		/// </summary>
		public static void FixConflicts() => GetOrCreateInstance().p_FixConflicts();
		#endregion
	}
}
