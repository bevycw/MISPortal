using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MISPortal.Models;
using MISPortal.SFServiceReference;

namespace MISPortal.DAL
{
    public class SalesforceManagement
    {
        private static string username = System.Web.Configuration.WebConfigurationManager.AppSettings["SFUsername"];
        private static string password = System.Web.Configuration.WebConfigurationManager.AppSettings["SFPassword"];
        private static string tokenId = System.Web.Configuration.WebConfigurationManager.AppSettings["SFTokenID"];


        // to store session variable name   
        private string _username = username;
        // to store session variable name
        private string _password = password;
        // generated tokenid from salesforce
        private string _tokenID = tokenId;//const 
        // to store session variable name
        private string _sessionId;
        // next login time
        private DateTime _nextLoginTime;
        // instance for login result
        private LoginResult _loginResult;
        // instance creation for SforceService
        private SforceService _sForceRef;


        // Authentication and Get Sessioninfo  
        public void getSessionInfo()
        {
            _sForceRef = new SforceService();//if gives error replace ListViewRecordColumn[][] to ListViewRecordColumn[] in Reference.cs
            // Pass username and password + tokenID in login method  
            _loginResult = _sForceRef.login(_username, _password + _tokenID);
            _sessionId = _loginResult.sessionId;
            HttpContext.Current.Session["_sessionId"] = _loginResult.sessionId;
            HttpContext.Current.Session["_serverUrl"] = _loginResult.serverUrl;
            HttpContext.Current.Session["_nextLoginTime"] = DateTime.Now;

            _sForceRef.Url = _loginResult.serverUrl;
            _sForceRef.SessionHeaderValue = new SessionHeader();
            _sForceRef.SessionHeaderValue.sessionId = _sessionId;
        }

        // Check whether session connected or not   
        public bool IsConnected()
        {
            bool blnResult = false;
            if (!string.IsNullOrEmpty(_sessionId) & _sessionId != null)
            {
                if (DateTime.Now > _nextLoginTime)
                    blnResult = false;
                else
                    blnResult = true;
            }
            else
                blnResult = false;

            return blnResult;
        }

        public void SaveRecord(SalesforceModel sf)
        {
            try
            {
                if (!IsConnected())
                    getSessionInfo();

                //lead
                Lead newLead = new Lead();
                newLead.Email = sf._EmailAddress;
                newLead.Street = sf._Address;
                newLead.City = sf._City;
                newLead.State = sf._State;
                newLead.PostalCode = sf._Postcode;
                newLead.Country = sf._Country;
                newLead.FirstName = sf._FirstName;
                if (sf._LastName != null)
                {
                    newLead.LastName = sf._LastName;
                }
                else { newLead.LastName = "."; }
                newLead.MobilePhone = sf._Mobile;
                newLead.Company = sf._Company;
                SaveResult[] saveResults = _sForceRef.create(new sObject[] { newLead });

                string result = string.Empty;
                if (saveResults[0].success)
                {
                    result = saveResults[0].id; // id of the newly inserted record
                }
                else
                {
                    result = saveResults[0].errors[0].message; // failure message
                }
            }
            catch
            {
 
            }
        }


    }
}
