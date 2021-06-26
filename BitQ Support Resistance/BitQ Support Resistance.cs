using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using BitQIndicator;
using System.Collections;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class BitQSupportResistance : Indicator
    {
        [Parameter(DefaultValue = 2)]
        public double minRS { get; set; }

        [Parameter(DefaultValue = 10)]
        public int minRSbar { get; set; }

        private BitQPeakTrough peakTrough;
        private int lastTroughIndex = 0;
        private int lastPeakIndex = 0;
        private Utils.Func funcHelper;

        protected override void Initialize()
        {
            // Initialize and create nested indicators
            peakTrough = Indicators.GetIndicator<BitQPeakTrough>(false, 0, -10);
            peakTrough.reset();
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            // Result[index] = ...
            funcHelper = new Utils.Func();
            peakTrough.Calculate(index);
            var peakData = peakTrough.getPeakData();
            var troughData = peakTrough.getTroughData();
            if (peakData.Count % 300 == 0)
            {
                findResistanceLine(peakData);
                // flatten
            }

            if (troughData.Count % 300 == 0)
            {
                findSupportLine(troughData);
            }

        }

        public void findResistanceLine(ArrayList peakData)
        {
            var rangePeakData = peakData.GetRange(lastPeakIndex, 300);
            ArrayList listArray = new ArrayList();
            var index = 0;
            foreach (Utils.Base.Point point in rangePeakData)
            {
                // add first element that point;
                if (listArray.Count == index)
                {
                    ArrayList arr = new ArrayList();
                    arr.Add(point);
                    listArray.Add(arr);
                }
                for (var k = 0; k < listArray.Count; k++)
                {
                    ArrayList arr = (ArrayList)listArray[k];
                    Utils.Base.Point firstPoint = (Utils.Base.Point)arr[0];
                    if (Math.Abs(firstPoint.yValue - point.yValue) <= minRS && Math.Abs(firstPoint.barIndex - point.barIndex) >= minRSbar && !funcHelper.hasHigherPointAtRange(Math.Max(firstPoint.yValue, point.yValue), firstPoint.barIndex, point.barIndex, Bars))
                    {
                        arr.Add(point);
                        listArray[k] = arr;
                    }
                }
                index++;
            }
            foreach (ArrayList arr in listArray)
            {
                if (arr.Count >= 3)
                {
                    Utils.Base.Point firstPoint = (Utils.Base.Point)arr[0];
                    Utils.Base.Point lastPoint = (Utils.Base.Point)arr[arr.Count - 1];
                    ChartRectangle rectangle = Chart.DrawRectangle("aaa_" + firstPoint.barIndex + "_" + lastPoint.barIndex, firstPoint.barIndex, firstPoint.yValue, lastPoint.barIndex, lastPoint.yValue, Color.FromHex("#66FFFFFF"));
                    rectangle.IsFilled = true;
                }
            }
            lastPeakIndex = peakData.Count;
        }

        public void findSupportLine(ArrayList troughData)
        {
            var rangePeakData = troughData.GetRange(lastTroughIndex, 300);
            ArrayList listArray = new ArrayList();
            var index = 0;
            foreach (Utils.Base.Point point in rangePeakData)
            {
                // add first element that point;
                if (listArray.Count == index)
                {
                    ArrayList arr = new ArrayList();
                    arr.Add(point);
                    listArray.Add(arr);
                }
                for (var k = 0; k < listArray.Count; k++)
                {
                    ArrayList arr = (ArrayList)listArray[k];
                    Utils.Base.Point firstPoint = (Utils.Base.Point)arr[0];
                    if (Math.Abs(firstPoint.yValue - point.yValue) <= minRS && Math.Abs(firstPoint.barIndex - point.barIndex) >= minRSbar && !funcHelper.hasLowerPointAtRange(Math.Min(firstPoint.yValue, point.yValue), firstPoint.barIndex, point.barIndex, Bars))
                    {
                        arr.Add(point);
                        listArray[k] = arr;
                    }
                }
                index++;
            }
            foreach (ArrayList arr in listArray)
            {
                if (arr.Count >= 3)
                {
                    Utils.Base.Point firstPoint = (Utils.Base.Point)arr[0];
                    Utils.Base.Point lastPoint = (Utils.Base.Point)arr[arr.Count - 1];
                    ChartRectangle rectangle = Chart.DrawRectangle("aaa_" + firstPoint.barIndex + "_" + lastPoint.barIndex, firstPoint.barIndex, firstPoint.yValue, lastPoint.barIndex, lastPoint.yValue, Color.FromHex("#664BCA0C"));
                    rectangle.IsFilled = true;
                }
            }
            lastTroughIndex = troughData.Count;
        }



        public void findSupportResistanceLine(ArrayList peakDatas, ArrayList troughDatas)
        {

        }

    }
}
