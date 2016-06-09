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
    }

    public partial class newSessionForm : Form
    {
        //new session variables
        private string getCourseNum = "";
        private string getSessionNum = "";
        private string getCourseName = "";

        //List of files for index.html
        string[] filearray, filepaths;

        //Arrays for course codes and names
        string[] courselistarray = new string[100];
        string[] coursecodearray = new string[100];

        //Lists to display in dropdowns
        List<string> courseList = new List<string>();
        List<string> filelist = new List<string>();

        //File paths for folder structures
        public static string courseCodePath, sessionCodePath, coursesString;

        //Tells if session level is checked
        public static bool sessionLevel = false;       
         
        //Initialize new session form        
        public newSessionForm()
        {
            InitializeComponent();

            //Course level radio button checked by default
            radioButton1.Checked = true;

            //New Course labels and textboxes hidden by default
            label2.Visible = false;
            label3.Visible = false;
            courseCode.Visible = false;
            courseName.Visible = false;

            //Create list of courses from text file
            createCourses();
        }

        //Existing Course radio button
        private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
        {
            //Existing Course Radio Button is checked
            if (radioButton1.Checked)
            {
                //Hide new course labels and text boxes
                label2.Visible = false;
                label3.Visible = false;
                courseCode.Visible = false;
                courseName.Visible = false;
            }
        }

        //New Course radio button
        private void radioButton2_CheckedChanged(object sender, System.EventArgs e)
        {
            //New Course Radio Button is checked
            if (radioButton2.Checked)
            {
                //Show new course labels and text boxes
                label2.Visible = true;
                label3.Visible = true;
                courseCode.Visible = true;
                courseName.Visible = true;
            }
        }

        //Session level checked
        private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBox1.Checked)
            {
                //Show Session Number label and text box
                label4.Visible = true;
                sessionCode.Visible = true;
                sessionLevel = true;
            }
            else
            {
                //Hide Session Number label and text box
                label4.Visible = false;
                sessionCode.Visible = false;
                sessionLevel = false;
            }
        }

        //Cancel Button
        private void button2_Click(object sender, System.EventArgs e)
        {
            //Close Window
            this.Close();
        }

        // Next button clicked
        private void button1_Click(object sender, System.EventArgs e)
        {

            //New Course Created
            if (radioButton2.Checked)
            {
                if (coursecodearray.Contains(courseCode.Text))
                {
                    MessageBox.Show("That course code already exists");
                }
                else {
                    try
                    {
                        //Assign Course Name
                        getCourseName = courseName.Text;
                        coursesString += "xx" + courseName.Text + "xx  yy" + courseCode.Text + "yy";
                        System.IO.File.WriteAllText("courses.txt", coursesString);
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Invalid Course Name");
                    }
                    try
                    {
                        //Assign Course Code
                        getCourseNum = courseCode.Text;
                    }
                    catch (NullReferenceException)
                    {
                        MessageBox.Show("Invalid Course Number");
                    }

                    //Hide first form and show the second form
                    panel2.Visible = false;
                    panel1.Visible = true;
                    button1.Visible = false;
                    button2.Visible = false;

                }
            }
            //Existing course selected from drop down list
            else if (radioButton1.Checked)
            {

                try
                {
                    //   Assign Course Name
                    int x = Array.IndexOf(courselistarray, comboBox1.Text);
                    getCourseNum = coursecodearray[x];

                }
                catch (NullReferenceException)
                {
                    MessageBox.Show("Invalid Course Name");
                }

                //Hide the first form and show the second form
                panel2.Visible = false;
                panel1.Visible = true;
                button1.Visible = false;
                button2.Visible = false;
            }
            //Session code entered
            if (checkBox1.Checked)
            {
                //Assign Session Number
                getSessionNum = sessionCode.Text;
            }
            createFolders();
            if (System.IO.File.Exists(sessionCodePath + "\\materials\\index.html"))
            {
                populateIndex();
            }
            else
            {

            }
        }

        //Previous Button Clicked
        private void prevButton_Click(object sender, System.EventArgs e)
        {
            //Display the previous form
            panel1.Visible = false;
            panel2.Visible = true;
        }

        //Add files for upload
        private void addFilesButton_Click(object sender, System.EventArgs e)
        {
            OpenFileDialog addFiles = new OpenFileDialog();
            addFiles.Multiselect = true;
            addFiles.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
            if (addFiles.ShowDialog() == DialogResult.OK)
            {
                filearray = addFiles.SafeFileNames;
                filepaths = addFiles.FileNames;
            }
            foreach (string i in filearray)
            {
                filelist.Add(i);
            }
            listBox1.DataSource = filelist;
        }

        //Delete file from upload list
        private void deleteFile_Click(object sender, System.EventArgs e)
        {
            string temp = (string)listBox1.SelectedValue;
            filelist.Remove(temp);
            listBox1.DataSource = null; 
            listBox1.DataSource = filelist;
            listBox1.Update();
        }

        //Upload button clicked
        private void uploadButton_Click(object sender, System.EventArgs e)                                                                     
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;

            //Create Index File with links
            SaveFileDialog saveIndex = new SaveFileDialog();
            string indexString = "";
            indexString += "<!DOCTYPE html>\n";
            indexString += "<html>\n";
            indexString += "<body>";
            indexString += "<body lang=EN-US link=blue vlink=purple style='tab-interval:.5in'> ";
            indexString += "<ul style=\"list - style - type:circle\"> ";

            //ADD LINKS
            try {
                foreach (string i in filearray)
                {
                    if (i == "index.html")
                    {
                        //Ignore index.html
                    }
                    else {
                        indexString += "<li><a href=\"" + i + "\" target=\"_blank\">" + i.Remove(i.Length - 4) + "</a></li>";
                    }
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("No Materials were added");
            }

            //Save file
            if (sessionLevel)
            {
                saveIndex.FileName = sessionCodePath + "\\materials\\index.html";
            }
            else
            {
                saveIndex.FileName = courseCodePath + "\\materials\\index.html";
            }

            indexString += "</ul>";
            indexString += "</body>";
            indexString += "</html>";
            System.IO.File.WriteAllText(saveIndex.FileName, indexString);
        }

        //Create folder structure
        private void createFolders()
        {
            //Check if successfully mapped to network drive
            if (!System.IO.Directory.Exists("L:"))
            {
                //If no connection ask if they want to store locally
                DialogResult dResult = MessageBox.Show("Unable to establish a connection to the network drive.  Would you like select a local folder to store your files?", "Unable to Connect", MessageBoxButtons.YesNo);
                if(dResult == DialogResult.Yes)
                {
                    //Get local root folder
                    FolderBrowserDialog fbd = new FolderBrowserDialog();

                    DialogResult result = fbd.ShowDialog();

                    courseCodePath = fbd.SelectedPath + "\\" + getCourseNum;
                }
                else
                {
                    //Close the form
                    this.Close();
                }
            }else
            {
                //Set course code path to newly mapped network drive
                courseCodePath = "L:\\" + getCourseNum;
            }

            if (!System.IO.Directory.Exists(courseCodePath))
            {
                //Create folders
                System.IO.Directory.CreateDirectory(courseCodePath);
                System.IO.Directory.CreateDirectory(courseCodePath + "\\materials");
                System.IO.Directory.CreateDirectory(courseCodePath + "\\schedule");
            }

            if (getSessionNum.Length > 0)
            {
                MessageBox.Show("Here");

                sessionCodePath = courseCodePath + "\\" + getSessionNum;
                if (!System.IO.Directory.Exists(sessionCodePath))
                {
                    System.IO.Directory.CreateDirectory(sessionCodePath);
                    System.IO.Directory.CreateDirectory(sessionCodePath + "\\materials");
                    System.IO.Directory.CreateDirectory(sessionCodePath + "\\schedule");

                    try {
                        int x = 0;
                        foreach (string i in filepaths)
                        {
                            System.IO.File.Copy(i, sessionCodePath + "\\materials\\" + filearray[x], true);
                            x++;
                        }
                    }
                    catch (NullReferenceException)
                    {

                    }
                }
            }
            else
            {
                int x = 0;
                try {
                    foreach (string i in filepaths)
                    {
                        System.IO.File.Copy(i, courseCodePath + "\\materials\\" + filearray[x], true);
                        x++;
                    }
                }catch (NullReferenceException)
                {

                }
            }
            MessageBox.Show(sessionCodePath);

        }

        //Panel 3 prev button clicked
        private void button3_Click(object sender, System.EventArgs e)
        {
            panel3.Visible = false;
            panel1.Visible = true;
        }

        //exit form and open xml generator
        private void finishButton_Click(object sender, System.EventArgs e)
        {
            this.Close();
            xmlGenerator xmlForm = new xmlGenerator();
            xmlForm.Show();
        }

        //List of courses and course codes in drop down list
        private void createCourses()
        {
            System.IO.StreamReader fileReader = new System.IO.StreamReader("courses.txt");
            string nextLine = "";
            string thiscoursename = "xx(.*?)xx";
            string thiscoursecode = "yy(.*?)yy";
            int count = 0;
            while ((nextLine = fileReader.ReadLine()) != null)
            {
                    Match matchcoursename = Regex.Match(nextLine, thiscoursename);
                    if (matchcoursename.Success)
                    {
                        string tempName = matchcoursename.Groups[1].Value;
                        courselistarray[count] = tempName;
                        coursesString += "xx" + tempName + "xx     ";
                    }
                    Match matchcoursecode = Regex.Match(nextLine, thiscoursecode);
                    if (matchcoursecode.Success)
                    {
                        string tempCode = matchcoursecode.Groups[1].Value;
                        coursecodearray[count] = tempCode;
                        coursesString += "yy" + tempCode + "yy     " + Environment.NewLine;
                    }
                    count++;
 
            }

            for (int i = 0; i < courselistarray.Length; i++)
            {
                courseList.Add(courselistarray[i]);
            }
            comboBox1.DataSource = courseList;
            comboBox1.Text = courseList[0];
            fileReader.Close();
        }

        //populate the content from index.html file into the form
        private void populateIndex()
        {
            if (sessionLevel)
            {
                string materialsPath = sessionCodePath + "\\Materials";
                filepaths = Directory.GetFiles(materialsPath);

                for (int i = 0; i < filepaths.Length; i++)
                {
                    OpenFileDialog filei = new OpenFileDialog();
                    filei.FileName = filepaths[i];
                    filearray[i] = filei.FileName;
                }

                foreach (string i in filearray)
                {
                    filelist.Add(i);
                }
                listBox1.DataSource = filelist;
            }
            else
            {
                string materialsPath = courseCodePath + "\\Materials";
                filepaths = Directory.GetFiles(materialsPath);

                for(int i=0; i<filepaths.Length; i++)
                {
                    OpenFileDialog filei = new OpenFileDialog();
                    filei.FileName = filepaths[i];
                    filearray[i] = filei.SafeFileName;
                }

                foreach (string i in filearray)
                {
                    filelist.Add(i);
                }
                listBox1.DataSource = filelist;

            }
        }
    }
}
