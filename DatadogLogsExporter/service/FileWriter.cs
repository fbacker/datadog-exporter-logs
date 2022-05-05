using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace DatadogLogsExporter.service
{
    public class FileWriter
    {
        private int rotationIndex = 0;
        private double currentBytes = 0;
        private double maxBytesSize;
        private string basePath;
        private StreamWriter writer;

        public FileWriter(string basePath, double maxBytesSize)
        {
            this.basePath = basePath;
            this.maxBytesSize = maxBytesSize;
        }

        public void Write(dynamic data) {


            var sizeBytes = GetObjectSize(data);
            var newSizeMeasure = currentBytes + sizeBytes;
            if (writer is null || newSizeMeasure > maxBytesSize) rotateFile();

            currentBytes += sizeBytes;
            writer.WriteLine(data);

        }

        private int GetObjectSize(object TestObject)
        {
            var d = JsonSerializer.SerializeToUtf8Bytes(TestObject);
            var size = d.Length;
            return size;
        }

        private void rotateFile()
        {
            Dispose();

            rotationIndex++;
            currentBytes = 0;

            var fileName = $"output_{rotationIndex}.log";
            writer = new StreamWriter(Path.Combine(this.basePath, fileName));

            Console.WriteLine($"Setup new file for output: {fileName}");
        }



        public void Dispose()
        {
            if (writer is not null)
            {
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }
    }
}
