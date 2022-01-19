using System;
using System.Text;
using System.IO;
using System.Reflection;
using Amicitia.IO;

namespace Env_Convert_Maybe
{
    public static class Program
    {
        public static int filePlace = 0;
        public static int fileAmount;
        public static string versionGot;
        public static string moisty;
        public static int lengthGot;
        public static int versionNum;
        public static bool yn = true;
        public static bool decideVanilla = false;
        public static bool decision;
        static void Main()
        {
            System.IO.File.Delete("resultfile");
            Console.WriteLine("Welcome To Fucking");
            int envPlace = 0;
        start:
            versionGot = null;
            System.IO.Directory.CreateDirectory("output");
            string[] envFileInput = Directory.GetFiles("input\\", "*.env", SearchOption.TopDirectoryOnly);
            fileAmount = envFileInput.Length;
            System.IO.File.Delete("resultfile");
            System.IO.File.Copy("basefile", "resultfile");
            File.Delete("output\\" + envFileInput[envPlace]);
            EnvLength(envFileInput[envPlace]); //gets env length
            EnvCheck(envFileInput[envPlace]); //gets env version
            if (yn == true)
            {
                EnvRead(envFileInput[envPlace]);
                System.IO.File.Delete("output\\" + envFileInput[envPlace]);
                deMoist();
                System.IO.File.Copy("resultfile", "output\\" + envFileInput[envPlace]);
                System.IO.File.Delete("resultfile");
                Console.WriteLine("\nSuccessfully Converted\n");
                envPlace++;
                if (envPlace !=fileAmount)
                {
                    goto start;
                }
                else
                {
                    Console.WriteLine("Conversion success! Press any button to close the program");
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                envPlace++;
                if (envPlace != fileAmount)
                {
                    goto start;
                }
                else
                {
                    Console.WriteLine("\nConversion success! Press any button to close the program");
                    Console.ReadKey();
                    return;
                }
            }
        }
        public static void EnvRead(string fileInput)
        {            
            int envStep = 0;    //split up the byte replacing into chunks
            int lightStep = 0; //split up the light value replacing into chunks
            int[] envCoords = new int[4];
        start:
            if (envStep == 0)
            {
                envCoords[0] = 0x10;
                envCoords[1] = 0x1B9;
                envCoords[2] = 0x10; //Field, character, and fog sections (same as vanilla)
            }
            else if (envStep == 1)
            {
                envCoords[0] = 0x24D;
                envCoords[1] = 104;
                envCoords[2] = lengthGot - 104; //color grading (21 bytes), physics section (34 bytes), second unknown section (41 bytes?), and sky coloring (4 bytes).
            }
            else if (envStep == 2)
            {
                envCoords[0] = 0x1F2;
                envCoords[1] = 91;
                envCoords[2] = lengthGot - 211; //unknown section (59 bytes), Field Shadow Section (32 bytes)
            }
            else if (envStep == 3) //field lighting section
            {
                if (lightStep == 0)
                {
                    envCoords[0] = 0x1C9;
                    envCoords[1] = 4;
                    envCoords[2] = 0x1C9; // 4 bools at the beginning
                }
                else if (lightStep == 1 && (versionNum == 01105090))
                {
                    envCoords[0] = 0x1CD;
                    envCoords[1] = 20;
                    envCoords[2] = 0x1D3; // bloom amount - glare sensitivity
                }
                else if (lightStep == 1)
                {
                    envCoords[0] = 0x1CD;
                    envCoords[1] = 20;
                    envCoords[2] = 0x1D4; // bloom amount - glare sensitivity
                }
                else if (lightStep == 2 && (versionNum == 01105090))
                {
                    envCoords[0] = 0x1E1;
                    envCoords[1] = 16;
                    envCoords[2] = 0x237; // glare length - glare mode
                }
                else if (lightStep == 2)
                {
                    envCoords[0] = 0x1E1;
                    envCoords[1] = 16;
                    envCoords[2] = 0x240; // glare length - glare mode
                }
            }
            if (envStep < 3)
            {
                envStep++;
            }   
            else
            {
                lightStep++;
            }
            if (lightStep == 3)
            {
                envStep = 4;
            }
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
            if (envStep <= 3) //checks which step of my patented ENV copy paste process the program is on
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
            int versionNum = Convert.ToInt32(versionGot);
            Console.WriteLine("\nEnv Version: " + versionGot);
            if (versionNum < 01105100)
            {
                if (decideVanilla == false)
                {
                    Console.WriteLine("\nOne of the ENVs in the input seems to already work in vanilla p5, and may not work properly after conversion. Would you like to convert them anyway? This question will only be asked once. y/n \n");
                    string yesno = Console.ReadLine();
                    if (yesno == "y")
                    {
                        decision = true;
                    }
                    else
                    {
                        decision = false;
                    }
                    decideVanilla = true;
                }
                yn = decision;
            }   
            else
            {
                yn = true;
                return;
            }
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
        static void deMoist() //divide f32 Field1F2 by 4
        {
            moisty = null;
            BinaryReader checker = new BinaryReader(new FileStream("resultfile", FileMode.Open));
            for (int i = 0x1F2; i <= 0x1F5; i++)
            {
                checker.BaseStream.Position = i;
                moisty += checker.ReadByte().ToString("X2");
            }
            checker.Close();
            uint num = uint.Parse(moisty, System.Globalization.NumberStyles.AllowHexSpecifier);
            byte[] floatVals = BitConverter.GetBytes(num);
            float f = BitConverter.ToSingle(floatVals, 0);
            f = f / 4;
            var bytes = BitConverter.GetBytes(f);
            var s = BitConverter.ToInt32(bytes, 0);
            string newfloat = s.ToString("X2");
            byte[] byteArray = StringToByteArrayFastest(moisty);
            BinaryWriter div4 = new BinaryWriter(File.OpenWrite("resultfile"));
            int x = 0x1F2;
            div4.BaseStream.Position = x;
            div4.Write(byteArray);
            div4.Close();

        }
        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        public static int GetHexVal(char hex)
        {
            int val = (int)hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}
