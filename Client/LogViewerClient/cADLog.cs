using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogViewerClient
{
    public enum cObjectTypes
    {
        Computer, User
    };

    public enum cEventTypes
    {
        New, Disabled, Enabled, DNChanged, GroupChanged
    };
    
    public class cADObject
    {
        private cObjectTypes m_objType;
        private string m_Name;
        private int m_SQLID;

        public override string ToString()
        {
            return m_Name;
        }

        public cADObject(int SQLID,string Name,cObjectTypes Type)
        {
            m_Name = Name;
            m_SQLID = SQLID;
            m_objType = Type;
        }

        public cObjectTypes objTypeValue
        {
            get
            {
                return m_objType;
            }
        }

        public string objType
        {
            get
            {
                switch (m_objType)
                {
                    case cObjectTypes.Computer: return "АРМ";
                    case cObjectTypes.User: return "Польз.";
                    default: return "";
                }
            
            }

        }

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        public int SQLID
        {
            get
            {
                return m_SQLID;
            }
        }

        public static bool compareObjects(cADObject obj1,cADObject obj2)
        {
            if (obj1.objType == obj2.objType)
                if (obj1.Name == obj2.Name)
                    if (obj1.SQLID == obj2.SQLID)
                        return true;
            return false;
        }    
    
    }

      
    
    public class cADLogRecord
    {
        private cADObject m_object;
        private cEventTypes m_eventType;
        private DateTime m_dateOfEvent;
        private string m_CurrentValue;
        private string m_prevValue;
        private List<LogViewerService.cDomainGroup> m_CurrentGroups;
        private List<LogViewerService.cDomainGroup> m_prevGroups;

        public cADLogRecord(cADObject adObject, DateTime evtDate, string Value, cEventTypes evType = cEventTypes.New )
        {
            m_object = adObject;
            m_eventType = evType;
            m_dateOfEvent = evtDate;
            m_CurrentValue = Value;

            m_CurrentGroups = new List<LogViewerService.cDomainGroup>();
            m_prevGroups = new List<LogViewerService.cDomainGroup>();
        }
        public List<LogViewerService.cDomainGroup> domainGroups
        {
            get
            {
                return m_CurrentGroups;

            }
        }

        public bool HavePrevGroups
        {
            get
            {
                if (m_prevGroups.Count == 0) return false;
                return true;

            }
        }

        public void addCurrentGroup(LogViewerService.cDomainGroup grp)
        {
            m_CurrentGroups.Add(grp);
        }

        public void addPreviousGroup(LogViewerService.cDomainGroup grp)
        {
            m_prevGroups.Add(grp);

        }

        public cADObject Object
        {
            get
            {
                return m_object;
            }
        }

        public cEventTypes eventTypeValue
        {
            get
            {
                return m_eventType;
            }
        }

        public string objType
        {
            get
            {
                return m_object.objType;
            }

        }

        public string eventType
        {
            get
            {
                switch (m_eventType)
                {
                    case cEventTypes.New: return "Новый"; 
                    case cEventTypes.GroupChanged: return "Изменилось членство в группах";                  
                    case cEventTypes.Enabled: return "Аккаунт включен";
                    case cEventTypes.Disabled: return "Аккаунт выключен"; 
                    case cEventTypes.DNChanged: return "Изменился Юнит"; 
                    default: return ""; 
                }
            }
        }

        public DateTime eventTime
        {
            get
            {
                return m_dateOfEvent;
            }
        }

        public string currentValue
        {
            get
            {
                if (m_eventType != cEventTypes.GroupChanged)
                {
                    if (m_CurrentValue.EndsWith(Environment.NewLine))
                        return m_CurrentValue.Substring(0, m_CurrentValue.LastIndexOf(Environment.NewLine));
                    else
                        return m_CurrentValue;
                 
                }
                else
                {
                    string res = "";
                    foreach (LogViewerService.cDomainGroup grp in m_CurrentGroups)
                    {
                        bool Found = false;
                        foreach (LogViewerService.cDomainGroup grpBefore in m_prevGroups)
                        {
                            if (grpBefore.DN == grp.DN && grp.SQLID == grpBefore.SQLID)
                            {
                                Found = true;
                                break;
                            }

                        }
                        if (!Found)
                            res += "(+)" + fMain.translateDN(grp.DN) + Environment.NewLine;

                    }
                    foreach (LogViewerService.cDomainGroup grp in m_prevGroups)
                    {
                        bool Found = false;
                        foreach (LogViewerService.cDomainGroup grpBefore in m_CurrentGroups)
                        {
                            if (grpBefore.DN == grp.DN && grp.SQLID == grpBefore.SQLID)
                            {
                                Found = true;
                                break;
                            }

                        }
                        if (!Found)
                            res += "(-)" + fMain.translateDN(grp.DN) + Environment.NewLine;

                    }
                    if (res.EndsWith(Environment.NewLine))
                        return res.Substring(0, res.LastIndexOf(Environment.NewLine));
                    else
                        return res;
                }
            }
        
        }

        public string oldValue
        {
            get
            {
                if (m_eventType != cEventTypes.GroupChanged)
                {
                    if (m_prevValue == null)
                        return "";
                    if (m_prevValue.EndsWith(Environment.NewLine))
                        return m_prevValue.Substring(0, m_CurrentValue.LastIndexOf(Environment.NewLine));
                    else
                        return m_prevValue;
                }
                else
                {
                    string res = "";
                    foreach (LogViewerService.cDomainGroup grpBefore in m_prevGroups)
                    {
                        res += fMain.translateDN(grpBefore.DN) + Environment.NewLine;
                      
                    }
                    if (res.EndsWith(Environment.NewLine))
                        return  res.Substring(0, res.LastIndexOf(Environment.NewLine));
                    else
                        return res;

                }

            }
            set
            {
                m_prevValue = value;
                if (m_eventType == cEventTypes.New)
                    m_eventType = cEventTypes.DNChanged;
            }
        }
    }
}
