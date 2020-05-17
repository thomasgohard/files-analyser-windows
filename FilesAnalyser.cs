using System;
using System.IO;

namespace thomasgohard.FilesAnalyser {
	class FilesAnalyser {
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
				DirectoryInfo pathToAnalyseInfo = new DirectoryInfo(rootPathToAnalyse);
				FileInfo[] filesToAnalyseInfo = pathToAnalyseInfo.GetFiles();
				
				Console.WriteLine(filesToAnalyseInfo.Length + " files found.");

				foreach(FileInfo fileToAnalyseInfo in filesToAnalyseInfo) {
					Console.WriteLine(fileToAnalyseInfo.Name + "," + fileToAnalyseInfo.DirectoryName + "," + fileToAnalyseInfo.Extension + "," + fileToAnalyseInfo.Length);
				}
			} catch(Exception e) {
				Console.WriteLine(e.GetType().Name + ": " + e.Message);
				return 1;
			}

			return 0;
		}
	}
}
