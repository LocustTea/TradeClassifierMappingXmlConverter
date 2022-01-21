using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TradeClassifierMappingXmlConverter
{
    public class Sector
    {
        public Sector()
        {
            Children = new List<SectorChild>();
        }
        public string Id { get; set; }

        public List<SectorChild> Children { get; set; }


        public static List<Sector> GetRootSectors(HashSet<string> rootClassifiers, Dictionary<string, Sector> sectors)
        {
            var rootSectors = new List<Sector>();

            foreach (var rootClassifier in rootClassifiers)
            {
                rootSectors.Add(sectors[rootClassifier]);
            }

            return rootSectors;
        }

        public static Dictionary<string, Sector> LoadSectors(string directoryPath)
        {
            var sectors = new Dictionary<string, Sector>();

            var filePathes = Directory.GetFiles(directoryPath);

            foreach (var filePath in filePathes)
            {
                var sector = LoadSector(filePath);
                sectors[sector.Id] = sector;
            }

            var sector99 = new Sector()
            {
                Id = "99",
                Children = new List<SectorChild>()
            };

            sectors.Add("99", sector99);

            return sectors;
        }
        private static Sector LoadSector(string path)
        {
            var sector = new Sector();

            using (var xmlReader = XmlReader.Create(path))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ClassifierId")
                    {
                        ReaderXmlReader(xmlReader);
                        sector.Id = xmlReader.Value;
                    }

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Children")
                    {
                        var sectorChild = LoadSectorChild(xmlReader);
                        sector.Children.Add(sectorChild);
                    }

                }
            }

            return sector;
        }
        private static SectorChild LoadSectorChild(XmlReader xmlReader)
        {
            var sectorChild = new SectorChild();

            while (true)
            {
                if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "Children")
                {
                    break;
                }
                ReaderXmlReader(xmlReader);
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ClassifierId")
                {
                    ReaderXmlReader(xmlReader);
                    sectorChild.Child = xmlReader.Value;
                }
            }

            return sectorChild;
        }
        public static void ReaderXmlReader(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Whitespace)
                {
                    break;
                }
            }
        }
    }

    public class SectorChild
    {
        public SectorChild()
        {
            Aggregate = false;
        }
        public string Child { get; set; }
        public bool Aggregate { get; set; }
    }
}
