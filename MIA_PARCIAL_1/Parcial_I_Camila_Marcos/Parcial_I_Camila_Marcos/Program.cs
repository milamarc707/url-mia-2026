using System;
using System.IO;
using System.Linq;

namespace Parcial_I_Camila_Marcos
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingresa nombre de usuario");
            string nombreApellido = Console.ReadLine();

            Console.WriteLine("Ingrese la ruta del archivo: ");
            string rutaArchivo = Console.ReadLine();
            
            string[] lineas = File.ReadAllLines(rutaArchivo);
            int totalLineas = lineas.Length;
            int totalCaracteres = lineas.Sum(l => l.Length);
            
            int totalVocales = lineas.SelectMany(l => l.ToLower()).Count(c => "aeiouáéíóúü".Contains(c));

            string directorio = Path.GetDirectoryName(rutaArchivo) ?? Directory.GetCurrentDirectory();
            string nombreSalida = $"resultados_{nombreApellido}.csv";
            string rutaSalida = Path.Combine(directorio, nombreSalida);

            string contenidoCsv = $"{nombreApellido},{totalLineas},{totalVocales},{totalCaracteres}";
            File.WriteAllText(rutaSalida, contenidoCsv);

            Console.WriteLine($"\n¡Proceso exitoso!");
        }
    }
}