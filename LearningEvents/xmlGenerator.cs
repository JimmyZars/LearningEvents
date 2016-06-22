using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;


namespace LearningEvents
{
    public partial class xmlGenerator : Form
    {
        //Strings of XML output to be saved to file, and to be shown in the Output Window : Tempname = temporary naming string for creating events
        protected string xmlOutput, xmlOutputWindow, tempname;

        //Array Size
        protected static int rows = 50, cols = 50;

        //Array of XML Events
        xmlEvent[,] events = new xmlEvent[rows, cols];

        //Day number and event number used to keep track of event elements in the array
        protected int day, eventnum;

        //Creates the path to the course and/or session schedules
        string sessionSched = newSessionForm.sessionCodePath + "\\schedule\\agenda.xml", courseSched = newSessionForm.courseCodePath + "\\schedule\\agenda.xml";

        //Initialize Generator
        public xmlGenerator()
        {
            InitializeComponent();
            day = 1;
            eventnum = 1;
            daylabel.Text = "Day " + day;
            eventlabel.Text = "Event " + eventnum;
            textBox1.Focus();
           // label10.Visible = false;

            //If a session level schedule already exists
            if (File.Exists(sessionSched))
            {
                DialogResult result = MessageBox.Show("An XML schedule already exists at the session level for this course.  Would you like to open it?", "", MessageBoxButtons.YesNo);

                //Open existing xml
                if (result.Equals(DialogResult.Yes))
                {
                    openExistingFile();
                }
                //Open fresh xml
                else if (result.Equals(DialogResult.No))
                {
                    DialogResult warn = MessageBox.Show("This will overwrite the previous XML schedule.  Do you wish to continue?", "", MessageBoxButtons.YesNo);

                    if (warn.Equals(DialogResult.Yes))
                    {
                        //Open new form
                    }
                    else if (warn.Equals(DialogResult.No))
                    {
                        DialogResult exitForm = MessageBox.Show("Do you wish to exit the form?  If not the existing schedule will be loaded.", "", MessageBoxButtons.YesNo);
                        if (exitForm.Equals(DialogResult.Yes))
                        {
                            this.Close();
                        }
                        else if (exitForm.Equals(DialogResult.No))
                        {
                            //Open existing XML
                            openExistingFile();
                        }
                    }
                }
            }
            //If a course level schedule already exists
            else if (File.Exists(courseSched))
            {
                DialogResult result = MessageBox.Show("An XML schedule already exists at the course level for this course.  Would you like to open it?", "", MessageBoxButtons.YesNo);

                //Open existing xml
                if (result.Equals(DialogResult.Yes))
                {
                    openExistingFile();
                }
                //Open fresh xml
                else if (result.Equals(DialogResult.No))
                {
                    DialogResult warn = MessageBox.Show("This will overwrite the previous XML schedule.  Do you wish to continue?", "", MessageBoxButtons.YesNo);

                    if (warn.Equals(DialogResult.Yes))
                    {
                        //Open new form
                    }
                    else if (warn.Equals(DialogResult.No))
                    {
                        DialogResult exitForm = MessageBox.Show("Do you wish to exit the form?  If not the existing schedule will be loaded.", "", MessageBoxButtons.YesNo);
                        if (exitForm.Equals(DialogResult.Yes))
                        {
                            this.Close();
                        }
                        else if (exitForm.Equals(DialogResult.No))
                        {
                            //Open existing XML
                            openExistingFile();
                        }
                    }
                }
            }
            

        }

        //Next Event Button
        protected void nextEventButton_Click(object sender, EventArgs e)
        {
            bool error1 = false;

            error1 = saveEvent();

            if (error1)
            {
                //
            }
            else {
                xmlOutputWindow = generateXML();
                outputWindow.Text = xmlOutputWindow;
                eventnum++;
                try
                {
                    if (events[day, eventnum].name.Length > 0)
                    {
                        displayEvent();
                    }
                }
                catch (NullReferenceException)
                {
                    clearForm();
                }
            }

        }

