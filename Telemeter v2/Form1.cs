using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using HtmlAgilityPack;

namespace Telemeter_v2
{
    public partial class Telemeter : Form
    {
        WebScraper scraper;
        Boolean update = true;
        HtmlPanel htmlPanel;
        Logger logger;

        public Telemeter()
        {
            InitializeComponent();
            CheckAdmin();
            logger = new Logger();
            htmlPanel = new HtmlPanel();
            htmlPanel.Dock = DockStyle.Fill;
            htmlPanel.Text = String.Empty;
            Controls.Add(htmlPanel);
            this.FormClosing += Telemeter_FormClosing;
            this.MaximizeBox = false;
        }

        private void CheckAdmin()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                MessageBox.Show("Pls run as admin", "Run as admin pls", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.FailFast("RIP Telemeter");
            }
        }

        void Telemeter_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
            e.Cancel = true;        
        }

        private void Telemeter_Load(object sender, EventArgs e)
        {
            webBrowser.ScriptErrorsSuppressed = true;
            scraper = new WebScraper(webBrowser, this, logger);

            if (Properties.Settings.Default.username.Equals(""))
            {
                NewUser();
            }
            else
            {
                scraper.GoTo(WebScraper.Links.Login);
            }            
        }

        public void UpdateForm(Data data)
        {
            logger.Write("Start display data to form.", Logger.Type.Info);
            string html = "";
            string predictionStatus = "";
            double predictionDiff = data.limitPiek - data.predictionPiek;

            if (predictionDiff > 0)
            {
                predictionStatus = "Voorspelling status: <b><font color=\"green\">Trek maar raak! (+" + predictionDiff + ")</font></b>";
            }
            else
            {
                predictionStatus = "Voorspelling status: <b><font color=\"red\">Danger Zone! (" + predictionDiff + ")</font></b>";
            }

            html =  "<table border=\"0\" style=\"border-spacing: 5px 10px; font-family: arial; font-size: 9px; margin-top: 25px; margin-left: 8px; width: 600px;\">" +
                    "<tr><td>Abonnement: <b>" + data.abbo + "</b></td><td></td></tr>" +
                    "<tr><td>Periode van <b>" + data.startPeriod.ToShortDateString() + "</b> tot <b>" + data.endPeriod.ToShortDateString() + "</b></td><td style=\"text-align: right\">Dag <b>" + data.day + "</b> van de aanrekeningsperiode</td></tr>" +
                    "<tr><td>Verbruik piek: <b>" + data.usagePiek + " GB</b></td><td style=\"text-align: right\">Voorspelling verbruik piek: <b>" + data.predictionPiek + " GB</b></td></tr>" +
                    "<tr><td>Verbruik dal: <b>" + data.usageDal + " GB</b></td><td style=\"text-align: right\">" + predictionStatus + "</td></tr>" +
                    "<tr><td>Verbruik Totaal: <b>" + data.usageTotal + " GB</b></td><td></td></tr>" + 
                    "<tr><td>Laatst geupdate: <b>" + data.updated.ToString() + "</b></td><td></td></tr>" +
                    "</table>";

            pgbPiek.Maximum = (int)500;
            pgbPiek.Value = (int)data.usagePiek;
            htmlPanel.Text = html;
            picLoading.Visible = false;
            logger.Write("End display data to form.", Logger.Type.Info);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void newUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewUser();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void Exit()
        {
            this.Dispose();
        }
        
        private void Refresh()
        {
            logger.Write("Refreshing.", Logger.Type.Info);
            scraper.setUpdate(true);
            scraper.GoTo(WebScraper.Links.Telemeter);
        }

        private void NewUser()
        {
            AddUser form = new AddUser(this);
            form.Show();
        }

        public void GetData()
        {
            scraper.GoTo(WebScraper.Links.Login);
        }
    }
}
