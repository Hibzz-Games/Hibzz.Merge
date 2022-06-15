using System.Diagnostics;

namespace Hibzz.Merge
{
    public static class ProcessExtension
    {
        public static int Run(this Process process, string application, string arguments, string workingDirectory, out string output, out string errors)
		{
            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                FileName = application,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                Verb = "runas"
            };

            string out_s = "";
            string err_s = "";

            // bind the strings to output and error events to store any incoming data
		    process.OutputDataReceived += (_, args) => { out_s += args.Data; };
            process.ErrorDataReceived  += (_, args) => { err_s += args.Data; };

            // run the process
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            // store any output and error from the events to the output
            output = out_s;
            errors = err_s;

            return process.ExitCode;
		}
	}
}
