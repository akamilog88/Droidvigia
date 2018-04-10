using System;

namespace DroidToneDecoder
{
	public interface AudioClipListener
	{
		void heard(byte[] data, int sampleRate);
	}
}

