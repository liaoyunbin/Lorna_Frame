using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EscapeGame.Building.Config;
using Sirenix.Utilities;
using UnityEngine;
using LornaGame.ModuleExtensions;

namespace EscapeGame.Building{
    public class ShellInterface{
        //指令数组
        public readonly List<IBuildingCommand> ShellCmds = new(){
                                                                    new BuildingCmdVersion(),
                                                                    new BuildCmdReimportAll(),
                                                                    new BuildingCmdTexture(),
                  
                                                                    new BuildingCmdExportConfig(),
                                          
                                                                     new BuildingCmdHybrid(),
                                                     
                                                                    new BuildCmdPackSVC(),
                                                         
                                                                    new BuildingCmdBuildApp(),
                                                                    new BuildingCmdUploadAppSteam(),
                                                                };

        public readonly List<IBuildingCommand> PreBuildCmds = new(){
            new BuildingCmdHybridPlugins(),
            new BuildingCmdPlatform(),
            };

        public static void ShellEntrance(){
            var shell = new ShellInterface();
            _ = shell.Run(CommandParameters.LoadFromFile());
        }

        //执行指令
        public async Task<bool> Run(CommandParameters sp){
            //获取打包输出
            Dictionary<string, string> output = new Dictionary<string, string>();
            //执行指令
            bool result = false;
            foreach (var cmd in ShellCmds){
                var pkgStr = cmd.GetType().GetCustomAttribute<CommandPkgStrAttribute>();
                if (sp.TryGetValue(pkgStr.FullCommand, out var val)){
                    UnityEngine.Debug.Log($"开始执行{pkgStr.FullCommand}指令");
                    try{
                        result = await cmd.OnExecute(val, output);
                        if (false == result){
                            UnityEngine.Debug.LogError($"打包异常:{pkgStr.FullCommand},参数:{val}".RedColor());
                            break;
                        }
                    }
                    catch (Exception e){
                        UnityEngine.Debug.LogError($"打包throw异常:{pkgStr.FullCommand},参数:{val},exception:{e.Message}".RedColor());
                    }
                }
            }

            // string content = JsonUtils.ObjectToJson(output);
            string content = JsonUtility.ToJson(output);
            UnityEngine.Debug.LogWarning($"打包输出:{content}");
            DirectoryInfo dataPathDir          = Directory.CreateDirectory(Application.dataPath);
            string        _unity_output_string = Path.Combine(dataPathDir.Parent.FullName, PackingConfIns.Ins.PackingOutConf);
            File.WriteAllText(_unity_output_string, content, new UTF8Encoding(false));
            return result;
        }
    }
}