using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;

namespace DroidvigiaCompat.Utils
{
	public static class StringExtensions
	{
		public static String[] DaysOfWeak = { "Domingo","Lunes","Martes","Miercoles","Jueves","Viernes","Sabado"};

		public static bool IsSimilarTo(this String str,String str2,double percent) {
			List<char> result1= str.ToCharArray().Intersect(str2.ToCharArray()).ToList();
			double d1=(result1.Count/Convert.ToDouble(str2.Length));

			if(d1>percent ){
				return true;
			}
			return false;
		}

		public static String CleanNumber(this String str) {
			string result = str.Replace ("*99","");
			return result.Replace ("+53","");
		}
		/// <summary>
		/// Cada caracter es una representacion ASCII de un codigo DTMF?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsDTMFSecuence(this String value)
		{
			var objNotWholePattern = new Regex("[^0-9]*[#][*][A-D]");
			return !objNotWholePattern.IsMatch(value);
		}
  		public static DayOfWeek? ToDayOfWeakEnum(this String value)
		{		
			if(value == DaysOfWeak[0])
				return DayOfWeek.Sunday;				
			if(value == DaysOfWeak[1])
				return DayOfWeek.Monday;	
			if(value == DaysOfWeak[2])
				return DayOfWeek.Tuesday;	
			if(value == DaysOfWeak[3])
				return DayOfWeek.Wednesday;	
			if(value == DaysOfWeak[4])
				return DayOfWeek.Thursday;	
			if(value == DaysOfWeak[5])
				return DayOfWeek.Friday;	
			if(value == DaysOfWeak[6])
				return DayOfWeek.Saturday;	
			return null;
		}

		public static String ToDayOfWeakStringFromEnum(DayOfWeek value)
		{		
			// "Domingo","Lunes","Martes","Miercoles","Jueves","Viernes","Sabado"
			if(value == DayOfWeek.Sunday)
				return  DaysOfWeak[0];				
			if(value == DayOfWeek.Monday)
				return  DaysOfWeak[1];	
			if(value == DayOfWeek.Tuesday)
				return  DaysOfWeak[2];	
			if(value == DayOfWeek.Wednesday)
				return   DaysOfWeak[3];	
			if(value ==DayOfWeek.Thursday)
				return DaysOfWeak[4]  ;	
			if(value == DayOfWeek.Friday )
				return  DaysOfWeak[5];	
			if(value == DayOfWeek.Saturday )
				return  DaysOfWeak[6];	
			return "";
		}
		public static String ToLittleCase(this String value)
		{
			string sval = "";
			if (value != null && value.ToString().Length >= 1)
				sval = value.ToString().ToLower();
			else
				return "";
			char[] cval = sval.ToCharArray();
			cval[0]= cval[0].ToString().ToUpper().ToCharArray()[0];
			string pattern = @"\b[a-z]+";
			var matches = Regex.Matches(sval, pattern);
			foreach (Match item in matches)
			{
				if (item.Length >= 3)
					cval[item.Index] = cval[item.Index].ToString().ToUpper().ToCharArray()[0];
			}
			string result = "";
			cval.ToList().ForEach(s => result += s);
			return result;
		}
		public static byte[] GZipCompress(this String value)
		{
			byte[] result;
			//var dummyMS = new MemoryStream(ASCIIEncoding.UTF8.GetBytes(value));
			//result = ASCIIEncoding.ASCII.GetString(dummyMS.ToArray());

			using (StreamReader sourceStream = new StreamReader(new MemoryStream(ASCIIEncoding.Default.GetBytes(value))))
			{              
				using (MemoryStream compressedStream = new MemoryStream())
				{
					using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
					{
						using (StreamWriter w = new StreamWriter(zipStream)) {
							w.Write( sourceStream.ReadToEnd());
							w.Flush();
						}
						result = ((MemoryStream)compressedStream).ToArray();
					}
				}
			}
			return result;
		}
		public static String GZipDecompress(this byte[] value)
		{
			string result = "";
			using (Stream sourceStream = new MemoryStream(value))
			{
				using (var zipStream = new GZipStream(sourceStream, CompressionMode.Decompress))
				{
					using (StreamReader reader = new StreamReader(zipStream)) {
						result= reader.ReadToEnd();
					}
				}
			}
			return result;
		}
		public static bool IsPassFormat(this String value)
		{
			var tieneNum = new Regex("[0-9]+");
			var tieneLetra = new Regex("[a-zA-Z]+");
			var tieneCE = new Regex(@"(\W)+");
			return tieneNum.IsMatch(value) && tieneLetra.IsMatch(value) &&
				tieneCE.IsMatch(value);
		}

		/// <summary>
		/// Es una cadena vacía?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsEmpty(this String value)
		{
			return (value.Length == 0);
		}

		/// <summary>
		/// Es un número natural?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsNaturalNumber(this String value)
		{
			var objNotNaturalPattern = new Regex("[^0-9]");
			var objNaturalPattern = new Regex("0*[1-9][0-9]*");
			return !objNotNaturalPattern.IsMatch(value) &&
				objNaturalPattern.IsMatch(value);
		}

		/// <summary>
		/// Es un número entero positivo?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsWholeNumber(this String value)
		{
			var objNotWholePattern = new Regex("[^0-9]");
			return !objNotWholePattern.IsMatch(value);
		}
	
		/// <summary>
		/// Es un número entero?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsInteger(this String value)
		{
			var objNotIntPattern = new Regex("[^0-9-]");
			var objIntPattern = new Regex("^-[0-9]+$|^[0-9]+$");
			return !objNotIntPattern.IsMatch(value) && objIntPattern.IsMatch(value);
		}

		/// <summary>
		/// Es un número positivo?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsPositiveNumber(this String value)
		{
			if (value == "")
				return true;
			var objNotPositivePattern = new Regex("[^0-9.]");
			var objPositivePattern = new Regex("^[.][0-9]+$|[0-9]*[.]*[0-9]+$");
			var objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			return !objNotPositivePattern.IsMatch(value) &&
				objPositivePattern.IsMatch(value) &&
					!objTwoDotPattern.IsMatch(value);
		}

		/// <summary>
		/// Es un número?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsNumber(this String value)
		{
			var objNotNumberPattern = new Regex("[^0-9.-]");
			var objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
			var objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
			const string strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
			const string strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
			var objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");
			return !objNotNumberPattern.IsMatch(value) &&
				!objTwoDotPattern.IsMatch(value) &&
					!objTwoMinusPattern.IsMatch(value) &&
					objNumberPattern.IsMatch(value);
		}

		/// <summary>
		/// Solo tiene letras?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsAlpha(this String value)
		{
			var objAlphaPattern = new Regex("[^a-zA-ZñÑ]");
			return !objAlphaPattern.IsMatch(value);
		}

		/// <summary>
		/// Tiene solo letras y números?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsAlphaNumeric(this String value)
		{
			var objAlphaNumericPattern = new Regex("[^a-zA-Z0-9]");
			return !objAlphaNumericPattern.IsMatch(value);
		}

		/// <summary>
		/// Es una dirección de correo?
		/// </summary>
		/// <returns>True en caso verdadero, False en otro caso</returns>
		public static bool IsEmail(this String value)
		{
			if (value == "")
				return true;
			var objEmailPattern =
				new Regex(
					@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
			return objEmailPattern.IsMatch(value);
		}
	}
}
