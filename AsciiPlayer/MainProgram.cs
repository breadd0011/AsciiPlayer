using AsciiPlayer.Dictionaries;
using AsciiPlayer.Services;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AsciiPlayer
{
	public class MainProgram
	{
		public static string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");
		public static string temptxtPath = Path.Combine(Directory.GetCurrentDirectory(), @"Temp\txts");
		public static string filePath;
		public static string audioPath = Path.Combine(tempPath, "audio.mp3");

		private static Timer timer;
		private static FileExtensionDictionary extensionsDictionary = new();
		private static int frameIndex;
		private static int temptxtPathFileLength;
		public static void Main(string[] args)
		{
			while (true)
			{
				Initialize();

				string currentExtension = Path.GetExtension(filePath);

				if (extensionsDictionary.VideoFormats.ContainsKey(currentExtension))
				{
					PlayVideo();
					break;
				}
				if (extensionsDictionary.ImageFormats.ContainsKey(currentExtension))
				{
					DisplayImage();
					break;
				}

				else
				{
					Console.WriteLine("The format of your file is not supported, press any key to go back.");
					Console.ReadKey();
					Console.Clear();
					continue;
				}
			}
			

		}
		private static void PlayVideo()
		{
			FileProcessingService.Instance.ExtractAudioFromVideo(filePath, audioPath);
			FileProcessingService.Instance.WriteVideoToTxtFiles(filePath, temptxtPath);
			temptxtPathFileLength = Directory.GetFiles(temptxtPath).Length;

			using(var waveOut = new WaveOutEvent())
			{
				using(var audioFile = new Mp3FileReader(audioPath))
				{
					waveOut.Init(audioFile);
					waveOut.Play();
					timer = new Timer(DisplayNextFrame, null, 0, FileProcessingService.Instance.GetIntervalBetweenFrames(filePath));
					Console.ReadLine();
				}
			
			}

		}

		private static void DisplayImage()
		{
			FileProcessingService.Instance.WriteImageToTxtFile(filePath, temptxtPath);
			string text = FileProcessingService.Instance.ReadTextFromTxt(temptxtPath + @"\temp.txt");
			Console.WriteLine(text);
			Console.ReadLine();
		}
		private static void DisplayNextFrame(object? state)
		{
			string txtPath = Path.Combine(temptxtPath, $"{frameIndex}.txt");
			string text = FileProcessingService.Instance.ReadTextFromTxt(txtPath);
			Console.SetCursorPosition(0, 0);
			Console.Write(text);

			if (frameIndex == temptxtPathFileLength - 1)
			{
				Console.WriteLine("\n\nAll frames were displayed.");
				timer.Dispose();
				Console.ReadLine();		
			}
            else
            {
				frameIndex++;
			}    
		}

		private static void Initialize()
		{
			DeleteTempFiles();
			CreateTempFolders();
			GetFilePath();

			Console.SetCursorPosition(0, 0);
			Console.CursorVisible = false;
		}

		private static void GetFilePath()
		{
			Console.WriteLine("Drag in your file and press enter:");

			filePath = Console.ReadLine();

			if (filePath == null)
			{
				Console.WriteLine("Incorrect input, press any key and try again. (Make sure your file and path had no spaces in their name!)");
				Console.ReadKey();
				Console.Clear();
				return;
			}

			if (!File.Exists(filePath))
			{
				Console.WriteLine("Incorrect input, press any key and try again. (Make sure your file and path had no spaces in their name!)");
				Console.ReadKey();
				Console.Clear();
				return;
			}

			Console.Clear();
		}

		private static void DeleteTempFiles()
		{
			if (Directory.Exists(tempPath))
			{
				string[] files = Directory.GetFiles(tempPath);
				foreach (var file in files)
				{
					File.Delete(file);
				}
			}
			if (Directory.Exists(temptxtPath))
			{
				string[] files = Directory.GetFiles(temptxtPath);
				foreach (var file in files)
				{
					File.Delete(file);
				}
			}
		}

		private static void CreateTempFolders()
		{
			if (!Directory.Exists(tempPath))
			{
				Directory.CreateDirectory(tempPath);
			}
			if (!Directory.Exists(temptxtPath))
			{
				Directory.CreateDirectory(temptxtPath);
			}
		}
	}
}