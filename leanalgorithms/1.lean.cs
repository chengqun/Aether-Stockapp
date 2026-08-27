using System.Collections.Generic;
using QuantConnect;
using QuantConnect.Algorithm;
using QuantConnect.Data;
using QuantConnect.Indicators;
using Acme.Trading.Domain;
using Acme.Trading.Runtime.Lean;

public class SmaCrossLeanAlgo : LeanAlgorithmBase
{
    private Symbol _symbol;
    private SimpleMovingAverage _fast;
    private SimpleMovingAverage _slow;

    // UI 据此静态成员渲染参数输入框（静态约定，免实例化）。无参数则返回空。
    public static IReadOnlyList<StrategyParameterDef> ParameterDefinitions => new[]
    {
        new StrategyParameterDef("FastPeriod", "快线周期", "5"),
        new StrategyParameterDef("SlowPeriod", "慢线周期", "20"),
    };

    public override void Initialize()
    {
        // 从 Config 读 start/end/cash 并设置（runner 透传）。用户参数直接 GetParameter("FastPeriod")。
        ConfigureFromConfig();

        int fast = GetParameter("FastPeriod") is { Length: > 0 } f && int.TryParse(f, out var fv) ? fv : 5;
        int slow = GetParameter("SlowPeriod") is { Length: > 0 } s && int.TryParse(s, out var sv) ? sv : 20;

        // 必须用 AddCtpData 订阅本地 CtpBar（回测读预取 CSV）。写 AddSecurity 拿不到本地数据。
        _symbol = AddCtpData(GetParameter("symbol") ?? "600000", Resolution.Daily);
        _fast = SMA(_symbol, fast, Resolution.Daily);
        _slow = SMA(_symbol, slow, Resolution.Daily);
        SetBenchmark(_symbol);
    }

    public override void OnData(Slice slice)
    {
        if (!(_fast.IsReady && _slow.IsReady)) return;
        if (_fast > _slow && !Portfolio.Invested)
            SetHoldings(_symbol, 1.0m);          // Lean 原生仓位 API——可自由 sizing（区别于 IStrategy 壳的 1/N 均分）
        else if (_fast < _slow && Portfolio.Invested)
            Liquidate(_symbol);
    }
}
