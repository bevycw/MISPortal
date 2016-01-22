using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MISPortal.DAL
{
    public class GreenidManagement
    {
        //Only Dynamic form web service calling functions

        private static string Account = System.Web.Configuration.WebConfigurationManager.AppSettings["GreenIdAccount"];
        private static string Password = System.Web.Configuration.WebConfigurationManager.AppSettings["GreenIdPassword"];

        public static greenid.registerVerificationResponse registerVerification()
        {
            greenid.registerVerificationResponse res = new greenid.registerVerificationResponse();
            
            try
            {
                greenid.registerVerification req = new greenid.registerVerification();
                req.name = new greenid.name();
                req.currentResidentialAddress = new greenid.ausAddress();
                req.dob = new greenid.dateOfBirth();

                req.accountId = "crescentwealth";
                req.password = "R5a-PLn-tvG-WrY";
                req.ruleId = "default";
                req.name.givenName = "Test";
                req.name.middleNames = "";
                req.name.surname = "McTester";
                req.email = "test@example.com";
                req.currentResidentialAddress.streetName = "Macquarie";
                req.currentResidentialAddress.streetNumber = "1";
                req.currentResidentialAddress.streetType = "ST";
                req.currentResidentialAddress.suburb = "Sydney";
                req.currentResidentialAddress.state = "NSW";
                req.currentResidentialAddress.postcode = "2000";
                req.currentResidentialAddress.country = "AUS";
                req.dob.day = 1;
                req.dob.month = 1;
                req.dob.year = 1980;
                req.generateVerificationToken = true;


                greenid.DynamicFormsServiceClient df = new greenid.DynamicFormsServiceClient();
                res = df.registerVerification(req);

                return res;
            }
            catch
            {
                return res;
            }
        }

        public static bool DriverLicence(string DriverLicenceNumber)
        {
            try
            {
                    greenid.registerVerificationResponse res = registerVerification();
                    greenid.DynamicFormsServiceClient df = new greenid.DynamicFormsServiceClient();

                    greenid.getFields gfreq = new greenid.getFields();
                    gfreq.accountId = "crescentwealth";
                    gfreq.password = "R5a-PLn-tvG-WrY";
                    gfreq.sourceId = "actrego";
                    gfreq.verificationId = res.@return.verificationResult.userId;
                    gfreq.verificationToken = "";

                    greenid.getFieldsResponse gfres = new greenid.getFieldsResponse();
                    gfres = df.getFields(gfreq);

                    greenid.setFields sfreq = new greenid.setFields();
                    sfreq.accountId = "crescentwealth";
                    sfreq.password = "R5a-PLn-tvG-WrY";
                    sfreq.sourceId = "actrego";
                    sfreq.verificationId = res.@return.verificationResult.userId;
                    sfreq.verificationToken = "";
                    sfreq.inputFields = new greenid.nameValuePair[5];

                    greenid.nameValuePair[] nVP = new greenid.nameValuePair[5];

                    greenid.nameValuePair nVP1 = new greenid.nameValuePair();
                    nVP1.name = gfres.@return.sourceFields.fieldList[0].name; nVP1.value = DriverLicenceNumber;
                    nVP[0] = nVP1;
                    greenid.nameValuePair nVP2 = new greenid.nameValuePair();
                    nVP2.name = gfres.@return.sourceFields.fieldList[1].name; nVP2.value = gfres.@return.sourceFields.fieldList[1].value;
                    nVP[1] = nVP2;
                    greenid.nameValuePair nVP3 = new greenid.nameValuePair();
                    nVP3.name = gfres.@return.sourceFields.fieldList[2].name; nVP3.value = gfres.@return.sourceFields.fieldList[2].value;
                    nVP[2] = nVP3;
                    greenid.nameValuePair nVP4 = new greenid.nameValuePair();
                    nVP4.name = gfres.@return.sourceFields.fieldList[3].name; nVP4.value = gfres.@return.sourceFields.fieldList[3].value;
                    nVP[3] = nVP4;
                    greenid.nameValuePair nVP5 = new greenid.nameValuePair();
                    nVP5.name = gfres.@return.sourceFields.fieldList[4].name; nVP5.value = "on";
                    nVP[4] = nVP5;

                    sfreq.inputFields = nVP;

                    greenid.setFieldsResponse sfres = new greenid.setFieldsResponse();
                    sfres = df.setFields(sfreq);

                    if (sfres.@return.checkResult.state == "VERIFIED")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            catch
            {
                return false;
            }
        }

        public static string GetuserToken(string VerificationId)
        {
            string userToken = "";
            try
            {
                greenid.DynamicFormsServiceClient df = new greenid.DynamicFormsServiceClient();

                greenid.getUserToken req = new greenid.getUserToken();
                req.account = Account;
                req.password = Password;
                req.verificationId = VerificationId;

                greenid.getUserTokenResponse res = new greenid.getUserTokenResponse();
                res = df.getUserToken(req);
                userToken = res.@return;
                return userToken;
            }
            catch
            {
                return userToken;
            }
        }

        public static bool CheckVerificationStatus()
        {
            try
            {
                greenid.DynamicFormsServiceClient df = new greenid.DynamicFormsServiceClient();

                
                
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}