using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RainbowInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            string bitmapPath;
            if (args.Length > 0) //get path to bitmap from arguments
            {
                bitmapPath = args[0];
            }
            else //get path to bitmap from command line input
            {
                do
                {
                    Console.Write("Path to bitmap file: ");
                    bitmapPath = Console.ReadLine();
                } while (String.IsNullOrWhiteSpace(bitmapPath));
            }

            Bitmap bitmap;

            try
            {
                bitmap = new Bitmap(bitmapPath);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("No file found at " + bitmapPath);
                return;
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("No file found at " + bitmapPath);
                return;
            }

            string[] hexArray = bitmapToHex(bitmap);
            
            //debug program print
            //foreach (string s in hexArray)
            //{
            //    Console.WriteLine(s); 
            //}


            //load interpeter and execute program
            Interpreter rainbowInterpreter = new Interpreter(hexArray);
            ExitStatus status;
            try
            {
                status = rainbowInterpreter.Execute();
            }
            catch (ExitException e)
            {
                if (e.ExitStatus != ExitStatus.ProgramException)
                    Console.WriteLine("\n{0}: {1}", e.ExitStatus, e.Message);

                status = e.ExitStatus;
            }
            catch (Exception e)
            {
                Console.WriteLine("\n{0}: {1}", ExitStatus.InternalException, e.Message);
                status = ExitStatus.InternalException;
            }
            
            Console.WriteLine("\nProgram exited with status: {0}", status.ToString());
        }

        public static string[] bitmapToHex(Bitmap bitmap)
        {
            string[] hexValues = new string[bitmap.Width * bitmap.Height];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    Color pixel = bitmap.GetPixel(j,i);
                    hexValues[i * bitmap.Width + j] = pixel.R.ToString("X2") + pixel.G.ToString("X2") + pixel.B.ToString("X2");
                }
            }

            return hexValues;
        }
    }
}
