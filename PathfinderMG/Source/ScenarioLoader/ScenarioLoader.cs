using PathfinderMG.Core.Source.ScenarioCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace PathfinderMG.Core.Source.ScenarioLoader
{
    class ScenarioLoader
    {
        private readonly string DEFAULT_PATH, DEFAULT_PATH_SCENARIOS;
        private const string SCENARIO_FOLDER = "Scenarios";
        private const string SEARCH_PATTERN = "*.xml";

        public ScenarioLoader()
        {
            DEFAULT_PATH = GetDefaultPath();
            DEFAULT_PATH_SCENARIOS = Path.GetFullPath(Path.Combine(DEFAULT_PATH, SCENARIO_FOLDER));
        }

        public Dictionary<string, XDocument> FetchAllScenarios()
        {
            Dictionary<string, XDocument> output = new Dictionary<string, XDocument>();
            List<string> files;
            Stream fileStream;

            if (!Directory.Exists(DEFAULT_PATH_SCENARIOS))
            {
                Directory.CreateDirectory(DEFAULT_PATH_SCENARIOS);
                GenerateSampleScenarios();
            }

            try
            {
                files = Directory.GetFiles(DEFAULT_PATH_SCENARIOS, SEARCH_PATTERN).ToList();
            }
            catch (Exception)
            {
                // Show a messagebox if failed
                // For now let's just crash
                throw;
            }

            foreach (var file in files)
            {
                fileStream = File.OpenRead(file);
                output.Add(file.Substring(file.LastIndexOf("\\") + 1), XDocument.Load(fileStream));
            }

            return output;
        }

        public ScenarioWrapper LoadScenario(XDocument doc)
        {
            //XmlSerializer xmlSerializer = new XmlSerializer(typeof(ScenarioWrapper));

            //using (var reader = doc.Root.CreateReader())            
            //    return (ScenarioWrapper)xmlSerializer.Deserialize(reader);

            ScenarioWrapper output = new ScenarioWrapper()
            {
                Title = doc.Root.Element("Title").Value,
                Author = doc.Root.Element("Author").Value,
                DateCreated = DateTime.Parse(doc.Root.Element("DateCreated").Value)
            };

            List<XElement> rows = (from t in doc.Root.Descendants("Row")
                                   select t).ToList();

            List<string> data = new List<string>();
            foreach (var item in rows)
                data.Add(item.Value);

            output.Data = data;

            return output;
        }

        private string GetDefaultPath()
        {
            string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var assembly = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            string companyName = assembly.CompanyName;
            string productName = assembly.ProductName;

            return Path.Combine(defaultPath, companyName, productName);
        }

        // Or... download them?
        private void GenerateSampleScenarios()
        {
            // Copy Demo Scenarios to UserData
            string startDirectory = Path.GetFullPath("Content/DemoScenarios");

            foreach (string filename in Directory.EnumerateFiles(startDirectory))
            {
                using (FileStream SourceStream = File.Open(filename, FileMode.Open))
                {
                    using (FileStream DestinationStream = File.Create(Path.GetFullPath(Path.Combine(DEFAULT_PATH_SCENARIOS + filename.Substring(filename.LastIndexOf(Path.DirectorySeparatorChar))))))
                    {                        
                        SourceStream.CopyTo(DestinationStream);
                    }
                }
            }
        }
    }
}
