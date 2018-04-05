using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Security.Cryptography;
using VigiaCryptoFunctions;

namespace DroidvigiaBasicoCompat
{
	public class LicenseUtility
	{
		const String PUBLIC_KEY=@"<RSAKeyValue><Modulus>yQB1hXktKZYfTwr/8HeAfok4bvPm0yJHNtuIMsXDUrLZfI7o3Z9GJiWnKUEO/lsFvqHoT26cW6JDleffYKhspsHTjUWXh4njfhyiaRlAOwFxMOm3WVGRk/PhNHPeWBVzRfBYbCxg6deI6LuPwV1cTKgKAbHgmwjABVc3SCP6+p8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

		public static bool IsValidSerial(String signed,String LincenseInfo){
			var public_key = new RSACryptoServiceProvider ();
			public_key.FromXmlString (PUBLIC_KEY);
			VigiaCryptografi vcripto = new VigiaCryptografi(public_key,LincenseInfo,"SHA1");
			if (signed == "" || LincenseInfo == "")
				return false;
			bool result = false;
			try{
				result = vcripto.VerifySignature (Convert.FromBase64String( signed));
			}catch(Exception){
				result = false;
			}
			return result;
		}
	}
}