using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace digital_signature
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
		DSAParameters dsaparams;
		byte[] signaturebytes;

		private void file_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;

            richTextBox1.Text =  System.IO.File.ReadAllText(filename);
        }

        private void create_Click(object sender, EventArgs e)
        {
			//get original message as byte array
			byte[] messagebytes = Encoding.UTF8.GetBytes(
				richTextBox1.Text);

			//create digest of original message using SHA1
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			byte[] hashbytes =
				sha1.ComputeHash(messagebytes);

			//display hash bytes in hex format
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hashbytes.Length; i++)
			{
				sb.Append(String.Format(
					"{0,2:X2} ", hashbytes[i]));
			}
			

			//create DSA object using default key
			DSACryptoServiceProvider dsa =
				new DSACryptoServiceProvider();

			//sign hash using OID for SHA-1
			signaturebytes =
				dsa.SignHash(hashbytes, "1.3.14.3.2.26");

			//provide DSA parameters to verification
			dsaparams = dsa.ExportParameters(false);

			
		}

        private void load_Click(object sender, EventArgs e)
        {
			//get possibly modified message as byte array
			byte[] messagebytes = Encoding.UTF8.GetBytes(
				richTextBox1.Text);

			//create digest of original message using SHA1
			SHA1 sha1 = new SHA1CryptoServiceProvider();
			byte[] hashbytes =
				sha1.ComputeHash(messagebytes);

			//create DSA object using parameters from signing
			DSACryptoServiceProvider dsa =
				new DSACryptoServiceProvider();
			dsa.ImportParameters(dsaparams);

			//do verification on hash using OID for SHA-1
			bool match = dsa.VerifyHash(
				hashbytes, "1.3.14.3.2.26", signaturebytes);

			//show message box with result of verification
			String strResult;
			if (match)
				strResult = "VerifySignature returned TRUE";
			else
				strResult = "VerifySignature returned FALSE";
			MessageBox.Show(
				strResult,
				"Result From Calling VerifySignature",
				MessageBoxButtons.OK,
				MessageBoxIcon.Exclamation);

		}
    }
}
