using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions.Utility
{
    /// <summary>
    /// Counts the TD sequential bars
    /// </summary>
    public class TdBarCounter
    {
        #region Fields

        private readonly DataSeries _source;

        private readonly Bars _bars;

        #endregion Fields

        public TdBarCounter(DataSeries source, Bars bars)
        {
            _source = source;

            _bars = bars;

            SequentialBars = new SortedSet<TdBar>();

            CountdownBars = new SortedSet<TdBar>();

            ReversalSetups = new SortedSet<TdReversalSetup>();
        }

        #region Properties

        public int MaxSequentialBarsNumber { get; set; } = 9;

        public int MaxCountdownBarsNumber { get; set; } = 13;

        public int Period { get; set; } = 4;

        public bool PriceFlip { get; set; }

        public SortedSet<TdBar> SequentialBars { get; }

        public SortedSet<TdBar> CountdownBars { get; }

        public SortedSet<TdReversalSetup> ReversalSetups { get; }

        public TdBar LastSequentialBar { get; private set; }

        public int AlertOnCountdownBarNumber { get; set; } = -1;

        public int AlertOnSequentialBarNumber { get; set; } = -1;

        public Action<int, TradeType, bool> TriggerAlertAction { get; set; }

        public Action<TdReversalSetup> PlotPerfectReversalSetupAction { get; set; }

        public Action<int, TdPriceFlipType> PlotPriceFlipAction { get; set; }

        public Action<TdBar> PlotSequentialBarNumberAction { get; set; }

        public Action<TdBar> PlotCountdownBarNumberAction { get; set; }

        public Action<string> Print { get; set; }

        #endregion Properties

        #region Methods

        public TdPriceFlipType GetPriceFlipType(int index)
        {
            if (_source[index - 1] < _source[index - (Period + 1)] && _source[index] > _source[index - Period])
            {
                return TdPriceFlipType.Bullish;
            }
            else if (_source[index - 1] > _source[index - (Period + 1)] && _source[index] < _source[index - Period])
            {
                return TdPriceFlipType.Bearish;
            }

            return TdPriceFlipType.None;
        }

        public void Calculate(int index)
        {
            // Sets last sequential bar to Null if the setup interrupted
            if (LastSequentialBar != null)
            {
                if (index > LastSequentialBar.Index)
                {
                    SequentialBars.Add(LastSequentialBar);
                }

                CancelCountIfInvalidated(index);
            }

            var priceFlipType = GetPriceFlipType(index);

            PlotPriceFlipAction?.Invoke(index, priceFlipType);

            // Start new counting
            if (LastSequentialBar == null)
            {
                StartNewSequentialCount(index, priceFlipType);
            }
            // Continue count
            else
            {
                var continueResult = ContinueSequentialCount(index);

                if (!continueResult)
                {
                    StartNewSequentialCount(index, priceFlipType);

                    var reversalSetup = new TdReversalSetup
                    {
                        Type = LastSequentialBar.Type == BarType.Bullish ? TdReversalSetupType.Sell : TdReversalSetupType.Buy,
                        FirstSequentialBarIndex = LastSequentialBar.Index - MaxSequentialBarsNumber,
                        LastSequentialBarIndex = LastSequentialBar.Index
                    };

                    if (IsReversalSetupPerfect(reversalSetup))
                    {
                        PlotPerfectReversalSetupAction?.Invoke(reversalSetup);
                    }

                    ReversalSetups.Add(reversalSetup);
                }
            }

            if (LastSequentialBar != null)
            {
                PlotSequentialBarNumberAction?.Invoke(LastSequentialBar);
            }

            ContinueCountdownCount(index - 1);
        }

        private void CancelCountIfInvalidated(int index)
        {
            if (LastSequentialBar.Type == BarType.Bullish && _source[index] <= _source[index - Period])
            {
                LastSequentialBar = null;
            }
            else if (LastSequentialBar.Type == BarType.Bearish && _source[index] >= _source[index - Period])
            {
                LastSequentialBar = null;
            }
        }

        private void StartNewSequentialCount(int index, TdPriceFlipType priceFlipType)
        {
            if (PriceFlip && priceFlipType != TdPriceFlipType.None)
            {
                LastSequentialBar = new TdBar
                {
                    Type = priceFlipType == TdPriceFlipType.Bullish ? BarType.Bullish : BarType.Bearish
                };
            }
            else if (!PriceFlip)
            {
                if (_source[index] > _source[index - Period])
                {
                    LastSequentialBar = new TdBar
                    {
                        Type = BarType.Bullish
                    };
                }
                else if (_source[index] < _source[index - Period])
                {
                    LastSequentialBar = new TdBar
                    {
                        Type = BarType.Bearish
                    };
                }
            }

            if (LastSequentialBar != null)
            {
                LastSequentialBar.Index = index;
                LastSequentialBar.Number = 1;
            }
        }

        private bool ContinueSequentialCount(int index)
        {
            var result = true;

            if (index == LastSequentialBar.Index)
            {
                return result;
            }

            // Setup completed
            if (LastSequentialBar.Number >= MaxSequentialBarsNumber)
            {
                result = false;
            }
            // Not completed yet
            else
            {
                LastSequentialBar.Index = index;
                LastSequentialBar.Number += 1;
            }

            if (LastSequentialBar.Number == AlertOnSequentialBarNumber)
            {
                TriggerAlertAction?.Invoke(index, LastSequentialBar.Type == BarType.Bullish ? TradeType.Sell : TradeType.Buy, false);
            }

            return result;
        }

        private void ContinueCountdownCount(int index)
        {
            var setupsCopy = ReversalSetups.ToList();

            foreach (var setup in setupsCopy)
            {
                var lastCountdownBarNumber = setup.CountdownBarNumber;

                if (setup.Type == TdReversalSetupType.Buy && _source[index] <= _bars.LowPrices[index - 1] && _source[index] <= _bars.LowPrices[index - 2])
                {
                    if (setup.CountdownBarNumber == MaxCountdownBarsNumber - 1 && _bars.LowPrices[index] > _source[setup.EighthCountdownBarIndex])
                    {
                        continue;
                    }

                    setup.CountdownBarNumber += 1;
                }
                else if (setup.Type == TdReversalSetupType.Sell && _source[index] >= _bars.HighPrices[index - 1] && _source[index] >= _bars.HighPrices[index - 2])
                {
                    if (setup.CountdownBarNumber == MaxCountdownBarsNumber - 1 && _bars.HighPrices[index] < _source[setup.EighthCountdownBarIndex])
                    {
                        continue;
                    }

                    setup.CountdownBarNumber += 1;
                }

                if (lastCountdownBarNumber == setup.CountdownBarNumber)
                {
                    continue;
                }

                var bar = new TdBar
                {
                    Index = index,
                    Number = setup.CountdownBarNumber,
                    Type = setup.Type == TdReversalSetupType.Buy ? BarType.Bearish : BarType.Bullish
                };

                CountdownBars.Add(bar);

                PlotCountdownBarNumberAction?.Invoke(bar);

                if (setup.CountdownBarNumber == 1)
                {
                    setup.FirstCountdownBarIndex = index;
                }
                else if (setup.CountdownBarNumber == 8)
                {
                    setup.EighthCountdownBarIndex = index;
                }

                if (setup.CountdownBarNumber == MaxCountdownBarsNumber)
                {
                    setup.LastCountdownBarIndex = MaxCountdownBarsNumber;

                    ReversalSetups.Remove(setup);
                }

                if (setup.CountdownBarNumber == AlertOnCountdownBarNumber)
                {
                    TriggerAlertAction?.Invoke(index, setup.Type == TdReversalSetupType.Buy ? TradeType.Buy : TradeType.Sell, true);
                }
            }
        }

        private bool IsReversalSetupPerfect(TdReversalSetup setup)
        {
            if (setup.Type == TdReversalSetupType.Buy)
            {
                var lastBarLow = _bars.LowPrices[setup.LastSequentialBarIndex];
                var previousBarLow = _bars.LowPrices[setup.LastSequentialBarIndex - 1];
                var sixthBarLow = _bars.LowPrices[setup.FirstSequentialBarIndex + 5];
                var seventhBarLow = _bars.LowPrices[setup.FirstSequentialBarIndex + 6];

                if (lastBarLow <= sixthBarLow && lastBarLow <= seventhBarLow || previousBarLow <= sixthBarLow && previousBarLow <= seventhBarLow)
                {
                    return true;
                }
            }
            else
            {
                var lastBarHigh = _bars.HighPrices[setup.LastSequentialBarIndex];
                var previousBarHigh = _bars.HighPrices[setup.LastSequentialBarIndex - 1];
                var sixthBarHigh = _bars.HighPrices[setup.FirstSequentialBarIndex + 5];
                var seventhBarHigh = _bars.HighPrices[setup.FirstSequentialBarIndex + 6];

                if (lastBarHigh >= sixthBarHigh && lastBarHigh >= seventhBarHigh || previousBarHigh >= sixthBarHigh && previousBarHigh >= seventhBarHigh)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Methods
    }
}