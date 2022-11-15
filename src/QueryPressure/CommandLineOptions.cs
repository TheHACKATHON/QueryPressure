using CommandLine;

namespace QueryPressure;

internal sealed class CommandLineOptions
{
  [Option('c', "config", Required = true, HelpText = "Configuration files to be processed.")]
  public IEnumerable<string> ConfigFiles { get; set; } = null!;


  [Option('s', "script", Required = true, HelpText = "The script file that we will run.")]
  public string ScriptFile { get; set; } = null!;
}
