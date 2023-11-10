using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;
using OpenCvSharp;
using System.Diagnostics;

namespace AsciiPlayer.Services
{
	public class FileProcessingService
	{
		private static readonly Lazy<FileProcessingService> lazy = new(() => new());
		public static FileProcessingService Instance { get { return lazy.Value; } }
		private FileProcessingService() { }

		public string[] asciiChars = { "@", "B", "%", "8", "W", "M", "#", "*", "o", ":", ".", " ", " " };

		public void WriteVideoToTxtFiles(string vidPath, string tempTxtPath)
		{
			using (VideoCapture capture = new VideoCapture(vidPath))
			{
				Mat frame = new Mat();
				int frameIndex = 0;
				int asciiLength = asciiChars.Length - 1;

				while (true)
				{
					capture.Read(frame);

					if (frame.Empty())
					{
						break;
					}
					
					ResizeMat(frame);

					string modifiedPath = Path.Combine(tempTxtPath, $"{frameIndex}.txt");
					using (var streamWriter = new StreamWriter(modifiedPath))
					{
						//Frame Conversion to Ascii characters

						for (int i = 0; i < frame.Height; i++)
						{
							streamWriter.WriteLine();

							for (int j = 0; j < frame.Width; j++)
							{
								Vec3b bgrPixel = frame.At<Vec3b>(i, j);
								int b = bgrPixel.Item0;
								int g = bgrPixel.Item1;
								int r = bgrPixel.Item2;

								double brightness = (0.299 * r) + (0.587 * g) + (0.114 * b);

								double mapping = Math.Floor((0 - brightness) / (0 - 255) * (0 - asciiLength) + asciiLength);
								int charIndex = Convert.ToInt32(mapping);

								streamWriter.Write(asciiChars[charIndex]);
								streamWriter.Write(asciiChars[charIndex]);
							}
						}
						//Frame conversion to Ascii Characters
					}
					frameIndex++;
				}

				capture.Release();
			}
		}

		public void WriteImageToTxtFile(string imgPath, string tempTxtPath)
		{
			Mat img = Cv2.ImRead(imgPath);
			int asciiLength = asciiChars.Length - 1;

			ResizeMat(img);

			string modifiedPath = Path.Combine(tempTxtPath, "temp.txt");
			using (var streamWriter = new StreamWriter(modifiedPath))
			{
				//Frame Conversion to Ascii characters

				for (int i = 0; i < img.Height; i++)
				{
					streamWriter.WriteLine();

					for (int j = 0; j < img.Width; j++)
					{
						Vec3b bgrPixel = img.At<Vec3b>(i, j);
						int b = bgrPixel.Item0;
						int g = bgrPixel.Item1;
						int r = bgrPixel.Item2;

						double brightness = (0.299 * r) + (0.587 * g) + (0.114 * b);

						double mapping = Math.Floor((0 - brightness) / (0 - 255) * (0 - asciiLength) + asciiLength);
						int charIndex = Convert.ToInt32(mapping);

						streamWriter.Write(asciiChars[charIndex]);
						streamWriter.Write(asciiChars[charIndex]);
					}
				}
				//Frame conversion to Ascii Characters
			}
		}

		public string ReadTextFromTxt(string txtPath)
		{
			using(var streamReader = new StreamReader(txtPath))
			{
				streamReader.ReadLine(); // to skip first empty line

				return streamReader.ReadToEnd();
			}
		}

		public int GetIntervalBetweenFrames(string vidPath)
		{
			VideoCapture videoCapture = new VideoCapture(vidPath);
			double fps = videoCapture.Fps;
			double interval =  1000 / fps;
			return (int)interval;
		}

		private void ResizeMat(Mat mat)
		{
			int newWidth = mat.Width;
			int newHeight = mat.Height;
			int minWidth = 1;
			int maxWidth = Math.Max(mat.Width, mat.Height);

			while (minWidth <= maxWidth)
			{
				int midWidth = (minWidth + maxWidth) / 2;
				double ratioX = (double)midWidth / mat.Width;
				double ratioY = (double)midWidth / mat.Height;
				double ratio = Math.Min(ratioX, ratioY);
				newWidth = (int)Math.Round((mat.Width * ratio));
				newHeight = (int)Math.Round((mat.Height * ratio));

				if (newWidth == Console.LargestWindowWidth -10 || newHeight == Console.LargestWindowHeight -2)
				{
					break;
				}
				else if (newWidth < Console.LargestWindowWidth - 10 && newHeight < Console.LargestWindowHeight -2)
				{
					minWidth = midWidth + 1;
				}
				else
				{
					maxWidth = midWidth - 1;
				}
			}

			Cv2.Resize(mat, mat, new OpenCvSharp.Size(newWidth, newHeight), interpolation: InterpolationFlags.Lanczos4);
		}

		public void ExtractAudioFromVideo(string vidPath, string audioPath)
		{
			string ffmpegPath = Path.Combine(Directory.GetCurrentDirectory(), @"ffmpeg\ffmpeg.exe");

			string arguments = $"-i \"{vidPath}\" -vn -acodec libmp3lame -q:a 2 -ar 44100 -ac 2 \"{audioPath}\"";

			Process process = new Process();
			process.StartInfo.FileName = ffmpegPath;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;

			process.Start();
			process.WaitForExit();
		}
	}
}
