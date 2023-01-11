using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Quote2022.Helpers
{
    public class ZipReader: IEnumerable<ZipReaderItem>, IDisposable
    {
        private readonly ZipArchive _zip;
        public ZipReader(string filename) => _zip = ZipFile.Open(filename, ZipArchiveMode.Read);
        public void Dispose() => _zip.Dispose();

        public IEnumerator<ZipReaderItem> GetEnumerator()
        {
            foreach (var entry in _zip.Entries)
            {
                using (var entryStream = entry.Open())
                using (var reader = new StreamReader(entryStream, System.Text.Encoding.UTF8, true))
                {
                    var context = reader.ReadToEnd();
                    var item = new ZipReaderItem()
                    {
                        Content = context,
                        Created = entry.LastWriteTime.DateTime,
                        Length = entry.Length,
                        FullName = entry.FullName
                    };
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ZipReaderItem
    {
        public string Content;
        public string FullName;
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FullName);
        public long Length;
        public DateTime Created;
    }

    public class ZipReaderStream : IEnumerable<ZipReaderStreamItem>, IDisposable
    {
        private readonly ZipArchive _zip;
        public ZipReaderStream(string filename) => _zip = ZipFile.Open(filename, ZipArchiveMode.Read);
        public void Dispose() => _zip.Dispose();

        public IEnumerator<ZipReaderStreamItem> GetEnumerator()
        {
            foreach (var entry in _zip.Entries)
            {
                using (var entryStream = entry.Open())
                using (var reader = new StreamReader(entryStream, System.Text.Encoding.UTF8, true))
                {
                    var item = new ZipReaderStreamItem()
                    {
                        Reader = reader,
                        Created = entry.LastWriteTime.DateTime,
                        Length = entry.Length,
                        FullName = entry.FullName
                    };
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class ZipReaderStreamItem
    {
        public StreamReader Reader;
        public string FullName;
        public string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FullName);
        public long Length;
        public DateTime Created;
    }
}
