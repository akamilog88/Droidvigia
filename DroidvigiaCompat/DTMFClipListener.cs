using System;
using System.Collections;
using System.Linq;

namespace DroidToneDecoder
{
	public class DTMFClipListener : AudioClipListener
	{
		int CurrentTone = -1;
		int TC = 0;

		public bool DetectSilence { get; set; }

		private DTMFTokenParser parser;
		public bool IsTokenBuildEnabled{ get; private set;}
		object _lockObj = new object();

		public event EventHandler<DTMFDetectedArgs> ToneDetected;
		public event EventHandler<DTMFTokenBuildedEventArgs> NewToken;
		//public event EventHandler<EventArgs> EndRecording;


		public DTMFClipListener(bool buildToken=false)
		{
			DetectSilence = false;
			IsTokenBuildEnabled = buildToken;
			if (IsTokenBuildEnabled) {
				parser = new DTMFTokenParser ();
				parser.NewToken += (sender, e) => {
					if(NewToken!=null)
						NewToken.BeginInvoke(sender,e,null,null);
				};
			}
		}

		void DetectformDataChunk(byte[] chunk, int realLen)
		{

			int iterations = realLen / (DTMFConstanst.N);

			for (int i = 0; i < iterations; i++)
			{
				int x = decode(chunk.Skip(i * DTMFConstanst.N).Take(DTMFConstanst.N).ToArray());
				bool newTone = false;
				if (x >= 0)
				{
					if (CurrentTone == x)
					{
						TC++;
						if (TC >= DTMFConstanst.FLUSH_TIME && x != DTMFConstanst.DSIL)
						{
							if(ToneDetected!=null)
							ToneDetected.BeginInvoke(this, new DTMFDetectedArgs(CurrentTone, (double)(TC * DTMFConstanst.N) / DTMFConstanst.FSAMPLE), null, null);
							TC = 0;
							newTone = true;
						}
						else
						{
							if (x == DTMFConstanst.DSIL && DetectSilence && TC >= DTMFConstanst.FLUSH_TIME)
							{
								if(ToneDetected!=null)
								ToneDetected.BeginInvoke(this, new DTMFDetectedArgs(CurrentTone, (double)(TC * DTMFConstanst.N) / DTMFConstanst.FSAMPLE), null, null);
								TC = 0;
								newTone = true;
							}
						}
					}
					else
					{
						TC++;
						if (ToneDetected != null)
						{
							newTone = true;
							if(ToneDetected!=null)
							ToneDetected.BeginInvoke(this, new DTMFDetectedArgs(CurrentTone, (double)(TC * DTMFConstanst.N) / DTMFConstanst.FSAMPLE), null, null);
						}
						TC = 0;
						CurrentTone = x;
					}
					if (IsTokenBuildEnabled && newTone)
						parser.Digest (x);
				}
			}
		}

		int calc_power(byte[] data, float[] power)
		{
			float[] u0 = new float[DTMFConstanst.NUMTONES];
			float[] u1 = new float[DTMFConstanst.NUMTONES];
			float t, inn;
			int i, j;

			for (j = 0; j < DTMFConstanst.NUMTONES; j++)
			{
				u0[j] = 0.0f;
				u1[j] = 0.0f;
			}
			for (i = 0; i < (DTMFConstanst.N) - 1; i += 2)
			{   /* feedback */
				//inn = (float)((data[i] / 128.0)+(data[i+1]/128.0));
				int br = data[(i) + 1];
				int bl = data[i] << 8;
				inn = (float)((br + bl) / 65535.0);
				for (j = 0; j < DTMFConstanst.NUMTONES; j++)
				{
					t = u0[j];
					u0[j] = inn + DTMFConstanst.coef[j] * u0[j] - u1[j];
					u1[j] = t;
				}
			}
			for (j = 0; j < DTMFConstanst.NUMTONES; j++)   /* feedforward */
				power[j] = u0[j] * u0[j] + u1[j] * u1[j] - DTMFConstanst.coef[j] * u0[j] * u1[j];
			return (0);
		}

