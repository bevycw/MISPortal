using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MISPortal.Models;
using MISPortal.DAL;
using System.Threading;

namespace MISPortal.Controllers
{
    public class ApplicantController : Controller
    {

        public ActionResult Index()
        {
            if (Request.Cookies["ApplicantID"] != null)
            {
                Response.Cookies["ApplicantID"].Expires = DateTime.Now.AddDays(-1);
            }
            Session["ApplicantID"] = null;
            Session["ApplicantNo"] = null;
            Session.Abandon();
            Session["StepNo"] = 1;
            return View();
        }

        [HttpPost]
        public ActionResult AddEditIndividualApplicant(ApplicantPersonalDetails APD)
        { 
            bool Transaction;
            Transaction = ApplicantManagement.AddEditIndividualApplicant(APD);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"].ToString());
            ApplicantPersonalDetails apd = ApplicantManagement.GetApplicantDetails(ApplicantID);
            EmailManagement.SendEmailResumeApplication(apd._LeadEmail, apd._FullName, ApplicantID);
            
            Session["StepNo"] = 2;
            return RedirectToAction("Step2");
        }

        [HttpPost]
        public ActionResult AddEditJointApplicant(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            bool Transaction;
            Transaction = ApplicantManagement.AddEditJointApplicant(APDList);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"].ToString());
            ApplicantPersonalDetails apd = ApplicantManagement.GetApplicantDetails(ApplicantID);
            EmailManagement.SendEmailResumeApplication(apd._LeadEmail, apd._FullName, ApplicantID);

            Session["StepNo"] = 2;
            return RedirectToAction("Step2");
        }

        [HttpPost]
        public ActionResult AddEditTrustIndividualApplicant(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            bool Transaction;
            Transaction = ApplicantManagement.AddEditTrustIndividualApplicant(APDList);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"].ToString());
            ApplicantPersonalDetails apd = ApplicantManagement.GetApplicantDetails(ApplicantID);
            EmailManagement.SendEmailResumeApplication(apd._LeadEmail, apd._FullName, ApplicantID);

            Session["StepNo"] = 2;
            return RedirectToAction("Step2");
        }

        [HttpPost]
        public ActionResult AddEditTrustCompanyApplicant(Company APD)
        {
            bool Transaction;
            Transaction = ApplicantManagement.AddEditTrustCompanyApplicant(APD);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"].ToString());
            ApplicantPersonalDetails apd = ApplicantManagement.GetApplicantDetails(ApplicantID);
            EmailManagement.SendEmailResumeApplication(apd._EmailAddress, apd._FullName, ApplicantID);

            Session["StepNo"] = 3;
            return RedirectToAction("Step3");
        }

        [HttpPost]
        public ActionResult IdentityCheck()
        {
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"].ToString());
            bool Transaction = ApplicantManagement.IdentityVerification(ApplicantID);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            Session["StepNo"] = 3;
            return RedirectToAction("Step3");
        }

        [HttpPost]
        public ActionResult AddEditInvestmentDetailApplicant(InvestmentDetails ID)
        {
            bool Transaction;
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
            Transaction = ApplicantManagement.AddEditInvestmentDetailApplicant(ID, ApplicantID);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            Session["StepNo"] = 4;
            return RedirectToAction("Step4");
        }

        [HttpPost]
        public ActionResult AddEditAuthRepFinAdvDetailApplicant(Step4 S4)
        {
            bool Transaction;
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
            Transaction = ApplicantManagement.AddEditAuthRepFinAdvDetailApplicant(S4, ApplicantID);
            if (!Transaction)
            {
                return PartialView("Error");
            }
            List<ApplicantPersonalDetails> APDList = ApplicantManagement.GetApplicantDetailsList(ApplicantID);
            Session["StepNo"] = 5;
            return RedirectToAction("Step5", APDList);
        }

        [HttpPost]
        public ActionResult ConfirmAndSubmitApplicant()
        {
            bool Transaction;
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
            HttpCookie myCookie = new HttpCookie("ApplicantID");
            myCookie.Value = ApplicantID.ToString();
            Response.Cookies.Add(myCookie);

            Transaction = ApplicantManagement.ConfirmAndSubmitApplicant(ApplicantID);
            if (!Transaction)
            {
                return PartialView("Error");
            }

            if (ApplicantID == 0 || ApplicantID == null)
            {
                myCookie = Request.Cookies["ApplicantID"];
                ApplicantID = decimal.Parse(myCookie.Value.ToString());
            }
            if (ApplicantManagement.CreateApplicantPDF(ApplicantID))
            {
                ApplicantPersonalDetails apd = ApplicantManagement.GetApplicantDetails(ApplicantID);
                EmailManagement.SendCompletionEmailtoClient(apd._LeadEmail, apd._FullName, ApplicantID, apd._AccountType);
                EmailManagement.SendCompletionEmailtoCrescentWealth();
            }
            Session["StepNo"] = null;
            return RedirectToAction("Completed");
        }

        //=================================Edit-Application==================================//

        [HttpPost]
        public ActionResult EditIndividualJointTrustIndividual(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            bool transaction = ApplicantManagement.EditIndividualJointTrustIndividual(APDList);
            if (!transaction)
            {
                return View("Error");
            }
            else
            {
                return RedirectToAction("Step5");
            }

        }

        [HttpPost]
        public ActionResult EditTrustCompany(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            bool transaction = ApplicantManagement.EditTrustCompany(APDList);
            if (!transaction)
            {
                return View("Error");
            }
            else
            {
                return RedirectToAction("Step5");
            }

        }


