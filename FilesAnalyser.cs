using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace thomasgohard.FilesAnalyser {
	class FilesAnalyser {
		static private bool recursive = false;
		static private bool calculateHash = false;
		static private StreamWriter outputStream;
		static int Main(string[] args) {
			string rootPathToAnalyse;
			/*bool recursive = false;
			bool calculateHash = false;*/
			
			if (args.Length < 1) {
				Console.WriteLine("Invalid number of arguments: Please provide a path to a directory to analyse at a minimum.");
				return 1;
			}

			for (int i = 0; i < (args.Length - 1); i++) {
				if (args[i].Substring(0, 1) == "-") {	// option flag
					switch (args[i].Substring(1)) {
						case "c":
							calculateHash = true;
							break;
						case "r":
							recursive = true;
							break;
						default:
							Console.WriteLine("Invalid option flag: " + args[i].Substring(1) + ". Ignoring flag.");
							break;
					}
					/*if (args[i].Substring(1) == "r") {
						recursive = true;
					}*/
				}
			}
			rootPathToAnalyse = args[args.Length - 1];

			Console.Write("Analysing path " + rootPathToAnalyse + " in ");
			if (!recursive) {
				Console.Write("non-");
			}
			Console.WriteLine("recursive mode.");

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
				using(outputStream = new StreamWriter("file-inventory.csv", false, Encoding.UTF8)) {
					outputStream.WriteLine("Name,Path,Type,Size,Hash");
					AnalyseDirectory(rootPathToAnalyse, recursive);
				}
			} catch(Exception e) {
				Console.WriteLine(e.GetType().Name + ": " + e.Message);
				return 1;
			}

			return 0;
		}

		static void AnalyseDirectory(string rootPathToAnalyse, bool recursive) {
			try {
				DirectoryInfo pathToAnalyseInfo = new DirectoryInfo(rootPathToAnalyse);
				FileInfo[] filesToAnalyseInfo = pathToAnalyseInfo.GetFiles();
				DirectoryInfo[] directoriesToAnalyseInfo = pathToAnalyseInfo.GetDirectories();
				
				Console.WriteLine(filesToAnalyseInfo.Length + " files and " + directoriesToAnalyseInfo.Length + " directories found.");

				foreach(FileInfo fileToAnalyseInfo in filesToAnalyseInfo) {
					string fileHash = "";
					if (calculateHash) {
						fileHash = getFileHash(fileToAnalyseInfo.FullName);
					}
					outputStream.WriteLine(fileToAnalyseInfo.Name + "," + fileToAnalyseInfo.DirectoryName + "," + fileToAnalyseInfo.Extension + "," + fileToAnalyseInfo.Length + "," + fileHash);
				}

				if (recursive) {
					foreach(DirectoryInfo directoryToAnalyseInfo in directoriesToAnalyseInfo) {
						Console.Write("Analysing " + Path.GetFullPath(rootPathToAnalyse) + ": ");
						AnalyseDirectory(directoryToAnalyseInfo.FullName, recursive);
					}
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
