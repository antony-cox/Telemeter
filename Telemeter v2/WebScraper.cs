using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Telemeter_v2
{
    class WebScraper
    {
        Data data;
        Converter converter;
        Encryptor encryptor;
        Logger logger;
        WebBrowser browser;
        HtmlDocument doc;
        Telemeter telemeter;
        Boolean update;
        public enum Links
        {
            Login, Telemeter
        }

        public WebScraper(WebBrowser browser, Telemeter telemeter, Logger logger)
        {
            data = new Data();
            converter = new Converter(data, logger);
            encryptor = new Encryptor();
            this.browser = browser;
            browser.DocumentCompleted += browser_DocumentCompleted;
            browser.ScriptErrorsSuppressed = true;
            this.telemeter = telemeter;
            update = true;
            this.logger = logger;
        }

        void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            PageLoaded();
        }

        public void GoTo(Links link)
        {
            string url = "";

            if (link == Links.Login)
            {
                url = "https://mijn.telenet.be/mijntelenet/login/login.do";
            }
            else if (link == Links.Telemeter)
            {
                url = "https://mijn.telenet.be/mijntelenet/telemeter/showFupUsage.do?identifier=" + Properties.Settings.Default.username;
            }

            try
            {
                browser.Navigate(url);
                logger.Write("Navigate to " + url, Logger.Type.Info);
            }
            catch (Exception e)
            {
                string msg = "Fout bij het navigeren naar " + url;
                throw new Exception(msg, e);
                logger.Write(msg + "\n" + e.Message, Logger.Type.Error);
            }
        }

        private void Login(string username, string pwd)
        {
            doc = browser.Document;
            doc.GetElementById("mtLogin").SetAttribute("value", username);
            doc.GetElementById("mtPwd").SetAttribute("value", pwd);
            doc.GetElementById("loginButton").InvokeMember("click");
        }

        public void PageLoaded()
        {
            doc = browser.Document;
            if (doc.Title.ToLower().Equals("mijn telenet"))
            {
                try
                {
                    //LOGIN
                    logger.Write("Start login", Logger.Type.Info);
                    string username = Properties.Settings.Default.username;
                    string password = encryptor.decrypt(Properties.Settings.Default.password);
                    Login(username, password);
                    logger.Write("End login", Logger.Type.Info);
                 }
                catch (Exception e)
                {
                    string msg = "Fout bij inloggen.";
                    throw new Exception(msg, e);
                    logger.Write(msg + "\n" + e.Message, Logger.Type.Error);
                }
            }
            else if (doc.Title.ToLower().Equals("mijn producten"))
            {
                //GO TO TELEMETER PAGE
                GoTo(Links.Telemeter);
            }
            else if (doc.Title.ToLower().Equals("telemeter"))
            {
                if (update)
                {
                    try
                    {
                        logger.Write("Start converting", Logger.Type.Info);
                        data = converter.Convert(browser.DocumentText);
                        update = false;
                        telemeter.UpdateForm(data);
                        logger.Write("End converting", Logger.Type.Info);
                    }
                    catch (Exception e)
                    {
                        string msg = "Fout by ophalen van Telemeter Data.";
                        throw new Exception(msg, e);
                        logger.Write(msg + "\n" + e.Message, Logger.Type.Error);
                    }
                }
            }
        }

        public void setUpdate(Boolean update)
        {
            this.update = update;
        }
    }
}
