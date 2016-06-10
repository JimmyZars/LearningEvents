using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace LearningEvents
{
    public partial class Form1 : Form
    {
        //Initialize Form
        public Form1()
        {
            //Show Form
            InitializeComponent();
            
            //Full screen window
            WindowState = FormWindowState.Maximized;

            //Map network drive to save files
            mapNetworkDrive();
        }

        //New Session Button Clicked
        private void addButton_Click(object sender, EventArgs e)
        {
            //Open a new session form
            newSessionForm sessionForm = new newSessionForm();
            sessionForm.Show();
        }

        //Open Session Button Clicked
        private void openButton_Click(object sender, EventArgs e)
        {
           //Nothing yet
        }

        //Map network drive to save files
        private void mapNetworkDrive()
        {
            //Run batch file to connect to network drive
            System.Diagnostics.Process.Start("LEMap.bat");
        }

        //Add a new session
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Open a new session form
            newSessionForm sessionForm = new newSessionForm();
            sessionForm.Show();
        }

        //Close the program
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
