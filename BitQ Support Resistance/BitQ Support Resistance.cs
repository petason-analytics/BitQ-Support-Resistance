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
        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }

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
            if (peakData.Count % 300 == 0 )
            {
                findResistanceLine(peakData);
                // flatten
            }

            if(troughData.Count % 300 == 0)
            {
                findSupportLine(troughData);
            }

        }

        public void findResistanceLine(ArrayList peakData)
        {
            var BASE_LEAST_RANGE = 10;
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
                    if (Math.Abs(firstPoint.yValue - point.yValue) <= 2 && 
                        Math.Abs(firstPoint.barIndex-point.barIndex) >= BASE_LEAST_RANGE &&
                        !funcHelper.hasHigherPointAtRange(Math.Max(firstPoint.yValue, point.yValue), firstPoint.barIndex, point.barIndex, Bars))
                    {
                        arr.Add(point);
                        listArray[k] = arr;
                    }
                }
                index++;
            }
            foreach(ArrayList arr in listArray)
            {
                if(arr.Count >= 3)
                {
                    Utils.Base.Point firstPoint = (Utils.Base.Point)arr[0];
                    Utils.Base.Point lastPoint = (Utils.Base.Point)arr[arr.Count - 1];
                    Chart.DrawRectangle("aaa_" + firstPoint.barIndex + "_" + lastPoint.barIndex, firstPoint.barIndex, firstPoint.yValue, lastPoint.barIndex, lastPoint.yValue, Color.White); ;
                }
            }
            lastPeakIndex = peakData.Count;
        }

        public void findSupportLine(ArrayList troughData)
        {
            var BASE_LEAST_RANGE = 10;
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
                    if (Math.Abs(firstPoint.yValue - point.yValue) <= 2 &&
                        Math.Abs(firstPoint.barIndex - point.barIndex) >= BASE_LEAST_RANGE &&
                        !funcHelper.hasLowerPointAtRange(Math.Max(firstPoint.yValue, point.yValue), firstPoint.barIndex, point.barIndex, Bars))
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
                    Chart.DrawRectangle("aaa_" + firstPoint.barIndex + "_" + lastPoint.barIndex, firstPoint.barIndex, firstPoint.yValue, lastPoint.barIndex, lastPoint.yValue, Color.Green) ;
                }
            }
            lastTroughIndex = troughData.Count;
        }



        public void findSupportResistanceLine(ArrayList peakDatas, ArrayList troughDatas)
        {

        }

    }
}
