using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace Communicate_with_Arduino
{
    public partial class RGB_LED_Controller : Form
    {
        const int MF_BYPOSITION = 0x400;
        [DllImport("User32")]
        private static extern int RemoveMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("User32")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("User32")]

        private static extern int GetMenuItemCount(IntPtr hWnd);

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);


        SerialPort port;
        bool isConnected = false;
        bool hidden = false;
        private string in_data;
        public RGB_LED_Controller()
        {
            InitializeComponent();
            controls(isConnected);
            string[] ports = SerialPort.GetPortNames();
            foreach (string thing in ports)
            {
                combo_COMPort.Items.Add(thing);
                if (ports[0] != null)
                    combo_COMPort.SelectedItem = ports[0];
                if (ports[1] != null)
                    combo_COMPort.SelectedItem = ports[1];
            }
            this.Text = string.Empty;
            this.ControlBox = false;

            trackBar1.Maximum = 255;
            trackBar2.Maximum = 255;
            trackBar3.Maximum = 255;
            trackBar4.Maximum = 255;
            trackBar5.Maximum = 255;
            trackBar6.Maximum = 255;

            Task.Delay(2000);
            connectToArduino();
            controls(isConnected);
            Task.Delay(500);
            combo_Profile.SelectedIndex = 1;
            trackBar4.Value = 255;
            trackBar5.Value = 255;
            Task.Delay(500);
            force_update();
        }
        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = combo_COMPort.GetItemText(combo_COMPort.SelectedItem);
            port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            port.DataReceived += port_Data_Received;
            port.Open();
            button1.Text = "Disconnect";
        }
        private void disconnectFromArduino()
        {
            if (isConnected)
            {
                port.Write(">00<");
                port.Close();
                isConnected = false;
            }
            button1.Text = "Connect";
        }

        private void RGB_LED_Controller_Load(object sender, EventArgs e)
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            int menuItemCount = GetMenuItemCount(hMenu);

            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
        }
        private void RGB_LED_Controller_Closed(object sender, FormClosedEventArgs e)
        {
            disconnectFromArduino();
        }
        private void RGB_LED_Controller_Resize(Object sender, FormClosedEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo_Profile.SelectedIndex == 2)
            {
                label2.Visible = true;
                label4.Visible = true;
                label5.Visible = true;
                trackBar1.Visible = true;
                trackBar2.Visible = true;
                trackBar3.Visible = true;
            }
            else
            {
                label2.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                trackBar1.Visible = false;
                trackBar2.Visible = false;
                trackBar3.Visible = false;
            }

            if (combo_Profile.SelectedIndex == 1 || combo_Profile.SelectedIndex == 3)
            {
                label6.Visible = true;
                label7.Visible = true;
                trackBar4.Visible = true;
                trackBar5.Visible = true;
            }
            else
            {
                label6.Visible = false;
                label7.Visible = false;
                trackBar4.Visible = false;
                trackBar5.Visible = false;
            }

            if (combo_Profile.SelectedIndex == 3)
            {
                label8.Visible = true;
                trackBar6.Visible = true;
            }
            else
            {
                label8.Visible = false;
                trackBar6.Visible = false;
            }
            force_update();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void controls(bool status)
        {
            button_update.Enabled = status;
            combo_Profile.Enabled = status;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            isConnected = !isConnected;
            if (isConnected)
                connectToArduino();
            else
                disconnectFromArduino();
            controls(isConnected);
        }

        private void button_update_Click(object sender, EventArgs e)
        {
            force_update();
        }

        private void force_update()
        {
            if (combo_Profile.SelectedIndex == 1)
                port.Write((">0" + combo_Profile.SelectedIndex.ToString() + "S" + trackBar4.Value.ToString() + "B" + trackBar5.Value.ToString() + "<"));
            else if (combo_Profile.SelectedIndex == 2)
                port.Write((">0" + combo_Profile.SelectedIndex.ToString() + "R" + trackBar1.Value.ToString() + "G" + trackBar2.Value.ToString() + "B" + trackBar3.Value.ToString() + "<"));
            else if (combo_Profile.SelectedIndex == 3)
                port.Write((">0" + combo_Profile.SelectedIndex.ToString() + "S" + trackBar4.Value.ToString() + "B" + trackBar5.Value.ToString() + "H" + trackBar6.Value.ToString() + "<"));
            else
                port.Write((">0" + combo_Profile.SelectedIndex.ToString() + "<"));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            disconnectFromArduino();
            this.Close();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (hidden)
            {
                this.WindowState = FormWindowState.Normal;
                Show();
            }
            else
            {
                Hide();
            }
            hidden = !hidden;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            disconnectFromArduino();
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            hidden = true;
            Hide();
        }

        private void RGB_LED_Controller_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        private void port_Data_Received(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                in_data = port.ReadLine();
                this.Invoke(new EventHandler(displaydata_event));
            }
            catch (Exception)
            {
                MessageBox.Show("exception caught");
            }
        }

        private void displaydata_event(object sender, EventArgs e)
        {
            textBox1.AppendText(in_data + "\n");
        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            force_update();
        }

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            force_update();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            force_update();
        }

        private void trackBar5_Scroll(object sender, EventArgs e)
        {
            force_update();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            force_update();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void trackBar6_Scroll(object sender, EventArgs e)
        {
            force_update();
        }
    }
}
