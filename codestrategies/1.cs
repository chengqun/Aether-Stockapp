using System;
using System.Collections.Generic;
using Acme.Trading.Domain;
using Acme.Trading.Domain.Indicators;

public class SmaCrossStrategy : IStrategy
{
    private IIndicator _fast = null!;
    private IIndicator _slow = null!;

    public int WarmUpBars { get; private set; }

    // 策略参数定义：UI 据此动态渲染输入框。默认空；策略可选覆盖。
    public IReadOnlyList<StrategyParameterDef> ParameterDefinitions => new[]
    {
        new StrategyParameterDef("FastPeriod", "快线周期", "5"),
        new StrategyParameterDef("SlowPeriod", "慢线周期", "20"),
    };

    public void Initialize(StrategyParameters parameters)
    {
        int fast = parameters.Get("FastPeriod", 5);
        int slow = parameters.Get("SlowPeriod", 20);
        // 复用与 YAML 策略同一套增量指标库（IndicatorFactory）——两形底层同源，对照回测结果一致。
        _fast = IndicatorFactory.Create("sma", new Dictionary<string, string> { ["period"] = fast.ToString() }, parameters);
        _slow = IndicatorFactory.Create("sma", new Dictionary<string, string> { ["period"] = slow.ToString() }, parameters);
        WarmUpBars = Math.Max(_fast.RequiredBars, _slow.RequiredBars);
    }

    public SignalDirection EvaluateSignal(MarketBar bar)
    {
        _fast.Update(bar);
        _slow.Update(bar);
        if (!_fast.IsReady || !_slow.IsReady) return SignalDirection.Hold;
        // 快线在慢线上方→做多，否则清仓（均线带策略；穿越信号可自行加 prev 比较）
        return _fast.Value > _slow.Value ? SignalDirection.Long : SignalDirection.Flat;
    }
}
