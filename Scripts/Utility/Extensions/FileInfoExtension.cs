using System.IO;

namespace Hibzz.Merge
{
    public static class FileInfoExtension
    {
        /// <summary>
        /// Get the full name of the file without the extension
        /// </summary>
        public static string FullNameWithoutExtension(this FileInfo fi)
		{
            return fi.FullName.Substring(0, fi.FullName.LastIndexOf('.'));
		}

        /// <summary>
        /// Get the file name without the the extension
        /// </summary>
        public static string NameWithoutExtension(this FileInfo fi)
		{
            return fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
		}
    }
}
