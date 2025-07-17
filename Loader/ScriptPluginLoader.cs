using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using PluginAPI.Core;

namespace PluginAPI.Loader
{
    public static class ScriptPluginLoader
    {
        public static void LoadPlugins(string directory)
        {
            string scriptsPath = Path.Combine(directory, "PluginScripts");
            if (!Directory.Exists(scriptsPath))
                Directory.CreateDirectory(scriptsPath);

            foreach (string csFile in Directory.GetFiles(scriptsPath, "*.cs"))
            {
                try
                {
                    Log.Info($"[ScriptLoader] Компиляция {Path.GetFileName(csFile)}...");
                    var code = File.ReadAllText(csFile);
                    var provider = new CSharpCodeProvider();
                    var parameters = new CompilerParameters
                    {
                        GenerateInMemory = true,
                        GenerateExecutable = false,
                        CompilerOptions = "/optimize"
                    };

                    parameters.ReferencedAssemblies.Add("System.dll");
                    parameters.ReferencedAssemblies.Add("System.Core.dll");
                    parameters.ReferencedAssemblies.Add("Assembly-CSharp.dll");
                    parameters.ReferencedAssemblies.Add("PluginAPI.dll");

                    var result = provider.CompileAssemblyFromSource(parameters, code);

                    if (result.Errors.HasErrors)
                    {
                        Log.Error($"[ScriptLoader] Ошибка компиляции: {string.Join("\n", result.Errors.Cast<CompilerError>().Select(e => e.ToString()))}");
                        continue;
                    }

                    var pluginAssembly = result.CompiledAssembly;
                    foreach (var type in pluginAssembly.GetTypes())
                    {
                        if (typeof(IPlugin).IsAssignableFrom(type))
                        {
                            var pluginInstance = (IPlugin)Activator.CreateInstance(type);
                            pluginInstance.OnEnabled();
                            Log.Info($"[ScriptLoader] Плагин {type.Name} активирован.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"[ScriptLoader] Ошибка загрузки {csFile}: {ex.Message}");
                }
            }
        }
    }
}
