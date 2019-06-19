using cAlgo.API.Internals;
using Stats.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace cAlgo.API.Extensions.Models
{
    public class SignalContainer
    {
        #region Fields

        private readonly NewSignalSettings _newSignalSettings;

        private readonly SignalStatsSettings _signalStatsSettings;

        private readonly Symbol _symbol;

        private readonly TimeFrame _timeFrame;

        #endregion Fields

        public SignalContainer()
        {
        }

        public SignalContainer(string algoName, Symbol symbol, TimeFrame timeFrame) : this(algoName, symbol, timeFrame, null)
        {
        }

        public SignalContainer(string algoName, Symbol symbol, TimeFrame timeFrame, NewSignalSettings newSignalSettings) : this(algoName, symbol, timeFrame, newSignalSettings, null)
        {
        }

        public SignalContainer(string algoName, Symbol symbol, TimeFrame timeFrame, NewSignalSettings newSignalSettings,
            SignalStatsSettings signalStatsSettings)
        {
            Signals = new List<Signal>();

            AlgoName = algoName;

            _symbol = symbol;

            SymbolName = symbol.Code;

            _timeFrame = timeFrame;

            _newSignalSettings = newSignalSettings;

            _signalStatsSettings = signalStatsSettings;
        }

        #region Properties

        public List<Signal> Signals { get; set; }

        public string AlgoName { get; set; }

        public string SymbolName { get; set; }

        #endregion Properties

        #region Methods

        public void Export()
        {
            string doucmentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string symbolSignalsDirecotryPath = Path.Combine(doucmentsDirectoryPath, "cAlgo", AlgoName, SymbolName);

            if (!Directory.Exists(symbolSignalsDirecotryPath))
            {
                Directory.CreateDirectory(symbolSignalsDirecotryPath);
            }

            string filePath = Path.Combine(symbolSignalsDirecotryPath, string.Format("{0}.xml", _timeFrame));

            Export(filePath);
        }

        public void Export(string filePath)
        {
            using (FileStream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SignalContainer));

                serializer.Serialize(fileStream, this);
            }
        }

        public void AddSignal(int index, TradeType tradeType)
        {
            AddSignal(index, tradeType, string.Empty);
        }

        public void AddSignal(int index, TradeType tradeType, string comment)
        {
            if (_newSignalSettings == null)
            {
                throw new NullReferenceException("The signal container NewSignalSettings object is null");
            }

            Signal signal = new Signal
            {
                Index = index,
                Time = _newSignalSettings.MarketSeries.OpenTime[index],
                Symbol = _symbol,
                TradeType = tradeType,
                Comment = comment
            };

            Signals.Add(signal);

            if (_newSignalSettings.AlertCallback != null)
            {
                _newSignalSettings.AlertCallback(index, tradeType);
            }

            if (tradeType == TradeType.Buy)
            {
                _newSignalSettings.BuySignal[index] = _newSignalSettings.MarketSeries.Low[index] - _newSignalSettings.SignalDistance;
            }
            else
            {
                _newSignalSettings.SellSignal[index] = _newSignalSettings.MarketSeries.High[index] + _newSignalSettings.SignalDistance;
            }
        }

        public void CalculateStats(int index)
        {
            if (_newSignalSettings == null)
            {
                throw new NullReferenceException("The signal container NewSignalSettings object is null");
            }
            else if (_signalStatsSettings == null)
            {
                throw new NullReferenceException("The signal container SignalStatsSettings object is null");
            }

            if (_signalStatsSettings.MarketSeries.GetIndex() == index)
            {
                index--;
            }

            int startIndex = index - _signalStatsSettings.MaxLookupBarsNumber;

            for (int iBarIndex = startIndex; iBarIndex < index; iBarIndex++)
            {
                int nextBarIndex = iBarIndex + 1;

                double moveUp = _signalStatsSettings.MarketSeries.High.Maximum(nextBarIndex, index) - _signalStatsSettings.MarketSeries.Close[iBarIndex];
                double moveDown = _signalStatsSettings.MarketSeries.Close[iBarIndex] - _signalStatsSettings.MarketSeries.Low.Minimum(nextBarIndex, index);

                Signal signal = Signals.FirstOrDefault(iSignal => iSignal.Index == iBarIndex);

                if (!double.IsNaN(_newSignalSettings.BuySignal[iBarIndex]))
                {
                    _signalStatsSettings.ProfitableSignal[iBarIndex] = double.NaN;
                    _signalStatsSettings.LosingSignal[iBarIndex] = double.NaN;

                    if (signal != null)
                    {
                        signal.MaxUpMove = moveUp;
                        signal.MaxDownMove = moveDown;
                    }

                    if ((_signalStatsSettings.IsCloseBased && _signalStatsSettings.MarketSeries.Close[index] > _signalStatsSettings.MarketSeries.Close[iBarIndex]) || (!_signalStatsSettings.IsCloseBased && signal.RewardRiskRatio >= _signalStatsSettings.MinRewardRiskRatio))
                    {
                        _signalStatsSettings.ProfitableSignal[iBarIndex] = _newSignalSettings.BuySignal[iBarIndex] - _signalStatsSettings.SignalDistance;
                    }
                    else
                    {
                        _signalStatsSettings.LosingSignal[iBarIndex] = _newSignalSettings.BuySignal[iBarIndex] - _signalStatsSettings.SignalDistance;
                    }
                }
                else if (!double.IsNaN(_newSignalSettings.SellSignal[iBarIndex]))
                {
                    _signalStatsSettings.ProfitableSignal[iBarIndex] = double.NaN;
                    _signalStatsSettings.LosingSignal[iBarIndex] = double.NaN;

                    if (signal != null)
                    {
                        signal.MaxUpMove = moveUp;
                        signal.MaxDownMove = moveDown;
                    }

                    if ((_signalStatsSettings.IsCloseBased && _signalStatsSettings.MarketSeries.Close[index] < _signalStatsSettings.MarketSeries.Close[iBarIndex]) || (!_signalStatsSettings.IsCloseBased && signal.RewardRiskRatio >= _signalStatsSettings.MinRewardRiskRatio))
                    {
                        _signalStatsSettings.ProfitableSignal[iBarIndex] = _newSignalSettings.SellSignal[iBarIndex] + _signalStatsSettings.SignalDistance;
                    }
                    else
                    {
                        _signalStatsSettings.LosingSignal[iBarIndex] = _newSignalSettings.SellSignal[iBarIndex] + _signalStatsSettings.SignalDistance;
                    }
                }
            }
        }

        public string GetStatsDisplayText(int index)
        {
            return GetStatsDisplayText(index, string.Format("{0} Signals Stats", AlgoName));
        }

        public string GetStatsDisplayText(int index, string title)
        {
            if (_newSignalSettings == null)
            {
                throw new NullReferenceException("The signal container NewSignalSettings object is null");
            }
            else if (_signalStatsSettings == null)
            {
                throw new NullReferenceException("The signal container SignalStatsSettings object is null");
            }

            if (!Signals.Any())
            {
                return string.Empty;
            }

            IEnumerable<Signal> profitableSignals;

            if (_signalStatsSettings.IsCloseBased)
            {
                profitableSignals = Signals.Where(iSignal => IsProfitable(iSignal, index));
            }
            else
            {
                profitableSignals = Signals.Where(iSignal => iSignal.RewardRiskRatio >= _signalStatsSettings.MinRewardRiskRatio);
            }

            IEnumerable<Signal> losingSignals = Signals.Where(iSignal => !profitableSignals.Contains(iSignal));

            StringBuilder stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(title))
            {
                stringBuilder.AppendLine(title);

                stringBuilder.AppendLine();
            }

            // Strong signals statistics
            double strongSignalsAccuracy = (profitableSignals.Count() / (double)Signals.Count) * 100;

            double strongProfitableSignalsMedianRiskRewardRatio = profitableSignals.Select(iSignal => iSignal.RewardRiskRatio).Median();
            double strongLosingSignalsMedianRiskRewardRatio = losingSignals.Select(iSignal => iSignal.RewardRiskRatio).Median();

            double strongProfitableSignalsMedianLossInPips = _symbol.ToPips(profitableSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxDownMove : iSignal.MaxUpMove).Median());
            double strongProfitableSignalsMedianGainInPips = _symbol.ToPips(profitableSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxUpMove : iSignal.MaxDownMove).Median());

            double strongLosingSignalsMedianLossInPips = _symbol.ToPips(losingSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxDownMove : iSignal.MaxUpMove).Median());
            double strongLosingSignalsMedianGainInPips = _symbol.ToPips(losingSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxUpMove : iSignal.MaxDownMove).Median());

            double strongAllSignalsMedianLossInPips = _symbol.ToPips(Signals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxDownMove : iSignal.MaxUpMove).Median());
            double strongAllSignalsMedianGainInPips = _symbol.ToPips(Signals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxUpMove : iSignal.MaxDownMove).Median());

            double profitableSignalsTotalGainInPips = _symbol.ToPips(profitableSignals.Select(iSignal => CalculateGain(iSignal, index)).Sum());
            double losingSignalsGainInPips = _symbol.ToPips(losingSignals.Select(iSignal => CalculateLoss(iSignal, index)).Sum());

            stringBuilder.AppendLine(string.Format("Signals #: {0}", Signals.Count));
            stringBuilder.AppendLine(string.Format("Signals Accuracy: {0}%", Math.Round(strongSignalsAccuracy, 2)));
            stringBuilder.AppendLine(string.Format("Profitable Signals Median Reward:Risk: {0}", Math.Round(strongProfitableSignalsMedianRiskRewardRatio, 2)));
            stringBuilder.AppendLine(string.Format("Losing Signals Median Reward:Risk: {0}", Math.Round(strongLosingSignalsMedianRiskRewardRatio, 2)));
            stringBuilder.AppendLine(string.Format("Profitable Signals Median Loss (Pips): {0}", Math.Round(strongProfitableSignalsMedianLossInPips, 2)));
            stringBuilder.AppendLine(string.Format("Profitable Signals Median Gain (Pips): {0}", Math.Round(strongProfitableSignalsMedianGainInPips, 2)));
            stringBuilder.AppendLine(string.Format("Losing Signals Median Loss (Pips): {0}", Math.Round(strongLosingSignalsMedianLossInPips, 2)));
            stringBuilder.AppendLine(string.Format("Losing Signals Median Gain (Pips): {0}", Math.Round(strongLosingSignalsMedianGainInPips, 2)));
            stringBuilder.AppendLine(string.Format("All Signals Median Loss (Pips): {0}", Math.Round(strongAllSignalsMedianLossInPips, 2)));
            stringBuilder.AppendLine(string.Format("All Signals Median Gain (Pips): {0}", Math.Round(strongAllSignalsMedianGainInPips, 2)));
            stringBuilder.AppendLine(string.Format("Signals Total Gain (Pips): {0}", Math.Round(profitableSignalsTotalGainInPips - losingSignalsGainInPips, 2)));

            stringBuilder.AppendLine();

            return stringBuilder.ToString();
        }

        private double CalculateGain(Signal signal, int index)
        {
            int enbBarIndex = signal.Index + _signalStatsSettings.MaxLookupBarsNumber > index ? index : signal.Index + _signalStatsSettings.MaxLookupBarsNumber;

            return signal.TradeType == TradeType.Buy ? _signalStatsSettings.MarketSeries.Close[enbBarIndex] - _signalStatsSettings.MarketSeries.Close[signal.Index] : _signalStatsSettings.MarketSeries.Close[signal.Index] - _signalStatsSettings.MarketSeries.Close[enbBarIndex];
        }

        private double CalculateLoss(Signal signal, int index)
        {
            int enbBarIndex = signal.Index + _signalStatsSettings.MaxLookupBarsNumber > index ? index : signal.Index + _signalStatsSettings.MaxLookupBarsNumber;

            return signal.TradeType == TradeType.Buy ? _signalStatsSettings.MarketSeries.Close[signal.Index] - _signalStatsSettings.MarketSeries.Close[enbBarIndex] : _signalStatsSettings.MarketSeries.Close[enbBarIndex] - _signalStatsSettings.MarketSeries.Close[signal.Index];
        }

        private bool IsProfitable(Signal signal, int index)
        {
            int enbBarIndex = signal.Index + _signalStatsSettings.MaxLookupBarsNumber > index ? index : signal.Index + _signalStatsSettings.MaxLookupBarsNumber;

            return (signal.TradeType == TradeType.Buy && _signalStatsSettings.MarketSeries.Close[enbBarIndex] > _signalStatsSettings.MarketSeries.Close[signal.Index]) || (signal.TradeType == TradeType.Sell && _signalStatsSettings.MarketSeries.Close[enbBarIndex] < _signalStatsSettings.MarketSeries.Close[signal.Index]);
        }

        #endregion Methods
    }
}