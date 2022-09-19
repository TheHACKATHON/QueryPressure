using System.Diagnostics;
using QueryPressure.Core.Interfaces;

namespace QueryPressure.Core;

public class QueryExecutor
{
    private readonly IExecutable _executable;
    private readonly IProfile _loadProfile;

    public QueryExecutor(IExecutable executable, IProfile loadProfile)
    {
        _executable = executable;
        _loadProfile = loadProfile;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        while (!cancellationToken.IsCancellationRequested)
        {
            var descriptor = await _loadProfile.WhenNextCanBeExecutedAsync(cancellationToken);
            var _ = _executable.ExecuteAsync(cancellationToken).ContinueWith(async _ =>
            {
                await _loadProfile.OnQueryExecutedAsync(descriptor, cancellationToken);
            }, cancellationToken);
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}