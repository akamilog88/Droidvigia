using System;

namespace DroidToneDecoder
{    
	public class DTMFConstanst
	{

		//8000---44100
		//240 --- x
		public const int FSAMPLE = 44100;
		public const int N = 2600;

		public readonly int[] k = new int[]{10,13,14,18,21,23,25,27,28,32,36,38,39,44,48,50,71,77};

		/* coefficients for above k's as:
         *   2 * cos( 2*pi* k/N )
         */
		public static readonly float[] coef = new float[] { 1.997664f, 1.996053f, 1.995423f, 1.992436f, 1.989707f, 1.987655f, 1.985418f, 1.982995f, 1.981714f, 1.976127f, 1.969802f, 1.966363f, 1.964574f, 1.954945f, 1.946419f, 1.941884f, 1.883393f, 1.86309f };


		public const int X1 = 0;    /* 350 dialtone */
		public const int X2 = 1;    /* 440 ring, dialtone */
		public const int X3 = 2;    /* 480 ring, busy */
		public const int X4 = 3;    /* 620 busy */

		public const int R1 = 4;    /* 697, dtmf row 1 */
		public const int R2 = 5;    /* 770, dtmf row 2 */
		public const int R3 = 6;    /* 852, dtmf row 3 */
		public const int R4 = 8;    /* 941, dtmf row 4 */
		public const int C1 = 10;    /* 1209, dtmf col 1 */
		public const int C2 = 12;    /* 1336, dtmf col 2 */
		public const int C3 = 13;    /* 1477, dtmf col 3 */
		public const int C4 = 14;    /* 1633, dtmf col 4 */

		public const int B1 = 4;    /* 700, blue box 1 */
		public const int B2 = 7;    /* 900, bb 2 */
		public const int B3 = 9;    /* 1100, bb 3 */
		public const int B4 = 11;    /* 1300, bb4 */
		public const int B5 = 13;    /* 1500, bb5 */
		public const int B6 = 15;    /* 1700, bb6 */
		public const int B7 = 16;    /* 2400, bb7 */
		public const int B8 = 17;    /* 2600, bb8 */

		public const int NUMTONES = 18;

		/* values returned by detect 
         *  0-9     DTMF 0 through 9 or MF 0-9
         *  10-11   DTMF *, #
         *  12-15   DTMF A,B,C,D
         *  16-20   MF last column: C11, C12, KP1, KP2, ST
         *  21      2400
         *  22      2600
         *  23      2400 + 2600
         *  24      DIALTONE
         *  25      RING
         *  26      BUSY
         *  27      silence
         *  -1      invalid
         */
		public const int D0 = 0;
		public const int D1 = 1;
		public const int D2 = 2;
		public const int D3 = 3;
		public const int D4 = 4;
		public const int D5 = 5;
		public const int D6 = 6;
		public const int D7 = 7;
		public const int D8 = 8;
		public const int D9 = 9;
		public const int DSTAR = 10;
		public const int DPND = 11;
		public const int DA = 12;
		public const int DB = 13;
		public const int DC = 14;
		public const int DD = 15;
		public const int DC11 = 16;
		public const int DC12 = 17;
		public const int DKP1 = 18;
		public const int DKP2 = 19;
		public const int DST = 20;
		public const int D24 = 21;
		public const int D26 = 22;
		public const int D2426 = 23;
		public const int DDT = 24;
		public const int DRING = 25;
		public const int DBUSY = 26;
		public const int DSIL = 27;

		/* translation of above codes into text */
		public static readonly String[] dtran = {
			"0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
			"*", "#", "A", "B", "C", "D", 
			"+C11", "+C12", "KP1+", "KP2+", "+ST",
			"2400", "2600", "2400+2600",
			"DIALTONE", "RING", "BUSY","SILENCE" };


		public const double RANGE = 0.1;           /* any thing higher than RANGE*peak is "on" */
		public const double THRESH = 5000;         /* minimum level for the loudest tone */
		//public const double FLUSH_TIME = 100;       /* 100 frames = 3 seconds */
		public const int FLUSH_TIME = 1;   /* (8 * 240 = 2000) ---- 8000 = 1 second  2000/8000 = 1/4 second  */
		/* (8 * 1300 = 10400) ---- 44100 = 1 second  10400/44100 = 1/4 second  */
	}

}


