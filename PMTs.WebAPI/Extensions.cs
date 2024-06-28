using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PMTs.WebAPI
{
    public class Extensions
    {
        public const string AUTHEN_FIAL = "You don't have permission to access Web API";

        public static void Decrypt(string cipherString, ref string appName, ref bool isAuthen)
        {
            try
            {
                List<string> allowApp =
                [
                    "PMTs","Presale","InterfaceMO","InterfaceHandshake","InterfaceKIWI","AppTCNK","Nerp","OrderTracking"
                ];

                byte[] keyArray;

                byte[] toEncryptArray = Convert.FromBase64String(cipherString);

                var hashmd5 = MD5.Create();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes("PMTs"));
                hashmd5.Clear();

                var tdes = TripleDES.Create();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.CBC;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();

                string decryptNAme = appName = UTF8Encoding.UTF8.GetString(resultArray);
                int find = allowApp.FindIndex(s => s.Contains(decryptNAme));

                if (find < 0)
                {
                    isAuthen = false;
                }
                else
                {
                    isAuthen = true;
                }

            }
            catch
            {
                isAuthen = false;
                // log error
            }
        }

    }
}
