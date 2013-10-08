using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System.Windows;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace pogodynkaCsharp
{
    class Program
    {
        public static DateTime ostatniaAktualizacja=DateTime.MinValue;
        public static double tempOdcz=0;
        public static double tempOdcz2=0;
        public static double tempOdcz1=0;
        //Variables
        public static string place = "";
        public static string obs_time = "";
        public static string weather1 = "";
        public static string temperature_string = "";
        public static string relative_humidity = "";
        public static string wind_string = "";
        public static string windKPH="";
        public static string pressure_mb = "";
        public static string dewpoint_string = "";
        public static string visibility_km = "";
        public static string latitude = "";
        public static string longitude = "";
        public static string tempC="";
        public static string feelslike_c="";
        public static string month="";
        public static List<ForecastDay> dni= new List<ForecastDay>();
        public static List<ForecastDay> dni2= new List<ForecastDay>(); //txt_forecast
        public static List<ForecastDay> SFDay = new List<ForecastDay>();

        static void Main(string[] args)
        {

            /*
             * Pobieranie danych przez HttpWebRequest
             */
            string target=string.Empty;
            string url = "http://api.wunderground.com/api/c9d15b10ff3ed303/conditions/astronomy/lang:PL/q/Poland/Lublin.json";
            HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create(url);
            HttpWReq.Method="POST";
            HttpWReq.ContentLength=0;
            HttpWReq.ContentType="text/json";
            try
            {
                HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

                Console.WriteLine(HttpWResp.StatusCode);
                Console.WriteLine(HttpWResp.GetResponseStream());
                try
                {
                    StreamReader streamReader = new StreamReader(HttpWResp.GetResponseStream(), true);
                    try
                    {
                        target = streamReader.ReadToEnd();
                        Console.WriteLine(target.ToString());
                        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\test.txt");
                        //file.WriteLine(target);

                        //file.Close();
                    }
                    finally
                    {
                        streamReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                HttpWResp.Close();

                string wunderground_key ="c9d15b10ff3ed303";
                parse("http://api.wunderground.com/api/" + wunderground_key + "/conditions/lang:PL/q/Poland/Lublin.xml");
                Console.ReadKey();
                var temp=Math.Round(tempOdcz, 1);
                Console.WriteLine("TEMPERATURA ODCZUWALNA W MAIN \n"+temp+" C");
                Console.ReadLine();

                /*
                 * FORECAST z użyciem XML
                 */
                Console.WriteLine("Zabawa z forecast");
                string furl = "http://api.wunderground.com/api/c9d15b10ff3ed303/forecast/lang:PL/q/Poland/Lublin.json";
                HttpWReq = (HttpWebRequest)WebRequest.Create(furl);
                HttpWReq.Method="POST";
                HttpWReq.ContentLength=0;
                HttpWReq.ContentType="text/json";

                HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

                Console.WriteLine(HttpWResp.StatusCode);
                Console.WriteLine(HttpWResp.GetResponseStream());
                try
                {
                    StreamReader streamReader = new StreamReader(HttpWResp.GetResponseStream(), true);
                    try
                    {
                        target = streamReader.ReadToEnd();
                        //Console.WriteLine(target.ToString());
                        //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\test2.txt");
                        //file.WriteLine(target);

                        //file.Close();
                    }
                    finally
                    {
                        streamReader.Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                HttpWResp.Close();
                Console.WriteLine("W  main: "+ostatniaAktualizacja);
                //parseForecast("http://api.wunderground.com/api/c9d15b10ff3ed303/forecast/lang:PL/q/Poland/Lublin.xml");
                parseForecastX("http://api.wunderground.com/api/c9d15b10ff3ed303/forecast/lang:PL/q/Poland/Lublin.xml");

            }
            catch (Exception ex)
            {
                Console.WriteLine("W main: "+ex.Message);
            }
            Console.ReadLine();

        }

        private static void parseForecastX(string input_xml)
        {
            Console.WriteLine("Zabawa z XDocument");
            string weather="";
            string tmp="";
            try
            {
                var cli = new WebClient();
                weather = cli.DownloadString(input_xml);
                XmlReader reader = XmlReader.Create(new StringReader(weather));
                XDocument doc = XDocument.Load(reader);
                var txt_forecast = (from d in doc.Descendants()
                                    where (d.Name.LocalName == "txt_forecast")
                                    select d).ToList();

                var forecast = (from d in txt_forecast.Descendants()
                                where (d.Name.LocalName=="forecastday")
                                select d).ToList();

                foreach (var item in forecast)
                {
                    Console.WriteLine("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx");

                    Console.WriteLine(item);
                    ForecastDay d = new ForecastDay();
                    d.period = item.Element("period").Value;
                    d.icon=item.Element("icon").Value;
                    d.iconUrl=item.Element("icon_url").Value;
                    d.fcttext=item.Element("fcttext").Value;
                    d.fcttextMetric=item.Element("fcttext_metric").Value;
                    d.title=item.Element("title").Value;
                    d.pop=item.Element("pop").Value;
                    dni2.Add(d);
                }
                //var data = from item in doc.Descendants("forecastday")
                //           select new
                //           {
                //               period = item.Element("period").Value,
                //               icon = item.Element("icon").Value,
                //               iconUrl = item.Element("iconUrl").Value,
                //               title=item.Element("title").Value,
                //               fcttext=item.Element("fcttext").Value,
                //               fcttextMetric=item.Element("fcttextMetric").Value,
                //               pop=item.Element("pop").Value,
                //               data=new
                //               {
                //                   day=item.Element("day").Value,
                //                   month=item.Element("month").Value,
                //                   year=item.Element("year").Value,
                //                   hour=item.Element("hour").Value,
                //                   min=item.Element("min").Value,
                //                   yday=item.Element("yday").Value,
                //                   monthName=item.Element("monthName").Value
                //               }

                //           };

                foreach (var d in dni2)
                {
                    Console.WriteLine("* {0}, {1}, {2}, {3}, {4}, {5}, {6}. ", d.period, d.icon, d.iconUrl, d.title, d.fcttext, d.fcttextMetric, d.pop);
                }

                var simpleForecast = (from d in doc.Descendants()
                                      where (d.Name.LocalName=="simpleforecast")
                                      select d).ToList();

                var smplFrcstDay = (from d in simpleForecast.Descendants()
                                    where (d.Name.LocalName=="forecastday")
                                    select d).ToList();

                //simple forecast
                foreach (var item in smplFrcstDay)
                {
                    string sTmp="";
                    int iTmp=0;
                    Console.WriteLine("****"+item);
                    ForecastDay fd = new ForecastDay();

                    fd.period = item.Element("period").Value;
                    fd.icon=item.Element("icon").Value;
                    fd.iconUrl=item.Element("icon_url").Value;
                    fd.conditions=item.Element("conditions").Value;
                    fd.pop=item.Element("pop").Value;

                    //DATA
                    Date d = new Date();
                    var data = (from x in item.Descendants()
                                where x.Name.LocalName=="date"
                                select x).FirstOrDefault();

                    Console.WriteLine("XXXXXXX: "+data.Element("day").Value);
                    d.day=data.Element("day").Value;
                    d.epoch=data.Element("epoch").Value;
                    d.hour=data.Element("hour").Value;
                    d.min=data.Element("min").Value;
                    d.month=data.Element("month").Value;
                    d.monthName=data.Element("monthname").Value;
                    d.weekDay=data.Element("weekday").Value; //albo weekday_short , czyli skrót nazwy dnia tygodnia
                    d.pretty=data.Element("pretty").Value;
                    d.yday=data.Element("yday").Value;
                    d.year=data.Element("year").Value;
                    d.prettyShort=data.Element("pretty_short").Value;

                    fd.data=d;


                    //WIND NIE DZIAŁA, DOKOŃCZYĆ

                    //MAX WIND
                    var wnd = (from x in item.Descendants()
                               where x.Name.LocalName=="maxwind"
                               select x).FirstOrDefault();
                    sTmp=wnd.Element("mph").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.maxwind_mph=iTmp;
                    sTmp=wnd.Element("kph").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.maxwind_kph=iTmp;
                    fd.maxwind_dir=wnd.Element("dir").Value;
                    fd.maxwind_degrees=wnd.Element("degrees").Value;

                    //AVERAGE WIND
                    wnd = (from x in item.Descendants()
                           where x.Name.LocalName=="avewind"
                           select x).FirstOrDefault();
                    sTmp=wnd.Element("mph").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.avewind_mph=iTmp;
                    sTmp=wnd.Element("kph").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.avewind_kph=iTmp;
                    fd.avewind_dir=wnd.Element("dir").Value;
                    fd.avewind_degrees=wnd.Element("degrees").Value;

                    Console.WriteLine(sTmp);
                    // HUMIDITY
                    sTmp=item.Element("avehumidity").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.avehumidity=iTmp;
                    sTmp=item.Element("maxhumidity").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.maxhumidity=iTmp;
                    sTmp=item.Element("minhumidity").Value;
                    if (int.TryParse(sTmp, out iTmp))
                        fd.minhumidity=iTmp;

                    //TEMPERATURE
                    var temp2 = from m in item.Descendants()
                                where (m.Name.LocalName=="high")
                                select m;
                    var temeperatura = from m in temp2.Descendants()
                                       where m.Name.LocalName=="celsius"
                                       select m.Value;

                    sTmp = temeperatura.First();
                    fd.highTempC=sTmp;
                    temp2 = from m in item.Descendants()
                            where (m.Name.LocalName=="low")
                            select m;
                    temeperatura = from m in temp2.Descendants()
                                   where m.Name.LocalName=="celsius"
                                   select m.Value;
                    sTmp = temeperatura.First();
                    fd.lowTempC=sTmp;
                    SFDay.Add(fd);
                }

                ostatniaAktualizacja=DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wyjątek: "+ex.Source+" "+ex.Message);
            }
        }


        protected static void parseForecast(string input_xml)
        {
            Console.WriteLine("W parse forecast");
            string weather="";
            string tmp="";
            try
            {
                var cli = new WebClient();
                weather = cli.DownloadString(input_xml);
                //Console.WriteLine(weather.ToString());
                //System.IO.StreamWriter file = new System.IO.StreamWriter("D:\\test3.txt");
                //file.WriteLine(weather);

                //file.Close();
                ostatniaAktualizacja=DateTime.Now;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Wyjątek: "+ex.Source+" "+ex.Message);
            }
            XmlReader reader = XmlReader.Create(new StringReader(weather));
            XmlDocument doc=new XmlDocument();
            doc.Load(input_xml);
            XmlNodeList forecastDays = doc.GetElementsByTagName("forecastdays");
            int z=0;
            foreach (XmlNode elem in forecastDays)
            {
                z++;
                //Console.WriteLine("XMLNode: "+elem.Name+" inner: "+elem.InnerXml);
                Console.WriteLine(z+", parent: "+elem.ParentNode.Name);
                if ((elem.HasChildNodes)&&(elem.ParentNode.Name.Equals("txt_forecast")))
                {
                    Console.WriteLine("ELEM: "+elem.ChildNodes.Count);
                    for (int i = 0; i < elem.ChildNodes.Count; i++)
                    {
                        //Console.WriteLine(i+": "+elem.ChildNodes.Item(i).Name+"  "+elem.ChildNodes.Item(i).InnerText);
                        if (elem.ChildNodes.Item(i).Name.Equals("forecastday"))
                        {
                            if (elem.ChildNodes.Item(i).HasChildNodes)
                            {
                                int x=0;
                                ForecastDay f = new ForecastDay();
                                foreach (XmlNode item in elem.ChildNodes.Item(i))
                                {
                                    if (item.Name=="period")
                                        f.period=item.InnerText;
                                    if (item.Name=="icon")
                                        f.icon=item.InnerText;
                                    if (item.Name=="icon_url")
                                        f.iconUrl=item.InnerText;
                                    if (item.Name=="title")
                                        f.title=item.InnerText;
                                    if (item.Name=="fcttext")
                                        f.fcttext=item.InnerText;
                                    if (item.Name=="fcttext_metric")
                                        f.fcttextMetric=item.InnerText;
                                    if (item.Name=="pop")
                                        f.pop=item.InnerText;
                                }
                                dni.Add(f);
                                //foreach (XmlNode item in elem.ChildNodes.Item(i))
                                //{
                                //    if ((item.Name=="period")&&(item.InnerText=="1"))
                                //    {
                                //    }
                                //    Console.WriteLine(x+" FOREACH: "+item.Name+" "+item.InnerText);
                                //    x++;
                                //}

                            }
                        }

                    }
                }
                if ((elem.HasChildNodes)&&(elem.ParentNode.Name.Equals("simpleforecast")))
                {

                }


            }
            //using (XmlReader reader = XmlReader.Create(new StringReader(weather)))
            //{

            //    while (reader.Read())
            //    {
            //        switch (reader.NodeType)
            //        {
            //            case XmlNodeType.Element:
            //                if (reader.Name.Equals("feature"))
            //                {
            //                    reader.Read();
            //                    tmp=reader.Value;
            //                    //Console.WriteLine("Buuu");
            //                    Console.WriteLine(tmp.ToString());
            //                }
            //                if (reader.Name.Equals("forecastday"))
            //                {

            //                    Console.WriteLine(tmp.ToString());
            //                }
            //                break;

            //        }

            //    }
            //    Console.WriteLine("W parse: "+ostatniaAktualizacja);
            //}
            Console.ReadLine();
            Console.ReadLine();

            Console.WriteLine("NA KONCU :");
            foreach (ForecastDay d in dni)
            {
                Console.WriteLine("* {0}, {1}, {2}, {3}, {4}, {5}, {6}. ", d.period, d.icon, d.iconUrl, d.title, d.fcttext, d.fcttextMetric, d.pop);
            }

            Console.ReadLine();
        }


        protected static void parse(string input_xml)
        {


            var cli = new WebClient();
            string weather = cli.DownloadString(input_xml);

            using (XmlReader reader = XmlReader.Create(new StringReader(weather)))
            {
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("full"))
                            {
                                reader.Read();
                                place = reader.Value;
                            }
                            else if (reader.Name.Equals("observation_time"))
                            {
                                reader.Read();
                                obs_time = reader.Value;
                            }
                            else if (reader.Name.Equals("weather"))
                            {
                                reader.Read();
                                weather1 = reader.Value;
                            }
                            else if (reader.Name.Equals("temperature_string"))
                            {
                                reader.Read();
                                temperature_string = reader.Value;
                            }
                            else if (reader.Name.Equals("temp_c"))
                            {
                                reader.Read();
                                tempC=reader.Value;
                            }
                            else if (reader.Name.Equals("relative_humidity"))
                            {
                                reader.Read();
                                relative_humidity = reader.Value;
                            }
                            else if (reader.Name.Equals("wind_string"))
                            {
                                reader.Read();
                                wind_string = reader.Value;
                            }
                            else if (reader.Name.Equals("pressure_mb"))
                            {
                                reader.Read();
                                pressure_mb = reader.Value;
                            }
                            else if (reader.Name.Equals("dewpoint_string"))
                            {
                                reader.Read();
                                dewpoint_string = reader.Value;
                            }
                            else if (reader.Name.Equals("visibility_km"))
                            {
                                reader.Read();
                                visibility_km = reader.Value;
                            }
                            else if (reader.Name.Equals("latitude"))
                            {
                                reader.Read();
                                latitude = reader.Value;
                            }
                            else if (reader.Name.Equals("longitude"))
                            {
                                reader.Read();
                                longitude = reader.Value;
                            }
                            else if (reader.Name.Equals("wind_kph"))
                            {
                                reader.Read();
                                windKPH = reader.Value;
                            }
                            else if (reader.Name.Equals("feelslike_c"))
                            {
                                reader.Read();
                                feelslike_c=reader.Value;
                            }

                            break;
                    }
                }
            }

            Console.WriteLine("********************");
            Console.WriteLine("Place:             " + place);
            Console.WriteLine("Observation Time:  " + obs_time);
            Console.WriteLine("Weather:           " + weather1);
            Console.WriteLine("Temperature:       " + temperature_string);
            Console.WriteLine("Relative Humidity: " + relative_humidity);
            Console.WriteLine("Wind:              " + wind_string);
            Console.WriteLine("Pressure (mb):     " + pressure_mb);
            Console.WriteLine("Dewpoint:          " + dewpoint_string);
            Console.WriteLine("Visibility (km):   " + visibility_km);
            Console.WriteLine("Location:          " + longitude + ", " + latitude);
            Console.ReadLine();

            double tempWC=0;
            double windKnH=0;
            double V=0;
            if (double.TryParse(tempC, out tempWC))
            {
                Console.WriteLine("Temperatura w C: "+tempWC);
            }
            if (double.TryParse(windKPH, out windKnH))
            {
                Console.WriteLine("Wiatr km/h: "+windKnH);
            }
            if (double.TryParse(feelslike_c, out tempOdcz1))
            {
                Console.WriteLine("Temp feelslike: "+tempOdcz1);
            }
            string newString = obs_time.Substring(16);
            month = newString.Substring(0, newString.IndexOf(' ') + 1);
            V=Math.Pow(windKnH, 0.16);
            tempOdcz=13.12+(0.6215*tempWC)-(11.37*V)+(0.3965*tempWC*V);
            Console.WriteLine("Temp odczuwalna: "+tempOdcz);

            tempOdcz2=33+((0.478+(0.237*Math.Sqrt(V))-0.0124*V)*(tempWC-33));
            Console.WriteLine("Temp odcz. 2: "+tempOdcz2);
            Console.WriteLine("Feelslike_c:  "+feelslike_c);
            Console.WriteLine("Month: "+month);
            Console.ReadLine();
        }
    }
}


