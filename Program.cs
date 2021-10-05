using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Cacher
{
    class Program
    {

        static Char[] delimiters = { '\'','"', '`',' ', ',', '.', ':', ';', '(', '(', '+', '-', '=', '[', ']', '/', '\\', '{', '}', '<', '>', '_', '\r', '\n', '\t', '~','!','?' };
        static void Main(string[] args)
        {
            if(args.Length < 3)
            {
                Console.WriteLine("Usage: matcher input_file_name check_file output_file_name");
                return;
            }

            String inFile = args[0];
            String checkFile = args[1];
            String outFile = args[2];
            

          
            String  inData;
           
            inData = File.ReadAllText(inFile);

            String sBase = PackString(inData);


            String checkData = File.ReadAllText(checkFile);

            String sQry = PackString(checkData);

            String[] src = inData.Split(delimiters);


            DateTime st = DateTime.Now;
            int patternSz = 25;
            int i;
            bool ok = false;
            int foundAt = 0;
           
            for (i=0; i < sQry.Length-patternSz; i+=2)
            {

                int foundPtr = 0;
                String sub = sQry.Substring(i, patternSz);
                foundAt = 0;
                while (foundAt >= 0)
                {
                    if(foundAt==0)
                       foundAt = sBase.IndexOf(sub);
                    else
                        foundAt = sBase.IndexOf(sub, foundAt+1);

                    if (foundAt >= 0)
                    {
                        

                        if (! ok )
                        {
                            foundPtr = i;
                            Console.WriteLine("Match found at " + foundAt.ToString());
                            Console.WriteLine("Seek pattern at " + foundPtr.ToString());
                            
                            Console.WriteLine("Checking matches ...");
                          
                            Console.WriteLine("at Base Text");
                            
                            for (int j = 0; j < patternSz; j++)
                            {
                                if (foundAt / 2 + j < src.Length)
                                    Console.Write(src[foundAt/2 + j] + " ");
                            }

                            Console.WriteLine("");

                            String[] dst = checkData.Split(delimiters);

                            Console.WriteLine("");
                            Console.WriteLine("at Seek Text");
                            
                            for (int j = 0; j < patternSz; j++)
                            {
                                if(foundPtr / 2 + j < dst.Length)
                                    Console.Write(dst[foundPtr/2 + j] + " ");
                            }
                            Console.WriteLine("");
                        }
                        ok = true;
                    }
                }
            }
            if(!ok) Console.WriteLine("No matches found!");
            Console.WriteLine("string matching time: " + (DateTime.Now - st).TotalMilliseconds.ToString() + " ms");

         
            File.WriteAllText(outFile, sBase);

            File.WriteAllText(outFile +".Q", sQry);


        }


        static String PackString(String inData)
        {
            //return inData;
            Console.WriteLine("packing for " + inData.Length.ToString() + " chars.");
            DateTime st = DateTime.Now;
            StringBuilder words = new StringBuilder();
            String newWord = "";
            String[] src = inData.Split(delimiters);
            Console.WriteLine("expected ratio: " + (src.Length * 100 / inData.Length).ToString() +"%" );
            for (int i = 0; i < src.Length; i++)
            {
                if (i % 10000 == 9999)
                    Console.Write(".");
                newWord += Word2Char(src[i]);
            }

            if (newWord.Length > 0)
            {
               words.Append(newWord);
            }


            String sOut = words.ToString();
            Console.WriteLine("");
            Console.WriteLine("packing time: " + (DateTime.Now -st).TotalMilliseconds.ToString() +" ms. Size:" + sOut.Length.ToString());
            return sOut;

        }

        static string Word2Char( String s)
        {
            String map = " qwertyuiopasdfghjklzxcvbnm1234567890ёйцукенгшщзхъфывапролджэячсмитьбю@#$%^&*№";
            int divider = map.Length;
            Char[] chars = s.ToLower().ToCharArray();
            int sum = 0; // 31068;
            int pos;
            int idx=0;
            foreach(char c in chars)
            {
                pos = map.IndexOf(c);
                if(pos >= 0)
                {
                    idx++;
                    sum += pos * idx;
                }
            }
            return map.Substring((sum / divider) % divider, 1) +  map.Substring(sum % divider, 1);
        }
    }
}
