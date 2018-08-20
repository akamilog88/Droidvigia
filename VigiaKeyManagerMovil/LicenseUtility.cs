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

namespace VigiaCryptoFunctions
{
    public class LicenseUtility
    {
        const String PUBLIC_KEY = @"<RSAKeyValue><Modulus>yQB1hXktKZYfTwr/8HeAfok4bvPm0yJHNtuIMsXDUrLZfI7o3Z9GJiWnKUEO/lsFvqHoT26cW6JDleffYKhspsHTjUWXh4njfhyiaRlAOwFxMOm3WVGRk/PhNHPeWBVzRfBYbCxg6deI6LuPwV1cTKgKAbHgmwjABVc3SCP6+p8=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        const String FULL_KEY = @"<RSAKeyValue><Modulus>yQB1hXktKZYfTwr/8HeAfok4bvPm0yJHNtuIMsXDUrLZfI7o3Z9GJiWnKUEO/lsFvqHoT26cW6JDleffYKhspsHTjUWXh4njfhyiaRlAOwFxMOm3WVGRk/PhNHPeWBVzRfBYbCxg6deI6LuPwV1cTKgKAbHgmwjABVc3SCP6+p8=</Modulus><Exponent>AQAB</Exponent><P>7evbMg+QYhCMK3J/kCRjrc0PlU1Z8qhPRSx359z5syyU81IhB++Qta+W8X6v3xbUbae+uV+NkBjMFDQ6/tzktw==</P><Q>2EZt1xAUYF/ener5WJzxIL+2yCHpt1OksENTP2/+wKagSgVdmR0QdpMX4l9Klfwcx7BsgmNFSGQMBoEwmstBWQ==</Q><DP>VLbBIm8GQtSPhDzNjN5YG59DKC8VkuA48XFm9EjrI9AA7S5WEoRLa1WtEinAp1dypeSvdUO/nQonSB5czziJuw==</DP><DQ>QvHaAh4KEKLcR8l6EUHTKa3uKBjHPWX4rk7Ir/Q8yOlK6F6s0D484Fc7p2iTr8VwKyejDuEnivbc+g53OxL7UQ==</DQ><InverseQ>gKVsunSX5VYb8TDDy7t3lUyGqvUn3tk5atM1U8hXw5u65zLC0X0slXlgcY0BAwt1w8OZn9fzVustRKrXJ73bKA==</InverseQ><D>yNIimLZW/wO0SD7PM6vRv5rNvV0GM9A9ont3+nJIEioKzT3jPvnRFm/sMXWEy3CZrrsI5cO3iH34VJ+UOHhOBBA8MXbQuM1ZFefOKsPpJYmXWTTLGIeVIIMRQIl1fTx4NfcCaFMaZHhq85OftQGDuVBAier9GnPNyjZ38KZq12E=</D></RSAKeyValue>";

        public static bool IsValidSerial(String signed, String LincenseInfo)
        {
            var public_key = new RSACryptoServiceProvider();
            public_key.FromXmlString(PUBLIC_KEY);
            VigiaCryptografi vcripto = new VigiaCryptografi(public_key, LincenseInfo, "SHA1");
            if (signed == "" || LincenseInfo == "")
                return false;
            bool result = false;
            try
            {
                result = vcripto.VerifySignature(Convert.FromBase64String(signed));
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static String GenFirma(String request) {
            byte[] SignedData = new byte[128];
            String SignedBase64 = "";
            RSACryptoServiceProvider public_rsa;
            VigiaCryptografi cfunction;

            public_rsa = new RSACryptoServiceProvider();
            public_rsa.FromXmlString(PUBLIC_KEY);
            cfunction = new VigiaCryptografi(public_rsa, "", "SHA1");

            var private_rsa = new RSACryptoServiceProvider();
            private_rsa.FromXmlString(FULL_KEY);
            byte[] shash = cfunction.Sign(request, private_rsa);
            SignedData = shash;
            SignedBase64 = Convert.ToBase64String(SignedData);
            return SignedBase64;
        }
    }
}

