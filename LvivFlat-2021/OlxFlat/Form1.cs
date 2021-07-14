using System;
using System.Diagnostics;
using System.Windows.Forms;
using OlxFlat.Helpers;

namespace OlxFlat
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ShowStatus(string message)
        {
            lblSecond.Text = message;
            Application.DoEvents();
        }

        private void btnOlxList_LoadFromWeb_Click(object sender, EventArgs e) => Download.OlxList_Download(ShowStatus);
        private void btnOlxList_Parse_Click(object sender, EventArgs e) => Parse.OlxList_Parse(ShowStatus);


        private void btnOlxDetails_LoadFromWeb_Click(object sender, EventArgs e) => Download.OlxDetails_Download(ShowStatus);
        private void btnOlxDetails_Parse_Click(object sender, EventArgs e) => Parse.OlxDetails_Parse(ShowStatus);

        private void btnUpdateOlxData_Click(object sender, EventArgs e)
        {
            ShowStatus("Update Olx data in DB. Started");
            SaveToDb.OlxDataUpdate();
            ShowStatus("Update Olx data in DB. Finished");
        }

        private void btnOlxUpdateAll_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            lblFirst.Text = @"STAGE 1. ";
            Application.DoEvents();
            Download.OlxList_Download(ShowStatus);

            lblFirst.Text = @"STAGE 2. ";
            Application.DoEvents();
            Parse.OlxList_Parse(ShowStatus);

            lblFirst.Text = @"STAGE 3. ";
            Application.DoEvents();
            Download.OlxDetails_Download(ShowStatus);

            lblFirst.Text = @"STAGE 4. ";
            Application.DoEvents();
            Parse.OlxDetails_Parse(ShowStatus);

            lblFirst.Text = @"STAGE 5. Update Olx data in DB.";
            lblSecond.Text = @"";
            Application.DoEvents();
            SaveToDb.OlxDataUpdate(); ;

            sw.Stop();
            var secs = Convert.ToInt32(sw.Elapsed.TotalSeconds);
            lblFirst.Text = $@"ALL STAGES FINISHED! Update time: {secs} seconds";
        }

        private void btnDomRiaList_LoadFromWeb_Click(object sender, EventArgs e) => Download.DomRia_Download(ShowStatus);

        private void btnDomRiaParseDetails_Click(object sender, EventArgs e) => Parse.DomRiaDetails_Parse(ShowStatus);

        private void btnDomRiaUpdateAll_Click(object sender, EventArgs e)
        {
            var sw = new Stopwatch();
            sw.Start();

            lblFirst.Text = @"STAGE 1. ";
            Application.DoEvents();
            Download.DomRia_Download(ShowStatus);

            lblFirst.Text = @"STAGE 2. ";
            Application.DoEvents();
            Parse.DomRiaDetails_Parse(ShowStatus);

            sw.Stop();
            var secs = Convert.ToInt32(sw.Elapsed.TotalSeconds);
            lblFirst.Text = $@"ALL STAGES FINISHED! Update time: {secs} seconds";
        }
    }
}
