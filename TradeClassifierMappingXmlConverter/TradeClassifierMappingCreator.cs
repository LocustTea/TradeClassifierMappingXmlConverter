using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeClassifierMappingXmlConverter
{
    public class TradeClassifierMappingCreator
    {
        public static List<XmlContainer> CreateInstrumentXmlContainers(List<TradeClassifierMapping> tradeClassifierMappings)
        {
            var xmlContainers = new List<XmlContainer>();

            foreach (var tradeClassifierMapping in tradeClassifierMappings)
            {
                var xmlContent = new StringBuilder();

                xmlContent.Append($"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n");
                xmlContent.Append($"<Cl.Analyst.Finance.SaRegulation.ClSaRegTradeClassifierMappingData>\n");

                xmlContent.Append($"  <Instrument>\n");
                xmlContent.Append($"    <InstrumentID>{tradeClassifierMapping.InstrumentId}</InstrumentID>\n");
                xmlContent.Append($"  </Instrument>\n");


                foreach (var classifier in tradeClassifierMapping.Classifiers)
                {
                    xmlContent.Append($"  <TradeClassifiers>\n");
                    xmlContent.Append($"    <ClassifierId>{classifier}</ClassifierId>\n");
                    xmlContent.Append($"  </TradeClassifiers>\n");
                }

                xmlContent.Append($"  <Issuer>\n");
                xmlContent.Append($"    <LegalEntityID>{tradeClassifierMapping.Issuer}</LegalEntityID>\n");
                xmlContent.Append($"  </Issuer>\n");


                xmlContent.Append($"  <IsRSA>{tradeClassifierMapping.IsRsa.ToString()}</IsRSA>\n");
                xmlContent.Append($"</Cl.Analyst.Finance.SaRegulation.ClSaRegTradeClassifierMappingData>");


                var xmlContainer = new XmlContainer
                {
                    FileName = $"Cl.Analyst.Finance.SaRegulation.ClSaRegTradeClassifierMappingData;{tradeClassifierMapping.InstrumentId}.xml",
                    Content = xmlContent.ToString()
                };

                xmlContainers.Add(xmlContainer);
            }

            return xmlContainers;
        }

    }
}
