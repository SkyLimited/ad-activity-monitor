using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.DirectoryServices;
using System.IO;
using System.Net;
using System.Collections;
using System.Configuration.Install;
using System.Runtime.Serialization.Formatters.Soap;
using System.Threading;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace LogViewerService
{


    public partial class LogViewerService : ServiceBase
    {
        public static Settings sets;
        private List<String> Servers;
        private static List<cDNSItem> dnsEntries;
        List<Thread> thList;

        private Thread adEnumerator;
        private Thread dnsEnumerator;
        private Thread mainThread;
        private Thread adCMPEnumerator;

        private cDataPrecache DataPrecache;

        public LogViewerService()
        {
            InitializeComponent();
        }

        public static void Log(string logMessage)
        {
            try
            {
                StreamWriter w = File.AppendText(@"C:\cfg\log");
                w.WriteLine("{0}", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " - " + logMessage);
                w.Close();
            }
            catch (Exception) { }
        }

        public void ServerCheckThread(object Server)
        {

            try
            {
                using (SqlConnection cs = new SqlConnection(((cServ)Server).SQLConnectionString))
                {
                    while (cs.State != System.Data.ConnectionState.Open)
                    {
                        try
                        {
                            cs.Open();
                        }
                        catch (Exception e)
                        {
                            LogViewerService.Log("Unable to set connection: " + e.Message + "\n" + e.StackTrace);
                        }
                        Thread.Sleep(1000);

                    }
                    Log("Conn is set.");


                    EventLog[] remoteEventLogs = null;
                    try
                    {
                        remoteEventLogs = EventLog.GetEventLogs(((cServ)Server).ServerName);

                    }
                    catch (Exception e)
                    {
                        Log("Unable to get logs from  \"" + ((cServ)Server).ServerName + "\". Reason was:" + e.Message);
                        return;
                    }
                    Log("Got logs. Count = " + remoteEventLogs.Count().ToString());
                    foreach (EventLog log in remoteEventLogs)
                    {

                        if (log.Log == "Security")
                        {
                            foreach (EventLogEntry entry in log.Entries)
                            {
                                if (entry.InstanceId == 4624)
                                {
                                    using (SqlCommand co = new SqlCommand("SELECT TOP 1 ID_Action FROM Actions WHERE WindowsActionID=@waid", cs))
                                    {
                                        co.Parameters.AddWithValue("waid", entry.Index);
                                        object rs = null;
                                        try
                                        {
                                            rs = co.ExecuteScalar();
                                        }
                                        catch (Exception e)
                                        {
                                            Log("Exception caught in scalar: " + e.Message);
                                            return;
                                        }
                                        if (rs != System.DBNull.Value && rs != null)
                                            continue;
                                    }
                                    string IPAddr;
                                    string HostName = "";
                                    IPAddr = entry.ReplacementStrings[18];
                                    try
                                    {
                                        if (IPAddr.Length < 7) continue;
                                        
                                       
                                        IPHostEntry IPHostEntryObject = Dns.GetHostEntry(IPAddr);
                                        HostName = IPHostEntryObject.HostName.Replace(".seckga.local", "");
                                        if (HostName.Trim().ToUpper() == "SECKGA")
                                        {
                                            lock (dnsEntries)
                                            {
                                                if (dnsEntries.Any(x => x.belongToIP(IPAddr)))
                                                {
                                                    HostName = dnsEntries.Find(x => x.belongToIP(IPAddr)).HostName;
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        using (SqlCommand ins = new SqlCommand("EXECUTE dbo.spAddRecord @HostName,@UserName,@Server,@IP,@EntryID,@ActionTS,@Domain,@RecType", cs))
                                        {
                                            ins.Parameters.AddWithValue("HostName", "*Unresolved*");
                                            ins.Parameters.AddWithValue("UserName", entry.ReplacementStrings[5]);
                                            ins.Parameters.AddWithValue("Server", ((cServ)Server).ServerName);
                                            ins.Parameters.AddWithValue("IP", IPAddr);
                                            ins.Parameters.AddWithValue("EntryID", entry.Index);
                                            ins.Parameters.AddWithValue("ActionTS", entry.TimeGenerated);
                                            ins.Parameters.AddWithValue("Domain", entry.ReplacementStrings[6]);
                                            ins.Parameters.AddWithValue("RecType", 2);
                                            try
                                            {
                                                ins.ExecuteNonQuery();
                                            }
                                            catch (Exception ew)
                                            {
                                                Log("Exc. was caught at unrslvd query:" + ew.Message);
                                            }
                                        }
                                        continue;
                                    }
                                    //3rd part of Availability of network share would be added later
                                    //Here should be process of 
                                    /*lock(DataPrecache){
                                    var lUC = DataPrecache.AvailabilityChecks.FirstOrDefault(x => x.IP == IPAddr); 
                                    bool Add = false;
                                    if (lUC == null)
                                        Add = true;
                                    else
                                        if(  (DateTime.Now - lUC.CheckDate).TotalMinutes>30)
                                            Add = true;
                                        }
                                    var task = new Task(() =>
                                    {
                                        var fi = new FileInfo(@"\\" + IPAddr + @"\C$");
                                        using (SqlConnection cs2 = new SqlConnection(sets.SqlServerString))
                                        {
                                            cs.Open();
                                            using (SqlCommand cmd2 = new SqlCommand("", cs2))
                                            {
                                                //fi.Exists

                                                cmd2.ExecuteNonQuery();
                                            }

                                        }
                                    });*/
                                        

                                    using (DirectoryEntry deDomain = new DirectoryEntry(@"LDAP://seckga.local/DC=seckga,DC=local"))
                                    {
                                        using (DirectorySearcher search = new DirectorySearcher(deDomain))
                                        {
                                            search.Filter = "(&(objectCategory=computer)(name=" + HostName + "))";
                                            search.PropertiesToLoad.Add("computer");
                                            SearchResult result = search.FindOne();
                                            if (result == null)
                                            {
                                                using (SqlCommand ins = new SqlCommand("EXECUTE dbo.spAddRecord @HostName,@UserName,@Server,@IP,@EntryID,@ActionTS,@Domain,@RecType", cs))
                                                {
                                                    ins.Parameters.AddWithValue("HostName", HostName);
                                                    ins.Parameters.AddWithValue("UserName", entry.ReplacementStrings[5]);
                                                    ins.Parameters.AddWithValue("Server", ((cServ)Server).ServerName);
                                                    ins.Parameters.AddWithValue("IP", IPAddr);
                                                    ins.Parameters.AddWithValue("EntryID", entry.Index);
                                                    ins.Parameters.AddWithValue("ActionTS", entry.TimeGenerated);
                                                    ins.Parameters.AddWithValue("Domain", entry.ReplacementStrings[6]);
                                                    ins.Parameters.AddWithValue("RecType", 1);
                                                    try
                                                    {
                                                        ins.ExecuteNonQuery();
                                                    }
                                                    catch (Exception ew)
                                                    {
                                                        Log("Exc. was caught at stdt query:" + ew.Message);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                using (SqlCommand ins = new SqlCommand("EXECUTE dbo.spAddRecord @HostName,@UserName,@Server,@IP,@EntryID,@ActionTS,@Domain,@RecType", cs))
                                                {
                                                    ins.Parameters.AddWithValue("HostName", HostName);
                                                    ins.Parameters.AddWithValue("UserName", entry.ReplacementStrings[5]);
                                                    ins.Parameters.AddWithValue("Server", ((cServ)Server).ServerName);
                                                    ins.Parameters.AddWithValue("IP", IPAddr);
                                                    ins.Parameters.AddWithValue("EntryID", entry.Index);
                                                    ins.Parameters.AddWithValue("ActionTS", entry.TimeGenerated);
                                                    ins.Parameters.AddWithValue("Domain", entry.ReplacementStrings[6]);
                                                    ins.Parameters.AddWithValue("RecType", 0);
                                                    try
                                                    {
                                                        ins.ExecuteNonQuery();
                                                    }
                                                    catch (Exception ew)
                                                    {
                                                        Log("Exc. was caught at stdt-0 query:" + ew.Message);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Log("CheckThread got exception:" + e.Message);
            }

        }

        public void regenerateDNSList()
        {

            while (true)
            {
                try
                {

                    using (DirectoryEntry deDomain = new DirectoryEntry(@"LDAP://seckga.local/DC=seckga,DC=local"))
                    {
                        using (DirectorySearcher search = new DirectorySearcher(deDomain))
                        {
                            search.Filter = "(&(objectCategory=computer))";
                            search.PropertiesToLoad.Add("name");
                            search.PageSize = 10000;
                            SearchResultCollection result = search.FindAll();
                            lock (dnsEntries)
                            {
                                dnsEntries.Clear();
                                foreach (SearchResult res in result)
                                {
                                    if (res.Properties["name"].Count > 0)
                                    {
                                        cDNSItem item = new cDNSItem(Convert.ToString(res.Properties["name"][0]));
                                        Log("DNS Progress: " + item.HostName + " as " + item.IP);
                                        dnsEntries.Add(item);
                                    }

                                }
                            }
                        }
                    }
                    Thread.Sleep(10 * 60 * 1000);
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException)
                        return;
                    Log("DNS enumerator thread got exception:" + e.Message + "\n" + e.StackTrace);
                }
            }

        }


        public void MainWorkerThread()
        {

            if (thList != null)
            {
                lock (thList)
                {
                    thList = new List<Thread>();
                }
            }
            else
                thList = new List<Thread>();
            Log("Entered main thread");
            //Infinite loop
            while (true)
            {
                try
                {
                    foreach (string cServName in Servers)
                    {
                        try
                        {
                            cServ sv = new cServ();
                            sv.ServerName = cServName;
                            sv.SQLConnectionString = LogViewerService.sets.SqlServerString;
                            sv.WaitTime = LogViewerService.sets.WaitTime * (-1);

                            Thread checkTh = new Thread(ServerCheckThread);
                            checkTh.Start((object)sv);
                            lock (thList)
                            {
                                thList.Add(checkTh);
                            }
                        }
                        catch (Exception e)
                        {
                            Log("Exception was caught in main thread:" + e.Message);
                        }

                    }

                    Thread.Sleep(LogViewerService.sets.WaitTime * 60000);
                    bool IsAlive = true;
                    int cnt = 0;
                    //Checking that every thread has stopped
                    while (IsAlive)
                    {
                        IsAlive = false;
                        lock (thList)
                        {
                            foreach (Thread t in thList)
                            {
                                if (t.ThreadState == System.Threading.ThreadState.Running)
                                {
                                    IsAlive = true;
                                    // Log("Warning: thread " + t.ManagedThreadId.ToString() + " is still  alive. Maybe should adjust wait time.");
                                    if (cnt > 100)
                                    {
                                        Log("Thread " + t.ManagedThreadId.ToString() + " seems to be hang up... Trying to abort.");
                                        try
                                        {
                                            t.Abort();
                                            Log("Thread " + t.ManagedThreadId.ToString() + " aborted!");
                                        }
                                        catch (Exception e)
                                        {
                                            Log("Abort of thread " + t.ManagedThreadId.ToString() + " failed! Reason:" + e.Message);
                                        }
                                    }
                                }
                            }
                        }
                        if (!IsAlive)
                            break;
                        Thread.Sleep(3000);
                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException)
                        return;
                    Log("Main thread got exception: " + e.Message + "\n\n" + e.StackTrace);

                }

            }

        }


        void ADCmpEmnumeratorThread()
        {
            
            while (true)
            {
                Log("adCMPenumerator thread session started.");
                try
                {

                    using (DirectoryEntry deDomain = new DirectoryEntry(@"LDAP://seckga.local/DC=seckga,DC=local"))
                    {
                        using (DirectorySearcher search = new DirectorySearcher(deDomain))
                        {
                            search.Filter = "(&(objectCategory=computer))";
                            search.PropertiesToLoad.Add("memberOf");
                            search.PropertiesToLoad.Add("name");
                            search.PropertiesToLoad.Add("distinguishedname");
                            search.PropertiesToLoad.Add("userAccountControl");
                            search.PageSize = 10000;
                            SearchResultCollection result = search.FindAll();
                            Log("count = " + result.Count);
                            List<cDomainComputer> domComps = new List<cDomainComputer>();
                            foreach (SearchResult res in result)
                            {
                                if (res.Properties["name"].Count <= 0) continue;
                                cDomainComputer cmp = DataPrecache.findCompByDN(Convert.ToString(res.Properties["name"][0]));
                                if (cmp == null)
                                {
                                    cmp = new cDomainComputer(Convert.ToString(res.Properties["name"][0]));
                                    DataPrecache.addComp(cmp);
                                }
                                cmp.clearGroups();
                                if (res.Properties["userAccountControl"].Count > 0)
                                {
                                    int uac = Convert.ToInt32(res.Properties["userAccountControl"][0]);
                                    bool Disabled = ((uac & 2) > 0);
                                    if (Disabled)
                                        cmp.Enabled = false;
                                    else
                                        cmp.Enabled = true;
                                }
                                else
                                    cmp.Enabled = false;
                                if (res.Properties["distinguishedname"].Count > 0)
                                    cmp.DN = Convert.ToString(res.Properties["distinguishedname"][0]);
                                foreach (string grName in res.Properties["memberOf"])
                                {
                                    cDomainGroup gr = DataPrecache.findGroupByName(grName);
                                    if (gr == null)
                                    {
                                        gr = new cDomainGroup(grName);
                                        DataPrecache.addGroup(gr);
                                    }
                                    cmp.addDomainGroup(gr);
                                }
                                cmp.processDBChanges();
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException)
                        return;
                    Log("adCMPEnumerator thread catched exception: " + e.Message + "\n" + e.StackTrace + "\n\n");
                }
                Log("adCMPenumerator thread session ended.");
                Thread.Sleep(sets.ADWaitTime * 60 * 60 * 1000);
            }

        }

        void ADEmnumeratorThread()
        {
            //Infinite loop           
            while (true)
            {
                Log("adenumerator thread session started...");

                try
                {
                    using (DirectoryEntry deDomain = new DirectoryEntry(@"LDAP://seckga.local/DC=seckga,DC=local"))
                    {
                        using (DirectorySearcher search = new DirectorySearcher(deDomain))
                        {
                            search.Filter = "(&(objectCategory=user))";
                            search.PropertiesToLoad.Add("memberOf");
                            search.PropertiesToLoad.Add("name");
                            search.PropertiesToLoad.Add("distinguishedname");
                            search.PropertiesToLoad.Add("userAccountControl");
                            search.PropertiesToLoad.Add("samAccountName");
                            search.PageSize = 10000;
                            SearchResultCollection result = search.FindAll();
                            Log("count = " + result.Count);
                            List<cDomainUser> domUsers = new List<cDomainUser>();
                            foreach (SearchResult res in result)
                            {
                                if (res.Properties["samAccountName"].Count <= 0)
                                {
                                    string data = "";
                                    foreach (DictionaryEntry t in res.Properties)
                                    {
                                        System.DirectoryServices.ResultPropertyValueCollection cl = (ResultPropertyValueCollection)t.Value;
                                        foreach (object g in cl)
                                            data += t.Key.ToString() + "=" + g.ToString() + ";";

                                    }                                  
                                    continue;
                                }
                                cDomainUser us = DataPrecache.findUserBySamAccount(Convert.ToString(res.Properties["samAccountName"][0]));
                                us.clearGroups();
                                if (us == null)
                                {
                                    us = new cDomainUser(Convert.ToString(res.Properties["samAccountName"][0]));
                                    DataPrecache.addUser(us);
                                }
                            //    Log(Convert.ToString(res.Properties["samAccountName"][0]));

                                if (res.Properties["name"].Count > 0)
                                    us.UserName = Convert.ToString(res.Properties["name"][0]);

                                if (res.Properties["userAccountControl"].Count > 0)
                                {
                                    int uac = Convert.ToInt32(res.Properties["userAccountControl"][0]);
                                    bool Disabled = ((uac & 2) > 0);
                                    if (Disabled)
                                        us.Enabled = false;
                                    else
                                        us.Enabled = true;
                                }
                                else
                                    us.Enabled = false;
                                if (res.Properties["distinguishedname"].Count > 0)
                                    us.DN = Convert.ToString(res.Properties["distinguishedname"][0]);
                                foreach (string grName in res.Properties["memberOf"])
                                {
                                    //Log( us.UserName  + " Member of =" + grName); 
                                    cDomainGroup gr = DataPrecache.findGroupByName(grName);
                                    if (gr == null)
                                    {
                                        gr = new cDomainGroup(grName);
                                        DataPrecache.addGroup(gr);
                                    }
                                    us.addDomainGroup(gr);
                                }
                                us.processDBChanges();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if (e is ThreadAbortException)
                        return;
                    Log("adEnumerator thread catched exception: " + e.Message + "\n" + e.StackTrace + "\n\n");
                }
                Log("adEnumerator session ended... Sleep");
                Thread.Sleep(sets.ADWaitTime * 60 * 60 * 1000);
            }

        }

        protected override void OnStart(string[] args)
        {

            try
            {
                Log("SLV3 Service started... Copyright (c) SkyLimited 2013");


                if (args.Count() < 1)
                {
                    Log(@"No arguments, fallback to default: C:\CFG\Settings.xml");
                    LogViewerService.sets = new Settings(@"C:\CFG\Settings.xml");

                }
                else
                {
                    Log("Reading configuration from - " + args[0]);
                    LogViewerService.sets = new Settings(args[0]);
                }
            }
            catch (Exception e)
            {
                Log("Exception was caught in loading settings: " + e.Message);
                return;
            }
            Log("Prooving SQL Server Connection");
            using (SqlConnection cs = new SqlConnection(LogViewerService.sets.SqlServerString))
            {
                dnsEntries = new List<cDNSItem>();
                while (cs.State != System.Data.ConnectionState.Open)
                {
                    try
                    {
                        cs.Open();
                    }
                    catch (Exception e)
                    {
                        LogViewerService.Log("Unable to set connection: " + e.Message + "\n" + e.StackTrace);
                    }
                    Thread.Sleep(1000);

                }
                Log("Connection ok.");

                using (SqlCommand co = new SqlCommand("SELECT * FROM Servers WHERE ByPass=0", cs))
                {
                    using (SqlDataReader ro = co.ExecuteReader())
                    {
                        if (!ro.HasRows)
                        {
                            Log("No servers defined.");
                            this.Stop();
                            return;
                        }
                        Servers = new List<String>();
                        while (ro.Read())
                        {
                            Servers.Add(Convert.ToString(ro["ServerName"]));
                        }
                    }
                }
                Log("Readed " + Servers.Count() + " servers.");

            }

            Log("Initializing Precache Technology...");
            DataPrecache = new cDataPrecache(sets);
            Log("Woah! Precache is set and ready!");
            Log("Starting SCU Thread...");
            mainThread = new Thread(MainWorkerThread);
            mainThread.Start();
            Log("Starting ADE Thread...");
            adEnumerator = new Thread(ADEmnumeratorThread);
            adEnumerator.Start();
            Log("Starting ADC Thread...");
            adCMPEnumerator = new Thread(ADCmpEmnumeratorThread);
            adCMPEnumerator.Start();
            Log("Starting DNS-Helper Thread...");
            dnsEnumerator = new Thread(regenerateDNSList);
            dnsEnumerator.Start();
            Log("All threads started, service will countinue to work on...");
        }


        protected override void OnStop()
        {
            Log("stopping threadz...");
            if (adEnumerator.ThreadState == System.Threading.ThreadState.Suspended || adEnumerator.ThreadState == System.Threading.ThreadState.Running)
                adEnumerator.Abort();
            if (dnsEnumerator.ThreadState == System.Threading.ThreadState.Suspended || dnsEnumerator.ThreadState == System.Threading.ThreadState.Running)
                dnsEnumerator.Abort();
            if (mainThread.ThreadState == System.Threading.ThreadState.Suspended || mainThread.ThreadState == System.Threading.ThreadState.Running)
                mainThread.Abort();
            if (adCMPEnumerator.ThreadState == System.Threading.ThreadState.Suspended || adCMPEnumerator.ThreadState == System.Threading.ThreadState.Running)
                adCMPEnumerator.Abort();
            if (thList != null)
            {
                lock (thList)
                {
                    foreach (Thread t in thList)
                        t.Abort();
                }
            }
            Log("SLV3 Service stopped.");

        }
    }


}
