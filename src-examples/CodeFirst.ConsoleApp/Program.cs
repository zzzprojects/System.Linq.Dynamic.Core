using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using CodeFirst.DataAccess.Factories;
using CodeFirst.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CodeFirst.ConsoleApp;

public static class Program
{
    private static async Task Main()
    {
        var sqlFactory = new SqlServerDbContextFactory();

        await using var context = sqlFactory.CreateDbContext(Array.Empty<string>());

        var stopwatch = ValueStopwatch.StartNew();
        var query1 = context.Movies.Include(x => x.Copies).Where("Title.Contains(\"e\")").ToDynamicList();
        Console.WriteLine($"Elapsed time: {stopwatch.GetElapsedTime().TotalMilliseconds}ms");
        query1.ToList().ToList().ForEach(Console.WriteLine);

        Console.WriteLine(new string('-', 80));

        stopwatch = ValueStopwatch.StartNew();
        var query2 = context.Movies.Include(x => x.Copies).Where("Title.Contains(\"e\")").ToDynamicList();
        Console.WriteLine($"Elapsed time: {stopwatch.GetElapsedTime().TotalMilliseconds}ms");
        query2.ToList().ToList().ForEach(Console.WriteLine);

        //var sqlServerRepo = new MoviesRepository(sqlFactory.CreateDbContext(Array.Empty<string>()));
        //var movies = await sqlServerRepo.GetAllAsync();
        //movies.ToList().ForEach(Console.WriteLine);
        //var getById = await sqlServerRepo.GetByIdAsync(3);
        //Console.WriteLine(getById.Title);

        //var postFactory = new PostgresDbContextFactory();
        //var postgresRepo = new MoviesRepository(postFactory.CreateDbContext(Array.Empty<string>()));
        //movies = await postgresRepo.GetAllAsync();
        //movies.ToList().ForEach(Console.WriteLine);
        //getById = await postgresRepo.GetByIdAsync(3);
        //Console.WriteLine(getById.Title);
    }

    /// <summary>
    /// Copied from https://github.com/dotnet/aspnetcore/blob/main/src/Shared/ValueStopwatch/ValueStopwatch.cs
    /// </summary>
    internal readonly struct ValueStopwatch
    {
#if !NET7_0_OR_GREATER
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
#endif

        private readonly long _startTimestamp;

        public bool IsActive => _startTimestamp != 0;

        private ValueStopwatch(long startTimestamp)
        {
            _startTimestamp = startTimestamp;
        }

        public static ValueStopwatch StartNew() => new(Stopwatch.GetTimestamp());

        public TimeSpan GetElapsedTime()
        {
            // Start timestamp can't be zero in an initialized ValueStopwatch. It would have to be literally the first thing executed when the machine boots to be 0.
            // So it being 0 is a clear indication of default(ValueStopwatch)
            if (!IsActive)
            {
                throw new InvalidOperationException("An uninitialized, or 'default', ValueStopwatch cannot be used to get elapsed time.");
            }

            var end = Stopwatch.GetTimestamp();

#if !NET7_0_OR_GREATER
            var timestampDelta = end - _startTimestamp;
            var ticks = (long)(TimestampToTicks * timestampDelta);
            return new TimeSpan(ticks);
#else
            return Stopwatch.GetElapsedTime(_startTimestamp, end);
#endif
        }
    }
}