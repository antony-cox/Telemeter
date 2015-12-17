using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Diagnostics;

namespace Telemeter_v2
{
    class Converter
    {
        Data data;
        Logger logger;

        public Converter(Data data, Logger logger)
        {
            this.data = data;
            this.logger = logger;
        }

        public Data Convert(string html)
        {
            try
            {
                logger.Write("Getting source code.", Logger.Type.Info);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);

                logger.Write("Getting main div.", Logger.Type.Info);
                //GET TELEMETER MAIN DIV
                HtmlNode mainDiv = doc.DocumentNode.SelectSingleNode("//div[@class='telemeter ng-scope']");

                logger.Write("Mapping usage data.", Logger.Type.Info);
                //GET & MAP USAGE VALUES
                HtmlNodeCollection usageValues = mainDiv.SelectNodes("//p[@class='ng-binding']");
                data.usagePiek = Double.Parse(usageValues[0].InnerText.Split('&')[0]);
                data.usageDal = Double.Parse(usageValues[1].InnerText.Split('&')[0]);
                data.usageTotal = data.usagePiek + data.usageDal;

                logger.Write("Mapping abo data.", Logger.Type.Info);
                //GET & MAP ABBO
                HtmlNode abbo = mainDiv.SelectSingleNode("//p[@class='bigtext nomargin']");
                data.abbo = abbo.InnerText.Split(':')[1].TrimStart().Split('(')[0].TrimEnd();
                if (data.abbo.ToLower().Equals("internet fiber 100"))
                {
                    data.limitPiek = 150;
                }
                else
                {
                    data.limitPiek = 500;
                }

                if (data.usagePiek > data.limitPiek)
                {
                    data.status = "Grootverbruiker";
                }
                else
                {
                    data.status = "Vrij verbruik";
                }

                logger.Write("Mapping period data.", Logger.Type.Info);
                //GET & MAP STARTPERIOD, ENDPERIOD
                HtmlNode period = mainDiv.SelectNodes("//option")[0].NextSibling;
                data.startPeriod = DateTime.Parse(period.InnerText.Split(' ')[0]);
                data.endPeriod = DateTime.Parse(period.InnerText.Split(' ')[4]);
                TimeSpan zzz = DateTime.Now - data.startPeriod;
                data.day = zzz.Days;
                data.updated = DateTime.Now;

                logger.Write("Calculating predication data.", Logger.Type.Info);
                //CALCULATE PREDICTION
                double totalSeconds = (data.endPeriod - data.startPeriod).TotalSeconds;
                double seconds = zzz.TotalSeconds;
                data.predictionPiek = Math.Round((data.usagePiek / seconds) * totalSeconds, 2);
            }
            catch (Exception e)
            {
                string msg = "Fout bij het ophalen van Telemeter Data.";
                throw new Exception(msg, e);
                logger.Write(msg + "\n" + e.Message, Logger.Type.Error);
            }

            return data;
        }
    }
}
