using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using LabAPI;

public class FrameworkPlugin : ILabApiPlugin {
    public override void OnEnabled() {
        var dir = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        Directory.CreateDirectory(dir);
        foreach (var f in Directory.GetFiles(dir, "*.dll")) {
            var asm = new PluginLoader(f).LoadFromAssemblyPath(f);
            RegisterHandlers(asm);
        }

        var ev = LabAPI.Events.Server;
        ev.PlayerJoined += p => PluginEvents.Raise("OnPlayerJoined", p);
        ev.PlayerLeft   += p => PluginEvents.Raise("OnPlayerLeft", p);
        ev.RoundStarted += args => PluginEvents.Raise("OnRoundStart", args);
        ev.RoundEnded   += args => PluginEvents.Raise("OnRoundEnd", args);
        ev.PlayerChangeRole += (p,o,n) => PluginEvents.Raise("OnPlayerChangeRole", p, o, n);
        ev.PlayerVerified += p => PluginEvents.Raise("OnPlayerVerified", p);
        Console.WriteLine("[PluginAPI] Enabled with full event mapping");
    }

    private void RegisterHandlers(Assembly asm) {
        foreach (var type in asm.GetTypes())
            foreach (var m in type.GetMethods(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance|BindingFlags.Static)) {
                var attr = m.GetCustomAttribute<EventHandlerAttribute>();
                if (attr == null) continue;
                var handler = Delegate.CreateDelegate(
                    GetDelegateType(m), m.IsStatic ? null : Activator.CreateInstance(type), m);
                PluginEvents.AddListener(attr.EventName, handler);
            }
    }
    
    private Type GetDelegateType(MethodInfo m) {
        var types = m.GetParameters().Select(p => p.ParameterType).ToList();
        if (m.ReturnType == typeof(void)) return Expression.GetActionType(types.ToArray());
        types.Add(m.ReturnType);
        return Expression.GetFuncType(types.ToArray());
    }
}