		/*
         * detect which signals are present.
         *
         * return values defined in the include file
         * note: DTMF 3 and MF 7 conflict.  To resolve
         * this the program only reports MF 7 between
         * a KP and an ST, otherwise DTMF 3 is returned
         */
		int decode(byte[] data)
		{
			float[] power = new float[DTMFConstanst.NUMTONES];
			float thresh;
			float maxpower;
			int[] on = new int[DTMFConstanst.NUMTONES];
			int on_count;
			int bcount, rcount, ccount;
			int row = 0, col = 0, b1 = 0, b2 = 0, i = 0;
			int[] r = new int[4];
			int[] c = new int[4];
			int[] b = new int[8];
			bool MFmode = false;

			calc_power(data, power);

			for (i = 0, maxpower = 0.0f; i < DTMFConstanst.NUMTONES; i++)
				if (power[i] > maxpower)
					maxpower = power[i];

			if (maxpower < DTMFConstanst.THRESH)  /* silence? */
				return (DTMFConstanst.DSIL);
			thresh = (float)(DTMFConstanst.RANGE * maxpower);    /* allowable range of powers */
			for (i = 0, on_count = 0; i < DTMFConstanst.NUMTONES; i++)
			{
				if (power[i] > thresh)
				{
					on[i] = 1;
					on_count++;
				}
				else
					on[i] = 0;
			}

			if (on_count == 1)
			{
				if (on[DTMFConstanst.B7] == 1)
					return (DTMFConstanst.D24);
				if (on[DTMFConstanst.B8] == 1)
					return (DTMFConstanst.D26);
				return (-1);
			}

			if (on_count == 2)
			{
				if (on[DTMFConstanst.X1] == 1 && on[DTMFConstanst.X2] == 1)
					return (DTMFConstanst.DDT);
				if (on[DTMFConstanst.X2] == 1 && on[DTMFConstanst.X3] == 1)
					return (DTMFConstanst.DRING);
				if (on[DTMFConstanst.X3] == 1 && on[DTMFConstanst.X4] == 1)
					return (DTMFConstanst.DBUSY);

				b[0] = on[DTMFConstanst.B1]; b[1] = on[DTMFConstanst.B2]; b[2] = on[DTMFConstanst.B3]; b[3] = on[DTMFConstanst.B4];
				b[4] = on[DTMFConstanst.B5]; b[5] = on[DTMFConstanst.B6]; b[6] = on[DTMFConstanst.B7]; b[7] = on[DTMFConstanst.B8];
				c[0] = on[DTMFConstanst.C1]; c[1] = on[DTMFConstanst.C2]; c[2] = on[DTMFConstanst.C3]; c[3] = on[DTMFConstanst.C4];
				r[0] = on[DTMFConstanst.R1]; r[1] = on[DTMFConstanst.R2]; r[2] = on[DTMFConstanst.R3]; r[3] = on[DTMFConstanst.R4];

				for (i = 0, bcount = 0; i < 8; i++)
				{
					if (b[i] == 1)
					{
						bcount++;
						b2 = b1;
						b1 = i;
					}
				}
				for (i = 0, rcount = 0; i < 4; i++)
				{
					if (r[i] == 1)
					{
						rcount++;
						row = i;
					}
				}
				for (i = 0, ccount = 0; i < 4; i++)
				{
					if (c[i] == 1)
					{
						ccount++;
						col = i;
					}
				}

				if (rcount == 1 && ccount == 1)
				{   /* DTMF */
					if (col == 3)  /* A,B,C,D */
						return (DTMFConstanst.DA + row);
					else
					{
						if (row == 3 && col == 0)
							return (DTMFConstanst.DSTAR);
						if (row == 3 && col == 2)
							return (DTMFConstanst.DPND);
						if (row == 3)
							return (DTMFConstanst.D0);
						if (row == 0 && col == 2)
						{   /* DTMF 3 conflicts with MF 7 */
							if (!MFmode)
								return (DTMFConstanst.D3);
						}
						else
							return (DTMFConstanst.D1 + col + row * 3);
					}
				}

				if (bcount == 2)
				{       /* MF */
					/* b1 has upper number, b2 has lower */
					switch (b1)
					{
						case 7: return ((b2 == 6) ? DTMFConstanst.D2426 : -1);
						case 6: return (-1);
						case 5: if (b2 == 2 || b2 == 3)  /* KP */
						MFmode = true;
						if (b2 == 4)  /* ST */
							MFmode = false;
						return (DTMFConstanst.DC11 + b2);
						/* MF 7 conflicts with DTMF 3, but if we made it
                         * here then DTMF 3 was already tested for 
                         */
						case 4: return ((b2 == 3) ? DTMFConstanst.D0 : DTMFConstanst.D7 + b2);
						case 3: return (DTMFConstanst.D4 + b2);
						case 2: return (DTMFConstanst.D2 + b2);
						case 1: return (DTMFConstanst.D1);
					}
				}
				return (-1);
			}

			if (on_count == 0)
				return (DTMFConstanst.DSIL);
			return (-1);
		}
		void AudioClipListener.heard(byte[] data, int sampleRate){
			//byte[] copy = new byte[data.Length];
			//data.CopyTo (copy,0);
			//DetectformDataChunk (copy, data.Length);
			DetectformDataChunk (data, data.Length);
		}
	}
}


