using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Net;
using MISPortal.Models;

namespace MISPortal.DAL
{
    public class EmailManagement
    {
        private static string Email = System.Web.Configuration.WebConfigurationManager.AppSettings["EmailAddress"];
        private static string Password = System.Web.Configuration.WebConfigurationManager.AppSettings["EmailPassword"];
        private static string SMTP = System.Web.Configuration.WebConfigurationManager.AppSettings["SMTP"];
        private static string PORT = System.Web.Configuration.WebConfigurationManager.AppSettings["PORT"];

        public static bool SendLeadEmail(string ToEmail, string UserName)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = SMTP,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Port = int.Parse(PORT),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(Email, Password)
                };

                MailMessage thisMessage = new MailMessage(Email, ToEmail);
                thisMessage.IsBodyHtml = true;
                thisMessage.Subject = "Crescent Wealth Managed Funds Application – Resume Application";

                string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/LeadEmail.htm"));

                thisMessage.Body = body;
                if (UserName != "")
                {
                    body = body.Replace("#UserName#", UserName);
                }
                else { body = body.Replace("#UserName#", "Investor"); }
                //foreach (DataRow thisEmail in dtEmails.Rows)
                //{
                thisMessage.Bcc.Add(ToEmail);

                //if (thisMessage.Bcc.Count > 500)
                //{
                smtp.Send(thisMessage);
                thisMessage.Bcc.Clear();
                //}
                //}

