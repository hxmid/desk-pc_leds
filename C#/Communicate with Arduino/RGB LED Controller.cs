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

        SerialPort port;
        bool isConnected = false;
        public RGB_LED_Controller()
        {
            InitializeComponent();
            controls(isConnected);
            string[] ports = SerialPort.GetPortNames();
            numericUpDown1.Maximum = 1000000;
            foreach (string thing in ports)
            {
                combo_COMPort.Items.Add(thing);
                if (ports[0] != null)
                {
                    combo_COMPort.SelectedItem = ports[0];
                }
            }
        }
        private void connectToArduino()
        {
            isConnected = true;
            string selectedPort = combo_COMPort.GetItemText(combo_COMPort.SelectedItem);
            int baud_rate = Convert.ToInt32(Math.Round(numericUpDown1.Value));
            port = new SerialPort(selectedPort, baud_rate, Parity.None, 8, StopBits.One);
            port.Open();
            button1.Text = "Disconnect";
        }
        private void RGB_LED_Controller_FormClosed(Object sender, FormClosedEventArgs e)
        {
            disconnectFromArduino();
        }
        private void disconnectFromArduino()
        {
            isConnected = false;
            port.Write("0");
            port.Close();
            button1.Text = "Connect";
        }

        private void RGB_LED_Controller_Load(object sender, EventArgs e)
        {
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            int menuItemCount = GetMenuItemCount(hMenu);

            RemoveMenu(hMenu, menuItemCount - 1, MF_BYPOSITION);
        }

        private void label1_Click(object sender, EventArgs e)
        {

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

        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
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
            port.Write((combo_Profile.SelectedIndex).ToString());
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            disconnectFromArduino();
            this.Close();
        }
    }
}
