using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace FilterEmails
{
    class FilterEmailsData
    {
        private Dictionary<String,String> filterEmails;
        private UInt64 emailsExtracted, emailsRemoved, duplicateEmails, validEmails;
        //private String[] notRequiredDomains;

        public FilterEmailsData()
        {
            filterEmails = new Dictionary<String,String>();
            emailsExtracted = 0;
            emailsRemoved = 0;
            duplicateEmails = 0;
            validEmails = 0;
        }

        public Dictionary<String,String> getFilterEmailsData()
        {
            return filterEmails;
        }

        public void setFilterEmailsData(Dictionary<String, String> filterEmails)
        {
            this.filterEmails = filterEmails;
        }

        public UInt64 getNumberOfEmailsExtracted() 
        {
            return emailsExtracted;
        }

        public UInt64 getNumberOfEmailsRemoved()
        {
            return emailsRemoved;
        }

        public UInt64 getNumberOfDuplicateEmails()
        {
            return duplicateEmails;
        }

        public UInt64 getNumberOfValidEmails()
        {
            return validEmails;
        }

        public void add(String email) 
        {
            if (!filterEmails.ContainsKey(email))
            {
                filterEmails.Add(email, email.Split('@').Last());
                ++validEmails;
            }
            else
            {
                ++duplicateEmails;
            }
        }

        public List<String> ExtractEmails(String textToScrape)
        {
            Regex reg = new Regex(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
            Match match;

            List<String> results = new List<String>();
            for (match = reg.Match(textToScrape); match.Success; match = match.NextMatch())
            {
                if (!(results.Contains(match.Value)))
                {
                    results.Add(match.Value);
                    ++emailsExtracted;
                }
            }

            return results;
        }

        /*public void addNotRequiredDomains(String text)
        {
            notRequiredDomains = text.Split(new[] { ',', ' ' },
                                StringSplitOptions.RemoveEmptyEntries);
        }

        public Boolean isHostFoundInNotRequiredDomains(String email)
        {
            return (notRequiredDomains != null && Array.IndexOf(notRequiredDomains, (new MailAddress(email)).Host) > -1) ? true : false;
        }*/
    }
}
