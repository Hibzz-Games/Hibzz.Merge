using System.Collections.Generic;
using System.IO;

namespace Hibzz.Merge
{
	public class FileReader
	{
		#region Public Properties

		/// <summary>
		/// The line that's being read currently
		/// </summary>
		public int CurrentLine { get; private set; }

		/// <summary>
		/// Is the reader currently in a state of reading
		/// </summary>
		/// <remarks>
		/// The reader is set to have completed once all lines are read
		/// </remarks>
		public bool Reading 
		{ 
			get { return CurrentLine < lines.Length; } 
		}

		/// <summary>
		/// The total number of lines in the file read
		/// </summary>
		public int TotalLines 
		{
			get { return lines.Length; }
		}

		#endregion

		#region Private Fields

		// A list of lines read from a file
		private string[] lines;

		#endregion

		#region Public Functions

		/// <summary>
		/// Create FileReader object that reads the given file
		/// </summary>
		/// <param name="filepath">The file to read</param>
		/// <remarks>Assumes that the given file exists</remarks>
		public FileReader(string filepath)
		{
			lines = File.ReadAllLines(filepath);
			CurrentLine = 0;
		}

		/// <summary>
		/// Go to the given line and continue reading from there
		/// </summary>
		/// <param name="line">The line to read from</param>
		/// <returns>Was the request succesful?</returns>
		/// <remarks>
		/// The function would return false if the requested line was out of range. 
		/// Check <see cref="TotalLines"/> for max available range of the reader.
		/// </remarks>
		public bool GoTo(int line)
		{
			// If the requested line is not within range, then it can't read that line
			if(line < 0 && line >= TotalLines) { return false; }

			// Set the current line to read as the requested line and return true
			// indicating that the "Go to" request was successful
			CurrentLine = line;
			return true;
		}

		/// <summary>
		/// Advance and go to the next line
		/// </summary>
		/// <returns>
		/// Was the request successful? False if we have reached the end of file.
		/// </returns>
		public bool GoToNext()
		{
			// Only increment the line count when we can go to the next line
			if(CurrentLine < TotalLines)
			{
				CurrentLine++;
			}

			// Return whether we have reached the end
			return CurrentLine < TotalLines;
		}

		/// <summary>
		/// Go to the previous line
		/// </summary>
		/// <returns>
		/// Have we reached the start of the file? 
		/// Will return false if trying to go previous on the first line, aka, -1 
		/// </returns>
		public bool GoToPrevious()
		{
			// NOTE: This function will never go out-of-bound into the negative unlike GoToNext
			// Go to the previous line if possible.
			if(CurrentLine > 0) 
			{ 
				CurrentLine--;
				return true;
			}

			// When at line 0 (or somehow in negative), we can't go backward anymore
			return false;
		}

		/// <summary>
		/// Read the current line
		/// </summary>
		/// <returns>
		/// The string in the current line. If the reader is done reading, it'll return null.
		/// </returns>
		public string Read()
		{
			// If we are done reading, return null
			if (!Reading) { return null; }

			// Current line is guaranteed to be within the range
			return lines[CurrentLine];
		}

		/// <summary>
		/// If a next line is available, advance and read the next line
		/// </summary>
		/// <returns> 
		/// The next line in the file. Returns null if we have reached the end of file 
		/// </returns>
		public string ReadNext()
		{
			// No fancy if checks here.
			// Read will return null when it has reached end of file.
			GoToNext();
			return Read();
		}

		/// <summary>
		/// If a previous line is available, traverse back and read the previous line
		/// </summary>
		/// <returns>
		/// The previous line in the file. If we can't go further back, returns null.
		/// </returns>
		public string ReadPrevious()
		{
			// Read doesn't perform a lower bounds check because current line can never
			// be below zero. So, we are doing an if check in here to see if GoToPrevious
			// failed or not.
			if(GoToPrevious()) 
			{ 
				return Read(); 
			}

			// Failed. So we return null
			return null;
		}

		#endregion
	}
}