        //previous button clicked
        protected void previousButton_Click(object sender, EventArgs e)
        {
            if (eventnum > 1)
            {
                eventnum--;
                eventlabel.Text = "Event " + eventnum;
                displayEvent();
            }
            else if ((eventnum == 1) && (day > 1))
            {
                day--;
                eventnum = eventCount(day);
                daylabel.Text = "Day " + day;
                eventlabel.Text = "Event " + eventnum;
                displayEvent();
            }
        }

        //Next day button clicked
        private void nextDayButton_Click(object sender, EventArgs e)
        {
            bool error1 = false;

            error1 = saveEvent();

            if (error1)
            {
                //
            }
            else {
                day++;
                eventnum = 1;
                try
                {
                    if (events[day, eventnum].name.Length > 0)
                    {
                        displayEvent();
                    }
                }
                catch (NullReferenceException)
                {
                    clearForm();
                }
            }
        }

        //Previous day button clicked
        protected void prevDay_Click(object sender, EventArgs e)
        {
            if (day > 1)
            {
                day--;
                eventnum = 1;
                displayEvent();
                textBox1.Focus();
            }
        }

        //Get the number of events for the current day
        protected int eventCount(int daynum)
        {
            int count = 0;
            for (int i = 0; i < rows; i++)
            {
                try
                {
                    if (events[daynum, i].name.Length > 0)
                    {
                        count++;
                    }
                    else
                    {

                    }
                }
                catch (NullReferenceException)
                {
                    //Do nothing
                }
                catch (IndexOutOfRangeException)
                {

                }
            }
            return count;
        }

        // Display the current event in the generator
        protected void displayEvent()
        {
            try
            {
                xmlEvent tempevent = new xmlEvent();
                tempevent = events[day, eventnum];
                textBox1.Text = tempevent.name;
                sHour.Text = tempevent.starttime.getHour();
                sMin.Text = tempevent.starttime.getMin();
                sDayTime.Text = tempevent.starttime.getDayTime();
                eHour.Text = tempevent.endtime.getHour();
                eMin.Text = tempevent.endtime.getMin();
                eDayTime.Text = tempevent.endtime.getDayTime();
                loc.Text = tempevent.location;
                daylabel.Text = "Day " + day;
                eventlabel.Text = "Event " + eventnum;
                textBox1.Focus();
            }
            catch (NullReferenceException)
            {

            }
        }

