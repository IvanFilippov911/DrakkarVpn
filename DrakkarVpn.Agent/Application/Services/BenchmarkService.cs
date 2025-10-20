using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using DrakkarVpn.Agent.Application.Abstractions;
using DrakkarVpn.Shared;
using Microsoft.Extensions.Caching.Memory;

namespace DrakkarVpn.Agent.Application.Services;

public sealed class BenchmarkService : IBenchmarkService
{
    private readonly ILogger<BenchmarkService> _logger;
    private readonly IConfiguration _config;
    private readonly IMemoryCache _cache;

    private const string CacheKey = "BenchmarkResult:text";

    public BenchmarkService(ILogger<BenchmarkService> logger, IConfiguration config, IMemoryCache cache)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _cache  = cache  ?? throw new ArgumentNullException(nameof(cache));
    }

    public async Task<BenchmarkResultDto> RunBenchmarkAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKey, out BenchmarkResultDto cached))
            return cached;

        var res = await PerformAsync(ct);
        
        var ttl = (res.DownloadMbps > 0 && res.UploadMbps > 0)
            ? TimeSpan.FromMinutes(5)
            : TimeSpan.FromSeconds(20);

        _cache.Set(CacheKey, res, ttl);
        return res;
    }

    private async Task<BenchmarkResultDto> PerformAsync(CancellationToken ct)
    {
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromMinutes(2));

            var bin  = _config["Benchmark:LibreSpeedPath"] ?? "/root/speedtest-cli/librespeed-cli";
            var args = BuildArgs(_config["Benchmark:Arguments"]);

            if (!File.Exists(bin))
            {
                _logger.LogError("librespeed-cli not found: {Path}", bin);
                return Fail();
            }

            var psi = new ProcessStartInfo
            {
                FileName = bin,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError  = true,
                UseShellExecute = false,
                CreateNoWindow  = true
            };

            using var p = new Process { StartInfo = psi };
            var sw = Stopwatch.StartNew();
            p.Start();

            var stdoutTask = p.StandardOutput.ReadToEndAsync(cts.Token);
            var stderrTask = p.StandardError.ReadToEndAsync(cts.Token);

            await p.WaitForExitAsync(cts.Token);
            sw.Stop();

            var stdout = await stdoutTask;
            var stderr = await stderrTask;

            if (p.ExitCode != 0)
            {
                _logger.LogError("librespeed exit={Code} in {Ms}ms; stderr: {Err}; stdout: {Out}",
                    p.ExitCode, sw.ElapsedMilliseconds, stderr, Trunc(stdout, 400));
                return Fail();
            }

            var (down, up) = ParseSimple(stdout);
            if (down <= 0 || up <= 0)
            {
                _logger.LogWarning("librespeed simple parsed invalid (down={Down}, up={Up}). raw: {Out}",
                    down, up, Trunc(stdout, 400));
                return Fail();
            }

            _logger.LogInformation("librespeed simple ok in {Ms}ms: download={Down} Mbps, upload={Up} Mbps",
                sw.ElapsedMilliseconds, down, up);

            return new BenchmarkResultDto(down, up, DateTimeOffset.UtcNow);
        }
        catch (OperationCanceledException)
        {
            _logger.LogError("librespeed timeout");
            return Fail();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "librespeed unexpected failure");
            return Fail();
        }

        static BenchmarkResultDto Fail() => new(-1, -1, DateTimeOffset.UtcNow);
    }

    private static string BuildArgs(string? raw)
    {
        var args = string.IsNullOrWhiteSpace(raw) ? "--simple" : raw.Trim();
        if (!args.Contains("--simple", StringComparison.OrdinalIgnoreCase)
            && !args.Contains("--json", StringComparison.OrdinalIgnoreCase))
            args += " --simple";
        return args;
    }

    
    private static (double down, double up) ParseSimple(string output)
    {
        static double Num(string s)
        {
            s = s.Trim().Replace(',', '.');
            return double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : -1;
        }

        var rxDown = new Regex(@"(?im)^\s*Download(?:\s*rate)?\s*:\s*([\d\.,]+)\s*M(?:b|bit)/?s?\b");
        var rxUp   = new Regex(@"(?im)^\s*Upload(?:\s*rate)?\s*:\s*([\d\.,]+)\s*M(?:b|bit)/?s?\b");

        var mD = rxDown.Match(output);
        var mU = rxUp.Match(output);

        var down = mD.Success ? Num(mD.Groups[1].Value) : -1;
        var up   = mU.Success ? Num(mU.Groups[1].Value) : -1;

        
        if (down < 0)
        {
            mD = Regex.Match(output, @"(?im)^\s*Download\s*:\s*([\d\.,]+)\s*M(?:b|bit)/?s?\b");
            if (mD.Success) down = Num(mD.Groups[1].Value);
        }
        if (up < 0)
        {
            mU = Regex.Match(output, @"(?im)^\s*Upload\s*:\s*([\d\.,]+)\s*M(?:b|bit)/?s?\b");
            if (mU.Success) up = Num(mU.Groups[1].Value);
        }

        return (down, up);
    }

    private static string Trunc(string s, int max) =>
        string.IsNullOrEmpty(s) || s.Length <= max ? s : s[..max] + "…";
}
