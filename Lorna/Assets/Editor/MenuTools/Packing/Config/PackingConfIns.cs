namespace EscapeGame.Building.Config{
    [InsConfig("./Assets/Editor/MenuTools/Packing/Config/PackingConfig.json")]
    internal sealed class PackingConfIns : CmdInsConfigBase<PackingConfIns>{
        public string PackingInConf;
        public string PackingOutConf;
        public string PackingBuildNumber;
    }
}