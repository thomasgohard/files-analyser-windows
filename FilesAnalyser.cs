using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace thomasgohard.FilesAnalyser {
	class FilesAnalyser {
		static private StreamWriter outputStream;
		static int Main(string[] args) {
			string rootPathToAnalyse;
			
			if (args.Length != 1) {
				Console.WriteLine("Invalid number of arguments: Please provide a path to a directory to analyse.");
				return 1;
			}

			rootPathToAnalyse = args[0];
			try {
				FileAttributes pathAttributes = File.GetAttributes(rootPathToAnalyse);

				if ((pathAttributes & FileAttributes.Directory) != FileAttributes.Directory) {
					Console.WriteLine("Path doesn't point to a directory: Please provide a path to a directory to analyse.");
					return 1;
				}
			} catch(Exception e) {
				Console.WriteLine(e.GetType().Name + ": " + e.Message);
				return 1;
			}

			Console.Write("Analysing " + Path.GetFullPath(rootPathToAnalyse) + ": ");
			try {
				//outputStream = new StreamWriter("file-inventory.csv", false, Encoding.UTF8);
				using(outputStream = new StreamWriter("file-inventory.csv", false, Encoding.UTF8)) {
					outputStream.WriteLine("Name,Path,Type,Size,Hash");
					AnalyseDirectory(rootPathToAnalyse);
				}
			} catch(Exception e) {
				Console.WriteLine(e.GetType().Name + ": " + e.Message);
				return 1;
			}

			return 0;
		}

		static void AnalyseDirectory(string rootPathToAnalyse) {
			try {
				DirectoryInfo pathToAnalyseInfo = new DirectoryInfo(rootPathToAnalyse);
				FileInfo[] filesToAnalyseInfo = pathToAnalyseInfo.GetFiles();
				DirectoryInfo[] directoriesToAnalyseInfo = pathToAnalyseInfo.GetDirectories();
				
				Console.WriteLine(filesToAnalyseInfo.Length + " files and " + directoriesToAnalyseInfo.Length + " directories found.");

				foreach(FileInfo fileToAnalyseInfo in filesToAnalyseInfo) {
					//Console.WriteLine(fileToAnalyseInfo.Name + "," + fileToAnalyseInfo.DirectoryName + "," + fileToAnalyseInfo.Extension + "," + fileToAnalyseInfo.Length + "," + getFileHash(fileToAnalyseInfo.FullName));
					outputStream.WriteLine(fileToAnalyseInfo.Name + "," + fileToAnalyseInfo.DirectoryName + "," + fileToAnalyseInfo.Extension + "," + fileToAnalyseInfo.Length + "," + getFileHash(fileToAnalyseInfo.FullName));
				}

				foreach(DirectoryInfo directoryToAnalyseInfo in directoriesToAnalyseInfo) {
					Console.Write("Analysing " + Path.GetFullPath(rootPathToAnalyse) + ": ");
					AnalyseDirectory(directoryToAnalyseInfo.FullName);
				}
			} catch(Exception e) {
				throw;
			}
		}

		static string getFileHash(string filePath) {
			try {
				FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
				SHA512 FileHasher = new SHA512Managed();
				byte[] FileHash = FileHasher.ComputeHash(fs);
				
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < FileHash.Length; i++) {
					sb.Append(FileHash[i].ToString("x2"));
				}

				return sb.ToString();
			} catch(Exception e) {
				return "Unable to calculate file hash.";
			}
		}
	}
}
