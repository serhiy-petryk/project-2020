using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                using (var memoryStream = new MemoryStream())
                {
                    entryStream.CopyTo(memoryStream);
                    var context = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
                    var item = new ZipReaderItem()
                    {
                        Content = context, Created = entry.LastWriteTime.DateTime, Length = entry.Length,
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
}
