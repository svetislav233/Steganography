using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace WindowsFormsApplication11
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            button1.Enabled = false;
            button2.Enabled = false;
            radioButton3.Enabled = false;
            radioButton4.Enabled = false;
            radioButton5.Enabled = false;
        }

        string hashovanaSifra = "";

        private const string initVector = "tu89geji340t89u2";
        private const int keysize = 256;
        public static string Encrypt(string plainText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] cipherTextBytes = Convert.FromBase64String(cipherText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }


        
        Bitmap ustekan;
        string enkriptovan = "";
        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                try
                {
                    enkriptovan = Encrypt(textBox1.Text, hashovanaSifra);// hashovanaSifra);                
                    ustekan = Class1.embedText(enkriptovan, bitmap);
                    MessageBox.Show("Text encrypted (with AES) and hidden in image. Please save image now...");

                }
                catch
                {
                    MessageBox.Show("Error");
                }
            }
            else if(radioButton2.Checked == true)
            {
                try
                {
                    enkriptovan = TripleDes.Encrypt(textBox1.Text, hashovanaSifra);// hashovanaSifra);                   
                    ustekan = Class1.embedText(enkriptovan, bitmap);
                    MessageBox.Show("Text encrypted (with Triple DES) and hidden in image. Please save image now...");
                }
                catch
                {
                    MessageBox.Show("Error");
                }
            }
            
        }


        string putanja = "";
        Bitmap bitmap;
        string izvucen = "";
        private void button2_Click(object sender, EventArgs e)
        {
            if(radioButton1.Checked == true)
            {
                try
                {
                    izvucen = Class1.extractText(bitmap);
                    //textBox1.Text = Decrypt(izvucen, hashovanaSifra);
                    MessageBox.Show(Decrypt(izvucen, hashovanaSifra)); //hashovanaSifra));
                }
                catch
                {
                    MessageBox.Show("Error");
                }
            }
            else if(radioButton2.Checked == true)
            {
                try
                {
                    izvucen = Class1.extractText(bitmap);
                    MessageBox.Show(TripleDes.Decrypt(izvucen, hashovanaSifra));//hashovanaSifra));

                }
                catch
                {
                    MessageBox.Show("Error");
                }
            }
        }

        private void otvoriSlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog fajl = new OpenFileDialog();
                if (fajl.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    putanja = fajl.FileName;
                }
                bitmap = (Bitmap)Image.FromFile(putanja);
                pictureBox1.Image = bitmap;

                button1.Enabled = true;
                button2.Enabled = true;

            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void snimiSlikuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog snimi = new SaveFileDialog();
                snimi.Filter = "Bitmaps|*.bmp";
                if (snimi.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ustekan.Save(snimi.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                    MessageBox.Show("Image saved!");
                }
            }
            catch
            {
                MessageBox.Show("Error");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if(textBox2.Text != "")
            {
                radioButton3.Enabled = true;
                radioButton4.Enabled = true;
                radioButton5.Enabled = true;
            }     
        }




        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            HashAlgorithm sha256 = new SHA256CryptoServiceProvider();
            byte[] a = Encoding.ASCII.GetBytes(textBox2.Text);
            byte[] result = sha256.ComputeHash(a);
            hashovanaSifra = BitConverter.ToString(result);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            HashAlgorithm sha512 = new SHA512CryptoServiceProvider();
            byte[] a = Encoding.ASCII.GetBytes(textBox2.Text);
            byte[] result = sha512.ComputeHash(a);
            hashovanaSifra = BitConverter.ToString(result);
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            HashAlgorithm MD5 = new MD5CryptoServiceProvider();
            byte[] a = Encoding.ASCII.GetBytes(textBox2.Text);
            byte[] result = MD5.ComputeHash(a);
            hashovanaSifra = BitConverter.ToString(result);
        }

        private void authorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Svetislav Kirilov \nDate of Birth: 18.07.1992 \ne-mail: kirilov233@gmail.com");
        }

        private void programToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("ENCRYPTION:\nPress File --> Open image, then enter text,\nenter password, select hash function, select AES or Triple DES\npress Encrypt, and then go to File --> Save image\nDECRYPTION:\nFile --> Open Image, enter correct password, select correct hash function\nand AES or Triple DES (depending on the need)\nand press Decrypt");
        }
    }
}
