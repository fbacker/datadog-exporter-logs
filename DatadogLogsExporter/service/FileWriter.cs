using DatadogLogsExporter.helpers;
using System.Text.Json;

namespace DatadogLogsExporter.service
{
    public class FileWriter
    {
        private int _rotationIndex = 0;
        private double _currentBytes = 0;
        private double _maxBytesSize;
        private string _basePath;
        private StreamWriter _writer;

        public FileWriter(Config config)
        {
            // Docker container local path
            _basePath = "/files";

            //@TODO, set max log size from options, but in mb, max limit?
            double maxBytesSize = 5e+8;
            _maxBytesSize = maxBytesSize;
        }

        public void Write(dynamic data)
        {


            var sizeBytes = GetObjectSize(data);
            var newSizeMeasure = _currentBytes + sizeBytes;
            if (_writer is null || newSizeMeasure > _maxBytesSize) rotateFile();

            _currentBytes += sizeBytes;
            _writer.WriteLine(data);

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

            _rotationIndex++;
            _currentBytes = 0;

            var fileName = $"output_{_rotationIndex}.log";
            _writer = new StreamWriter(Path.Combine(_basePath, fileName));

            Console.WriteLine($"{DateTime.Now} Setup new file for output: {fileName}");
        }



        public void Dispose()
        {
            if (_writer is not null)
            {
                _writer.Flush();
                _writer.Close();
                _writer.Dispose();
            }
        }
    }
}
