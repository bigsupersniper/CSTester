using CSScriptLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTester.CSEngine
{
    public sealed class ScriptBootstrap
    {
        const string CS_DIR = "./Scripts";
        const string CS_CONFIG = CS_DIR + "/ScriptConfig.cs";

        static object locker = new object();

        public static bool Started { get; private set; }

        public static IList<IScript> Scripts { get; private set; }

        static void Load(bool reload)
        {
            Exception ex = null;

            lock (locker)
            {
                if (reload || !Started)
                {
                    try
                    {
                        if (Directory.Exists(CS_DIR))
                        {
                            if (File.Exists(CS_CONFIG))
                            {
                                CSScript.Evaluator.Reset(true);
                                var config = CSScript.Evaluator.LoadFile<IScriptConfig>(CS_CONFIG);
                                //加载dll
                                if (config.Assemblies != null && config.Assemblies.Length > 0)
                                {
                                    foreach (var ass in config.Assemblies)
                                    {
                                        CSScript.Evaluator.ReferenceAssemblyByName(ass);
                                    }
                                }
                                //加载类库
                                if (config.Preloads != null && config.Preloads.Length > 0)
                                {
                                    foreach (var pl in config.Preloads)
                                    {
                                        try
                                        {
                                            CSScript.Evaluator.LoadFile(CS_DIR + "/" + pl);
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("LoadFile \"" + pl + "\" Catch " + e);
                                        }
                                    }
                                }
                                //加载分析类
                                if (config.Scripts != null && config.Scripts.Length > 0)
                                {
                                    Scripts = new List<IScript>();
                                    foreach (var pl in config.Scripts)
                                    {
                                        try
                                        {
                                            Scripts.Add(CSScript.Evaluator.LoadFile<IScript>(CS_DIR + "/" + pl));
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("LoadFile \"" + pl + "\" Catch " + e);
                                        }
                                    }
                                }

                                Started = true;
                            }
                            else
                            {
                                throw new Exception("Colud not found 'ScriptConfig.cs' file");
                            }
                        }
                        else
                        {
                            throw new Exception("Could not found 'Scripts' folder");
                        }
                    }
                    catch (Exception e)
                    {
                        ex = e;
                    }
                }
            }

            if (ex != null) throw ex;
        }

        public static void Start()
        {
            Load(false);
        }

        public static void Restart()
        {
            if (Started)
            {
                CSScript.Evaluator.Reset(true);
            }

            Load(true);
        }
    }
}
