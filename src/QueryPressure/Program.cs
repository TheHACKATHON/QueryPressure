﻿// See https://aka.ms/new-console-template for more information

using Autofac;
using QueryPressure.App.Arguments;
using QueryPressure.App.Interfaces;
using QueryPressure.Exceptions;

var loader = new Loader();

IContainer container;

try
{
  container = loader.Load(args);
}
catch (ArgumentsParseException)
{
  return;
}

var builder = container.Resolve<IScenarioBuilder>();
var appArgs = container.Resolve<ApplicationArguments>();

var executor = await builder.BuildAsync(appArgs, default);

await executor.ExecuteAsync(default);
