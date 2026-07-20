using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Laboratorio_2_CSV_JSON
{
    class Program
    {
        static void Main(string[] args)
        {
            string ruta = "estudiantes.csv";
            string[] lineas = File.ReadAllLines(ruta);

            List<Esudiante> estudiantes = new List<Esudiante>();

            for (int i = 1; i < lineas.Length; i++)
            {
                string[] datos = lineas[i].Split(',');

                Esudiante estudiante = new Esudiante
                {
                    Id = int.Parse(datos[0]),
                    Nombre = datos[1],
                    Carrera = datos[2]
                };

                estudiantes.Add(estudiante);
            }

            foreach (var estudiante in estudiantes)
            {
                Console.WriteLine($"{estudiante.Id} - {estudiante.Nombre} - {estudiante.Carrera}");
            }

            var opciones = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(estudiantes, opciones);

            File.WriteAllText("estudiantes.json", json);
        }
    }
}