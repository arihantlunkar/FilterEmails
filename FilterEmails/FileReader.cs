using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FilterEmails
{
    public delegate void ExtractVerifyFilterHandler (String text);  

    class FileReader
    {
        public event ExtractVerifyFilterHandler ParseText;  

        public void read(String fileName) 
        {
            foreach (String line in File.ReadLines(fileName))
            {
                if (ParseText != null)
                {
                    ParseText(line);
                }
            }
        }
    }
}
