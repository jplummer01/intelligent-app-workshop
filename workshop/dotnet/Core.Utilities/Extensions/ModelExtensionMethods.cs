using Core.Utilities.Models;
using System.Text;

namespace Core.Utilities.Extensions
{
    public static class ModelExtensionMethods
    {
        public static string FormatStockData(this Stock stockData)
        {
            StringBuilder stringBuilder = new();

            stringBuilder.AppendLine("| Symbol | Price | Open | Low | High | Date ");
            stringBuilder.AppendLine("| ----- | ----- | ----- | ----- |");
            stringBuilder.AppendLine($"| {stockData.Symbol} | {stockData.Close} | {stockData.Open} | {stockData.Low} | {stockData.High} | {stockData.From} ");

            return stringBuilder.ToString();
        }
    }
}
