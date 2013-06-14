﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileService.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation 2011.  All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Microsoft.Support.Workflow.Authoring.AddIns.Utilities
{
    using System;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.AccessControl;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Class for all file system operations
    /// </summary>
    public static class FileService
    {
        private const string ContentDirectoryKeyName = "ContentDirectoryPath";
        private const string OutputDirectoryName = "Output";
        private const string AssembliesDirectoryName = "Assemblies";

        /// <summary>
        /// Get a directory path under current assembly. If not exists, create it.
        /// </summary>
        /// <param name="directoryName">Local directory name</param>
        /// <returns>The get local directory.</returns>
        private static string GetLocalDirectory(string directoryName)
        {
            string currentAssemblyPath = GetExecutingAssemblyPath();
            string result = Path.Combine(currentAssemblyPath, directoryName);

            if (!Directory.Exists(result))
            {
                Directory.CreateDirectory(result);
            }

            return result;
        }


        /// <summary>
        /// Gets the location of the current running assembly
        /// </summary>
        /// <returns>
        /// The path of the current executing assembly.
        /// </returns>
        public static string GetExecutingAssemblyPath()
        {
            const string path = ".";
            return path;
        }

        /// <summary>
        /// Get the temp directory path. If not exists, create it.
        /// </summary>
        /// <returns>
        /// The get temp directory path.
        /// </returns>
        public static string GetTempDirectoryPath()
        {
            return GetLocalDirectory(Path.GetTempPath());
        }

        /// <summary>
        /// Get the local assemblies directory path. If not exists, create it.
        /// </summary>
        /// <returns>
        /// The assemblies directory path.
        /// </returns>
        public static string GetAssembliesDirectoryPath()
        {
            return GetLocalDirectory(AssembliesDirectoryName);
        }

        /// <summary>
        /// Clear all sub-directories in compile output directory
        /// </summary>
        public static void ClearOutputDirectory()
        {
            string outputPath = GetLocalDirectory("Output");
            string[] subDirs = Directory.GetDirectories(outputPath);
            foreach (string subDir in subDirs)
            {
                DeleteDirectory(subDir);
            }
        }

        /// <summary>
        /// Clear all files in temp director
        /// </summary>
        public static void ClearTempDirectory()
        {
            string tempPath = GetTempDirectoryPath();
            ClearDirectory(tempPath);
        }

        /// <summary>
        /// Tree delete a directory
        /// </summary>
        /// <param name="dirPath">
        /// The director will be deleted
        /// </param>
        public static void DeleteDirectory(string dirPath)
        {
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            ClearDirectory(dirPath);

            try
            {
                Directory.Delete(dirPath);
            }
            catch (IOException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>
        /// Clear all files in a directory
        /// </summary>
        /// <param name="dirPath">
        /// The directory will be cleared
        /// </param>
        public static void ClearDirectory(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            try
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
            }
            catch (IOException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }

        /// <summary>
        /// Saves an image object to disk.
        /// </summary>
        /// <param name="fileName">Path of the file to save</param>
        /// <param name="imageSource">Image object to save</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2122", Justification = "Method is private and has been reviewed. Users "
            + "will not be able to use this method to call IsValidIdentifier().")]
        public static bool SaveImageToDisk(string fileName, BitmapSource imageSource)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            if (imageSource == null)
            {
                throw new ArgumentNullException("imageSource");
            }

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(imageSource));
                    encoder.Save(fs);
                }
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch (SecurityException)
            {
                return false;
            }
        }


    }
}
