using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DLLmapper
{
    class Program
    {
        static void Main(string[] args)
        {
            String currentDirectory = Directory.GetCurrentDirectory();

            List<String> files = new List<String>(Directory.GetFiles(currentDirectory));
            files.RemoveAll((f) => !f.EndsWith(".dll"));

            if (files.Count == 0)
            {
                Console.WriteLine("No DLLs found.");
                return;
            }

            StreamWriter sw = new StreamWriter("dlls.json");
            sw.WriteLine("{");

            foreach (String file in files)
            {
                var dll = Assembly.LoadFile(file);
                String dllName = dll.GetName().Name.ToUpper();

                Console.WriteLine(dllName + ":");
                sw.WriteLine("\t\"" + dllName + "\": [");

                var referencedDlls = dll.GetReferencedAssemblies();

                foreach (var referencedDllName in referencedDlls)
                {
                    var referencedDll = Assembly.Load(referencedDllName);

                    if (referencedDll.Location.Contains(currentDirectory))
                    {
                        String referencedDllNameStr = referencedDllName.Name.ToUpper();

                        Console.WriteLine(" - " + referencedDllNameStr);
                        sw.WriteLine("\t\t\"" + referencedDllNameStr + "\"" + 
                            (referencedDllName != referencedDlls[referencedDlls.Length - 1] ? "," : String.Empty));
                    }
                }
                sw.WriteLine("\t]" + (file != files[files.Count - 1] ? "," : String.Empty));
            }
            sw.WriteLine("}");

            sw.Close();

            Console.ReadKey();
        }
    }
}
