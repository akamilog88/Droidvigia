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

namespace VigiaCryptoFunctions
{
	public class VigiaCryptografi
	{
		private string origMessaje;
		private string hashAlgorithm;
		RSACryptoServiceProvider rsa_public_data;

		const String SALT="Ra9h21sB";

		public VigiaCryptografi(RSACryptoServiceProvider rsa_public_data,string origMessaje,string hashAlgorithm) {
			this.origMessaje = origMessaje;
			this.rsa_public_data = rsa_public_data;
			this.hashAlgorithm = hashAlgorithm;
		}
		public bool VerifySignature(byte[] signedHash) {
			//Convert the string into an array of bytes. 
			//UnicodeEncoding UE = new UnicodeEncoding();
			byte[] MessageBytes = ASCIIEncoding.ASCII.GetBytes(origMessaje+SALT);

			//Create a new instance of the SHA1Managed class to create  
			//the hash value.
			SHA1Managed SHhash = new SHA1Managed();

			//Create the hash value from the array of bytes.
			byte[] HashValue = SHhash.ComputeHash(MessageBytes);

			RSAPKCS1SignatureDeformatter RSADeformatter = new RSAPKCS1SignatureDeformatter(rsa_public_data);
			RSADeformatter.SetHashAlgorithm(hashAlgorithm);

			if (RSADeformatter.VerifySignature(HashValue, signedHash))
			{
				return true;
			}
			return false;
		}

		public bool VerifySignature(String signedHash)
		{
			return VerifySignature(  SignatureBytesEncode(signedHash));
		}

		public byte[] Sign(string message,RSACryptoServiceProvider rsa_private_data)
		{
			// UnicodeEncoding UE = new UnicodeEncoding();

			//Convert the string into an array of bytes. 
			byte[] MessageBytes = ASCIIEncoding.ASCII.GetBytes(message + SALT);

			//Create a new instance of the SHA1Managed class to create  
			//the hash value.
			SHA1Managed SHhash = new SHA1Managed();

			//Create the hash value from the array of bytes.
			byte[] HashValue = SHhash.ComputeHash(MessageBytes);

			RSAPKCS1SignatureFormatter RSAFormatter = new RSAPKCS1SignatureFormatter(rsa_private_data);

			//Set the hash algorithm to SHA1.
			RSAFormatter.SetHashAlgorithm(hashAlgorithm);

			//Create a signature for HashValue and assign it to 
			//SignedHashValue.
			return RSAFormatter.CreateSignature(HashValue);
			//return RSAFormatter.CreateSignature(MessageBytes);
		}

		public String SignatureStringEncode(byte[] Sign) {            
			return Convert.ToBase64String(Sign);
		}
		public byte[] SignatureBytesEncode(string Sign)
		{           
			return Convert.FromBase64String(Sign);
		}
	}
}

