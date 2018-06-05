using cAlgo.API.Internals;

namespace cAlgo.API.Extensions
{
    public static class PositionExtensions
    {
        /// <summary>
        /// Returns a position stop loss in pips
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="positionSymbol">Position symbol</param>
        /// <returns>double</returns>
        public static double? GetStopLossInPips(this Position position, Symbol positionSymbol)
        {
            double entryPrice = position.EntryPrice;

            double? stopLossPrice = position.StopLoss;

            double? stopLossPips = position.TradeType == TradeType.Buy ? entryPrice - stopLossPrice : stopLossPrice - entryPrice;

            return stopLossPips.HasValue ? (double?)positionSymbol.ToPips(stopLossPips.Value) : null;
        }

        /// <summary>
        /// Returns a position take profit in pips
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="positionSymbol">Position symbol</param>
        /// <returns>double</returns>
        public static double? GetTakeProfitInPips(this Position position, Symbol positionSymbol)
        {
            double entryPrice = position.EntryPrice;

            double? takeProfitPrice = position.TakeProfit;

            double? takeProfitPips = position.TradeType == TradeType.Buy ? takeProfitPrice - entryPrice : entryPrice - takeProfitPrice;

            return takeProfitPips.HasValue ? (double?)positionSymbol.ToPips(takeProfitPips.Value) : null;
        }
    }
}