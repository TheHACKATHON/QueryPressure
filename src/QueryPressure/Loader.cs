﻿using Autofac;
using CommandLine;
using QueryPressure;
using QueryPressure.App;
using QueryPressure.App.Arguments;
using QueryPressure.Core;
using QueryPressure.Exceptions;
using QueryPressure.Postgres.App;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;


internal class Loader
{
  public Loader()
  {
  }

  public IContainer Load(string[] args)
  {
    var builder = new ContainerBuilder();
    var appArgs = Merge(args);

    builder.RegisterInstance(appArgs).AsSelf();
    builder.RegisterModule<AppModule>();
    builder.RegisterModule<PostgresAppModule>();
    LoadPlugins(builder);
        
    return builder.Build();
  }
  
  private void LoadPlugins(ContainerBuilder builder)
  {
    var dir = new FileInfo(GetType().Assembly.Location).Directory;
    var dlls = dir.GetFiles("*.dll");
    var asms = AppDomain.CurrentDomain.GetAssemblies();
    foreach (var dll in dlls.Where(x => IsSuitable(x.FullName)))
    {
      var defaultContext = System.Runtime.Loader.AssemblyLoadContext.Default; // (!!!) Important
      var loaded = asms.FirstOrDefault(x => x.Location.ToLowerInvariant() == dll.FullName.ToLowerInvariant());
      if (loaded == null)
      {
        loaded = defaultContext.LoadFromAssemblyPath(dll.FullName);
      }
      builder.RegisterAssemblyModules(loaded);
    }
  }
  private bool IsSuitable(string path)
  {
    try
    {
      var type = typeof(QueryPressurePluginAttribute);
      var asm = Mono.Cecil.AssemblyDefinition.ReadAssembly(path); // (!!!) Important
      return asm
        .CustomAttributes
        .Any(attribute => attribute.AttributeType.Name == type.Name && attribute.AttributeType.Namespace == type.Namespace);
    }
    catch
    {
      return false;
    }
  }

  private ApplicationArguments Merge(string[] args)
  {
    ParserResult<CommandLineOptions>? parseResult = Parser.Default.ParseArguments<CommandLineOptions>(args)
      .WithNotParsed(_ => throw new ArgumentsParseException());

    CommandLineOptions options = parseResult.Value;
    var result = new ApplicationArguments();
    foreach (var configFile in options.ConfigFiles)
    {
      var appArgs = Deserialize(File.ReadAllText(configFile));
      foreach (var applicationArgument in appArgs)
      {
        result.Add(applicationArgument.Key, applicationArgument.Value);
      }
    }
    
    result.Add("script", new ArgumentsSection() {
      Type = "file",
      Arguments = new() {
        ["path"] = options.ScriptFile
      }
    });

    return result;
  }

  private ApplicationArguments Deserialize(string fileContent)
  {
    var deserializer = new DeserializerBuilder()
      .WithNamingConvention(CamelCaseNamingConvention.Instance)
      .Build();

    return deserializer.Deserialize<ApplicationArguments>(fileContent);
  }
}
