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

            SymbolName = symbol.Name;

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

        public void Export(int signalsNumberToExport)
        {
            string doucmentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string symbolSignalsDirecotryPath = Path.Combine(doucmentsDirectoryPath, "cAlgo", AlgoName, SymbolName);

            if (!Directory.Exists(symbolSignalsDirecotryPath))
            {
                Directory.CreateDirectory(symbolSignalsDirecotryPath);
            }

            string filePath = Path.Combine(symbolSignalsDirecotryPath, string.Format("{0}.xml", _timeFrame));

            Export(filePath, signalsNumberToExport);
        }

        public void Export(string filePath, int signalsNumberToExport)
        {
            SignalContainer signalContainer = new SignalContainer(AlgoName, _symbol, _timeFrame, _newSignalSettings, _signalStatsSettings)
            {
                Signals = signalsNumberToExport > 0 ? Signals.Skip(Signals.Count - signalsNumberToExport).ToList() : Signals,
            };

            using (FileStream fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SignalContainer));

                serializer.Serialize(fileStream, signalContainer);
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

            RemoveSignal(index, tradeType);

            Signal signal = new Signal
            {
                Index = index,
                Time = _newSignalSettings.MarketSeries.OpenTime[index],
                Symbol = _symbol,
                TradeType = tradeType,
                Comment = comment
            };

            Signals.Add(signal);

            _newSignalSettings.AlertCallback?.Invoke(index, tradeType);

            if (tradeType == TradeType.Buy && _newSignalSettings.BuySignal != null)
            {
                _newSignalSettings.BuySignal[index] = _newSignalSettings.MarketSeries.Low[index] - _newSignalSettings.SignalDistance;
            }
            else if (tradeType == TradeType.Sell && _newSignalSettings.SellSignal != null)
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

            foreach (Signal signal in Signals)
            {
                if (signal.Exited)
                {
                    continue;
                }

                signal.Exited = _signalStatsSettings.ExitFunction(signal);

                if (signal.Exited)
                {
                    signal.ExitBarIndex = _signalStatsSettings.MarketSeries.GetIndex() - 1;
                }

                signal.MaxUpMove = _signalStatsSettings.MarketSeries.High.Maximum(signal.Index, index) - _signalStatsSettings.MarketSeries.Close[signal.Index];
                signal.MaxDownMove = _signalStatsSettings.MarketSeries.Close[signal.Index] - _signalStatsSettings.MarketSeries.Low.Minimum(signal.Index, index);

                _signalStatsSettings.ProfitableSignal[signal.Index] = double.NaN;
                _signalStatsSettings.LosingSignal[signal.Index] = double.NaN;

                if (signal.TradeType == TradeType.Buy)
                {
                    if (_signalStatsSettings.MarketSeries.Close[index] > _signalStatsSettings.MarketSeries.Close[signal.Index])
                    {
                        _signalStatsSettings.ProfitableSignal[signal.Index] = _newSignalSettings.BuySignal[signal.Index] - _signalStatsSettings.SignalDistance;
                    }
                    else
                    {
                        _signalStatsSettings.LosingSignal[signal.Index] = _newSignalSettings.BuySignal[signal.Index] - _signalStatsSettings.SignalDistance;
                    }
                }
                else
                {
                    if (_signalStatsSettings.MarketSeries.Close[index] < _signalStatsSettings.MarketSeries.Close[signal.Index])
                    {
                        _signalStatsSettings.ProfitableSignal[signal.Index] = _newSignalSettings.SellSignal[signal.Index] + _signalStatsSettings.SignalDistance;
                    }
                    else
                    {
                        _signalStatsSettings.LosingSignal[signal.Index] = _newSignalSettings.SellSignal[signal.Index] + _signalStatsSettings.SignalDistance;
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

            IEnumerable<Signal> profitableSignals = Signals.Where(iSignal => IsProfitable(iSignal, index));

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

        public void RemoveSignal(int index, TradeType tradeType)
        {
            Signal signal = Signals.FirstOrDefault(iSignal => iSignal.Index == index && iSignal.TradeType == tradeType);

            if (signal != null)
            {
                Signals.Remove(signal);
            }
        }

        public void LoadSignals()
        {
            string doucmentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string symbolSignalsDirecotryPath = Path.Combine(doucmentsDirectoryPath, "cAlgo", AlgoName, _symbol.Name);

            if (!Directory.Exists(symbolSignalsDirecotryPath))
            {
                throw new DirectoryNotFoundException(string.Format("Couldn't find the symbol signal files directory at: {0}", symbolSignalsDirecotryPath));
            }

            string filePath = Path.Combine(symbolSignalsDirecotryPath, string.Format("{0}.xml", _timeFrame));

            LoadSignals(filePath);
        }

        public void LoadSignals(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("The signals file doesn't exist at: {0}", filePath));
            }

            using (FileStream fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                XmlSerializer serializer = new XmlSerializer(this.GetType());

                List<Signal> signals = (serializer.Deserialize(fileStream) as SignalContainer).Signals;

                Signals.AddRange(signals);
            }
        }

        private double CalculateGain(Signal signal, int index)
        {
            int exitBarIndex = signal.ExitBarIndex > index ? index : signal.ExitBarIndex;

            return signal.TradeType == TradeType.Buy ? _signalStatsSettings.MarketSeries.Close[exitBarIndex] - _signalStatsSettings.MarketSeries.Close[signal.Index] : _signalStatsSettings.MarketSeries.Close[signal.Index] - _signalStatsSettings.MarketSeries.Close[exitBarIndex];
        }

        private double CalculateLoss(Signal signal, int index)
        {
            int exitBarIndex = signal.ExitBarIndex > index ? index : signal.ExitBarIndex;

            return signal.TradeType == TradeType.Buy ? _signalStatsSettings.MarketSeries.Close[signal.Index] - _signalStatsSettings.MarketSeries.Close[exitBarIndex] : _signalStatsSettings.MarketSeries.Close[exitBarIndex] - _signalStatsSettings.MarketSeries.Close[signal.Index];
        }

        private bool IsProfitable(Signal signal, int index)
        {
            int exitBarIndex = signal.ExitBarIndex > index ? index : signal.ExitBarIndex;

            return (signal.TradeType == TradeType.Buy && _signalStatsSettings.MarketSeries.Close[exitBarIndex] > _signalStatsSettings.MarketSeries.Close[signal.Index]) || (signal.TradeType == TradeType.Sell && _signalStatsSettings.MarketSeries.Close[exitBarIndex] < _signalStatsSettings.MarketSeries.Close[signal.Index]);
        }

        #endregion Methods
    }
}