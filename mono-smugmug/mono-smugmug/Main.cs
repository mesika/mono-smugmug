using System;
using System.IO;

namespace monosmugmug
{
	/*
	 * Key: jsL9haotYDjlYlC99A9R6OdLRYVwW7z3
	 * Secret: af7944ce4d2360c9dc1ef9d2de4df80b
	 */


	class MainClass
	{
		private const string USAGE_STRING = 
			"Use the following parameters\n" +
			"-c CategoryName.\n" +
			"-s SubCategoryName.\n" +
			"-a AlbumName.\n" +
			"-f Upload Folder Location.\n" +
			"-u Username\n" +
			"-p Password\n";
			

		private static string _albumName = string.Empty;
		private static string _categoryName = string.Empty;
		private static string _subCategoryName = string.Empty;
		private static string _directory = string.Empty;
		private static string _username = string.Empty;
		private static string _password = string.Empty;

		public static void Main (string[] args)
		{
			try {
				Console.WriteLine ("mono-smugmug. A smugmug uploader tool.");
				
				if (args.Length == 0) {
					
					Console.WriteLine (USAGE_STRING);
					return;
				}
				
				for (int i=0; i<args.Length; i++) {
					
					switch (args[i])
					{
					case "-a":
						_albumName = args[i+1];
						break;
					case "-c":
						_categoryName = args[i+1];
						break;
					case "-s":
						_subCategoryName = args[i+1];
						break;
					case "-f":
						_directory = args[i+1];
						break;
					case "-u":
						_username = args[i+1];
						break;
					case "-p":
						_password = args[i+1];
						break;
					}
				}
				
				Console.WriteLine(string.Format("Album: {0}\n" +
				                                "Category: {1}\n" +
				                                "Sub: {2}\n" +
				                                "Folder: {3}\n" +
				                                "Username: {4}\n" +
				                                "Password: xxx\n",
				                                _albumName,_categoryName,_subCategoryName,_directory,
				                                _username));
				
				var configuration = CreateConfiguration();
				var uploader = new Uploader();
				uploader.ConfigureUploader(configuration);
				uploader.UploadFolder();
			} 
			catch (Exception ex) 
			{
				Console.WriteLine("Fatal!");
				Console.WriteLine(string.Format("Exception details: {0}",ex));
				System.Environment.Exit(10);
			}
			
			Console.Clear ();
			Console.WriteLine("Upload process has been completed. :)");
			Console.ReadLine();
			System.Environment.Exit(1);
		}

		private static UploaderConfiguration CreateConfiguration ()
		{
			if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
				throw new ArgumentException("Username or Password cannot be empty.");
			if (string.IsNullOrEmpty (_categoryName))
				_categoryName = "Other";
			if (string.IsNullOrEmpty (_subCategoryName))
				_subCategoryName = string.Empty;
			if (string.IsNullOrEmpty (_albumName)) {
				_albumName = Path.GetDirectoryName (_directory);
			}
			if (Directory.Exists (_directory) == false) {
				throw new DirectoryNotFoundException(string.Format("Cannot find folder {0}.", _directory));
			}
			else
			{
				_directory = Path.GetFullPath(_directory);
			}

			var configuration = new UploaderConfiguration
			{
				Username = _username,
				Password = _password,

				CategoryName = _categoryName,
				SubCategoryName = _subCategoryName,
				AlbumName = _albumName,

				Directory = _directory,

			};
			
			return configuration;
		}
	}
}