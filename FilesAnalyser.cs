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
				string[] filePathsToAnalyse = Directory.GetFiles(rootPathToAnalyse);
				Console.WriteLine(filePathsToAnalyse.Length + " files found.");

				foreach(string filePathToAnalyse in filePathsToAnalyse) {
					Console.WriteLine(Path.GetFullPath(filePathToAnalyse));
				}
			} catch(Exception e) {
				Console.WriteLine(e.GetType().Name + ": " + e.Message);
				return 1;
			}

			return 0;
		}
	}
}
