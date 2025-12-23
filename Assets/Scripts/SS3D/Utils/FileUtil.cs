using System;
using System.IO;
using UnityEngine;

namespace SS3D.Utils
{
    /// <summary>
    /// Utility class for file operations.
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Saves content to a file at the specified path.
        /// </summary>
        /// <param name="path">the path of the file</param>
        /// <param name="content">content of the file</param>
        /// <param name="overwrite">do you want to overwrite an existing file?</param>
        /// <returns>if the files is saved or not</returns>
        /// <exception cref="DirectoryNotFoundException">If the given path is not valid</exception>
        /// <exception cref="IOException">If a file already exists and overwrite is set to false</exception>
        public static bool SaveFile(string path, string content, bool overwrite = false)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(directoryPath))
                    throw new DirectoryNotFoundException("Directory path is null or empty.");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                if (File.Exists(path) && !overwrite)
                    throw new IOException($"File at {path} already exists and overwrite is set to false.");

                File.WriteAllText(path, content);

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save file at {path}: {e.Message}");

                return false;
            }
        }

        /// <summary>
        /// Deletes the file at the specified path.
        /// </summary>
        /// <param name="path">the path of file</param>
        /// <returns>if the file is deleted or not</returns>
        public static bool DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    return true;
                }

                Debug.LogWarning($"File at {path} does not exist.");

                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to delete file at {path}: {e.Message}");

                return false;
            }
        }
    }
}