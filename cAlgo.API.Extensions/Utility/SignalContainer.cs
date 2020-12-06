using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace cAlgo.API.Extensions.Utility
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

        public void Export()
        {
            string filePath;

            if (string.IsNullOrEmpty(_newSignalSettings.ExportFilePath))
            {
                var doucmentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var symbolSignalsDirecotryPath = Path.Combine(doucmentsDirectoryPath, "cAlgo", AlgoName, SymbolName);

                if (!Directory.Exists(symbolSignalsDirecotryPath))
                {
                    Directory.CreateDirectory(symbolSignalsDirecotryPath);
                }

                filePath = Path.Combine(symbolSignalsDirecotryPath, string.Format("{0}.xml", _timeFrame));
            }
            else
            {
                filePath = _newSignalSettings.ExportFilePath;
            }

            var signalContainer = new SignalContainer(AlgoName, _symbol, _timeFrame, _newSignalSettings, _signalStatsSettings)
            {
                Signals = _newSignalSettings.SignalsNumberToExport > 0 ? Signals.Skip(Signals.Count - _newSignalSettings.SignalsNumberToExport).ToList() : Signals,
            };

            using (var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(typeof(SignalContainer));

                serializer.Serialize(fileStream, signalContainer);
            }
        }

        public Signal AddSignal(int index, TradeType tradeType)
        {
            return AddSignal(index, tradeType, string.Empty);
        }

        public Signal AddSignal(int index, TradeType tradeType, string comment)
        {
            return AddSignal(index, tradeType, null, null, comment);
        }

        public Signal AddSignal(int index, TradeType tradeType, double? stopLoss, double? takeProfit)
        {
            return AddSignal(index, tradeType, stopLoss, takeProfit, string.Empty);
        }

        public Signal AddSignal(int index, TradeType tradeType, double? stopLoss, double? takeProfit, string comment)
        {
            if (_newSignalSettings == null)
            {
                throw new NullReferenceException("The signal container NewSignalSettings object is null");
            }

            RemoveSignal(index, tradeType);

            var signal = new Signal
            {
                Index = index,
                Time = _newSignalSettings.Bars.OpenTimes[index],
                Symbol = _symbol,
                TradeType = tradeType,
                StopLoss = stopLoss,
                TakeProfit = takeProfit,
                Comment = comment
            };

            Signals.Add(signal);

            _newSignalSettings.AlertCallback?.Invoke(index, tradeType);

            if (tradeType == TradeType.Buy && _newSignalSettings.BuyEntry != null)
            {
                _newSignalSettings.BuyEntry[index] = _newSignalSettings.Bars.LowPrices[index] - _newSignalSettings.SignalDistance;
            }
            else if (tradeType == TradeType.Sell && _newSignalSettings.SellEntry != null)
            {
                _newSignalSettings.SellEntry[index] = _newSignalSettings.Bars.HighPrices[index] + _newSignalSettings.SignalDistance;
            }

            if (_newSignalSettings.IsExportEnabled)
            {
                Export();
            }

            return signal;
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

            if (_signalStatsSettings.Bars.GetIndex() == index)
            {
                index--;
            }

            foreach (var signal in Signals)
            {
                if (signal.Exited)
                {
                    continue;
                }

                if (signal.StopLoss.HasValue && (signal.TradeType == TradeType.Buy && _signalStatsSettings.Bars.LowPrices[index] <= signal.StopLoss || signal.TradeType == TradeType.Sell && _signalStatsSettings.Bars.HighPrices[index] >= signal.StopLoss))
                {
                    signal.Exited = true;
                }

                if (signal.TakeProfit.HasValue && (signal.TradeType == TradeType.Buy && _signalStatsSettings.Bars.HighPrices[index] >= signal.TakeProfit || signal.TradeType == TradeType.Sell && _signalStatsSettings.Bars.LowPrices[index] <= signal.TakeProfit))
                {
                    signal.Exited = true;
                }

                if (!signal.Exited && _signalStatsSettings.ExitFunction != null)
                {
                    signal.Exited = _signalStatsSettings.ExitFunction(signal);
                }

                if (signal.Exited)
                {
                    signal.ExitIndex = index;

                    signal.HoldingTime = _signalStatsSettings.Bars.OpenTimes[signal.ExitIndex] - _signalStatsSettings.Bars.OpenTimes[signal.Index];

                    if (_signalStatsSettings.ShowExits)
                    {
                        var lineObjectName = string.Format("ExitLine_{0}_{1}_{2}", signal.Index, signal.ExitIndex, _signalStatsSettings.ChartObjectNamesSuffix);

                        var y1 = _signalStatsSettings.Bars.ClosePrices[signal.Index];
                        var y2 = _signalStatsSettings.Bars.ClosePrices[signal.ExitIndex];

                        var lineColor = signal.TradeType == TradeType.Buy ? _signalStatsSettings.BuySignalExitLineColor : _signalStatsSettings.SellSignalExitLineColor;

                        _signalStatsSettings.Chart.DrawTrendLine(lineObjectName, signal.Index, y1, signal.ExitIndex, y2, lineColor);

                        if (signal.TradeType == TradeType.Buy && _signalStatsSettings.BuyExit != null)
                        {
                            _signalStatsSettings.BuyExit[index] = _signalStatsSettings.Bars.HighPrices[index] + _newSignalSettings.SignalDistance;
                        }
                        else if (signal.TradeType == TradeType.Sell && _signalStatsSettings.SellExit != null)
                        {
                            _signalStatsSettings.SellExit[index] = _signalStatsSettings.Bars.LowPrices[index] - _newSignalSettings.SignalDistance;
                        }
                    }
                }

                signal.MaxUpMove = _signalStatsSettings.Bars.HighPrices.Maximum(signal.Index, index) - _signalStatsSettings.Bars.ClosePrices[signal.Index];
                signal.MaxDownMove = _signalStatsSettings.Bars.ClosePrices[signal.Index] - _signalStatsSettings.Bars.LowPrices.Minimum(signal.Index, index);

                _signalStatsSettings.ProfitableSignal[signal.Index] = double.NaN;
                _signalStatsSettings.LosingSignal[signal.Index] = double.NaN;

                if (signal.TradeType == TradeType.Buy)
                {
                    signal.ProfitInPips = _symbol.ToPips(_signalStatsSettings.Bars.ClosePrices[index] - _signalStatsSettings.Bars.ClosePrices[signal.Index]);

                    if (signal.ProfitInPips > 0)
                    {
                        _signalStatsSettings.ProfitableSignal[signal.Index] = _newSignalSettings.BuyEntry[signal.Index] - _signalStatsSettings.SignalDistance;
                    }
                    else
                    {
                        _signalStatsSettings.LosingSignal[signal.Index] = _newSignalSettings.BuyEntry[signal.Index] - _signalStatsSettings.SignalDistance;
                    }
                }
                else
                {
                    signal.ProfitInPips = _symbol.ToPips(_signalStatsSettings.Bars.ClosePrices[signal.Index] - _signalStatsSettings.Bars.ClosePrices[index]);

                    if (signal.ProfitInPips > 0)
                    {
                        _signalStatsSettings.ProfitableSignal[signal.Index] = _newSignalSettings.SellEntry[signal.Index] + _signalStatsSettings.SignalDistance;
                    }
                    else
                    {
                        _signalStatsSettings.LosingSignal[signal.Index] = _newSignalSettings.SellEntry[signal.Index] + _signalStatsSettings.SignalDistance;
                    }
                }
            }
        }

        public void DisplayText(int index)
        {
            DisplayText(index, string.Format("{0} Signals Stats", AlgoName));
        }

        public void DisplayText(int index, string title)
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
                return;
            }

            var profitableSignals = Signals.Where(iSignal => IsProfitable(iSignal, index));

            var losingSignals = Signals.Where(iSignal => !profitableSignals.Contains(iSignal));

            var stringBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(title))
            {
                stringBuilder.AppendLine(title);

                stringBuilder.AppendLine();
            }

            // Strong signals statistics
            var strongSignalsAccuracy = profitableSignals.Count() / (double)Signals.Count * 100;

            var strongProfitableSignalsMedianRiskRewardRatio = profitableSignals.Select(iSignal => iSignal.RewardRiskRatio).Median();
            var strongLosingSignalsMedianRiskRewardRatio = losingSignals.Select(iSignal => iSignal.RewardRiskRatio).Median();

            var strongProfitableSignalsMedianLossInPips = _symbol.ToPips(profitableSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxDownMove : iSignal.MaxUpMove).Median());
            var strongProfitableSignalsMedianGainInPips = _symbol.ToPips(profitableSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxUpMove : iSignal.MaxDownMove).Median());

            var strongLosingSignalsMedianLossInPips = _symbol.ToPips(losingSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxDownMove : iSignal.MaxUpMove).Median());
            var strongLosingSignalsMedianGainInPips = _symbol.ToPips(losingSignals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxUpMove : iSignal.MaxDownMove).Median());

            var strongAllSignalsMedianLossInPips = _symbol.ToPips(Signals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxDownMove : iSignal.MaxUpMove).Median());
            var strongAllSignalsMedianGainInPips = _symbol.ToPips(Signals.Select(iSignal => iSignal.TradeType == TradeType.Buy ? iSignal.MaxUpMove : iSignal.MaxDownMove).Median());

            var profitableSignalsTotalGainInPips = _symbol.ToPips(profitableSignals.Select(iSignal => CalculateGain(iSignal, index)).Sum());
            var losingSignalsGainInPips = _symbol.ToPips(losingSignals.Select(iSignal => CalculateLoss(iSignal, index)).Sum());

            var proftiableSignalsMedianHoldingTimeInMinutes = profitableSignals.Select(iSignal => iSignal.HoldingTime.TotalMinutes).Median();
            var losingSignalsMedianHoldingTimeInMinutes = losingSignals.Select(iSignal => iSignal.HoldingTime.TotalMinutes).Median();
            var allSignalsMedianHoldingTimeInMinutes = Signals.Select(iSignal => iSignal.HoldingTime.TotalMinutes).Median();

            var profitableSignalsMedianHoldingTime = TimeSpan.FromMinutes(double.IsNaN(proftiableSignalsMedianHoldingTimeInMinutes) ? 0 : proftiableSignalsMedianHoldingTimeInMinutes);
            var losingSignalsMedianHoldingTime = TimeSpan.FromMinutes(double.IsNaN(losingSignalsMedianHoldingTimeInMinutes) ? 0 : losingSignalsMedianHoldingTimeInMinutes);
            var allSignalsMedianHoldingTime = TimeSpan.FromMinutes(double.IsNaN(allSignalsMedianHoldingTimeInMinutes) ? 0 : allSignalsMedianHoldingTimeInMinutes);

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
            stringBuilder.AppendLine(string.Format("Profitable Signals Median Holding Time: {0}", profitableSignalsMedianHoldingTime));
            stringBuilder.AppendLine(string.Format("Losing Signals Median Holding Time: {0}", losingSignalsMedianHoldingTime));
            stringBuilder.AppendLine(string.Format("All Signals Median Holding Time: {0}", allSignalsMedianHoldingTime));

            stringBuilder.AppendLine();

            var text = stringBuilder.ToString();

            var objectName = string.Format("Stats_{0}", _signalStatsSettings.ChartObjectNamesSuffix);

            _signalStatsSettings.Chart.DrawStaticText(objectName, text, _signalStatsSettings.StatsVerticalAlignment, _signalStatsSettings.StatsHorizontalAlignment, _signalStatsSettings.StatsColor);
        }

        public void RemoveSignal(int index, TradeType tradeType)
        {
            var signal = Signals.FirstOrDefault(iSignal => iSignal.Index == index && iSignal.TradeType == tradeType);

            if (signal != null)
            {
                Signals.Remove(signal);
            }
        }

        public void LoadSignals()
        {
            var doucmentsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var symbolSignalsDirecotryPath = Path.Combine(doucmentsDirectoryPath, "cAlgo", AlgoName, _symbol.Name);

            if (!Directory.Exists(symbolSignalsDirecotryPath))
            {
                throw new DirectoryNotFoundException(string.Format("Couldn't find the symbol signal files directory at: {0}", symbolSignalsDirecotryPath));
            }

            var filePath = Path.Combine(symbolSignalsDirecotryPath, string.Format("{0}.xml", _timeFrame));

            LoadSignals(filePath);
        }

        public void LoadSignals(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(string.Format("The signals file doesn't exist at: {0}", filePath));
            }

            using (var fileStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var serializer = new XmlSerializer(this.GetType());

                var signals = (serializer.Deserialize(fileStream) as SignalContainer).Signals;

                Signals.AddRange(signals);
            }
        }

        private double CalculateGain(Signal signal, int index)
        {
            var exitBarIndex = signal.ExitIndex > index ? index : signal.ExitIndex;

            return signal.TradeType == TradeType.Buy ? _signalStatsSettings.Bars.ClosePrices[exitBarIndex] - _signalStatsSettings.Bars.ClosePrices[signal.Index] : _signalStatsSettings.Bars.ClosePrices[signal.Index] - _signalStatsSettings.Bars.ClosePrices[exitBarIndex];
        }

        private double CalculateLoss(Signal signal, int index)
        {
            var exitBarIndex = signal.ExitIndex > index ? index : signal.ExitIndex;

            return signal.TradeType == TradeType.Buy ? _signalStatsSettings.Bars.ClosePrices[signal.Index] - _signalStatsSettings.Bars.ClosePrices[exitBarIndex] : _signalStatsSettings.Bars.ClosePrices[exitBarIndex] - _signalStatsSettings.Bars.ClosePrices[signal.Index];
        }

        private bool IsProfitable(Signal signal, int index)
        {
            var exitBarIndex = signal.ExitIndex > index ? index : signal.ExitIndex;

            return signal.TradeType == TradeType.Buy && _signalStatsSettings.Bars.ClosePrices[exitBarIndex] > _signalStatsSettings.Bars.ClosePrices[signal.Index] || signal.TradeType == TradeType.Sell && _signalStatsSettings.Bars.ClosePrices[exitBarIndex] < _signalStatsSettings.Bars.ClosePrices[signal.Index];
        }

        #endregion Methods
    }
}