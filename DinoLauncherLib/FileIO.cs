﻿using System;
using System.IO;
using System.Security.AccessControl;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug; // We want to use the Windows debug out for, well, debugging

namespace DinoLauncherLib;

public class FileIO
{
    // Prepare file attribute variables
    public string lastWrite;
    public string creationDate;
    public string md5;
    // Keep ths around to check against the original rom_crack.z64 Md5 Checksum
    public string originalMd5 = "c4c1b52f9c4469c6c747942891de3cfd";

    // Get the application directory
    public string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

    // Paths and path extensions
    public string chosenPatchPath = "\\_PatchData\\dp-stable.xdelta";
    public string romCrackPath = "\\_PatchData\\rom_crack.z64";
    public string xdeltaPath = "\\_PatchData\\xdelta3.exe";
    public string patchedRomPath = "\\_Game\\dinosaurplanet.z64";
    public string gitWorkDir;

    public string musicPath = "\\_Resources\\Music.mp3";
    public string assemblyPath;

    /// <summary>
    /// Function to quickly build the folder structure required by DinoLauncher
    /// </summary>
    public void SetupFileStructure()
    {
        // Store executable directory
        string baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Set the current working directory to the application root
        System.IO.Directory.SetCurrentDirectory(baseDir);

        // Create a directory to handle all the things
        // Really just add a bunch of folders if they don't exist, write this better later, make it work for now
        if (!Directory.Exists($"{baseDir}\\_PatchData"))
        {
            CreateDirectory($"{baseDir}\\_PatchData");
        }
        if (!Directory.Exists($"{baseDir}\\_PatchData\\git"))
        {
            CreateDirectory($"{baseDir}\\_PatchData\\git");
        }
        if (!Directory.Exists($"{baseDir}\\_Resources"))
        {
            CreateDirectory($"{baseDir}\\_Resources");
        }
        if (!Directory.Exists($"{baseDir}\\_Game"))
        {
            CreateDirectory($"{baseDir}\\_Game");
        }
    }

    /// <summary>
    /// Helper class to create directories with specific permissions. 
    /// This should help with I/O permissions issues.
    /// </summary>
    void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);

        DirectoryInfo dir = new DirectoryInfo(path);
        var dirSec = dir.GetAccessControl();

        //dirSec.AddAccessRule(new FileSystemAccessRule(path, FileSystemRights.FullControl, AccessControlType.Allow));
        //dir.SetAccessControl(dirSec);
    }

    /// <summary>
    /// Simple tool for moving files (string source, string destination)
    /// </summary>
    public void MoveFile(string source, string destination)
    {
        if (!File.Exists(source))
        {
            Debug.WriteLine($"FileIO.MoveFile(): {source} does not exist.");
        }
        else
        {
            // .net standard does not allow overwriting on move
            // Will ALWAYS overwrite, be careful!
            if (File.Exists(destination))
                File.Delete(destination);
            File.Move(source, destination);
        }
    }

    /// <summary>
    /// Simple tool for copying files (string source, string destination)
    /// </summary>
    public void CopyFile(string source, string destination)
    {
        if (!File.Exists(source))
        {
            Debug.WriteLine($"FileIO.CopyFile(): {source} does not exist.");
        }
        else
        {
            File.Copy(source, destination, true); // Will ALWAYS overwrite, be careful!
        }
    }

    /// <summary>
    /// Simple tool for deleting files (string path)
    /// </summary>
    public Task<string> DeleteFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.WriteLine($"FileIO.DeleteFile(): {path} does not exist.");
        }
        else
        {
            // Need to give permission to this somehow
            File.Delete(path);
            System.Diagnostics.Debug.WriteLine($"FileIO.DeleteFile: Deleted {path}");
        }

        // Some magic internet C# BS 
        return Task.FromResult<string>(null);
    }

    public Task<string> DeleteDirectory(string path)
    {
        if (!File.Exists(path))
        {
            Debug.WriteLine($"FileIO.DeleteDirectory(): {path} does not exist.");
        }
        else
        {
            Directory.Delete(path, true);
        }
        
        // Some magic internet C# BS 
        return Task.FromResult<string>(null);
    }

    /// <summary>
    /// Returns a bool value depending on whether or not a file exists (string path)
    /// </summary>
    public bool DoesFileExist(string path) // Returns a bool depending on if the file was found or not
    {
        if (File.Exists(path))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Use to get the file's creation date, last write date, and generate an Md5 (string path)
    /// </summary>
    public void GetFileInfo(string path) // Parameter will need to be baseDir + chosenPatchPath (FileIO.currentDir + FileIO.chosenPatch?)
    {
        // Get the creation date of the most recent patch and format it as YYYYMMDD
        creationDate = File.GetCreationTime(path).ToString("yyyyMMdd");
        // Get the last write date, sometimes more accurate than creationDate, I like having both available
        lastWrite = File.GetLastWriteTime(path).ToString("yyyMMdd");
        md5 = CalculateMD5(path);
    }

    /// <summary>
    /// Used by GetFileInfo to generate an Md5
    /// </summary>
    public string CalculateMD5(string filename)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filename);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Alternate Md5 generator, returns a bool value after comparing to the base rom Md5 (string path)
    /// </summary>
    public bool MD5Checksum(string path)
    {
        md5 = CalculateMD5(path);

        if (md5 == originalMd5)
        {
            Debug.WriteLine("Md5 checksum passed!");
            return true;
        }
        else
        {
            Debug.WriteLine("Md5 checksum failed.");
            return false;
        }
    }
}