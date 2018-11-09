using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace FilterEmails
{
    public delegate void StatusProgressBarHandler(String status,UInt16 progressValue,Boolean isFilterButtonVisible,Boolean isCancelButtonVisible, Boolean isStatisticsButtonVisible);

    class FilterEmailsController
    {
        private static FilterEmailsController instanceFilterEmailsController = null;
        private static readonly object padlock = new object();

        private FileReader instanceFileReader;
        private FilterEmailsData instanceFilterEmailsData;
        private UInt64 currentFileCounter = 0;
        private UInt64 noOfFiles = 0;
        private Thread currentThread;
        private String outputPath= String.Empty;
        private UInt64 noOfEmailsActuallyDumped = 0;

        public event StatusProgressBarHandler UpdateStatusProgressBar;       

        public static FilterEmailsController Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instanceFilterEmailsController == null)
                    {
                        instanceFilterEmailsController = new FilterEmailsController();
                    }
                    return instanceFilterEmailsController;
                }
            }
        }


        public void abort()
        {
            if (null != currentThread && currentThread.IsAlive)
            {
                currentThread.Abort();
                currentThread = null;
            }
        }

        public void execute()
        {
            outputPath  = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                            "FilterEmails - Merge" + DateTime.Now.ToString("Mdyyyyhhmmss") + ".txt");

            var stopwatch = Stopwatch.StartNew();
            reset();
            
            currentThread = new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;

                if (String.Compare(InputData.FilteringOn, "File") == 0)
                {
                    noOfFiles = 1;
                    instanceFileReader.read(InputData.EmailLocation);
                    ++currentFileCounter;
                }
                else
                {
                    noOfFiles = UInt64.Parse(Directory.EnumerateFiles(InputData.EmailLocation, "*.txt", SearchOption.AllDirectories).Count().ToString());
                    DirSearch(InputData.EmailLocation);
                }

                write();

                if (null != UpdateStatusProgressBar)
                {
                    UpdateStatusProgressBar(currentFileCounter + " of " + noOfFiles + " File Successfully Filtered.",
                        100,
                        true,
                        false,
                        true);

                    String note = InputData.KeepAtLeastDomain ? "Note : Emails whose domains are repeating " + InputData.NoOfDomainsToKeep + " times are only dumped" : String.Empty;
                    
                    MessageBox.Show(
                        "Run Summary" + Environment.NewLine + Environment.NewLine +
                        "Time Taken : " + (stopwatch.ElapsedMilliseconds / 1000) * 60 + " Minutes"  + Environment.NewLine +
                        "Output : " + outputPath + Environment.NewLine + Environment.NewLine +
                        "Extracted Emails : " + instanceFilterEmailsData.getNumberOfEmailsExtracted() + Environment.NewLine +
                        "Duplicated Emails : " + instanceFilterEmailsData.getNumberOfDuplicateEmails() + Environment.NewLine +
                        "Emails Removed : " + instanceFilterEmailsData.getNumberOfEmailsRemoved() + Environment.NewLine +
                        "Valid Emails : " + instanceFilterEmailsData.getNumberOfValidEmails() + Environment.NewLine +
                        "Dumped Emails : " + noOfEmailsActuallyDumped + Environment.NewLine + Environment.NewLine +
                        note,
                        "Email Filter Tool", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            });
            currentThread.Start();
        }

        private void write() 
        {            
            Dictionary<String,String> filteredEmails = instanceFilterEmailsData.getFilterEmailsData();

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(outputPath))
            {
                if (InputData.KeepAtLeastDomain)
                {
                    var lookup = filteredEmails.ToLookup(x => x.Value, x => x.Key).Where(x => x.Count() >= InputData.NoOfDomainsToKeep);

                    UInt64 counter = 0;
                    foreach (var item in lookup)
                    {
                        String textToWrite = item.Aggregate("", (s, v) => s + Environment.NewLine + v);
                        file.WriteLine(textToWrite);
                        
                        ++counter;
                        noOfEmailsActuallyDumped += Convert.ToUInt64(textToWrite.Count(x => x == '@'));

                        if (null != UpdateStatusProgressBar)
                        {
                            UpdateStatusProgressBar("Dumping in chunks [" + counter + "], please wait ...",
                                Convert.ToUInt16(counter%100),
                                false,
                                true,
                                false);
                        }
                    }
                }
                else
                {
                    UInt64 counter = 0;
                    UInt64 totalEmails = Convert.ToUInt64(filteredEmails.Count);
                    noOfEmailsActuallyDumped = totalEmails;

                    foreach (KeyValuePair<String, String> email in filteredEmails)
                    {
                        file.WriteLine(email.Key);
                        ++counter;

                        if (null != UpdateStatusProgressBar && (counter%1000 == 0))
                        {
                            UpdateStatusProgressBar("Dumping in chunks [" + counter + "], please wait ...",
                                Convert.ToUInt16((counter * 100) / totalEmails),
                                false,
                                true,
                                false);
                        }
                    }
                }
            }
        }


        private FilterEmailsController()
        {
            instanceFileReader = new FileReader();
            instanceFileReader.ParseText += new ExtractVerifyFilterHandler(instanceFileReader_ParseText);
        }

        private void reset()
        {
            abort();

            currentFileCounter = 0;
            noOfEmailsActuallyDumped = 0;
            instanceFilterEmailsData = new FilterEmailsData();
        }


        private void DirSearch(String sDir)
        {
            foreach (String d in Directory.GetDirectories(sDir))
            {
                foreach (String f in Directory.GetFiles(d, "*.txt"))
                {
                    instanceFileReader.read(f);
                    ++currentFileCounter;

                    if (null != UpdateStatusProgressBar)
                    {
                        UpdateStatusProgressBar("Filtering " + f + " ...",
                            Convert.ToUInt16((currentFileCounter * 100) / noOfFiles),
                            false,
                            true,
                            false);
                    }
                }

                DirSearch(d);
            }
        }

        private void instanceFileReader_ParseText(String text)
        {
            List<String> emailAddressExtracted = instanceFilterEmailsData.ExtractEmails(text);

            foreach(String email in emailAddressExtracted)
            {
                instanceFilterEmailsData.add(email);
            }
        } 
    }
}
