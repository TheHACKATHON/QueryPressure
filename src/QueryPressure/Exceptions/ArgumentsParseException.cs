namespace QueryPressure.Exceptions;

public class ArgumentsParseException : ApplicationException
{
  public ArgumentsParseException(): base("Can not parse command line arguments")
  { }
}
