using MabiPale2.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MabiPale2
{
	public partial class FrmSettings : Form
	{
		public FrmSettings(string log)
		{
			InitializeComponent();

			TxtErrorLog.Text = log;
		}

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void FrmSettings_Load(object sender, EventArgs e)
		{
			ChkFilterRecvEnabled.Checked = Settings.Default.FilterRecvEnabled;
			ChkFilterSendEnabled.Checked = Settings.Default.FilterSendEnabled;
			TxtFilterRecv.Text = Regex.Replace(Settings.Default.FilterRecv.TrimStart(), "\r?\n\r?\n", Environment.NewLine);
			TxtFilterSend.Text = Regex.Replace(Settings.Default.FilterSend.TrimStart(), "\r?\n\r?\n", Environment.NewLine);
			if (Settings.Default.FilterExcludeModeActive)
				RadFilterExcludeMode.Checked = true;
			else
				RadFilterIncludeMode.Checked = true;

			try
			{
				TxtOpNames.Text = File.ReadAllText("ops.txt");
			}
			catch
			{
			}
		}

		private void FilterProfileBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			//BtnSelect.Enabled = true;
			ComboBox comboBox = (ComboBox)sender;
			string profile = (string)comboBox.SelectedItem;

			var configProfiles = new ConfigProfiles();
			var convertedConfigs = configProfiles.LoadConfigProfiles();

			var recvFilterFromConfig = convertedConfigs.configProfiles[profile].RecvFilter.ToList();
			var sendFilterFromConfig = convertedConfigs.configProfiles[profile].SendFilter.ToList();

			TxtFilterRecv.Clear();
			TxtFilterSend.Clear();

			StringBuilder test = new StringBuilder();
			StringBuilder test1 = new StringBuilder();

            foreach (var recv in recvFilterFromConfig)
            {
				test.Append(recv);
				test.Append(Environment.NewLine);
			}
			foreach (var send in sendFilterFromConfig)
			{
				test1.Append(send);
				test1.Append(Environment.NewLine);
			}

			TxtFilterRecv.Text = test.ToString();//Regex.Replace(newRecvText.TrimStart(), "\r?\n\r?\n?,? ", Environment.NewLine);
			TxtFilterSend.Text = test1.ToString();//Regex.Replace(newSendText.TrimStart(), "\r?\n\r?\n?,? ", Environment.NewLine);

			Settings.Default.FilterRecv = test.ToString();
			Settings.Default.FilterSend = test1.ToString();
		}

		private void FilterProfileBox_DropDownClosed(object sender, EventArgs e)
        {
			BeginInvoke(new Action(() => { FilterProfileBox.Select(FilterProfileBox.Text.Length, 0); }));
		}

		private void SaveProfileBtn_Click(object sender, EventArgs e)
        {
			string profile = FilterProfileBox.Text;

			var configProfiles = new ConfigProfiles();
			var convertedConfigs = configProfiles.LoadConfigProfiles();

			Dictionary<string, Filters> profileToAdd = new Dictionary<string, Filters>();
			Filters newFilter = new Filters();

			HashSet<string> recvFilter = new HashSet<string>();
			HashSet<string> sendFilter = new HashSet<string>();

			string[] separator = new string[] { "\r\n" };

			string[] splitUpRecvOps = TxtFilterRecv.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			string[] splitUpSendOps = TxtFilterSend.Text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

			foreach (var recv in splitUpRecvOps)
			{
				recvFilter.Add(recv);
			}
			foreach (var send in splitUpSendOps)
			{
				sendFilter.Add(send);
			}

			newFilter.RecvFilter = new List<string>(recvFilter);
			newFilter.SendFilter = new List<string>(sendFilter);

			profileToAdd.Add(profile, newFilter);

			convertedConfigs.SaveConfig(profileToAdd);

			Settings.Default.FilterRecv = TxtFilterRecv.Text;
			Settings.Default.FilterSend = TxtFilterSend.Text;

			Close();
		}

		private void BtnDeleteProfile_Click(object sender, EventArgs e)
        {
			string profileName = FilterProfileBox.Text;

			var configProfiles = new ConfigProfiles();
			var convertedConfigs = configProfiles.LoadConfigProfiles();

			convertedConfigs.RemoveProfile(profileName);

			Close();
		}

		private void FrmSettings_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Do not save if cancelling or Close button is clicked.
			if (DialogResult != DialogResult.OK)
				return;

			Settings.Default.FilterRecvEnabled = ChkFilterRecvEnabled.Checked;
			Settings.Default.FilterSendEnabled = ChkFilterSendEnabled.Checked;
			Settings.Default.FilterRecv = TxtFilterRecv.Text;
			Settings.Default.FilterSend = TxtFilterSend.Text;
			Settings.Default.FilterExcludeModeActive = RadFilterExcludeMode.Checked;
			Settings.Default.Save();

			try
			{
				File.WriteAllText("ops.txt", TxtOpNames.Text);
			}
			catch
			{
			}
		}
	}
}
