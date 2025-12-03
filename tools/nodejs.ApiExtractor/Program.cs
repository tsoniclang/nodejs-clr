using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace nodejs.ApiExtractor;

class Program
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "This is a reflection tool, not meant for AOT")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "This is a reflection tool, not meant for AOT")]
    static void Main(string[] args)
    {
        var assembly = typeof(nodejs.path).Assembly;
        var api = ExtractApiSignatures(assembly);

        var json = JsonSerializer.Serialize(api, new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        var outputPath = args.Length > 0 ? args[0] : "nodejs-clr-api.json";
        File.WriteAllText(outputPath, json);

        Console.WriteLine($"Extracted API signatures to {outputPath}");
        Console.WriteLine($"Found {api.Modules.Count} modules");
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "This is a reflection tool, not meant for AOT")]
    [UnconditionalSuppressMessage("Trimming", "IL2075", Justification = "This is a reflection tool, not meant for AOT")]
    static ApiDefinition ExtractApiSignatures(Assembly assembly)
    {
        var api = new ApiDefinition { Modules = new Dictionary<string, ModuleDefinition>() };

        // Get all public types in nodejs namespace
        var types = assembly.GetTypes()
            .Where(t => t.Namespace == "nodejs" && t.IsPublic)
            .OrderBy(t => t.Name);

        foreach (var type in types)
        {
            var module = new ModuleDefinition
            {
                Name = type.Name,
                IsClass = type.IsClass,
                IsStatic = type.IsAbstract && type.IsSealed,
                Methods = new List<MethodSignature>(),
                Properties = new List<PropertySignature>()
            };

            // Extract methods
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName) // Exclude property getters/setters
                .OrderBy(m => m.Name);

            foreach (var method in methods)
            {
                module.Methods.Add(new MethodSignature
                {
                    Name = method.Name,
                    IsStatic = method.IsStatic,
                    ReturnType = FormatType(method.ReturnType),
                    Parameters = method.GetParameters().Select(p => new ParameterSignature
                    {
                        Name = p.Name ?? "",
                        Type = FormatType(p.ParameterType),
                        IsOptional = p.IsOptional,
                        DefaultValue = p.HasDefaultValue ? p.DefaultValue?.ToString() : null
                    }).ToList()
                });
            }

            // Extract properties
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .OrderBy(p => p.Name);

            foreach (var property in properties)
            {
                module.Properties.Add(new PropertySignature
                {
                    Name = property.Name,
                    Type = FormatType(property.PropertyType),
                    IsStatic = property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic ?? false,
                    CanRead = property.CanRead,
                    CanWrite = property.CanWrite
                });
            }

            // Only add module if it has public members
            if (module.Methods.Count > 0 || module.Properties.Count > 0)
            {
                api.Modules[type.Name] = module;
            }
        }

        return api;
    }

    static string FormatType(Type type)
    {
        if (type == typeof(void))
            return "void";

        if (type == typeof(string))
            return "string";

        if (type == typeof(int))
            return "number";

        if (type == typeof(double))
            return "number";

        if (type == typeof(bool))
            return "boolean";

        if (type == typeof(object))
            return "any";

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != null)
            return FormatType(underlyingType) + " | null";

        // Handle arrays
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            return FormatType(elementType!) + "[]";
        }

        // Handle generic types
        if (type.IsGenericType)
        {
            var genericTypeDef = type.GetGenericTypeDefinition();
            var genericArgs = type.GetGenericArguments();

            if (genericTypeDef == typeof(List<>))
                return FormatType(genericArgs[0]) + "[]";

            if (genericTypeDef == typeof(Dictionary<,>))
                return $"Record<{FormatType(genericArgs[0])}, {FormatType(genericArgs[1])}>";

            if (genericTypeDef == typeof(Action<>))
                return $"({string.Join(", ", genericArgs.Select((t, i) => $"arg{i}: {FormatType(t)}"))}) => void";

            if (genericTypeDef == typeof(Func<>))
            {
                var returnType = genericArgs.Last();
                var paramTypes = genericArgs.Take(genericArgs.Length - 1);
                return $"({string.Join(", ", paramTypes.Select((t, i) => $"arg{i}: {FormatType(t)}"))}) => {FormatType(returnType)}";
            }
        }

        // Return the simple type name for other types
        return type.Name;
    }
}

class ApiDefinition
{
    public Dictionary<string, ModuleDefinition> Modules { get; set; } = new();
}

class ModuleDefinition
{
    public string Name { get; set; } = "";
    public bool IsClass { get; set; }
    public bool IsStatic { get; set; }
    public List<MethodSignature> Methods { get; set; } = new();
    public List<PropertySignature> Properties { get; set; } = new();
}

class MethodSignature
{
    public string Name { get; set; } = "";
    public bool IsStatic { get; set; }
    public string ReturnType { get; set; } = "";
    public List<ParameterSignature> Parameters { get; set; } = new();
}

class ParameterSignature
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public bool IsOptional { get; set; }
    public string? DefaultValue { get; set; }
}

class PropertySignature
{
    public string Name { get; set; } = "";
    public string Type { get; set; } = "";
    public bool IsStatic { get; set; }
    public bool CanRead { get; set; }
    public bool CanWrite { get; set; }
}
