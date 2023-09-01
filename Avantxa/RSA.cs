using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApiAvantxa.Security
{
    public class RSA
    {
        static public byte[] Encryption(string Data)
        {
            try
            {
                byte[] bytesData = Encoding.Unicode.GetBytes(Data);
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString("<RSAKeyValue><Modulus>vwPYr4xq0NjoDD5pax0qIo+dJ4M6LSxCGWARTbCqdtjmjtlZNWQiA73QY8iWc544xiBCrfi+z1n2ZxCYzoJL/ByFVPVHvTGJkQM62IytMdBy5gvr6rIXE3c4yiwCbxQENVOQYiyIfAnLHsCg341K1rTC09HETFWA+UPwFp8X/SAXcQhcZM6+Kwfuyk7hBqFZWc5gbcIn7dcE0FTAyaQeHb901wYLmnhT123bjyKKbxScllSyBLQRrejg+R7Nq1LjZghmE4NdllDeB6PZlE46+NeaJNQy83Sa2aqJX8POC6tM8suIfZ9xAe4NmE4ukl4NjqH3M67bUTp41AECwAQdyQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                    encryptedData = RSA.Encrypt(bytesData, false);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public string Decryption(byte[] Data)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.FromXmlString("<RSAKeyValue><Modulus>vwPYr4xq0NjoDD5pax0qIo+dJ4M6LSxCGWARTbCqdtjmjtlZNWQiA73QY8iWc544xiBCrfi+z1n2ZxCYzoJL/ByFVPVHvTGJkQM62IytMdBy5gvr6rIXE3c4yiwCbxQENVOQYiyIfAnLHsCg341K1rTC09HETFWA+UPwFp8X/SAXcQhcZM6+Kwfuyk7hBqFZWc5gbcIn7dcE0FTAyaQeHb901wYLmnhT123bjyKKbxScllSyBLQRrejg+R7Nq1LjZghmE4NdllDeB6PZlE46+NeaJNQy83Sa2aqJX8POC6tM8suIfZ9xAe4NmE4ukl4NjqH3M67bUTp41AECwAQdyQ==</Modulus><Exponent>AQAB</Exponent><P>+/l7rdGDGAbBMqdYWOBt1rhtnDmr7ZW3gpH+uapmruRL/oxBodC2ZyXfC6uOfUWbmVawkLsiw86DWS24vzATUFh1r1br/DQyR8bNQ8I4M4ZYixNNeTxk6RhbFXN4Uph82ZpBSdxa//XeuRbFpSIp0TGTPIO83wU5E+Qka0/sHZ8=</P><Q>whENm2NUsy4p5JKZIv2Rrsue7lZaY4xMhVfj2L+QcuOhXyy4x32IlpVSzMiSEbSn34DWXFGyRZ4zMihi1rAsyL0qDfmxkn7AA8HD5PZVT4Qvy+gAfP7hii2k85w7wWYjBjek8zdKzWhBIx8sXK3csEfhpBmMCQPiOTtXHpE1O5c=</Q><DP>lY3EPKyntHD95oSwyT+bseARHrKUOxWrr9HbcHOVMqTJ/jFdGx/3w00VD6YpkmzoJ6Dud1i7D6DZEOs0RjXQoNWANCvRMQYB+dwjJN534Q0SLKmuSBDyi+8q0JaSieN75uPJcH9be6SBJzY+5P5b95AHJdnDFlPRRt8YRqFPMV0=</DP><DQ>G4hz18Hl+G3qP6WU2GQFUTlOWR4jQNBc4uYvS0cSZVpqQKiFprfmswaIcslZ/+0TjCApSXvFwR7KwKj+LNtd3zUHTGhKizA0adQJyOgx/lAQv+swdotq0EHdjzpxN+UXvwASBPepQy8xXpxDnPqFvvTTzIZbWcQVSm1i1NFhLg0=</DQ><InverseQ>HAcRssI5Rt4n8qj9OhvA0LKljdqn67vY5kxG1N9lAiISBEJ+Vmg44SI0h+hZO8fpXVJjkdyp5e5ZnrhPASwW2lnxsx57QJv1TU8Hgu4Ztr16FXY8kPdXlyMYHyH5MGFHusQlCOxWXj9Ndr1gfjR/mI553shQaEpnb/+FpSA+wDc=</InverseQ><D>SSdk9h5VzjQz8nR4lRAvUdskebx1LFW13tf+6H0PW8LH0c+Frb/ykvI/++cXT6I55g722n/YO7sdde1SSEx4Y05c9eOWHOv63nGZONPvhJNrXZUVPeYElmzjb/IY4IeO8QlW03JwtuhdRSLKubL4BXBm6Q5+Gino4g3VxiQZ4NOds3ZZV9ZAyGbVW7GZC02mvkek622D3r2oI215OyUZ4pg6SER2XLDcvKVzHwXMUrnB8dIGbFObarSnPW3/ESPaHZ9nenTeng9bFZWH7DFLf9YVR7LkUDlE6PW0fHT5cQN+QMnv7t8vf415Nk1MywUSXg3Nh731xRpC97jiSKIaXQ==</D></RSAKeyValue>");
                    decryptedData = RSA.Decrypt(Data, false);
                }
                return Encoding.Unicode.GetString(decryptedData);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}