using System;
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
            lblStatus.Text = message;
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
    }
}
