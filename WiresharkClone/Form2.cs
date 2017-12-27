using PcapDotNet.Core;
using PcapDotNet.Packets;
using PcapDotNet.Packets.IpV4;
using PcapDotNet.Packets.Transport;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiresharkClone
{
    public partial class Form2 : Form
    {
        
        public static IList<LivePacketDevice> allDevices = LivePacketDevice.AllLocalMachine;
        static int deviceIndex;
        PacketDevice selectedDevice;
        public Form2()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        public Form2(Ortak o) : this()
        {
            deviceIndex = o.deviceIndex;

        }
        private void PaketYakala(String filtre="ip")
        {
            for (int i = 0; i != allDevices.Count; ++i)
            {
                LivePacketDevice device = allDevices[i];
            }

            selectedDevice = allDevices[deviceIndex - 1];
            using (PacketCommunicator communicator =
               selectedDevice.Open(65536,                                  // portion of the packet to capture
                                                                           // 65536 guarantees that the whole packet will be captured on all the link layers
                                   PacketDeviceOpenAttributes.Promiscuous, // promiscuous mode
                                   1000))                                  // read timeout
            {
                // Check the link layer. We support only Ethernet for simplicity.
                if (communicator.DataLink.Kind != DataLinkKind.Ethernet)
                {
                    Console.WriteLine("This program works only on Ethernet networks.");
                    return;
                }

                // Compile the filter
                using (BerkeleyPacketFilter filter = communicator.CreateFilter(filtre))
                {
                    // Set the filter
                    communicator.SetFilter(filter);
                }

                label2.Text = selectedDevice.Description;

                // start the capture
                communicator.ReceivePackets(0, PacketHandler);
            }

        }
        IpV4Datagram ip;
        UdpDatagram udp;
        TcpDatagram tcp;


        Dictionary<int, string> sozluk = new Dictionary<int, string>();
        Dictionary<int,Packet> paketler = new Dictionary<int,Packet>();
        //List<string> butun = new List<string>();
        int k = 0;

        private void PacketHandler(Packet packet)
        {
            // print timestamp and length of the packet
            // Console.WriteLine(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff") + " length:" + packet.Length);
            
            ip = packet.Ethernet.IpV4;
            udp = ip.Udp;
            tcp = ip.Tcp;
            if (ip != null)
            {
                // System.Windows.Forms.Form.CheckForIllegalCrossThreadCalls = false;

                Thread.Sleep(300);
                ListViewItem item = new ListViewItem(ip.Source.ToString());
                item.SubItems.Add(ip.Destination.ToString());
                item.SubItems.Add(packet.Timestamp.ToString("yyyy-MM-dd hh:mm:ss.fff"));
                item.SubItems.Add(packet.Length.ToString());
                item.SubItems.Add(ip.Protocol.ToString());
                listView1.Items.Add(item);




                paketler.Add(k,packet);
                k++;

            }



            // print ip addresses and udp ports
            // Console.WriteLine(ip.Source + ":" + udp.SourcePort + " -> " + ip.Destination + ":" + udp.DestinationPort);
        }


        private void Form2_Load(object sender, EventArgs e)
        {
          
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Source-Ip", 100);
            listView1.Columns.Add("Destination-Ip", 100);
            listView1.Columns.Add("TimetoLive", 100);
            listView1.Columns.Add("Length", 100);
            listView1.Columns.Add("Protocol", 100);
           

        }

        Thread start;

        private void button1_Click_1(object sender, EventArgs e)
        {
            listView1.Items.Clear();

            start = new Thread(() => PaketYakala());
            start.Start();
            button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text=="protocol")
            {
                start.Abort();
                listView1.Items.Clear();
                paketler.Clear();
                k = 0;
                start = new Thread(() => PaketYakala(textBox1.Text));
                start.Start();
            }
            if (comboBox1.Text == "source")
            {
                start.Abort();
                listView1.Items.Clear();
                paketler.Clear();
                k = 0;
                start = new Thread(() => PaketYakala("src "+textBox1.Text));
                start.Start();
            }
            if (comboBox1.Text == "destination")
            {
                start.Abort();
                listView1.Items.Clear();
                paketler.Clear();
                k = 0;
                start = new Thread(() => PaketYakala("dst " + textBox1.Text));
                start.Start();
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

            


            if (listView1.SelectedItems.Count > 0)
            {

                treeView1.Nodes.Clear();


                int a;
                a = (Int32.Parse(listView1.SelectedItems[0].Index.ToString()));




                ip = paketler[a].Ethernet.IpV4;
                udp = ip.Udp;
                tcp = ip.Tcp;
               

                if (ip.Protocol.ToString() == "Udp")
                {
                    TreeNode part1 = new TreeNode("destinationPort " + udp.DestinationPort.ToString());
                    TreeNode part2 = new TreeNode("sourcePort " + udp.SourcePort.ToString());
                    TreeNode part4 = new TreeNode("checksum " + udp.Checksum.ToString());

                    TreeNode part8 = new TreeNode("total length " + ip.TotalLength.ToString());
                    TreeNode part9 = new TreeNode("Identification " + ip.Identification.ToString());
                    TreeNode part10 = new TreeNode("time to live " + ip.Ttl.ToString());
                    TreeNode part11 = new TreeNode("Protocol " + ip.Protocol.ToString());
                    TreeNode part12 = new TreeNode("Header Checksum " + ip.HeaderChecksum.ToString());
                    TreeNode part13 = new TreeNode("source " + ip.Source.ToString());
                    TreeNode part14 = new TreeNode("destination " + ip.Destination.ToString());

                    Packet paket = paketler[a];

                    TreeNode part15 = new TreeNode("destination " + paket.Ethernet.Destination.ToString());
                    TreeNode part16 = new TreeNode("source " + paket.Ethernet.Source.ToString());
                    TreeNode part17 = new TreeNode("type " + paket.Ethernet.EtherType.ToString());

                    TreeNode part18 = new TreeNode("time " + paket.Timestamp.ToString());

                    TreeNode[] array1 = new TreeNode[] { part1, part2, part4 };
                    TreeNode[] array2 = new TreeNode[] { part8, part9, part10, part11, part12, part13, part14 };
                    TreeNode[] array3 = new TreeNode[] { part15, part16, part17 };
                    TreeNode[] array4 = new TreeNode[] { part18 };



                    TreeNode ekle1 = new TreeNode("Datagram", array1);
                    TreeNode ekle2 = new TreeNode("Internet Protocol Version", array2);
                    TreeNode ekle3 = new TreeNode("Ethernet", array3);
                    TreeNode ekle4 = new TreeNode("Frame", array4);

                    treeView1.Nodes.Add(ekle1);
                    treeView1.Nodes.Add(ekle2);
                    treeView1.Nodes.Add(ekle3);
                    treeView1.Nodes.Add(ekle4);



                }
                else if (ip.Protocol.ToString() == "Tcp")
                {
                    TreeNode part1 = new TreeNode("destinationPort " + tcp.DestinationPort.ToString());
                    TreeNode part2 = new TreeNode("sourcePort " + tcp.SourcePort.ToString());
                    TreeNode part3 = new TreeNode("Ack " + tcp.AcknowledgmentNumber.ToString());
                    TreeNode part4 = new TreeNode("checksum " + tcp.Checksum.ToString());
                    TreeNode part5 = new TreeNode("sequence number " + tcp.SequenceNumber.ToString());
                    TreeNode part6 = new TreeNode("window size " + tcp.Window.ToString());
                    TreeNode part7 = new TreeNode("urgent pointer " + tcp.UrgentPointer.ToString());

                    TreeNode part8 = new TreeNode("total length " + ip.TotalLength.ToString());
                    TreeNode part9 = new TreeNode("Identification " + ip.Identification.ToString());
                    TreeNode part10 = new TreeNode("time to live " + ip.Ttl.ToString());
                    TreeNode part11 = new TreeNode("Protocol " + ip.Protocol.ToString());
                    TreeNode part12 = new TreeNode("Header Checksum " + ip.HeaderChecksum.ToString());
                    TreeNode part13 = new TreeNode("source " + ip.Source.ToString());
                    TreeNode part14 = new TreeNode("destination " + ip.Destination.ToString());

                    Packet paket = paketler[a];

                    TreeNode part15 = new TreeNode("destination " + paket.Ethernet.Destination.ToString());
                    TreeNode part16 = new TreeNode("source " + paket.Ethernet.Source.ToString());
                    TreeNode part17 = new TreeNode("type " + paket.Ethernet.EtherType.ToString());

                    TreeNode part18 = new TreeNode("time " + paket.Timestamp.ToString());

                    TreeNode[] array1 = new TreeNode[] { part1, part2, part3, part4, part5, part6, part7 };
                    TreeNode[] array2 = new TreeNode[] { part8, part9, part10, part11, part12, part13, part14 };
                    TreeNode[] array3 = new TreeNode[] { part15, part16, part17 };
                    TreeNode[] array4 = new TreeNode[] { part18 };



                    TreeNode ekle1 = new TreeNode("Transmission", array1);
                    TreeNode ekle2 = new TreeNode("Internet Protocol Version", array2);
                    TreeNode ekle3 = new TreeNode("Ethernet", array3);
                    TreeNode ekle4 = new TreeNode("Frame", array4);

                    treeView1.Nodes.Add(ekle1);
                    treeView1.Nodes.Add(ekle2);
                    treeView1.Nodes.Add(ekle3);
                    treeView1.Nodes.Add(ekle4);
                }
            }


        }
        


    }

}
