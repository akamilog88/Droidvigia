using System;

namespace DroidToneDecoder
{
	public class DTMFDetectedArgs : EventArgs
	{
		public int TonConstant{ get; private set;}
		public double Duration { get; private set; }
		public String ToneAscii
		{
			get
			{
				if (TonConstant >= 0)
					return DTMFConstanst.dtran[TonConstant];
				else
					return "";
			}
		}

		public DTMFDetectedArgs(int ToneCodeConstant, double duration)
		{
			this.TonConstant = ToneCodeConstant;
			this.Duration = duration;
		}
	}
}