                //smtp.Send(thisMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendEmailResumeApplication(string ToEmail, string UserName, decimal ApplicantID)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = SMTP,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Port = int.Parse(PORT),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(Email, Password)
                };
                MailMessage thisMessage = new MailMessage(Email, ToEmail);
                thisMessage.IsBodyHtml = true;
                thisMessage.Subject = "Crescent Wealth Managed Funds Application – Resume Application";

                string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/ResumeApplication.htm"));
                body = body.Replace("#UserName#", UserName);
                body = body.Replace("#ResumeLink#", HttpUtility.UrlEncode(UtilityClass.Encrypt(ApplicantID.ToString())));
                //<a href='http://118.139.163.222:801/Applicant/ResumeApplication/?id=" + HttpUtility.UrlEncode(UtilityClass.Encrypt(ApplicantID.ToString())) + "' target='_blank'>please click here</a>
                thisMessage.Body = body;

                //Attachment att1 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/EmailAttachment/crescentwealth-email-banner.png"));
                //thisMessage.Attachments.Add(att1);
                //Attachment att2 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/EmailAttachment/CWSF - PDS - Investment Choice Guide - 01 Dec 2014.pdf"));
                //thisMessage.Attachments.Add(att2);
                //Attachment att3 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/EmailAttachment/CWSF - PDS - Insurance Booklet - 01 Dec 2014.pdf"));
                //thisMessage.Attachments.Add(att3);
                //Attachment att4 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/EmailAttachment/CWSF - PDS - Additional Information - 01 Dec 2014.pdf"));
                //thisMessage.Attachments.Add(att4);
                //Attachment att5 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/EmailAttachment/Crescent Wealth Super Fund - Product Disclosure Statement - 01 Dec 2014.pdf"));
                //thisMessage.Attachments.Add(att5);

                //foreach (DataRow thisEmail in dtEmails.Rows)
                //{
                thisMessage.Bcc.Add(ToEmail);

                //if (thisMessage.Bcc.Count > 500)
                //{
                smtp.Send(thisMessage);
                thisMessage.Bcc.Clear();
                //}
                //}

                //smtp.Send(thisMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendCompletionEmailtoClient(string ToEmail, string UserName, decimal ApplicantID, string AccountType)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = SMTP,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Port = int.Parse(PORT),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(Email, Password)
                };
                
                MailMessage thisMessage = new MailMessage(Email, ToEmail);
                thisMessage.IsBodyHtml = true;
                string body = "";

                List<ApplicantVerificationStatus> vslist = new List<ApplicantVerificationStatus>(); ;
                if (AccountType != "TrustCompany")
                {
                    vslist = ApplicantManagement.CheckGreenidVerificationStatus(ApplicantID);
                }

                if (AccountType == "Individual" || AccountType == "Joint")
                {
                    if (vslist[0]._overallStatus.Contains("VERIFIED"))
                    {
                        thisMessage.Subject = "Crescent Wealth Managed Funds Application – Further Action Required";
                        body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/IDPassIndividualJoint.htm"));
                    }
                    else
                    {
                        thisMessage.Subject = "Crescent Wealth Managed Funds Application – Further Action Required";
                        body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/IDFailIndividualJoint.htm"));
                    }
                }
                else if (AccountType == "TrustIndividual")
                {
                    if (vslist[0]._overallStatus.Contains("VERIFIED"))
                    {
                        thisMessage.Subject = "Crescent Wealth Managed Funds Application – Further Action Required";
                        body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/IDPassIndividualTrust.htm"));
                    }
                    else
                    {
                        thisMessage.Subject = "Crescent Wealth Managed Funds Application – Further Action Required";
                        body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/IDFailIndividualTrust.htm"));
                    }
                }
                else if (AccountType == "TrustCompany")
                {
                    thisMessage.Subject = "Crescent Wealth Managed Funds Application – Further Action Required";
                    body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/IDFailCorporateTrust.htm"));
                }
                
                body = body.Replace("#UserName#", UserName);

                if (AccountType != "TrustCompany")
                {
                    string status = "<table align='center' width='80%' border='1' bordercolor='#1c85c0' cellpadding='0' cellspacing='0'  class='width100 table-one-col'><thead><tr align='left' style='background-color:#1c85c0; color:#fff;'><th colspan='3' align='center' style='padding:5px 0px 5px 10px;'><b>Electronic Verification (EV) summary </b></th></tr>"
                                + "<tr style='background-color:lightblue;'><th  width='40%' align='left' style='padding:5px 5px 5px 10px;'>APPLICANT / ENTITY</th><th width='20%' align='left' style='padding:5px 5px 5px 10px;'>RESULT</th><th width='40%' align='left' style='padding:5px 5px 5px 10px;'>ACTION REQUIRED</th></tr></thead>";

                    foreach (var item in vslist)
                    {
                        if (item._ApplicantStatus.Contains("VERIFIED"))
                        {
                            status = status + "<tr><td style='padding:5px 5px 5px 10px;'>" + item._ApplicantName + "</td><td  style='padding:5px 5px 5px 10px;'>" + item._ApplicantStatus + "</td><td  style='padding:5px 5px 5px 10px;'> </td></tr>";
                        }
                        else
                        {
                            status = status + "<tr><td style='padding:5px 5px 5px 10px;'>" + item._ApplicantName + "</td><td  style='padding:5px 5px 5px 10px;'>" + item._ApplicantStatus + "</td><td  style='padding:5px 5px 5px 10px;'>Further ID verification required </td></tr>";
                        }
                    }
                    status = status + "</table>";

                    body = body.Replace("#ApplicantVerificationStatus#", status);
                }


                thisMessage.Body = body;

                Attachment att1 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/ApplicantDetails/ApplicantDetails.pdf"));
                thisMessage.Attachments.Add(att1);

                //foreach (DataRow thisEmail in dtEmails.Rows)
                //{
                thisMessage.Bcc.Add(ToEmail);

                //if (thisMessage.Bcc.Count > 500)
                //{
                smtp.Send(thisMessage);
                thisMessage.Bcc.Clear();
                //}
                //}

                //smtp.Send(thisMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool SendCompletionEmailtoCrescentWealth()
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = SMTP,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Port = int.Parse(PORT),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(Email, Password)
                };
                MailMessage thisMessage = new MailMessage("investors@crescentwealth.com.au", "investors@crescentwealth.com.au");
                thisMessage.IsBodyHtml = true;
                thisMessage.Subject = "Crescent Wealth Account Application";

                string body = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailTemplates/CompletionEmailtoCrescentWealth.htm"));

                thisMessage.Body = body;

                Attachment att1 = new Attachment(HttpContext.Current.Server.MapPath("~/App_Data/ApplicantDetails/ApplicantDetails.pdf"));
                thisMessage.Attachments.Add(att1);

                //foreach (DataRow thisEmail in dtEmails.Rows)
                //{
                thisMessage.Bcc.Add(Email);

                //if (thisMessage.Bcc.Count > 500)
                //{
                smtp.Send(thisMessage);
                thisMessage.Bcc.Clear();
                //}
                //}

                //smtp.Send(thisMessage);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


    }
}