        //Save the current event info in the array
        protected bool saveEvent()
        {
            bool errorx = false;

            xmlEvent tempevent = new xmlEvent();
            if (textBox1.Text.Length < 1)
            {
                MessageBox.Show("Please enter a valid name", "Invalid Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                errorx = true;
            }
            else
            {
                tempevent.name = textBox1.Text;
            }
            events[day, eventnum] = tempevent;
            try
            {
                xmltime sTime = new xmltime();
                sTime.setHour(int.Parse(sHour.Text));
                sTime.setMin(int.Parse(sMin.Text));
                sTime.setDayTime(sDayTime.Text);
                tempevent.starttime = sTime;
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is NullReferenceException)
                {
                    MessageBox.Show("Incorrect Start Time", "Start Time Entry Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    errorx = true;
                }
            }
            try
            {
                xmltime eTime = new xmltime();
                eTime.setHour(int.Parse(eHour.Text));
                eTime.setMin(int.Parse(eMin.Text));
                eTime.setDayTime(eDayTime.Text);
                tempevent.endtime = eTime;
            }
            catch (Exception ex)
            {
                if (ex is FormatException || ex is NullReferenceException)
                {
                    MessageBox.Show("Incorrect End Time", "Start Time Entry Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                errorx = true;
            }
            tempevent.location = loc.Text;
            return errorx;
        }

        private void outputWindow_TextChanged(object sender, EventArgs e)
        {

        }

        //Create a string of XML output from the Events array
        protected string generateXML()
        {
            //Start of File
            string xmlgen = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<activity>\n";
            for (int i = 1; i < rows; i++)
            {
                try
                {
                    if (events[i, 1].name.Length > 0)
                    {
                        xmlgen += "     <day count=\"" + i + "\">\n";
                        for (int j = 1; j < cols; j++)
                        {
                            try
                            {
                                if (events[i, j].name.Length > 0)
                                {
                                    xmlgen += "          <event id=\"" + j + "\">\n";
                                    xmlgen += "               <name>" + events[i, j].name + "</name>\n";

                                    if (events[i, j].starttime.getDayTime() == "AM")
                                    {
                                        xmlgen += "               <starttime>" + events[i, j].starttime.getHour() + ":" + events[i, j].starttime.getMin() + "</starttime>\n";
                                    }
                                    else if (events[i, j].starttime.getDayTime() == "PM")
                                    {
                                        xmlgen += "               <starttime>" + getPM(events[i, j].starttime.getHour()) + ":" + events[i, j].starttime.getMin() + "</starttime>\n";
                                    }

                                    if (events[i, j].endtime.getDayTime() == "AM")
                                    {
                                        xmlgen += "               <endtime>" + events[i, j].endtime.getHour() + ":" + events[i, j].endtime.getMin() + "</endtime>\n";
                                    }
                                    else if (events[i, j].endtime.getDayTime() == "PM")
                                    {
                                        xmlgen += "               <endtime>" + getPM(events[i, j].endtime.getHour()) + ":" + events[i, j].endtime.getMin() + "</endtime>\n";
                                    }
                                    xmlgen += "               <location>" + events[i, j].location + "</location>\n";
                                    xmlgen += "          </event>\n";
                                }
                            }
                            catch (NullReferenceException)
                            {
                                //No more events
                            }

                        }

                    }
                    xmlgen += "     </day>\n";
                }
                catch (NullReferenceException)
                {
                    //No data
                }

            }
            xmlgen += "</activity>";
            xmlgen = xmlgen.Replace("&", "&amp;");
            return xmlgen;
        }

        //Convert PM hour to military time
        protected string getPM(string am)
        {
            string pm = "";
            int l = int.Parse(am);
            int p = 0;
            for (int i = 1; i < 12; i++)
            {
                if (l == i)
                {
                    p = i + 12;
                    pm = "" + p;
                    return pm;
                }
                else if (am == "12")
                {
                    pm = "12";
                    return pm;
                }
            }
            return am;
        }

        //Delete button clicked (deletes current event)
        private void deleteButton_Click(object sender, EventArgs e)
        {

            for (int i = eventnum; i < rows; i++)
            {

                try
                {

                    if (eventnum > 1)
                    {

                        events[day, i].name = events[day, i + 1].name;
                        events[day, i + 1].name = null;
                        events[day, i + 1].starttime = null;
                        events[day, i + 1].endtime = null;
                        events[day, i + 1].location = null;
                        if (events[day, eventnum].name.Length > 0)
                        {
                            eventnum--;
                            displayEvent();
                            daylabel.Text = "Day " + day;
                            eventlabel.Text = "Event " + eventnum;
                            textBox1.Focus();
                        }
                        else
                        {
                            clearForm();
                        }
                    }
                    else
                    {
                        events[day, i].name = null;
                        events[day, i].starttime = null;
                        events[day, i].endtime = null;
                        events[day, i].location = null;

                        clearForm();
                    }
                }
                catch (NullReferenceException)
                {

                    try
                    {

                        if (events[day, eventnum].name.Length > 0)
                        {
                            if (eventnum > 1)
                            {
                                events[day, i].name = null;
                                events[day, i].starttime = null;
                                events[day, i].endtime = null;
                                events[day, i].location = null;
                                eventnum--;
                                displayEvent();
                                textBox1.Focus();
                                daylabel.Text = "Day " + day;
                                eventlabel.Text = "Event " + eventnum;
                            }
                        }
                        else
                        {
                            clearForm();
                        }
                    }
                    catch (NullReferenceException)
                    {

                        clearForm();
                    }
                    catch (IndexOutOfRangeException)
                    {
                        clearForm();
                    }
                }

                catch (IndexOutOfRangeException)
                {

                    try
                    {

                        if (events[day, eventnum].name.Length > 0)
                        {
                            eventnum--;
                            displayEvent();
                            daylabel.Text = "Day " + day;
                            eventlabel.Text = "Event " + eventnum;
                            textBox1.Focus();
                        }
                        else
                        {
                            clearForm();
                        }
                    }
                    catch (NullReferenceException)
                    {
                    }
                    catch (IndexOutOfRangeException)
                    {
                    }
                }
            }

            xmlOutputWindow = generateXML();
            outputWindow.Text = xmlOutputWindow;
        }

        //Finish button clicked (Prompts user to save the xml file)
        private void finishButton_Click_1(object sender, EventArgs e)
        {
            bool error1 = false;

            error1 = saveEvent();

            xmlOutput = generateXML();
            outputWindow.Text = xmlOutput;
            //  writetofile(xmlOutput);
            SaveFileDialog saveFile;

            //If at session level
            if (newSessionForm.sessionLevel)
            {
                saveFile = new SaveFileDialog();
                saveFile.FileName = sessionSched;
            }
            //Course level
            else
            {
                saveFile = new SaveFileDialog();
                saveFile.FileName = courseSched;
            }

            //If session level
            if (newSessionForm.sessionLevel)
            {
                //If file already exists
                if (File.Exists(sessionSched))
                {
                    DialogResult over = MessageBox.Show("Do you wish to overwrite the old XML schedule?", "", MessageBoxButtons.YesNo);
                    if (over.Equals(DialogResult.Yes))
                    {
                        //Overwrite file
                        System.IO.File.WriteAllText(saveFile.FileName, xmlOutput);
                        MessageBox.Show("The XML Schedule has been created successfully");
                    }
                    else
                    {

                    }
                }
                else
                {
                    //Save new file
                    System.IO.File.WriteAllText(saveFile.FileName, xmlOutput);
                    MessageBox.Show("The XML Schedule has been created successfully");
                    this.Close();
                }
            }
            else
            //course level
            {
                //If file exists
                if (File.Exists(courseSched))
                {
                    DialogResult over = MessageBox.Show("Do you wish to overwrite the old XML schedule?", "", MessageBoxButtons.YesNo);
                    if (over.Equals(DialogResult.Yes))
                    {
                        //Overwrite file
                        System.IO.File.WriteAllText(saveFile.FileName, xmlOutput);
                        MessageBox.Show("The XML Schedule has been created successfully");
                        this.Close();
                    }
                    else
                    {

                    }
                }
                else
                {
                    //Save new file
                    System.IO.File.WriteAllText(saveFile.FileName, xmlOutput);
                    MessageBox.Show("The XML Schedule has been created successfully");
                    this.Close();
                }
            }
            /*
            //Start progress bar
            progressBar1.Visible = true;
            progressBar1.Minimum = 1;
            progressBar1.Maximum = 3;
            label11.Visible = true;

            //Zip files
            //zipFiles();
           */
        }

        //Puts files into a zip folder (No longer needed)
        /*      private async void zipFiles()
              {

                  // End of File
                  string startPath = "C:\\ACLFiles\\";
                  string zipPath = "C:\\ZipFile\\ACLFiles.zip";
                  if (Directory.Exists(zipPath))
                  {
                      //do nothing
                  }
                  else
                  {
                      System.IO.Directory.CreateDirectory(zipPath);
                  }
                  await Task.Run(() =>
                  {
                      ZipFile.CreateFromDirectory(startPath, zipPath);
                  });
                  label11.Visible = false;
                  progressBar1.PerformStep();
                  label10.Visible = true;
              } */

        //Clear the current event form
        private void clearForm()
        {
            xmlOutputWindow = generateXML();
            outputWindow.Text = xmlOutputWindow;
            textBox1.Text = String.Empty;
            loc.Text = String.Empty;
            sHour.Text = String.Empty;
            sMin.Text = String.Empty;
            eMin.Text = String.Empty;
            eHour.Text = String.Empty;
            daylabel.Text = "Day " + day;
            eventlabel.Text = "Event " + eventnum;
            textBox1.Focus();
        }

        //Allows users to use arrow keys to navigate events
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Right)
            {
                nextEventButton.PerformClick();
                return true;
            }
            else if (keyData == Keys.Left)
            {
                previousButton.PerformClick();
                return true;
            }

            else if (keyData == Keys.Delete)
            {
                deleteButton.PerformClick();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        //Open an existing xml file, parse through the file and generate the array to edit
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearArray();

            xmlEvent[,] createEvents = new xmlEvent[rows, cols];
            xmltime[,] startTimes = new xmltime[rows, cols];
            xmltime[,] endTimes = new xmltime[rows, cols];

            OpenFileDialog openFile = new OpenFileDialog();

            openFile.InitialDirectory = Convert.ToString(Environment.SpecialFolder.MyDocuments);
            openFile.Filter = "Text Files (.xml)|*.xml|All Files (*.*)|*.*";
            openFile.FilterIndex = 1;
            openFile.Multiselect = false;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader fileReader = new System.IO.StreamReader(openFile.FileName);

                //Parse String
                string nextLine = "";
                string dayEx = "<day count=\"(.*?)\">";
                string eventEx = "<event id=\"(.*?)\">";
                string nameEx = "<name>(.*?)</name>";
                string startHourEx = "<starttime>(.*?):";
                string startMinEx = ":(.*?)</starttime>";
                string endHourEx = "<endtime>(.*?):";
                string endMinEx = ":(.*?)</endtime>";
                string locEx = "<location>(.*?)</location>";

                while ((nextLine = fileReader.ReadLine()) != null)
                {
                    Match matchday = Regex.Match(nextLine, dayEx);
                    if (matchday.Success)
                    {
                        string newDay = matchday.Groups[1].Value;
                        day = int.Parse(newDay);
                    }
                    Match matchevent = Regex.Match(nextLine, eventEx);
                    if (matchevent.Success)
                    {
                        createEvents[day, eventnum] = new xmlEvent(); ;
                        string newEvent = matchevent.Groups[1].Value;
                        try
                        {
                            eventnum = int.Parse(newEvent);
                        }
                        catch (NullReferenceException)
                        {

                        }


                    }
                    Match matchname = Regex.Match(nextLine, nameEx);
                    if (matchname.Success)
                    {
                        string newName = matchname.Groups[1].Value;

                        //   try {

                        xmlEvent tempEvent = new xmlEvent();
                        createEvents[day, eventnum] = tempEvent;
                        createEvents[day, eventnum].setName(newName);
                        events[day, eventnum] = createEvents[day, eventnum];

                    }
                    //Get Start Time Hour
                    Match matchstarth = Regex.Match(nextLine, startHourEx);
                    if (matchstarth.Success)
                    {
                        string newHour = matchstarth.Groups[1].Value;

                        xmltime tempTime = new xmltime();
                        startTimes[day, eventnum] = tempTime;
                        startTimes[day, eventnum].setHour(int.Parse(newHour));
                        if (int.Parse(newHour) < 12)
                        {
                            startTimes[day, eventnum].setDayTime("AM");
                        }
                        else
                        {
                            startTimes[day, eventnum].setDayTime("PM");
                            if (int.Parse(newHour) > 12)
                            {
                                startTimes[day, eventnum].setHour(int.Parse(newHour) - 12);
                            }
                        }
                        createEvents[day, eventnum].starttime = startTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];

                    }
                    //Get Start Time Minute
                    Match matchstartm = Regex.Match(nextLine, startMinEx);
                    if (matchstartm.Success)
                    {
                        string newMin = matchstartm.Groups[1].Value;

                        startTimes[day, eventnum].setMin(int.Parse(newMin));

                        createEvents[day, eventnum].starttime = startTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];
                    }
                    //Get End Time Hour
                    Match matchendh = Regex.Match(nextLine, endHourEx);
                    if (matchendh.Success)
                    {
                        string newHour = matchendh.Groups[1].Value;

                        xmltime tempTime = new xmltime();
                        endTimes[day, eventnum] = tempTime;
                        endTimes[day, eventnum].setHour(int.Parse(newHour));
                        if (int.Parse(newHour) < 12)
                        {
                            endTimes[day, eventnum].setDayTime("AM");
                        }
                        else
                        {
                            endTimes[day, eventnum].setDayTime("PM");
                            if (int.Parse(newHour) > 12)
                            {
                                endTimes[day, eventnum].setHour(int.Parse(newHour) - 12);
                            }
                        }
                        createEvents[day, eventnum].endtime = endTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];
                    }
                    //Get End Time Minute
                    Match matchendm = Regex.Match(nextLine, endMinEx);
                    if (matchendm.Success)
                    {
                        string newMin = matchendm.Groups[1].Value;

                        endTimes[day, eventnum].setMin(int.Parse(newMin));
                        createEvents[day, eventnum].endtime = endTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];
                    }
                    //Get Location
                    Match matchloc = Regex.Match(nextLine, locEx);
                    if (matchloc.Success)
                    {
                        string newLoc = matchloc.Groups[1].Value;

                        createEvents[day, eventnum].location = newLoc;
                        events[day, eventnum] = createEvents[day, eventnum];
                    }

                }


            }
            day = 1;
            eventnum = 1;
            displayEvent();
            xmlOutputWindow = generateXML();
            outputWindow.Text = xmlOutputWindow;
        }

        private void openExistingFile()
        {
            clearArray();

            xmlEvent[,] createEvents = new xmlEvent[rows, cols];
            xmltime[,] startTimes = new xmltime[rows, cols];
            xmltime[,] endTimes = new xmltime[rows, cols];

            System.IO.StreamReader fileReader;
            if (newSessionForm.sessionLevel)
            {
                //load session sched
                fileReader = new System.IO.StreamReader(sessionSched);
            }
            else
            {
                //load course sched
                fileReader = new System.IO.StreamReader(courseSched);
            }

            if (File.Exists(sessionSched))
            {

                //Parse String
                string nextLine = "";
                string dayEx = "<day count=\"(.*?)\">";
                string eventEx = "<event id=\"(.*?)\">";
                string nameEx = "<name>(.*?)</name>";
                string startHourEx = "<starttime>(.*?):";
                string startMinEx = ":(.*?)</starttime>";
                string endHourEx = "<endtime>(.*?):";
                string endMinEx = ":(.*?)</endtime>";
                string locEx = "<location>(.*?)</location>";

                while ((nextLine = fileReader.ReadLine()) != null)
                {
                    Match matchday = Regex.Match(nextLine, dayEx);
                    if (matchday.Success)
                    {
                        string newDay = matchday.Groups[1].Value;
                        day = int.Parse(newDay);
                    }
                    Match matchevent = Regex.Match(nextLine, eventEx);
                    if (matchevent.Success)
                    {
                        createEvents[day, eventnum] = new xmlEvent(); ;
                        string newEvent = matchevent.Groups[1].Value;
                        try
                        {
                            eventnum = int.Parse(newEvent);
                        }
                        catch (NullReferenceException)
                        {

                        }


                    }
                    Match matchname = Regex.Match(nextLine, nameEx);
                    if (matchname.Success)
                    {
                        string newName = matchname.Groups[1].Value;

                        //   try {

                        xmlEvent tempEvent = new xmlEvent();
                        createEvents[day, eventnum] = tempEvent;
                        createEvents[day, eventnum].setName(newName);
                        events[day, eventnum] = createEvents[day, eventnum];

                    }
                    //Get Start Time Hour
                    Match matchstarth = Regex.Match(nextLine, startHourEx);
                    if (matchstarth.Success)
                    {
                        string newHour = matchstarth.Groups[1].Value;

                        xmltime tempTime = new xmltime();
                        startTimes[day, eventnum] = tempTime;
                        startTimes[day, eventnum].setHour(int.Parse(newHour));
                        if (int.Parse(newHour) < 12)
                        {
                            startTimes[day, eventnum].setDayTime("AM");
                        }
                        else
                        {
                            startTimes[day, eventnum].setDayTime("PM");
                            if (int.Parse(newHour) > 12)
                            {
                                startTimes[day, eventnum].setHour(int.Parse(newHour) - 12);
                            }
                        }
                        createEvents[day, eventnum].starttime = startTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];

                    }
                    //Get Start Time Minute
                    Match matchstartm = Regex.Match(nextLine, startMinEx);
                    if (matchstartm.Success)
                    {
                        string newMin = matchstartm.Groups[1].Value;

                        startTimes[day, eventnum].setMin(int.Parse(newMin));

                        createEvents[day, eventnum].starttime = startTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];
                    }
                    //Get End Time Hour
                    Match matchendh = Regex.Match(nextLine, endHourEx);
                    if (matchendh.Success)
                    {
                        string newHour = matchendh.Groups[1].Value;

                        xmltime tempTime = new xmltime();
                        endTimes[day, eventnum] = tempTime;
                        endTimes[day, eventnum].setHour(int.Parse(newHour));
                        if (int.Parse(newHour) < 12)
                        {
                            endTimes[day, eventnum].setDayTime("AM");
                        }
                        else
                        {
                            endTimes[day, eventnum].setDayTime("PM");
                            if (int.Parse(newHour) > 12)
                            {
                                endTimes[day, eventnum].setHour(int.Parse(newHour) - 12);
                            }
                        }
                        createEvents[day, eventnum].endtime = endTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];
                    }
                    //Get End Time Minute
                    Match matchendm = Regex.Match(nextLine, endMinEx);
                    if (matchendm.Success)
                    {
                        string newMin = matchendm.Groups[1].Value;

                        endTimes[day, eventnum].setMin(int.Parse(newMin));
                        createEvents[day, eventnum].endtime = endTimes[day, eventnum];
                        events[day, eventnum] = createEvents[day, eventnum];
                    }
                    //Get Location
                    Match matchloc = Regex.Match(nextLine, locEx);
                    if (matchloc.Success)
                    {
                        string newLoc = matchloc.Groups[1].Value;

                        createEvents[day, eventnum].location = newLoc;
                        events[day, eventnum] = createEvents[day, eventnum];
                    }

                }


            }
            day = 1;
            eventnum = 1;
            displayEvent();
            xmlOutputWindow = generateXML();
            outputWindow.Text = xmlOutputWindow;
        }
        //Exit selected, exit the program

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Clear the old content when new or open is selected
        private void clearArray()
        {
            for (int i = 1; i < 50; i++)
            {
                for (int j = 1; j < 50; j++)
                {
                    events[i, j] = null;
                }
            }
        }

        //New selected, clear the current form and the array for a new file
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            clearArray();
            eventnum = 1;
            day = 1;
            clearForm();
            xmlOutputWindow = generateXML();
            outputWindow.Text = xmlOutputWindow;
        }


    }
}
