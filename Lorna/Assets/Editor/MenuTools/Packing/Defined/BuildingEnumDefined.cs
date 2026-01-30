namespace EscapeGame.Building{
    public enum CommandOperationType{
        Config        = 1 << 2, //配置表build
        Version       = 2 << 2, //版本修改
        Shader        = 3 << 2, //shader build
        VFX           = 4 << 2, //VFX save/build
        Scripts       = 5 << 2, //Scripts build
        Sources       = 6 << 2, //资源build
        UnityBuilding = 7 << 2, //引擎内建build
    }

    public enum BuildingCmdValueType{
        None,
        IntVal,
        StringVal,
        Choices,
    }
}