        //================================Resume-Application=================================//


        public ActionResult ResumeApplication()
        {
            Session["ApplicantID"] = UtilityClass.Decrypt(HttpUtility.UrlDecode(Request.QueryString["id"]));
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"].ToString());
            ApplicantPersonalDetails APD = ApplicantManagement.GetApplicantDetails(ApplicantID);

            if (APD._Percent == 20)      { Session["StepNo"] = 2; return RedirectToAction("Step2"); }
            else if (APD._Percent == 40) { Session["StepNo"] = 3; return RedirectToAction("Step3"); }
            else if (APD._Percent == 60) { Session["StepNo"] = 4; return RedirectToAction("Step4"); }
            else if (APD._Percent == 80) { Session["StepNo"] = 5; return RedirectToAction("Step5"); }

            return RedirectToAction("Completed");
        }

        //==============================End-Resume-Application===============================//

        public ActionResult Step2()
        {
            if (Session["StepNo"].ToString() == "2")
            {
                try
                {
                    decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
                    ApplicantPersonalDetails APD = ApplicantManagement.GetApplicantDetails(ApplicantID);
                    greenidVerification g = ApplicantManagement.GreenidRegisterVerification(APD);
                    ViewBag.GreenidVerificationID = g._GreenidVerificationID;
                    ViewBag.isRegistrationExists = g._isRegistrationExists;
                    ViewBag.verificationId = g._VerificationId;
                    ViewBag.VerificationResult = g._VerificationResult;
                    if (g._VerificationId == "" || g._VerificationId == null)
                    {
                    }
                    else
                    {
                        ApplicantManagement.GreenidVerificationId(ApplicantID, g._VerificationId, g._GreenidVerificationID);
                        ViewBag.userToken = GreenidManagement.GetuserToken(g._VerificationId);
                    }

                    return View(APD);
                }
                catch
                {
                    return View("Error");
                }
            }
            else
            {
                return PartialView("Error");
            }
        }

        public ActionResult Step3()
        {
            Session["ApplicantNo"] = null;

            if (Session["StepNo"].ToString() == "3")
            {
                return View();
            }
            else
            {
                return PartialView("Error");
            }
        }

        public ActionResult Step4()
        {
            if (Session["StepNo"].ToString() == "4")
            {
                return View();
            }
            else
            {
                return PartialView("Error");
            }
        }

        public ActionResult Step5()
        {
            if (Session["StepNo"].ToString() == "5")
            {
                decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
                List<ApplicantPersonalDetails> APDList = ApplicantManagement.GetApplicantDetailsList(ApplicantID);
                return View(APDList);
            }
            else
            {
                return PartialView("Error");
            }
            
        }

        public ActionResult Completed()
        {
            Session["StepNo"] = null;
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
            if (ApplicantID == 0 || ApplicantID == null) 
            {
                HttpCookie myCookie = Request.Cookies["ApplicantID"];
                ApplicantID = decimal.Parse(myCookie.Value);
            }
            
            string email = ApplicantManagement.GetLeadEmail(ApplicantID);
            ViewBag.Email = email;
            return View("Completed");
        }


        //====================================greenid======================================//

        [HttpPost]
        public JsonResult GreenidVerificationStatus(string VerificationStatus)
        {
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
           
            if (ApplicantManagement.GreenidVerificationStatus(ApplicantID, VerificationStatus))
            {
                int AppNo = int.Parse(Session["ApplicantNo"].ToString());
                AppNo++;
                Session["ApplicantNo"] = AppNo;
                return Json(AppNo);
            }
            else
            {
                return Json("");
            }
            
        }

        [HttpPost]
        public JsonResult GreenidVerificationId(string VerificationId, string Userdata, decimal GreenidVerificationID)
        {
            decimal ApplicantID = Convert.ToDecimal(Session["ApplicantID"]);
            if (ApplicantManagement.GreenidVerificationId(ApplicantID, VerificationId, GreenidVerificationID))
            {
                return Json("VerificationIdSaved");
            }
            else
            {
                return Json("");
            }
        }

        //not in use
        public ActionResult registerVerification(FormCollection formCollection)
        {

            bool verified = false;
            string verificationType = formCollection["verificationType"];
            if (verificationType == "DriverLicence")
            {
                string DriverLicenceNumber = formCollection["greenid_actrego_number"];
                verified = GreenidManagement.DriverLicence(DriverLicenceNumber);
            }
            else if (verificationType == "")
            {

            }
            if (!verified)
            {
                return View("Step2");
            }
            else
            {
                return RedirectToAction("IdentityCheck");
            }

        }


        //=====================================Lead=======================================//
        [HttpPost]
        public JsonResult Lead(string Name ,string Email, string MobileNo)
        {
            if (ApplicantManagement.Lead(Name ,Email,MobileNo))
            {
                return Json("leadSaved");
            }
            else
            {
                return Json("");
            }
        }

        [HttpPost]
        public JsonResult sendLeadEmail(string Email, string UserName)
        {
            if (EmailManagement.SendLeadEmail(Email,UserName))
            {
                return Json("leadMailed");
            }
            else
            {
                return Json("");
            }
        }


        //===================================End-Lead=====================================//

        public ActionResult GETCountryNames(string term)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();

            List<string> list = new List<string>();

            var Items = from c in db.Countries
                        select c;

            Items = Items.Where(p=>p.Description.Contains(term)).Take(10);

            foreach (var item in Items)
            {
                list.Add(item.Description);
            }
            return Json(list.ToArray(),JsonRequestBehavior.AllowGet);
        }


    }
}
