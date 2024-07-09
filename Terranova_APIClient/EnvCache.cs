using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terranova_APIClient.Models;

namespace Terranova_APIClient
{
    public static class EnvCache
    {
        public static Dictionary<string, int> EnvDictionary { get; set; } = new Dictionary<string, int>();
        public static HashSet<string> Blacklist { get; set; } = new HashSet<string>();
        private static object lockObj = new object();

        public static ReportApiInfo GetReportApiInfo(Dictionary<int, ReportApiInfo> reportApiInfos, string envUID, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(envUID))
            {
                return null;
            }

            if (!EnvDictionary.TryGetValue(envUID, out int instance))
            {
                lock (lockObj)
                {
                    if (!Blacklist.Contains(envUID))
                    {
                        foreach (var info in reportApiInfos)
                        {
                            //Get and update UID list for each instance
                            var guids = Task.Run(async () => await TerranovaAPIClient.GetUidsAsync(info.Value, logger))
                                .Result
                                .Select(x => x.ToString());

                            foreach (var guid in guids)
                            {
                                if (!EnvDictionary.ContainsKey(guid))
                                {
                                    EnvDictionary.TryAdd(guid, info.Key);
                                }

                                if (Blacklist.Contains(guid))
                                {
                                    Blacklist.Remove(guid);
                                }
                            }
                        }
                    }

                    //Second chance after updates
                    if (!EnvDictionary.TryGetValue(envUID, out instance))
                    {
                        //still not good? Blacklist !!!
                        if (!Blacklist.Contains(envUID))
                        {
                            Blacklist.Add(envUID);
                        }

                        return null;
                    }
                }
            }

            return reportApiInfos[instance];
        }

        public static void ClearCache()
        {
            lock (lockObj)
            {
                EnvDictionary.Clear();
                Blacklist.Clear();
            }
        }
    }
}
