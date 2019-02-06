using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;


namespace Exercise2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("This app opens a zip archive from user specified directory to another user specified directory");
            Console.WriteLine("Calculates compression ratio of the zip file");
            Console.WriteLine("Outputs the age of the oldest file in the zip archive");

            Console.WriteLine("For exiting to application type \"Exit\" before typing filename: \n");

            while (true)
            {
                try
                {
                    Console.WriteLine("Enter archive file full path with filename and extension: ");
                    var archiveFile = Console.ReadLine();
                    if (archiveFile == "Exit")
                    {
                        break;
                    }

                    FileCheck(archiveFile);
                    var fileName = Path.GetFileNameWithoutExtension(archiveFile);

                    var zippedSize = FileSizeCalculation(archiveFile);
                    var directory = Path.GetDirectoryName(archiveFile); // Get directory for determining default directory

                    Console.WriteLine("Enter desired directory for output.");
                    Console.WriteLine("Default directory is {0}, press Enter to continue with default ", directory);
                    string newDirectory = Console.ReadLine();

                    if (newDirectory != "")
                    {
                        directory = newDirectory;
                    }

                    DirectoryCheck(directory);

                    var outputDirectory = Path.Combine(directory, fileName);
                    UnZipTheFile(archiveFile, outputDirectory);

                    var unzippedSize = DirectorySizeCalculation(new DirectoryInfo(@outputDirectory));
                    if (unzippedSize == 0)
                    {
                        //Cannot divide to zero so inform the user.
                        Console.WriteLine("Unzipped file size is 0. Compression rate cannot be calculated.");
                    }
                    else
                    {
                        Console.WriteLine("Compress ratio is {0}%", CompressRatioCalculation(unzippedSize, zippedSize).ToString("F")); // 2 digits after dot
                    }
                    Console.WriteLine("Oldest file in zipfile is {0} days old.", OldestFileCalculation(archiveFile));

                }

                catch (FileNotFoundException)
                {
                    Console.WriteLine("This file is not exist. Please try again.");
                }

                catch (DirectoryNotFoundException)
                {

                    Console.WriteLine("This directory is not exist. Please try again.");
                }

                catch (Exception e)
                {
                    Console.WriteLine("An error occured. Please try again. Error details: ");
                    Console.WriteLine(e.Message);
                }

            }
        }

        public static void FileCheck(string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }
        }

        public static void DirectoryCheck(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException();
            }
        }

        public static long FileSizeCalculation(string file)
        {
            var info = new FileInfo(file);
            return info.Length;
        }

        public static long DirectorySizeCalculation(DirectoryInfo directory)
        {
            return directory.GetFiles().Sum(file => file.Length) +
                   directory.GetDirectories().Sum(subDirectory => DirectorySizeCalculation(subDirectory));
        }

        public static double CompressRatioCalculation(long unzippedSize, long zippedSize)
        {
            return ((double)zippedSize / unzippedSize) * 100; //Casting long to double for solving calculation problem
        }

        public static int OldestFileCalculation(string file)
        {
            var archive = ZipFile.Open(file, ZipArchiveMode.Read);
            var entries = archive.Entries;
            var today = DateTime.Now;
            var oldestDate = 0.0;

            foreach (var entry in entries)
            {
                if ((today - entry.LastWriteTime).TotalDays > oldestDate)
                {
                    oldestDate = (today - entry.LastWriteTime).TotalDays;
                }

            }

            return Convert.ToInt32(oldestDate); //Return int for better representation
        }

        public static void UnZipTheFile(string archiveFile, string outputDirectory)
        {
            ZipFile.ExtractToDirectory(archiveFile,outputDirectory);
            Console.WriteLine("Zip file extracted to " + outputDirectory);
        }
    }
}
