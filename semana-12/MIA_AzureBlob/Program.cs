using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

string connectionString =
   "key_azure(no podemos subir la clave a github)";
string containerName = "archivos";

BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
await containerClient.CreateIfNotExistsAsync();

bool salir = false;
while (!salir)
{
    MostrarMenu();
    string opcion = Console.ReadLine();

    switch (opcion)
    {
        case "1":
            await SubirArchivoAsync(containerClient);
            break;
        case "2":
            await ListarArchivosAsync(containerClient);
            break;
        case "3":
            await DescargarArchivoAsync(containerClient);
            break;
        case "4":
            await EliminarArchivoAsync(containerClient);
            break;
        case "5":
            salir = true;
            Console.WriteLine("Saliendo de la aplicación...");
            break;
        default:
            Console.WriteLine("Opción inválida. Intente nuevamente.\n");
            break;
    }
}

void MostrarMenu()
{
    Console.WriteLine("=================================");
    Console.WriteLine("     MIA - AZURE BLOB STORAGE");
    Console.WriteLine("=================================");
    Console.WriteLine("1. Subir archivo");
    Console.WriteLine("2. Listar archivos");
    Console.WriteLine("3. Descargar archivo");
    Console.WriteLine("4. Eliminar archivo");
    Console.WriteLine("5. Salir");
    Console.WriteLine("=================================");
    Console.Write("Seleccione una opción: ");
}

// 3.2 Subir archivo
async Task SubirArchivoAsync(BlobContainerClient container)
{
    Console.Write("\nIngrese la ruta completa del archivo local: ");
    string rutaArchivo = Console.ReadLine();

    if (!File.Exists(rutaArchivo))
    {
        Console.WriteLine("El archivo no existe. Verifique la ruta.\n");
        return;
    }

    string nombreArchivo = Path.GetFileName(rutaArchivo);
    BlobClient blobClient = container.GetBlobClient(nombreArchivo);

    try
    {
        using FileStream fs = File.OpenRead(rutaArchivo);
        await blobClient.UploadAsync(fs, overwrite: true);
        Console.WriteLine($"Archivo '{nombreArchivo}' subido exitosamente.\n");
    }
    catch (RequestFailedException ex)
    {
        Console.WriteLine($"Error al subir el archivo: {ex.Message}\n");
    }
}

// 3.3 Listar archivos
async Task ListarArchivosAsync(BlobContainerClient container)
{
    Console.WriteLine("\nNombre              Tamaño");
    Console.WriteLine("--------------------------------");

    try
    {
        bool hayArchivos = false;
        await foreach (BlobItem blobItem in container.GetBlobsAsync())
        {
            hayArchivos = true;
            Console.WriteLine($"{blobItem.Name,-20}{blobItem.Properties.ContentLength} bytes");
        }

        if (!hayArchivos)
            Console.WriteLine("(El container está vacío)");

        Console.WriteLine();
    }
    catch (RequestFailedException ex)
    {
        Console.WriteLine($"Error al listar archivos: {ex.Message}\n");
    }
}

// 3.4 Descargar archivo
async Task DescargarArchivoAsync(BlobContainerClient container)
{
    Console.Write("\nIngrese el nombre del blob a descargar: ");
    string nombreBlob = Console.ReadLine();

    BlobClient blobClient = container.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine("El archivo no existe en el container.\n");
        return;
    }

    Console.Write("Ingrese la carpeta de destino: ");
    string carpetaDestino = Console.ReadLine();

    if (!Directory.Exists(carpetaDestino))
    {
        Console.WriteLine("La carpeta de destino no existe.\n");
        return;
    }

    string rutaDestino = Path.Combine(carpetaDestino, nombreBlob);

    try
    {
        await blobClient.DownloadToAsync(rutaDestino);
        Console.WriteLine($"Archivo descargado en: {rutaDestino}\n");
    }
    catch (RequestFailedException ex)
    {
        Console.WriteLine($"Error al descargar el archivo: {ex.Message}\n");
    }
}

// 3.5 Eliminar archivo
async Task EliminarArchivoAsync(BlobContainerClient container)
{
    Console.Write("\nIngrese el nombre del blob a eliminar: ");
    string nombreBlob = Console.ReadLine();

    BlobClient blobClient = container.GetBlobClient(nombreBlob);

    if (!await blobClient.ExistsAsync())
    {
        Console.WriteLine("El archivo no existe en el container.\n");
        return;
    }

    Console.Write($"¿Confirma que desea eliminar '{nombreBlob}'? (S/N): ");
    string confirmacion = Console.ReadLine();

    if (confirmacion?.Trim().ToUpper() != "S")
    {
        Console.WriteLine("Operación cancelada.\n");
        return;
    }

    try
    {
        bool eliminado = await blobClient.DeleteIfExistsAsync();
        Console.WriteLine(eliminado
            ? $"Archivo '{nombreBlob}' eliminado exitosamente.\n"
            : "No se pudo eliminar el archivo.\n");
    }
    catch (RequestFailedException ex)
    {
        Console.WriteLine($"Error al eliminar el archivo: {ex.Message}\n");
    }
}

