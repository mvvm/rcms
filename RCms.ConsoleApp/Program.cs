using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using NLog;
using Raven.Client;
using Raven.Client.Indexes;
using RCms.ConsoleApp.Configuration;

namespace RCms.ConsoleApp
{
    class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static IUnityContainer _unityContainer = UnityConfig.GetConfiguredContainer();

        private static void WriteHelp()
        {
            Console.WriteLine("Choose what to do:");
            Console.WriteLine("1 - Run Migrations - not implemented yet");
            Console.WriteLine("2 - Update Transforms and Indexes");
            Console.WriteLine("Empty - Exit");
            Console.WriteLine("Press number and Enter.");
        }

        static void Main(string[] args)
        {
            var store = _unityContainer.Resolve<IDocumentStore>();
            WriteHelp();
            var input = Console.ReadLine();
            while (string.IsNullOrEmpty(input) == false)
            {
                try
                {
                    switch (input.ToLower())
                    {
                        case "1":
                            Console.WriteLine("Complete");
                            break;
                        case "2":
                            // IndexCreation.CreateIndexes(typeof(SaleToSaleListModelTransform).Assembly, store);
                            // IndexCreation.CreateIndexes(typeof(AgentsSuspenseCommentsIndex).Assembly, store);
                            Console.WriteLine("Complete");
                            break;
                        default:
                            Console.WriteLine("Command not found");
                            break;

                    }
                }
                catch (Exception exception)
                {
                    _logger.Error(exception, string.Format("Tried execute: {0}", input));
                    Console.WriteLine();
                    Console.WriteLine("Exception:");
                    Console.WriteLine(exception.ToString());
                }
                finally
                {
                    Console.WriteLine();
                    WriteHelp();
                    input = Console.ReadLine();
                }
            }
        }
    }
}
