using System;
using System.Collections;
using System.Linq;

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
        public DTMFDetectedArgs(char ToneKey, double duration)
        {
            this.TonConstant = ToneCodeFromKey(ToneKey);
            this.Duration = duration;
        }

        public static int ToneCodeFromKey(char key) {
            int code = -1;
            switch (key) {
                case '0':
                    code = DTMFConstanst.D0;
                    break;
                case '1':
                    code = DTMFConstanst.D1;
                    break;
                case '2':
                    code = DTMFConstanst.D2;
                    break;
                case '3':
                    code = DTMFConstanst.D3;
                    break;
                case '4':
                    code = DTMFConstanst.D4;
                    break;
                case '5':
                    code = DTMFConstanst.D5;
                    break;
                case '6':
                    code = DTMFConstanst.D6;
                    break;
                case '7':
                    code = DTMFConstanst.D7;
                    break;
                case '8':
                    code = DTMFConstanst.D8;
                    break;
                case '9':
                    code = DTMFConstanst.D9;
                    break;
                case '*':
                    code = DTMFConstanst.DSTAR;
                    break;
                case '#':
                    code = DTMFConstanst.DPND;
                    break;
                case 'A':
                    code = DTMFConstanst.DA;
                    break;
                case 'B':
                    code = DTMFConstanst.DB;
                    break;
                case 'C':
                    code = DTMFConstanst.DC;
                    break;
                case 'D':
                    code = DTMFConstanst.DD;
                    break;
                default:
                    code = DTMFConstanst.DSIL;
                    break;
            }
            return code;
        }
    }
}

