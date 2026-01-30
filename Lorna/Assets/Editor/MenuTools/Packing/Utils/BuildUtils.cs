using System;
using System.IO;
using System.Text.RegularExpressions;

namespace EscapeGame.Building.Utils{
    public static class BuildUtils{
        /// <summary>
        /// 获取资源版本
        /// 当前策略直接取最新时间即可(有多平台、多地域需求的时候再做额外考虑)
        /// </summary>
        public static string GetPackageVersion(){
            string totalMinutes = $"{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}";
            return DateTime.Now.ToString("yyyy-MM-dd") + "-" + totalMinutes;
        }

        /// <summary>
        /// 获取出包文件夹名
        /// </summary>
        /// <returns></returns>
        public static string GetPkgFolder(string tag){
            string totalDay     = $"{DateTime.Now.Year:D2}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}";
            string totalMinutes = $"{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}";
            string branch       = GetGitBranch();
            //PE项目单独写死
            if (branch == "BattleLevel"){
                branch = "BL";
            }
            else if (branch.Contains("tryplay")){
                branch = "TP";
            }

            string output = $"{totalDay}_{branch}_{tag}_{totalMinutes}";
            return output;
        }

        public static string GetGitBranch(){
            string branchName;
            // 获取 Unity 项目根目录
            string projectRoot     = Environment.CurrentDirectory;
            string parentDirectory = Directory.GetParent(projectRoot).FullName;
            string gitHeadPath     = Path.Combine(parentDirectory, ".git", "HEAD");
            if (File.Exists(gitHeadPath)){
                string headContent = File.ReadAllText(gitHeadPath).Trim();
                branchName = Regex.Replace(headContent, "ref: refs/heads/", "");
                return branchName;
            }
            else{ return "Unknown"; }
        }
    }
}