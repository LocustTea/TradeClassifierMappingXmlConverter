using System.Collections.Generic;
using System.Xml;

namespace TradeClassifierMappingXmlConverter
{
    public class ClassifierTree
    {
        public static HashSet<string> GetRootClassifiers(string path)
        {
            var rootClassifiers = new HashSet<string>();

            using (var xmlReader = XmlReader.Create(path))
            {
                while (xmlReader.Read())
                {
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ClassifierTrees")
                    {
                        while (true)
                        {
                            xmlReader.Read();
                            if (xmlReader.NodeType == XmlNodeType.EndElement && xmlReader.Name == "ClassifierTrees")
                            {
                                break;
                            }

                            string rootClassifier = null;
                            if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "TradeClassifier")
                            {
                                while (true)
                                {
                                    xmlReader.Read();

                                    if (xmlReader.NodeType == XmlNodeType.EndElement &&
                                        xmlReader.Name == "TradeClassifier")
                                    {
                                        break;
                                    }

                                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "ClassifierId")
                                    {
                                        Sector.ReaderXmlReader(xmlReader);
                                        rootClassifier = xmlReader.Value;
                                        rootClassifiers.Add(rootClassifier);
                                    }
                                }
                            }

                            if (rootClassifier != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            rootClassifiers.Add("99");
            return rootClassifiers;
        }
    }
}
