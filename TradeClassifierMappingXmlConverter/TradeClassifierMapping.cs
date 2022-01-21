using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace TradeClassifierMappingXmlConverter
{
    public class TradeClassifierMapping 
    {
        public TradeClassifierMapping()
        {
            Classifiers = new List<string>();
        }

        #region  Properties

        public string InstrumentId { get; set; }

        public List<string> Classifiers { get; set; }

        public string Issuer { get; set; }

        public string IsRsa { get; set; }

        #endregion

        public static List<TradeClassifierMapping> ReworkTradeClassifierMappings(string tradeClassifierMappingsPath, string sectorsDirectoryPath, string classifierTreePath)
        {
            var tradeClassifierMappings = TradeClassifierMapping.LoadTradeClassifierMappings(tradeClassifierMappingsPath);

            var mappingsWithoutDuplication = RemoveDuplicatedClassifiers(tradeClassifierMappings, sectorsDirectoryPath, classifierTreePath);
            mappingsWithoutDuplication = ReplaceTradeClassifierMappingsWIthDuplication(tradeClassifierMappings, mappingsWithoutDuplication);


            var mappingsWithoudDuplicationAndAggregation = RemoveAggregatedClassifierMappings(mappingsWithoutDuplication);


            return mappingsWithoudDuplicationAndAggregation;
        }



        #region Non-Public Methods


        #region Remove Duplicates

        private static List<TradeClassifierMapping> RemoveDuplicatedClassifiers(List<TradeClassifierMapping> tradeClassifierMappings, string sectorsDirectoryPath, string classifierTreePath)
        {
            var result = new List<TradeClassifierMapping>();


            var sectors = Sector.LoadSectors(sectorsDirectoryPath);
            var rootClassifiers = ClassifierTree.GetRootClassifiers(classifierTreePath);

            var sectorInstruments = TradeClassifierMapping.GetSectorInstruments(tradeClassifierMappings);
            var rootSectors = Sector.GetRootSectors(rootClassifiers, sectors);


            foreach (var rootSector in rootSectors)
            {
                ApplyRemoveDuplicatedClassifiers(rootSector, sectors, sectorInstruments, tradeClassifierMappings, result);
            }

            return result;
        }

        private static HashSet<string> ApplyRemoveDuplicatedClassifiers(Sector sector,
            Dictionary<string, Sector> sectors,
            Dictionary<string, HashSet<string>> sectorInstruments,
            List<TradeClassifierMapping> tradeClassifierMappings,
            List<TradeClassifierMapping> result)
        {
            var instruments = new HashSet<string>();
            if (sectorInstruments.TryGetValue(sector.Id, out var ins))
            {
                AddRange(instruments, ins);
            }

            foreach (var child in sector.Children)
            {
                var childSector = sectors[child.Child];
                var childInstruments = ApplyRemoveDuplicatedClassifiers(childSector, sectors, sectorInstruments, tradeClassifierMappings, result);

                if (instruments.Any() && childInstruments.Any())
                {
                    if (!childInstruments.Any(i => !instruments.Contains(i)))
                    {

                        var parentClassifierId = sector.Id;
                        var childClassifierId = childSector.Id;

                        var classifiersToUpdate = tradeClassifierMappings.Where(m =>
                                m.Classifiers.Contains(parentClassifierId) && m.Classifiers.Contains(childClassifierId))
                            .ToList();

                        foreach (var c in classifiersToUpdate)
                        {
                            c.Classifiers.Remove(parentClassifierId);
                        }

                        result.AddRange(classifiersToUpdate);

                        //aggregation is work

                        AddRange(instruments, childInstruments);
                    }
                }
            }

            return instruments;
        }

        #endregion

        private static List<TradeClassifierMapping> ReplaceTradeClassifierMappingsWIthDuplication(List<TradeClassifierMapping> tradeClassifierMappings, List<TradeClassifierMapping> mappingsWithoutDuplication)
        {

            var mappingsWithoutDuplicationIds = mappingsWithoutDuplication.Select(m => m.InstrumentId).ToHashSet();

            foreach (var m in tradeClassifierMappings)
            {
                if (!mappingsWithoutDuplicationIds.Contains(m.InstrumentId))
                {
                    mappingsWithoutDuplication.Add(m);
                }
            }

            return mappingsWithoutDuplication;
        }

        private static List<TradeClassifierMapping> RemoveAggregatedClassifierMappings(List<TradeClassifierMapping> mappingsWithoutDuplication)
        {
            var aggregatedClassifiers = new HashSet<string>() { "3(h)", "3(f)", "3(g)" };


            var resultWithoutAggregation = new List<TradeClassifierMapping>();
            var count = 0;
            foreach (var r in mappingsWithoutDuplication)
            {
                foreach (var aggregatedClassifier in aggregatedClassifiers)
                {
                    if (r.Classifiers.Contains(aggregatedClassifier))
                    {
                        r.Classifiers.Remove(aggregatedClassifier);
                    }
                }

                if (r.Classifiers.Any())
                {
                    resultWithoutAggregation.Add(r);
                }
                else
                {
                    count++;
                }
            }

            return resultWithoutAggregation;
        }
        private static Dictionary<string, HashSet<string>> GetSectorInstruments(
            List<TradeClassifierMapping> tradeClassifierMappings)
        {
            var result = new Dictionary<string, HashSet<string>>();
            foreach (var tradeClassifierMapping in tradeClassifierMappings)
            {
                foreach (var classifier in tradeClassifierMapping.Classifiers)
                {
                    if (result.ContainsKey(classifier))
                    {
                        result[classifier].Add(tradeClassifierMapping.InstrumentId);
                    }
                    else
                    {
                        result[classifier] = new HashSet<string> { tradeClassifierMapping.InstrumentId };
                    }
                }
            }

            return result;
        }

        #region Loading

        public static List<TradeClassifierMapping> LoadTradeClassifierMappings(string directoryPath)
        {
            var tradeClassifierMappings = new List<TradeClassifierMapping>();

            var tradeClassifierMappingFilePathes = Directory.GetFiles(directoryPath);

            foreach (var path in tradeClassifierMappingFilePathes)
            {
                var tradeClassifierMapping = LoadTradeClassifierMapping(path);
                tradeClassifierMappings.Add(tradeClassifierMapping);
            }

            return tradeClassifierMappings;
        }

        public static TradeClassifierMapping LoadTradeClassifierMapping(string path)
        {
            var tradeClassifierMapping = new TradeClassifierMapping();
            using (var xmlReader = XmlReader.Create(path))
            {
                while (xmlReader.Read())
                {
                    var xmlType = xmlReader.NodeType;

                    if (xmlType == XmlNodeType.Element)
                    {
                        var elementName = xmlReader.Name;

                        if (elementName == "InstrumentID")
                        {
                            xmlReader.Read();
                            tradeClassifierMapping.InstrumentId = xmlReader.Value;
                        }
                        else if (elementName == "ClassifierId")
                        {
                            xmlReader.Read();
                            tradeClassifierMapping.Classifiers.Add(xmlReader.Value);
                        }
                        else if (elementName == "LegalEntityID")
                        {
                            xmlReader.Read();
                            tradeClassifierMapping.Issuer = xmlReader.Value;
                        }
                        else if (elementName == "IsRSA")
                        {
                            xmlReader.Read();
                            tradeClassifierMapping.IsRsa = xmlReader.Value;
                        }
                    }
                }
            }

            return tradeClassifierMapping;
        }

        #endregion

        private static void AddRange(HashSet<string> hashSet, IEnumerable<string> elements)
        {
            foreach (var e in elements)
            {
                hashSet.Add(e);
            }
        }

        #endregion

    }
}
