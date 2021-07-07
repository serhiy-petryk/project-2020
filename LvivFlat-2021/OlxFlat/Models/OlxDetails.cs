using System.IO;
using System.Text;

namespace OlxFlat.Models
{
    public class OlxDetails
    {
        public OlxDetails(string filename)
        {
            var s = File.ReadAllText(filename, Encoding.UTF8);
            var fileDate = File.GetLastWriteTime(filename).Date;

        }
    }
}
