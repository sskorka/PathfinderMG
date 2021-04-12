using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace PathfinderMG.Core.Source.ScenarioCore
{
    [XmlRoot("Scenario")]
    public class ScenarioWrapper
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public DateTime DateCreated { get; set; }   
        public List<string> Data { get; set; }
    }
}
