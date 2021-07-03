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
        private int lastPeakTroughIndex = 0;
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
            // calculate peak trough resistance at start of each 300 candle.
            /**
             * Now we consider all Support Line, Resistance Line is just a line.
             * Depend on current price reaction we will consider it is Resistance(RS) or Support(SP)
             */
            if(index % 300 == 0)
            {
                findSupportResistanceLine(peakTrough.getPeakTroughData());
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



        public void findSupportResistanceLine(ArrayList peakTroughData)
        {
            if (peakTroughData.Count == 0) return;
            // get the range of array peaktrough that not calculated yet.
            var peakTroughNeedCal = peakTroughData.GetRange(lastPeakTroughIndex, peakTroughData.Count - 1 - lastPeakTroughIndex);

            ArrayList listArray = new ArrayList();
            var index = 0;
            /**
             * We consider line that be RS or SP need at least 3 points that was peak or trough already.
             * STEP 1: Create a small list item and add the first item relatively with index of the list item;
             * STEP 2: If got any next peak trough that satify the min distance of RS or SP --> Add it to that small list item;
             * STEP 3: If the small list item has at least 3 points --> Consider it was valid range;
             * NOTE:
             * This way will create may line that not RS and SP (many line)
             * So i will create addition condtion to make sure that at least is RS or SP.
             */
            foreach (Utils.Base.Point point in peakTroughNeedCal)
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
                    if (Math.Abs(firstPoint.yValue - point.yValue) <= minRS &&
                        Math.Abs(firstPoint.barIndex - point.barIndex) >= minRSbar)
                    {
                        if(arr.Count <= 2)
                        {
                            if ((
                            !funcHelper.hasHigherPointAtRange(Math.Max(firstPoint.yValue, point.yValue), firstPoint.barIndex, point.barIndex, Bars)
                            ||
                            !funcHelper.hasLowerPointAtRange(Math.Min(firstPoint.yValue, point.yValue), firstPoint.barIndex, point.barIndex, Bars)
                            ))
                            {
                                arr.Add(point);
                                listArray[k] = arr;
                            }
                        } else
                        {
                            arr.Add(point);
                            listArray[k] = arr;
                        }
                       
                        
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
                    ChartRectangle rectangle = Chart.DrawRectangle("line_" + firstPoint.barIndex + "_" + lastPoint.barIndex, firstPoint.barIndex, firstPoint.yValue, lastPoint.barIndex, lastPoint.yValue, Color.FromHex("#66FFFFFF"));
                    rectangle.IsFilled = true;
                }
            }
            lastPeakTroughIndex = peakTroughData.Count -2;
        }

    }
}
