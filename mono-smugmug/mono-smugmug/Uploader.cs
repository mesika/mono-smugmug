using System;
using System.Net;
using JSONDotNET;
using SmugMugModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using NLog;

namespace monosmugmug
{
	public class Uploader
	{ 
		private UploaderConfiguration _configuration;
		private readonly string[] AllowedExtensions;
		private Process _process;
		private string _currentFilename = string.Empty;
		private float _completionPercentage = 0.0f;
		private int _totalNumberOfFiles = 0;
		private int _currentFileIndex = 0;
		private float _kiloBytesPerSecond = 0.0f;
		private int _currentFileSizeInKB = 0;
		private int _bandWidthMultiplier = 0;
		private Logger _logger = LogManager.GetLogger("mono-smugmug");
		private IEnumerable<string> _albumExistingFilenames; // = new List<string>();
		
		public Uploader ()
		{
			AllowedExtensions = new [] { ".jpg",".png",".mov",".mp4",".avi",".mpg"};
			_process = Process.GetCurrentProcess();
		}

		public void ConfigureUploader (UploaderConfiguration configuration)
		{
			_configuration = configuration;
		}
		
		private ImageUpload CreateUploader()
		{
			_logger.Debug ("Starting... {0}.", DateTime.Now);
			_logger.Debug("Creating Site...");
			var site = new Site();
			_logger.Debug("Site Created");
			_logger.Debug("User Login..");
			var user = site.Login(_configuration.Username, _configuration.Password);
			_logger.Debug("User logged in.");
			_logger.Debug("Creating Album {0}",_configuration.AlbumName);
			var album = user.CreateAlbum(_configuration.AlbumName,true);
			_logger.Debug ("Album Created.");
			album.Protected = true;
			_logger.Debug("Album changed to protected.");
			album.ChangeSettings ();
			_logger.Debug("Committing changes.");
			
			_logger.Debug("Enumarating album images from smugmug.");
			
			var albumImages = album.GetImages(true);
			
//			var imm = album.Images;
//			var imm2 = album.ImageCount;
			//_albumExistingFilenames = album.GetImages (true).Select(x=>x.FileName).ToArray();
			_albumExistingFilenames = albumImages.Select(x=>x.FileName).ToArray();
			
			
			_logger.Debug("Got Album Images: {0} images.", _albumExistingFilenames.Count ());
			var uploader = album.CreateUploader();
			_logger.Debug("Created Uploader.");
			
			_logger.Info("Starting to upload to album:{0}. Number of images found in album:{1}.", _configuration.AlbumName,_albumExistingFilenames.Count ());			
			
			Console.Clear ();
			Console.SetCursorPosition(1,1);
			Console.Write(string.Format("Smugmug Uploader: {0}", DateTime.Now.ToLongTimeString()));
			
			uploader.UploadProgress += (object sender, UploadEventArgs e) => UpdateUploadProgress(e);
			return uploader;
		}
		
		public void UploadFolder()
		{
			var uploader = CreateUploader ();
			var allFiles = Directory.GetFiles(_configuration.Directory);
			var files = allFiles.Where(file=> AllowedExtensions.Any(ext => file.ToLower().EndsWith(ext))).ToArray();
			
			_totalNumberOfFiles = files.Count();
			
			var timer = new System.Timers.Timer(2000);
			timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => UpdateStatus();
			timer.AutoReset = true;
			timer.Start ();
			
			try
			{
				foreach (var file in files)
				{
					_logger.Debug("Starting to upload file:{0}.", file);
					System.IO.FileInfo fileInfo = new FileInfo(file);
					if (_albumExistingFilenames.Contains(fileInfo.Name))
					{
						_logger.Info("File {0} already exist, skipping.", fileInfo.Name);
						_currentFileIndex++;
						continue;
					}
					_currentFileSizeInKB = (int)fileInfo.Length / 1024;
					_currentFileIndex++;
					Console.SetCursorPosition(1,4);
					Console.Write ("".PadRight(Console.WindowWidth,' '));
					
					uploader.UploadImage(file);
					_logger.Debug("Finished Download.");
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Error occued while uploading: {0}", ex);
			}
			finally
			{
				timer.Stop ();
				timer.Enabled = false;
			}
		}
		
		private void UpdateStatus()
		{
			Console.Clear ();
			Console.SetCursorPosition(1,3);
			Console.Write (string.Format("Process Memory: {0:n0}MB  |  Max Speed: {1}KB/s", _process.WorkingSet64/1024, _bandWidthMultiplier));
			Console.SetCursorPosition(1,4);
			Console.Write (string.Format("Uploadind {0}, Progress {1:#.###}%", _currentFilename,_completionPercentage*100));
			Console.SetCursorPosition(1,5);
			Console.Write (string.Format("File size: {0:n0}KB, Average speed: {1:#.##}KB/s",_currentFileSizeInKB, _kiloBytesPerSecond));
			Console.SetCursorPosition(1,6);
			Console.Write(string.Format("Processing files: {0} / {1}", _currentFileIndex, _totalNumberOfFiles));
		}
		
		private void UpdateUploadProgress(UploadEventArgs e)
		{
			_currentFilename = e.FileName;
			_completionPercentage = e.PercentComplete;
			_kiloBytesPerSecond = e.KBperSecond;
			_bandWidthMultiplier = e.Bandwidth;
		}

		
	}
}

