using SimpleTrading.SettingsReader;

namespace Service.Simulation.FTX.Settings
{
    [YamlAttributesOnly]
    public class SettingsModel
    {
        [YamlProperty("SimulationFTX.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("SimulationFTX.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }

        [YamlProperty("SimulationFTX.InstrumentsOriginalSymbolToSymbol")]
        public string FtxInstrumentsOriginalSymbolToSymbol { get; set; }
    }
}