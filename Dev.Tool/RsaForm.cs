using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Dev.Tool
{
    public partial class RsaForm : Form
    {
        private AsymmetricCipherKeyPair _asymmetricCipherKeyPair;
        public RsaForm()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var strength = GetStrength();
            string pirvKey = string.Empty, pubKey = string.Empty;
            var keyFmt = cboxKeyFmt.Text ?? (cboxKeyFmt.Items[0] as string);
            switch (keyFmt)
            {
                case "pkcs#1":
                    GetRsaWithPem(strength, false, out pirvKey, out pubKey);
                    break;
                case "pkcs#8":
                    GetRsaWithPem(strength, true, out pirvKey, out pubKey);
                    break;
                case "xml":
                    GetRsaWithXml(strength, out pirvKey, out pubKey);
                    break;
                case "json":
                    GetRsaWithJson(strength, out pirvKey, out pubKey);
                    break;
                default:
                    GetRsaWithPem(strength, false, out pirvKey, out pubKey);
                    break;
            }
            
            txtPrivate.Text = pirvKey;
            txtPublic.Text = pubKey;
        }
        
        private Int32 GetStrength()
        {
            var strength = 256;

            var txt = cboxKeyStrength.Text ?? (cboxKeyStrength.Items[0] as string);
            if (!Int32.TryParse(txt, out strength))
            {
                strength = 256;
            }

            return strength;
        }

        private void GetRsaWithPem(Int32 strength, Boolean usePkcs8, out string pirvKey, out string pubKey)
        {
            IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            kpGen.Init(new KeyGenerationParameters(new SecureRandom(), strength));
            _asymmetricCipherKeyPair = kpGen.GenerateKeyPair();
            var privKeyParam = _asymmetricCipherKeyPair.Private;
            var pubKeyParam = _asymmetricCipherKeyPair.Public;

            using (StringWriter sw = new StringWriter())
            {
                PemWriter pWrt = new PemWriter(sw);
                if (usePkcs8)
                {
                    var pkcs8 = new Pkcs8Generator(privKeyParam);
                    pWrt.WriteObject(pkcs8);
                }
                else
                {
                    pWrt.WriteObject(privKeyParam);
                }
                
                pirvKey = sw.ToString();
            }

            using (StringWriter sw = new StringWriter())
            {
                PemWriter pWrt = new PemWriter(sw);
                pWrt.WriteObject(pubKeyParam);
                pubKey = sw.ToString();
            }
        }
        
        private string RsaParamToXml(RSAParameters rsaParams, bool includePrivateParameters)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<RSAKeyValue>");
            // Add the modulus
            sb.Append("<Modulus>" + Convert.ToBase64String(rsaParams.Modulus) + "</Modulus>");
            // Add the exponent
            sb.Append("<Exponent>" + Convert.ToBase64String(rsaParams.Exponent) + "</Exponent>");
            if (includePrivateParameters)
            {
                // Add the private components
                sb.Append("<P>" + Convert.ToBase64String(rsaParams.P) + "</P>");
                sb.Append("<Q>" + Convert.ToBase64String(rsaParams.Q) + "</Q>");
                sb.Append("<DP>" + Convert.ToBase64String(rsaParams.DP) + "</DP>");
                sb.Append("<DQ>" + Convert.ToBase64String(rsaParams.DQ) + "</DQ>");
                sb.Append("<InverseQ>" + Convert.ToBase64String(rsaParams.InverseQ) + "</InverseQ>");
                sb.Append("<D>" + Convert.ToBase64String(rsaParams.D) + "</D>");
            }
            sb.Append("</RSAKeyValue>");
            return (sb.ToString());
        }

        private void GetRsaWithXml(Int32 strength, out string pirvKey, out string pubKey)
        {
            IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            kpGen.Init(new KeyGenerationParameters(new SecureRandom(), strength));
            _asymmetricCipherKeyPair = kpGen.GenerateKeyPair();

            RSAParameters privKeyParam = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)_asymmetricCipherKeyPair.Private);
            RSAParameters pubKeyParam = DotNetUtilities.ToRSAParameters((RsaKeyParameters)_asymmetricCipherKeyPair.Public);
            pirvKey = RsaParamToXml(privKeyParam, true);
            pubKey = RsaParamToXml(pubKeyParam, false);
        }

        private string RsaParamToJson(RSAParameters rsaParams, bool includePrivateParameters)
        {
            var dict = new Dictionary<string, string>();
            dict.Add("Modulus", Convert.ToBase64String(rsaParams.Modulus));
            dict.Add("Exponent", Convert.ToBase64String(rsaParams.Exponent));

            if (includePrivateParameters)
            {
                // Add the private components
                dict.Add("P", Convert.ToBase64String(rsaParams.P));
                dict.Add("Q", Convert.ToBase64String(rsaParams.Q));
                dict.Add("DP", Convert.ToBase64String(rsaParams.DP));
                dict.Add("DQ", Convert.ToBase64String(rsaParams.DQ));
                dict.Add("InverseQ", Convert.ToBase64String(rsaParams.InverseQ));
                dict.Add("D", Convert.ToBase64String(rsaParams.D));                
            }

            return "{" + string.Join(",", dict
                .Where(m => !string.IsNullOrWhiteSpace(m.Value))
                .Select(m => string.Format("\"{0}\":\"{1}\"", m.Key, m.Value))) + "}";
        }

        private void GetRsaWithJson(Int32 strength, out string pirvKey, out string pubKey)
        {
            IAsymmetricCipherKeyPairGenerator kpGen = GeneratorUtilities.GetKeyPairGenerator("RSA");
            kpGen.Init(new KeyGenerationParameters(new SecureRandom(), strength));
            _asymmetricCipherKeyPair = kpGen.GenerateKeyPair();

            RSAParameters privKeyParam = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)_asymmetricCipherKeyPair.Private);
            RSAParameters pubKeyParam = DotNetUtilities.ToRSAParameters((RsaKeyParameters)_asymmetricCipherKeyPair.Public);
            pirvKey = RsaParamToJson(privKeyParam, true);
            pubKey = RsaParamToJson(pubKeyParam, false);
        }

        private void txtPublicEncrypt_Click(object sender, EventArgs e)
        {
            if (_asymmetricCipherKeyPair == null)
            {
                return ;
            }

            var privKeyParam = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)_asymmetricCipherKeyPair.Public);
            //var rsa = RSA.Create();
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(privKeyParam);
            var buffer = rsa.EncryptValue(Encoding.UTF8.GetBytes(txtPlaintext.Text));

            txtCiphertext.Text = Convert.ToBase64String(buffer);
        }

        private void btnPrivateDecrypt_Click(object sender, EventArgs e)
        {
            if (_asymmetricCipherKeyPair == null)
            {
                return;
            }
            var pubKeyParam = DotNetUtilities.ToRSAParameters((RsaKeyParameters)_asymmetricCipherKeyPair.Public);
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(pubKeyParam);
            var buffer = rsa.DecryptValue(Convert.FromBase64String(txtCiphertext.Text));

            txtPlaintext.Text = Encoding.UTF8.GetString(buffer);
        }
    }
}
