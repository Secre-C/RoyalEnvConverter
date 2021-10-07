using System;
using System.Text;
using System.IO;
using System.Reflection;

namespace Env_Convert_Maybe
{
    public static class Program
    {
        public static string versionGot;
        public static int lengthGot;
        static void Main()
        {
            System.IO.File.Delete("resultfile");
            start:
            System.IO.Directory.CreateDirectory("output");
            Console.WriteLine("Enter Royal ENV Name (without the .ENV)\n");
            string envFileInput = Console.ReadLine() + ".ENV";
            System.IO.File.Delete("resultfile");
            System.IO.File.Copy("basefile", "resultfile");
            File.Delete("output\\" + envFileInput);
            //checks to see if Input env is from Royal
            EnvLength(envFileInput);
            EnvCheck(envFileInput);
            EnvRead(envFileInput);
            System.IO.File.Delete("output\\" + envFileInput);
            System.IO.File.Copy("resultfile", "output\\" + envFileInput);
            System.IO.File.Delete("resultfile");
            Console.WriteLine("\nSuccessfully Converted (I think).\n");
            goto start;
        }
        public static void EnvRead(string fileInput)
        {
            //int envBuffer;
            //int envVersion = Convert.ToInt32(versionGot);
            /*if (envVersion > 01105040)
            {
                envBuffer = 16;
            }
            else
            {
                envBuffer = 0;
            } */
            int envStep = 0;    //split up the byte replacing into chunks
            int[] envCoords = new int[3];
        start:
            if (envStep == 0)
            {
                envCoords[0] = 0x11;
                envCoords[1] = 0x1B7;
                envCoords[2] = 0x11;
            }
            else if (envStep == 1)
            {
                envCoords[0] = 0x23C;
                envCoords[1] = 21;
                envCoords[2] = lengthGot - 100;
            }
            else if (envStep == 2)
            {
                envCoords[0] = 0x294;
                envCoords[1] = 4;
                envCoords[2] = lengthGot - 8;
            }

            envStep++;
            byte[] envBase = new byte[envCoords[1]];
            using (BinaryReader reader = new BinaryReader(new FileStream(fileInput, FileMode.Open)))
            {
                reader.BaseStream.Seek(envCoords[2], SeekOrigin.Begin);
                reader.Read(envBase, 0, envCoords[1]);
            }
            File.WriteAllBytes("output\\" + fileInput, envBase);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite("resultfile"));
            int x = envCoords[0];
            bw.BaseStream.Position = x;
            bw.Write(envBase);
            bw.Close();
            if (envStep <= 2)
            {
                goto start;
            }
        }
        static void EnvCheck(string fileInput)
        {
            BinaryReader checker = new BinaryReader(new FileStream(fileInput, FileMode.Open));
            for (int i = 0x04; i <= 0x07; i++)
            {
                checker.BaseStream.Position = i;    //check Byte six for env Version: "51" for Royal
                versionGot += checker.ReadByte().ToString("X2");
            }
            checker.Close();
        }
        static void EnvLength(string fileInput)
        {
        BinaryReader binReader = new BinaryReader(File.Open(fileInput, FileMode.Open));

        // get file length and alloc butter
        long lfileLength = binReader.BaseStream.Length;
        Byte[] btFile = new Byte[lfileLength];

        // read every byte of file's content
        for (long lIdx = 0; lIdx < lfileLength; lIdx++)
        {
            btFile[lIdx] = binReader.ReadByte();
        }
        lengthGot = Convert.ToInt16(lfileLength);
        Console.WriteLine("\nBytes: " + lfileLength);
        binReader.Close();
        }
    }
}
