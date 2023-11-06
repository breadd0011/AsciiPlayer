using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsciiPlayer.Dictionaries
{
	public class FileExtensionDictionary
	{
		public Dictionary<string, bool> VideoFormats { get; } = new Dictionary<string, bool>
		{
			{ ".avi", true },
			{ ".mp4", true },
			{ ".mkv", true },
			{ ".mov", true },
			{ ".wmv", true },
			{ ".flv", true },
			{ ".mpg", true },
			{ ".webm", true },
			{ ".3gp", true },
			{ ".h264", true },
			{ ".h265", true },
			{ ".vp9", true },
		};

		public Dictionary<string, bool> ImageFormats { get; } = new Dictionary<string, bool>
		{
			{ ".jpg", true },
			{ ".jpeg", true },
			{ ".png", true },
			{ ".gif", true },
			{ ".bmp", true },
			{ ".tif", true },
			{ ".tiff", true },
			{ ".webp", true },
		};
	}
}
