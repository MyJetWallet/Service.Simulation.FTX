using System.Collections.Generic;
using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Simulation.FTX.Settings
{
    public class SettingsModel
    {
        [YamlProperty("SimulationFTX.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("SimulationFTX.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("SimulationFTX.InstrumentsOriginalSymbolToSymbol")]
        public string FtxInstrumentsOriginalSymbolToSymbol { get; set; }

        [YamlProperty("SimulationFTX.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("SimulationFTX.Name")]
        public string Name { get; set; }
    }
}