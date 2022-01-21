using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeClassifierMappingXmlConverter
{
    public static class FileSaver
    {

        public static void SaveContainers(string path, string folderName, List<XmlContainer> xmlContainers)
        {
            foreach (var xmlContainer in xmlContainers)
            {
                var directoryPath = Path.Combine(path, folderName);
                var filePath = Path.Combine(directoryPath, xmlContainer.FileName);

                System.IO.Directory.CreateDirectory(directoryPath);

                File.WriteAllText(filePath, xmlContainer.Content, new UTF8Encoding(false));
            }
        }
    }
}
