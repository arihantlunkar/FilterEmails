using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FilterEmails
{
    static class InputData
    {
        private static String filteringOn;
        public static String FilteringOn
        {
            get { return filteringOn; }
            set { filteringOn = value; }
        }

        private static String emailLocation;
        public static String EmailLocation
        {
            get { return emailLocation; }
            set { emailLocation = value; }
        }

        private static Boolean removeDuplicates;
        public static Boolean RemoveDuplicates
        {
            get { return removeDuplicates; }
            set { removeDuplicates = value; }
        }

        private static Boolean keepAtLeastDomain;
        public static Boolean KeepAtLeastDomain
        {
            get { return keepAtLeastDomain; }
            set { keepAtLeastDomain = value; }
        }

        private static UInt16 noOfDomainsToKeep;
        public static UInt16 NoOfDomainsToKeep
        {
            get { return noOfDomainsToKeep; }
            set { noOfDomainsToKeep = value; }
        }
    }
}
