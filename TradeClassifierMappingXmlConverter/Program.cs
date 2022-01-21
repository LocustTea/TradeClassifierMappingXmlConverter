using System;

namespace TradeClassifierMappingXmlConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var tradeClassifierMappingsPath = @"D:\Work\projects\omsfin\helping programms\TempData\TradeClassifierMappings\";
            var sectorsDirectoryPath = @"D:\Work\projects\omsfin\helping programms\TempData\Sectors\";
            var classifierTreePath = @"D:\Work\projects\omsfin\helping programms\TempData\Cl.Analyst.Finance.SaRegulation.ClSaRegClassifierTreeData;ClassifierTree.xml";


            var result = TradeClassifierMapping.ReworkTradeClassifierMappings(tradeClassifierMappingsPath, sectorsDirectoryPath, classifierTreePath);


            var xmlContainers = TradeClassifierMappingCreator.CreateInstrumentXmlContainers(result);

            FileSaver.SaveContainers(@"D:\Work\projects\omsfin\helping programms\TempData\", "ResultTradeClassifierMapping", xmlContainers);


            Console.ReadKey();
        }
    }
}
