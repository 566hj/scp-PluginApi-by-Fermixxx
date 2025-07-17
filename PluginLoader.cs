using System.Reflection;
using System.Runtime.Loader;

public class PluginLoader : AssemblyLoadContext {
    private readonly AssemblyDependencyResolver resolver;
    public PluginLoader(string path) : base(true) {
        resolver = new AssemblyDependencyResolver(path);
    }
    protected override Assembly? Load(AssemblyName name) {
        var p = resolver.ResolveAssemblyToPath(name);
        return p != null ? LoadFromAssemblyPath(p) : null;
    }
}