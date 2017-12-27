using PcapDotNet.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiresharkClone
{
    public partial class Form1 : Form
    {
        int deviceIndex = 0;
        IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
        Form2 form = new Form2();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            if (allDevices.Count == 0)
            {
                Console.WriteLine("No interfaces found! Make sure WinPcap is installed.");
                return;
            }

            // Print the list
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i]; //deviceları listeye at
                if (device.Description != null)
                    comboBox1.Items.Add(" (" + device.Description + ")");//combobox device açıklaması eklendi.
                else
                    label1.Text=" (Ethernet kartı bulunamadı...)";
            }

            comboBox1.SelectedIndex = 0;

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Ortak ortak = new Ortak();
            ortak.deviceIndex = comboBox1.SelectedIndex + 1;
            Form2 frm = new Form2(ortak);
            frm.Show();
            this.Hide();
        }
    }
}
