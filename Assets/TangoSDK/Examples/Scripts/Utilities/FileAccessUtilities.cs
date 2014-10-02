//-----------------------------------------------------------------------
// <copyright file="FileAccessUtilities.cs" company="Google">
//
// Copyright 2014 Google. Part of the Tango project. CONFIDENTIAL. AUTHORIZED USE ONLY. DO NOT REDISTRIBUTE.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Linq;
using System.IO;
using UnityEngine;

/// <summary>
/// Static class to perform following file operations
/// - Retreive list of files from a directory.
/// </summary>
public static class FileAccessUtilities
{
    /// <summary>
    /// Used to retreive a list of files in the directory.
    /// </summary>
    /// <param name="directory"> Directory to retreive files from.</param>
    /// <param name="ignorePattern"> Ignores certain files from the list of files found if needed.
    /// Eg: if ignorePattern is ".txt", it ignores all text files.</param>
    /// <returns> List of files found in directory.</returns>
    public static string[] RetrieveFilesList(string directory, string ignorePattern)
    {
        string[] temp_Files = Directory.GetFiles(directory);

        // this loop separates file name from full path
        for (int i = temp_Files.Length - 1; i >= 0; i--)
        {
            temp_Files[i] = Path.GetFileName(temp_Files[i]);
        }

        if (ignorePattern != string.Empty)
        {
            // filter out .txt files and don't display them
            foreach (string fileName in temp_Files)
            {
                if (fileName.Contains(ignorePattern))
                {
                    temp_Files = temp_Files.Where(name => name != fileName).ToArray();
                }
            }
        }
        return temp_Files;
    }
}
