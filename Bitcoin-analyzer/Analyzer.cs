using System;
using System.Net;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Bitcoin_analyzer
{
    /// <summary>
    /// This class handles the request of data and analysis of it.
    /// </summary>
    
    public class Analyzer
    {
        public Analyzer() {}

        public int numberOfDays { get; set; }              // The longest downward trend in given date range.
        public double highestVolumeAmount { get; set; }    // The highest volume in euros.
        public bool badTrend { get; set; }                 // True if all days within range are bad days to buy or sell bitcoin.
        public string errors { get; set; }                 // Contains possible error messages.
        public DateTime highestVolumeDate { get; set; }    // The Date of the highest volume.
        public DateTime bestBuyDay { get; set; }           // Best day to buy bitcoins.
        public DateTime bestSellDay { get; set; }          // Best day to sell bitcoins.


        // Transforms datetimes to seconds for url parameters and sends a request to fetch data.
        public void HandleRequest(DateTime startDate, DateTime endDate)
        {
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0, DateTimeKind.Utc);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 0, 0, 0, DateTimeKind.Utc);
            long hourInSeconds = 3600;
            long start = new DateTimeOffset(startDate).ToUnixTimeSeconds();
            long end = new DateTimeOffset(endDate).ToUnixTimeSeconds() + hourInSeconds;

            if(startDate > endDate)
            {
                errors = "Your end date must be later than your start date!";
                return;
            }
            try
            {
                string url = "https://api.coingecko.com/api/v3/coins/bitcoin/market_chart/range?vs_currency=eur&from=" + start + "&to=" + end;
                WebRequest request = WebRequest.Create(url);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string json = reader.ReadToEnd();

                Analysis(json);
            }
            catch(WebException e)
            {
                errors = "There's something wrong with internet connection!";
            }
            catch(Exception e)
            {
                errors = "Oops, Something went wrong!";
            }
           
        }

        // Formats the requested data and runs the analysis of it.
        private void Analysis(string json)
        {
            try
            {
                var content = JsonSerializer.Deserialize<Dictionary<string, List<List<double>>>>(json);
                var prices = content["prices"];
                var tradingVolumes = content["total_volumes"];

                LongestDownwardTrend(prices);
                HighestTradingVolume(tradingVolumes);
                BestBuyAndSellDays(prices);

            }
            catch (Exception e)
            {
                errors = "Oops, Something went wrong!";
            }          
        }

        // Calculates the longest downward price trend in given date range.
        private void LongestDownwardTrend(List<List<double>> prices)
        {
            
            int previousRecord = 0;
            double previousPrice = 0;
            List<List<double>> dayPrices = new List<List<double>>();
            DateTime previousDate = new DateTime(1970, 1, 1);

            numberOfDays = 0;

            for (int i = 0; i < prices.Count; i++)
            {
                double milliseconds = prices[i][0];
                var date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(milliseconds);

                if (date.ToShortDateString() != previousDate.ToShortDateString())
                {
                    dayPrices.Add(prices[i]);
                   
                }

                previousDate = date;
            }

            for (int i = 0; i < dayPrices.Count; i++)
            {
                
                if (dayPrices[i][1] < previousPrice)
                {
                    numberOfDays++;
                    
                    if (numberOfDays >= previousRecord) 
                    {
                        previousRecord = numberOfDays;
                    }
                }
                else
                {
                    numberOfDays = 0;
                }
                previousPrice = dayPrices[i][1];
            }

            numberOfDays = previousRecord;
                     
        }

        // Calculates the highest trading volume in euros along with date.
        private void HighestTradingVolume(List<List<double>> tradingVolumes)
        {
            List<double> highestVolume = tradingVolumes[0];
            for (int i = 0; i < tradingVolumes.Count; i++)
            {
                if (tradingVolumes[i][1] > highestVolume[1])
                {
                    highestVolume = tradingVolumes[i];
                }

            }
            highestVolumeDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(highestVolume[0]);
            highestVolumeAmount = highestVolume[1];
        }

        // Calculates the best days to buy and sell bitcoins in given date range.
        private void BestBuyAndSellDays(List<List<double>> prices)
        {
            double highestPrice = 0;
            double lowestPrice = 0;
            double previousPrice = 0;
            int downwardTrend = 0;
            DateTime previousDate = new DateTime(1970, 1, 1);        
            List<List<double>> dayPrices = new List<List<double>>();
      
            for (int i = 0; i < prices.Count; i++)
            {
                double milliseconds = prices[i][0];
                var date = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(milliseconds);

                if (date.ToShortDateString() != previousDate.ToShortDateString())
                {
                    dayPrices.Add(prices[i]);
                    if(prices[i][1] > highestPrice)
                    {
                        bestSellDay = date;
                        highestPrice = prices[i][1];
             
                    }
                    if(prices[i][1] < lowestPrice || lowestPrice == 0)
                    {
                        bestBuyDay = date;
                        lowestPrice = prices[i][1];
                    }
                    if(prices[i][1] < previousPrice)
                    {
                        downwardTrend++;
                        
                    }
                    previousPrice = prices[i][1];
                }
                previousDate = date;
            }          
           
            if (downwardTrend == dayPrices.Count - 1)
            {
                badTrend = true;
            }
            else
            {
                badTrend = false;             
            }
        }    
    }
}
