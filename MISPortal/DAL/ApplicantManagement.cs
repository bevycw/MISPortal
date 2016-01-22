using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MISPortal.Models;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MISPortal.DAL
{
    public class ApplicantManagement
    {
        public static bool Lead(string Name, string Email, string MobileNo)
        {
            try
            {
                if (EmailManagement.IsValidEmail(Email))
                {
                    MISPortal_DBDataContext db = new MISPortal_DBDataContext();
                    MISPortal.VisitLead l = new MISPortal.VisitLead();
                    l.FullName = Name;
                    l.Email = Email;
                    l.MobileNo = MobileNo;
                    l.CreationDateTime = DateTime.Now;
                    db.VisitLeads.InsertOnSubmit(l);
                    db.SubmitChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool AddEditIndividualApplicant(ApplicantPersonalDetails APD)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                MISPortal.Applicant APDMaster1 = new Applicant();
                APDMaster1.ApplicantID = APD._ApplicantID;
                APDMaster1.CreationDateTime = DateTime.Now;
                APDMaster1.AccountType = "Individual";
                APDMaster1.Status = "InProgress";
                APDMaster1.Approved = true;
                APDMaster1.Percent = 20;
                db.Applicants.InsertOnSubmit(APDMaster1);
                db.SubmitChanges();

                decimal maxidM1 = db.Applicants.Select(x => x.ApplicantID).Max();
                HttpContext.Current.Session["ApplicantID"] = maxidM1; 

                MISPortal.PersonalDetail APDMaster2 = new PersonalDetail();
                APDMaster2.PersonalDetailID = APD._PersonalDetailID;
                APDMaster2.ApplicantID = maxidM1;
                APDMaster2.ApplicantNo = 1;
                APDMaster2.Title = APD._Title;
                APDMaster2.FullName = APD._FullName;
                APDMaster2.MiddleName = APD._MiddleName;
                APDMaster2.Surname = APD._Surname;
                //10/23/2015 11:10:11 AM
                APDMaster2.BirthDate = DateTime.ParseExact(APD._BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                APDMaster2.BirthCountry = APD._CountryOfBirth;
                APDMaster2.Occupation = APD._Occupation;
                APDMaster2.HomeNo = APD._HomeNo;
                APDMaster2.BusinessNo = APD._BusinessNo;
                APDMaster2.MobileNo = APD._MobileNo;
                HttpContext.Current.Session["MobileNo"] = APD._MobileNo;
                APDMaster2.AreaCodeH = APD._AreaCodeH;
                APDMaster2.AreaCodeB = APD._AreaCodeB;
                APDMaster2.EmailAddress = APD._EmailAddress;
                APDMaster2.LeadEmail = APD._LeadEmail;
                HttpContext.Current.Session["EmailAddress"] = APD._EmailAddress;
                if (APD._CountryTax != null)
                {
                    APDMaster2.IsAusResident = false;
                    APDMaster2.CountryTax = APD._CountryTax;
                }
                else {
                    APDMaster2.IsAusResident = true;
                    APDMaster2.TaxFileNoExemptionCode = APD._TaxFileNoExemptionCode;
                    APDMaster2.ExemptionReason = APD._ExemptionReason;
                }
                APDMaster2.IsUSCitizen = APD._IsUSCitizen;
                if (APD._IsUSCitizen == true)
                {
                    APDMaster2.IsFATCA = APD._IsFATCA;
                }

                db.PersonalDetails.InsertOnSubmit(APDMaster2);
                db.SubmitChanges();

                
                decimal maxidM2 = db.PersonalDetails.Select(x => x.PersonalDetailID).Max();

                MISPortal.Address APDMaster3 = new Address();
                APDMaster3.AddressID = APD._AddressID;
                APDMaster3.PersonalDetailID = maxidM2;
                APDMaster3.AddressType = "Residential";
                //APDMaster3.Address1 = APD._RAddress;
                APDMaster3.Unit = Convert.ToDecimal(APD._RUnit);
                APDMaster3.StreetNo = Convert.ToDecimal(APD._RStreetNo);
                APDMaster3.StreetName = APD._RStreetName;
                APDMaster3.StreetType = APD._RStreetType;
                APDMaster3.Suburb = APD._RSuburb;
                APDMaster3.State = APD._RState;
                APDMaster3.Postcode = APD._RPostcode;
                APDMaster3.Country = APD._RCountry;
                APDMaster3.IsSame = APD._IsSame;
                db.Addresses.InsertOnSubmit(APDMaster3);
                db.SubmitChanges();

                if (APD._IsSame == false)
                {
                    MISPortal.Address APDMaster31 = new Address();
                    APDMaster31.AddressID = APD._AddressID;
                    APDMaster31.PersonalDetailID = maxidM2;
                    APDMaster31.AddressType = "Postal";
                    //APDMaster31.Address1 = APD._PAddress;
                    APDMaster31.Unit = Convert.ToDecimal(APD._PUnit);
                    APDMaster31.StreetNo = Convert.ToDecimal(APD._PStreetNo);
                    APDMaster31.StreetName = APD._PStreetName;
                    APDMaster31.StreetType = APD._PStreetType;
                    APDMaster31.Suburb = APD._PSuburb;
                    APDMaster31.State = APD._PState;
                    APDMaster31.Postcode = APD._PPostcode;
                    APDMaster31.Country = APD._PCountry;
                    APDMaster31.IsSame = APD._IsSame;
                    db.Addresses.InsertOnSubmit(APDMaster31);
                    db.SubmitChanges();
                }

                MISPortal.ApplicantNoVerificationId A = new ApplicantNoVerificationId();
                A.ApplicantID = maxidM1;
                A.ApplicantNo = 1;
                db.ApplicantNoVerificationIds.InsertOnSubmit(A);
                db.SubmitChanges();


                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool AddEditJointApplicant(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                MISPortal.Applicant APDMaster1 = new Applicant();
                APDMaster1.ApplicantID = APDList[0]._ApplicantID;
                APDMaster1.CreationDateTime = DateTime.Now;
                APDMaster1.AccountType = "Joint";
                APDMaster1.Status = "InProgress";
                APDMaster1.Approved = true;
                APDMaster1.Percent = 20;
                db.Applicants.InsertOnSubmit(APDMaster1);
                db.SubmitChanges();

                decimal maxidM1 = db.Applicants.Select(x => x.ApplicantID).Max();
                HttpContext.Current.Session["ApplicantID"] = maxidM1;

                int ApplicantNo = 1;
                foreach (var APD in APDList)
                {
                    if (APD._FullName != null)
                    {
                        MISPortal.PersonalDetail APDMaster2 = new PersonalDetail();
                        APDMaster2.PersonalDetailID = APD._PersonalDetailID;
                        APDMaster2.ApplicantID = maxidM1;
                        APDMaster2.ApplicantNo = ApplicantNo;
                        APDMaster2.Title = APD._Title;
                        APDMaster2.FullName = APD._FullName;
                        APDMaster2.MiddleName = APD._MiddleName;
                        APDMaster2.Surname = APD._Surname;
                        APDMaster2.BirthDate = DateTime.ParseExact(APD._BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        APDMaster2.BirthCountry = APD._CountryOfBirth;
                        APDMaster2.Occupation = APD._Occupation;
                        APDMaster2.HomeNo = APD._HomeNo;
                        APDMaster2.BusinessNo = APD._BusinessNo;
                        APDMaster2.MobileNo = APD._MobileNo;
                        APDMaster2.AreaCodeH = APD._AreaCodeH;
                        APDMaster2.AreaCodeB = APD._AreaCodeB;
                        APDMaster2.EmailAddress = APD._EmailAddress;
                        APDMaster2.LeadEmail = APDList[0]._LeadEmail;
                        APDMaster2.JointInvestors = APDList[0]._JointInvestors;
                        if (APD._CountryTax != null)
                        {
                            APDMaster2.IsAusResident = false;
                            APDMaster2.CountryTax = APD._CountryTax;
                        }
                        else
                        {
                            APDMaster2.IsAusResident = true;
                            APDMaster2.TaxFileNoExemptionCode = APD._TaxFileNoExemptionCode;
                            APDMaster2.ExemptionReason = APD._ExemptionReason;
                        }
                        APDMaster2.IsUSCitizen = APD._IsUSCitizen;
                        if (APD._IsUSCitizen == true)
                        {
                            APDMaster2.IsFATCA = APD._IsFATCA;
                        }
                        db.PersonalDetails.InsertOnSubmit(APDMaster2);
                        db.SubmitChanges();


                        decimal maxidM2 = db.PersonalDetails.Select(x => x.PersonalDetailID).Max();

                        MISPortal.Address APDMaster3 = new Address();
                        APDMaster3.AddressID = APD._AddressID;
                        APDMaster3.PersonalDetailID = maxidM2;
                        APDMaster3.AddressType = "Residential";
                        //APDMaster3.Address1 = APD._RAddress;
                        APDMaster3.Unit = Convert.ToDecimal(APD._RUnit);
                        APDMaster3.StreetNo = Convert.ToDecimal(APD._RStreetNo);
                        APDMaster3.StreetName = APD._RStreetName;
                        APDMaster3.StreetType = APD._RStreetType;
                        APDMaster3.Suburb = APD._RSuburb;
                        APDMaster3.State = APD._RState;
                        APDMaster3.Postcode = APD._RPostcode;
                        APDMaster3.Country = APD._RCountry;
                        APDMaster3.IsSame = APD._IsSame;
                        db.Addresses.InsertOnSubmit(APDMaster3);
                        db.SubmitChanges();

                        if (APD._IsSame == false)
                        {
                            MISPortal.Address APDMaster31 = new Address();
                            APDMaster31.AddressID = APD._AddressID;
                            APDMaster31.PersonalDetailID = maxidM2;
                            APDMaster31.AddressType = "Postal";
                            //APDMaster31.Address1 = APD._PAddress;
                            APDMaster31.Unit = Convert.ToDecimal(APD._PUnit);
                            APDMaster31.StreetNo = Convert.ToDecimal(APD._PStreetNo);
                            APDMaster31.StreetName = APD._PStreetName;
                            APDMaster31.StreetType = APD._PStreetType;
                            APDMaster31.Suburb = APD._PSuburb;
                            APDMaster31.State = APD._PState;
                            APDMaster31.Postcode = APD._PPostcode;
                            APDMaster31.Country = APD._PCountry;
                            APDMaster31.IsSame = APD._IsSame;
                            db.Addresses.InsertOnSubmit(APDMaster31);
                            db.SubmitChanges();
                        }

                        MISPortal.ApplicantNoVerificationId A = new ApplicantNoVerificationId();
                        A.ApplicantID = maxidM1;
                        A.ApplicantNo = ApplicantNo;
                        db.ApplicantNoVerificationIds.InsertOnSubmit(A);
                        db.SubmitChanges();

                        ApplicantNo++;
                    }
                }

                HttpContext.Current.Session["MobileNo"] = APDList[0]._MobileNo;
                HttpContext.Current.Session["EmailAddress"] = APDList[0]._EmailAddress;

                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool AddEditTrustIndividualApplicant(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                MISPortal.Applicant APDMaster1 = new Applicant();
                APDMaster1.ApplicantID = APDList[0]._ApplicantID;
                APDMaster1.CreationDateTime = DateTime.Now;
                APDMaster1.AccountType = "TrustIndividual";
                APDMaster1.Status = "InProgress";
                APDMaster1.Approved = true;
                APDMaster1.Percent = 20;
                db.Applicants.InsertOnSubmit(APDMaster1);
                db.SubmitChanges();

                decimal maxidM1 = db.Applicants.Select(x => x.ApplicantID).Max();
                HttpContext.Current.Session["ApplicantID"] = maxidM1;

                int ApplicantNo = 1;
                foreach (var APD in APDList)
                {
                    if (APD._FullName != null)
                    {

                        MISPortal.PersonalDetail APDMaster2 = new PersonalDetail();
                        APDMaster2.PersonalDetailID = APD._PersonalDetailID;
                        APDMaster2.ApplicantID = maxidM1;
                        APDMaster2.ApplicantNo = ApplicantNo;
                        APDMaster2.Title = APD._Title;
                        APDMaster2.FullName = APD._FullName;
                        APDMaster2.MiddleName = APD._MiddleName;
                        APDMaster2.Surname = APD._Surname;
                        APDMaster2.BirthDate = DateTime.ParseExact(APD._BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        APDMaster2.BirthCountry = APD._CountryOfBirth;
                        APDMaster2.Occupation = APD._Occupation;
                        APDMaster2.IsBeneficiary = APD._IsBeneficiary;
                        APDMaster2.HomeNo = APD._HomeNo;
                        APDMaster2.BusinessNo = APD._BusinessNo;
                        APDMaster2.MobileNo = APD._MobileNo;
                        HttpContext.Current.Session["MobileNo"] = APD._MobileNo;
                        APDMaster2.AreaCodeH = APD._AreaCodeH;
                        APDMaster2.AreaCodeB = APD._AreaCodeB;
                        APDMaster2.EmailAddress = APD._EmailAddress;
                        APDMaster2.LeadEmail = APDList[0]._LeadEmail;
                        HttpContext.Current.Session["EmailAddress"] = APD._EmailAddress;
                        //if (APD._CountryTax != null)
                        //{
                        //    APDMaster2.IsAusResident = false;
                        //    APDMaster2.CountryTax = APD._CountryTax;
                        //}
                        //else
                        //{
                        //    APDMaster2.IsAusResident = true;
                        //    APDMaster2.TaxFileNoExemptionCode = APD._TaxFileNoExemptionCode;
                        //    APDMaster2.ExemptionReason = APD._ExemptionReason;
                        //}
                        //APDMaster2.IsUSCitizen = APD._IsUSCitizen;
                        //if (APD._IsUSCitizen == true)
                        //{
                        //    APDMaster2.IsFATCA = APD._IsFATCA;
                        //}
                        APDMaster2.NameTrust = APDList[0]._NameTrust;
                        APDMaster2.ABN = APDList[0]._ABN;
                        APDMaster2.AusTFN = APDList[0]._AusTFN;
                        db.PersonalDetails.InsertOnSubmit(APDMaster2);
                        db.SubmitChanges();


                        decimal maxidM2 = db.PersonalDetails.Select(x => x.PersonalDetailID).Max();

                        MISPortal.Address APDMaster3 = new Address();
                        APDMaster3.AddressID = APD._AddressID;
                        APDMaster3.PersonalDetailID = maxidM2;
                        APDMaster3.AddressType = "Residential";
                        //APDMaster3.Address1 = APD._RAddress;
                        APDMaster3.Unit = Convert.ToDecimal(APD._RUnit);
                        APDMaster3.StreetNo = Convert.ToDecimal(APD._RStreetNo);
                        APDMaster3.StreetName = APD._RStreetName;
                        APDMaster3.StreetType = APD._RStreetType;
                        APDMaster3.Suburb = APD._RSuburb;
                        APDMaster3.State = APD._RState;
                        APDMaster3.Postcode = APD._RPostcode;
                        APDMaster3.Country = APD._RCountry;
                        APDMaster3.IsSame = APD._IsSame;
                        db.Addresses.InsertOnSubmit(APDMaster3);
                        db.SubmitChanges();

                        if (APD._IsSame == false)
                        {
                            MISPortal.Address APDMaster31 = new Address();
                            APDMaster31.AddressID = APD._AddressID;
                            APDMaster31.PersonalDetailID = maxidM2;
                            APDMaster31.AddressType = "Postal";
                            //APDMaster31.Address1 = APD._PAddress;
                            APDMaster31.Unit = Convert.ToDecimal(APD._PUnit);
                            APDMaster31.StreetNo = Convert.ToDecimal(APD._PStreetNo);
                            APDMaster31.StreetName = APD._PStreetName;
                            APDMaster31.StreetType = APD._PStreetType;
                            APDMaster31.Suburb = APD._PSuburb;
                            APDMaster31.State = APD._PState;
                            APDMaster31.Postcode = APD._PPostcode;
                            APDMaster31.Country = APD._PCountry;
                            APDMaster31.IsSame = APD._IsSame;
                            db.Addresses.InsertOnSubmit(APDMaster31);
                            db.SubmitChanges();
                        }

                        MISPortal.ApplicantNoVerificationId A = new ApplicantNoVerificationId();
                        A.ApplicantID = maxidM1;
                        A.ApplicantNo = ApplicantNo;
                        db.ApplicantNoVerificationIds.InsertOnSubmit(A);
                        db.SubmitChanges();

                        ApplicantNo++;
                    }
                }
                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool AddEditTrustCompanyApplicant(Company APD)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                MISPortal.Applicant APDMaster1 = new Applicant();
                APDMaster1.ApplicantID = APD._ApplicantID;
                APDMaster1.CreationDateTime = DateTime.Now;
                APDMaster1.AccountType = "TrustCompany";
                APDMaster1.Status = "InProgress";
                APDMaster1.Approved = true;
                APDMaster1.Percent = 20;
                db.Applicants.InsertOnSubmit(APDMaster1);
                db.SubmitChanges();

                decimal maxidM1 = db.Applicants.Select(x => x.ApplicantID).Max();
                HttpContext.Current.Session["ApplicantID"] = maxidM1;

                MISPortal.TrustCompany APDMaster2 = new TrustCompany();
                APDMaster2.TrustCompanyID = Convert.ToDecimal(APD._TrustCompanyID);
                APDMaster2.ApplicantID = maxidM1;
                APDMaster2.FullName = APD._FullNameCompany;
                APDMaster2.ACN_ABN = APD._ACN_ABN_Company;
                APDMaster2.AusTFN = APD._AusTFN_Company;
                APDMaster2.TitleCP = APD._TitleCP;
                APDMaster2.FullNameCP = APD._FullNameCP;
                APDMaster2.SurnameCP = APD._SurnameCP;
                APDMaster2.BusinessNoCP = APD._BusinessNoCP;
                APDMaster2.MobileNoCP = APD._MobileNoCP;
                HttpContext.Current.Session["MobileNo"] = APD._MobileNoCP;
                APDMaster2.AreaCodeB = APD._AreaCodeB_CP;
                APDMaster2.EmailAddressCP = APD._EmailAddrCP;
                HttpContext.Current.Session["EmailAddress"] = APD._EmailAddrCP;
                APDMaster2.OccupationCP = APD._OccupationCP;
                //APDMaster2.FullNameFC = APD._FullNameFC;
                //APDMaster2.ARBN_FC = APD._ARBN_FC;
                //APDMaster2.IsRegByForeignRegBodyFC = APD._IsRegByForeignRegBodyFC;
                //APDMaster2.ForeignRegBodyNameFC = APD._ForeignRegBodyNameFC;
                //APDMaster2.StatusFC = APD._StatusFC;
                //APDMaster2.NameLocaLAgent = APD._NameLA;
                //APDMaster2.FullNameFCNR = APD._FullNameFCNR;
                //APDMaster2.IsRegByForeignRegBodyFCNR = APD._IsRegByForeignRegBodyFCNR;
                //APDMaster2.ForeignRegBodyNameFCNR = APD._ForeignRegBodyNameFCNR;
                //APDMaster2.ForeignRegBodyIdenNoFCNR = APD._ForeignRegBodyIdenNoFCNR;
                //APDMaster2.IsRegInUs = APD._IsRegInUs;
                APDMaster2.NameTrust = APD._NameTrust;
                APDMaster2.ABN_Trust = APD._ABN_Trust;
                APDMaster2.AusTFN_Trust = APD._AusTFN_Trust;
                APDMaster2.LeadEmail = APD._LeadEmail;
                db.TrustCompanies.InsertOnSubmit(APDMaster2);
                db.SubmitChanges();

                decimal maxidM2 = db.TrustCompanies.Select(x => x.TrustCompanyID).Max();

                MISPortal.TrustCompanyAddress APDMaster3 = new TrustCompanyAddress();
                APDMaster3.TrustCompanyAddressID = Convert.ToDecimal(APD._TrustCompanyAddressID);
                APDMaster3.TrustCompanyID = maxidM2;
                APDMaster3.AddressType = "Registered";
                APDMaster3.Address = APD._AddressCompany;
                APDMaster3.Suburb = APD._SuburbCompany;
                APDMaster3.State = APD._StateCompany;
                APDMaster3.Postcode = APD._PostcodeCompany;
                APDMaster3.Country = APD._CountryCompany;
                APDMaster3.IsSame = APD._IsSameCompany;
                db.TrustCompanyAddresses.InsertOnSubmit(APDMaster3);
                db.SubmitChanges();

                if (APD._IsSameCompany == false)
                {
                    MISPortal.TrustCompanyAddress APDMaster31 = new TrustCompanyAddress();
                    APDMaster31.TrustCompanyAddressID = Convert.ToDecimal(APD._TrustCompanyAddressID);
                    APDMaster31.TrustCompanyID = maxidM2;
                    APDMaster31.AddressType = "Postal";
                    APDMaster31.Address = APD._PAddressCompany;
                    APDMaster31.PPOBox = APD._PPOBoxCompany;
                    APDMaster31.Suburb = APD._PSuburbCompany;
                    APDMaster31.State = APD._PStateCompany;
                    APDMaster31.Postcode = APD._PPostcodeCompany;
                    APDMaster31.Country = APD._PCountryCompany;
                    APDMaster31.IsSame = APD._IsSameCompany;
                    db.TrustCompanyAddresses.InsertOnSubmit(APDMaster31);
                    db.SubmitChanges();
                }

                //MISPortal.TrustCompanyAddress APDMaster32 = new TrustCompanyAddress();
                //APDMaster32.TrustCompanyAddressID = Convert.ToDecimal(APD._TrustCompanyAddressID);
                //APDMaster32.TrustCompanyID = maxidM2;
                //APDMaster32.AddressType = "RegisteredOfficeInAustralia";
                //APDMaster32.Address = APD._AddressAus;
                //APDMaster32.Suburb = APD._SuburbAus;
                //APDMaster32.State = APD._StateAus;
                //APDMaster32.Postcode = APD._PostcodeAus;
                //db.TrustCompanyAddresses.InsertOnSubmit(APDMaster32);
                //db.SubmitChanges();

                //if (APD._StatusFC != "IsSame")
                //{
                //    MISPortal.TrustCompanyAddress APDMaster33 = new TrustCompanyAddress();
                //    APDMaster33.TrustCompanyAddressID = Convert.ToDecimal(APD._TrustCompanyAddressID);
                //    APDMaster33.TrustCompanyID = maxidM2;
                //    APDMaster33.AddressType = "LocalAgent";
                //    APDMaster33.Address = APD._AddressLA;
                //    APDMaster33.Suburb = APD._SuburbLA;
                //    APDMaster33.State = APD._StateLA;
                //    APDMaster33.Postcode = APD._PostcodeLA;
                //    db.TrustCompanyAddresses.InsertOnSubmit(APDMaster33);
                //    db.SubmitChanges();
                //}
                //MISPortal.TrustCompanyAddress APDMaster34 = new TrustCompanyAddress();
                //APDMaster34.TrustCompanyAddressID = Convert.ToDecimal(APD._TrustCompanyAddressID);
                //APDMaster34.TrustCompanyID = maxidM2;
                //APDMaster34.AddressType = "FCNR";
                //APDMaster34.Address = APD._AddressFCNR;
                //APDMaster34.Suburb = APD._SuburbFCNR;
                //APDMaster34.State = APD._StateFCNR;
                //APDMaster34.Postcode = APD._PostcodeFCNR;
                //APDMaster34.Country = APD._CountryFCNR;
                //db.TrustCompanyAddresses.InsertOnSubmit(APDMaster34);
                //db.SubmitChanges();

                MISPortal.TrustCompanyAddress APDMaster35 = new TrustCompanyAddress();
                APDMaster35.TrustCompanyAddressID = Convert.ToDecimal(APD._TrustCompanyAddressID);
                APDMaster35.TrustCompanyID = maxidM2;
                APDMaster35.AddressType = "PPFCNR";
                APDMaster35.Address = APD._AddressPPFCNR;
                APDMaster35.Suburb = APD._SuburbPPFCNR;
                APDMaster35.State = APD._StatePPFCNR;
                APDMaster35.Postcode = APD._PostcodePPFCNR;
                APDMaster35.Country = APD._CountryPPFCNR;
                db.TrustCompanyAddresses.InsertOnSubmit(APDMaster35);
                db.SubmitChanges();

                if (APD._IsPublicCompany == false)
                {
                    foreach (var sh in APD.Shareholders)
                    {
                        if (sh._FullName != null)
                        {
                            MISPortal.Shareholder APDMaster4 = new Shareholder();
                            APDMaster4.ShareholderID = sh._ShareholderID;
                            APDMaster4.TrustCompanyID = maxidM2;
                            APDMaster4.Title = sh._Title;
                            APDMaster4.FullName = sh._FullName;
                            APDMaster4.Surname = sh._Surname;
                            APDMaster4.Address = sh._Address;
                            APDMaster4.Suburb = sh._Suburb;
                            APDMaster4.State = sh._State;
                            APDMaster4.Postcode = sh._Postcode;
                            APDMaster4.Country = sh._Country;
                            db.Shareholders.InsertOnSubmit(APDMaster4);
                            db.SubmitChanges();
                        }
                    }
                }

                MISPortal.ApplicantNoVerificationId A = new ApplicantNoVerificationId();
                A.ApplicantID = maxidM1;
                A.ApplicantNo = 1;
                db.ApplicantNoVerificationIds.InsertOnSubmit(A);
                db.SubmitChanges();

                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool IdentityVerification(decimal ApplicantID)
        {
            try
            {
                MISPortal_DBDataContext db = new MISPortal_DBDataContext();
                var App = db.Applicants.SingleOrDefault(a => a.ApplicantID == ApplicantID);
                App.Percent = 40;
                db.SubmitChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool AddEditInvestmentDetailApplicant(InvestmentDetails ID, decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                foreach (var i in ID.IDL)
                {
                    if (i._InvestmentAmount != 0)
                    {
                        MISPortal.InvestmentDetail IDMaster1 = new InvestmentDetail();
                        IDMaster1.InvestmentDetailsID = i._InvestmentDetailsID;
                        IDMaster1.ApplicantID = ApplicantID;
                        IDMaster1.FundName = i._FundName;
                        IDMaster1.InvestmentAmount = i._InvestmentAmount;
                        IDMaster1.RegularSavingPlanPerMonth = i._RegularSavingPlanPerMonth;
                        IDMaster1.Reinvest = i._Reinvest;
                        IDMaster1.SourceofIncome = ID.IDL[0]._SourceofIncome;
                        db.InvestmentDetails.InsertOnSubmit(IDMaster1);
                        db.SubmitChanges();
                    }
                }
                if (ID.PM[0]._PaymentMethod == true)//"Cheque"
                {
                    MISPortal.PaymentMethod PMMaster11 = new PaymentMethod();
                    PMMaster11.PaymentMethodID = ID.PM[0]._PaymentMethodID;
                    PMMaster11.ApplicantID = ApplicantID;
                    PMMaster11.PaymentType = "Cheque";
                    db.PaymentMethods.InsertOnSubmit(PMMaster11);
                    db.SubmitChanges();
                }
                if (ID.PM[1]._PaymentMethod == true)
                {
                    //int index=01;
                    //if (ID.PM[0]._PaymentType == "DirectDebit"){
                    //    index = 0;
                    //}
                    //else if (ID.PM[0]._PaymentType == "DirectCreditEFT")
                    //{
                    //    index = 1;
                    //}
                    MISPortal.PaymentMethod PMMaster12 = new PaymentMethod();
                    PMMaster12.PaymentMethodID = ID.PM[1]._PaymentMethodID;
                    PMMaster12.ApplicantID = ApplicantID;
                    PMMaster12.PaymentType = "DirectDebit";
                    PMMaster12.FinancialInstituteName = ID.PM[1]._FinancialInstituteName;
                    PMMaster12.BranchName = ID.PM[1]._BranchName;
                    PMMaster12.BSBNumber = ID.PM[1]._BSBNumber;
                    PMMaster12.AccountNumber = ID.PM[1]._BSBNumber;
                    PMMaster12.AccountName = ID.PM[1]._AccountName;
                    db.PaymentMethods.InsertOnSubmit(PMMaster12);
                    db.SubmitChanges();
                }
                if (ID.PM[2]._PaymentMethod == true)
                {
                    MISPortal.PaymentMethod PMMaster13 = new PaymentMethod();
                    PMMaster13.PaymentMethodID = ID.PM[2]._PaymentMethodID;
                    PMMaster13.ApplicantID = ApplicantID;
                    PMMaster13.PaymentType = "DirectCreditEFT";
                    PMMaster13.FinancialInstituteName = ID.PM[2]._FinancialInstituteName;
                    PMMaster13.BranchName = ID.PM[2]._BranchName;
                    PMMaster13.BSBNumber = ID.PM[2]._BSBNumber;
                    PMMaster13.AccountNumber = ID.PM[2]._BSBNumber;
                    PMMaster13.AccountName = ID.PM[2]._AccountName;
                    db.PaymentMethods.InsertOnSubmit(PMMaster13);
                    db.SubmitChanges();
                }

                var App = db.Applicants.SingleOrDefault(a => a.ApplicantID == ApplicantID);
                App.Percent = 60;
                db.SubmitChanges();

                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool AddEditAuthRepFinAdvDetailApplicant(Step4 S4, decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                if (S4.ARD._IsAuthRep == true)
                {
                    MISPortal.AuthorisedRepresentativeDetail ARDMaster1 = new AuthorisedRepresentativeDetail();
                    ARDMaster1.AuthRepDetailID = S4.ARD._AuthRepDetailID;
                    ARDMaster1.ApplicantID = ApplicantID;
                    ARDMaster1.Title = S4.ARD._Title;
                    ARDMaster1.FullName = S4.ARD._FullName;
                    ARDMaster1.Surname = S4.ARD._Surname;
                    ARDMaster1.TelephoneNo = S4.ARD._TelephoneNo;
                    ARDMaster1.MobileNo = S4.ARD._MobileNo;
                    ARDMaster1.EmailAddress = S4.ARD._EmailAddress;
                    ARDMaster1.AFSL_LicenceNo = S4.ARD._AFSL_LicenceNo;
                    db.AuthorisedRepresentativeDetails.InsertOnSubmit(ARDMaster1);
                    db.SubmitChanges();
                }
                if (S4.FAD._IsFinAdv == true)
                {
                    MISPortal.FinancialAdviserDetail FADMaster1 = new FinancialAdviserDetail();
                    FADMaster1.FinancialAdviserDetailID = S4.FAD._FinancialAdviserDetailID;
                    FADMaster1.ApplicantID = ApplicantID;
                    FADMaster1.DealerGroupName = S4.FAD._DealerGroupName;
                    FADMaster1.AdviserName = S4.FAD._AdviserName;
                    FADMaster1.AFSL_Number = S4.FAD._AFSL_Number;
                    FADMaster1.ContactNo = S4.FAD._ContactNo;
                    FADMaster1.EmailAddress = S4.FAD._EmailAddress;
                    FADMaster1.BusinessNo = S4.FAD._BusinessNo;
                    FADMaster1.MobileNo = S4.FAD._MobileNo;
                    db.FinancialAdviserDetails.InsertOnSubmit(FADMaster1);
                    db.SubmitChanges();

                    decimal maxidM1 = db.FinancialAdviserDetails.Select(x => x.FinancialAdviserDetailID).Max();

                    MISPortal.FinancialAdviserAddress FAAMaster1 = new FinancialAdviserAddress();
                    FAAMaster1.FinancialAdviserAddressID = S4.FAD._FinancialAdviserAddressID;
                    FAAMaster1.FinancialAdviserDetailID = maxidM1;
                    FAAMaster1.AddressType = "Business";
                    FAAMaster1.Address = S4.FAD._BAddress;
                    FAAMaster1.Suburb = S4.FAD._BSuburb;
                    FAAMaster1.State = S4.FAD._BState;
                    FAAMaster1.Postcode = S4.FAD._BPostcode;
                    FAAMaster1.Country = S4.FAD._BCountry;
                    FAAMaster1.IsSame = S4.FAD._IsSame;
                    db.FinancialAdviserAddresses.InsertOnSubmit(FAAMaster1);
                    db.SubmitChanges();

                    if (S4.FAD._IsSame == false)
                    {
                        MISPortal.FinancialAdviserAddress FAAMaster2 = new FinancialAdviserAddress();
                        FAAMaster2.FinancialAdviserAddressID = S4.FAD._FinancialAdviserAddressID;
                        FAAMaster2.FinancialAdviserDetailID = maxidM1;
                        FAAMaster2.AddressType = "Postal";
                        FAAMaster2.Address = S4.FAD._PAddress;
                        FAAMaster2.POBox = S4.FAD._PPOBox;
                        FAAMaster2.Suburb = S4.FAD._PSuburb;
                        FAAMaster2.State = S4.FAD._PState;
                        FAAMaster2.Postcode = S4.FAD._PPostcode;
                        FAAMaster2.Country = S4.FAD._PCountry;
                        db.FinancialAdviserAddresses.InsertOnSubmit(FAAMaster2);
                        db.SubmitChanges();
                    }
                }

                var App = db.Applicants.SingleOrDefault(a => a.ApplicantID == ApplicantID);
                App.Percent = 80;
                db.SubmitChanges();
                
                
                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool ConfirmAndSubmitApplicant(decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();

            try
            {
                string AccountType = "";
                var A = db.Applicants.SingleOrDefault(p=>p.ApplicantID==ApplicantID);
                A.Status = "Complete(all)";
                AccountType = A.AccountType;
                A.Percent = 100;
                db.SubmitChanges();

                HttpContext.Current.Session["ApplicantNo"] = null;


                SalesforceManagement sfobj = new SalesforceManagement();
                SalesforceModel sf = new SalesforceModel();

                if (AccountType != "TrustCompany")
                {
                    var PDL = (from pd in db.PersonalDetails
                               where pd.ApplicantID == ApplicantID
                               select pd);
                    var PD = PDL.OrderBy(p => p.PersonalDetailID).FirstOrDefault();

                    var PDAL = (from pda in db.Addresses
                                where pda.PersonalDetailID == PD.PersonalDetailID && pda.AddressType == "Residential"
                                select pda);
                    var PDA = PDAL.OrderBy(p => p.AddressID).SingleOrDefault();

                    sf._EmailAddress = PD.LeadEmail;
                    sf._Address = PDA.Unit+"  "+PDA.StreetNo+"  "+PDA.StreetName+"  "+PDA.StreetType;
                    sf._City = PDA.Suburb;
                    sf._State = PDA.State;
                    sf._Postcode = PDA.Postcode;
                    sf._Country = PDA.Country;
                    sf._FirstName = PD.Title + " " + PD.FullName;
                    sf._LastName = PD.Surname;
                    sf._Mobile = PD.MobileNo;
                    sf._Company = "na";
                    sfobj.SaveRecord(sf);
                }
                else
                {
                    var C = db.TrustCompanies.SingleOrDefault(p => p.ApplicantID == ApplicantID);
                    var CA = db.TrustCompanyAddresses.SingleOrDefault(p => p.TrustCompanyID == C.TrustCompanyID && p.AddressType == "Registered");

                    sf._EmailAddress = C.LeadEmail;
                    sf._Address = CA.Address;
                    sf._City = CA.Suburb;
                    sf._State = CA.State;
                    sf._Postcode = CA.Postcode;
                    sf._Country = CA.Country;
                    sf._FirstName = C.TitleCP + " " + C.FullNameCP;
                    sf._LastName = C.SurnameCP;
                    sf._Mobile = C.MobileNoCP;
                    sf._Company = C.FullName;
                    sfobj.SaveRecord(sf);
                }

                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }


        public static string GetLeadEmail(decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();

            try
            {
                var App = db.Applicants.SingleOrDefault(a => a.ApplicantID == ApplicantID);

                string email = "";
                if (App.AccountType != "TrustCompany")
                {
                    var PD = db.PersonalDetails.SingleOrDefault(c => c.ApplicantID == ApplicantID && c.ApplicantNo == 1);
                    email = PD.LeadEmail;
                }
                else
                {
                    var TC = db.TrustCompanies.SingleOrDefault(p => p.ApplicantID == App.ApplicantID);
                    email = TC.LeadEmail;
                }

                return email;
            }
            catch
            {
                return "";
            }
        }

        public static ApplicantPersonalDetails GetApplicantDetails(decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();

            var App = db.Applicants.SingleOrDefault(a => a.ApplicantID == ApplicantID);

            ApplicantPersonalDetails app = new ApplicantPersonalDetails();
            app._AccountType = App.AccountType;
            string DT = Convert.ToDateTime(App.CreationDateTime).ToString("dd-MMM-yy");
            app._CreationDateTime = Convert.ToDateTime(DT);
            app._Status = App.Status;
            app._Percent = Convert.ToDecimal(App.Percent);

            if (App.AccountType != "TrustCompany")
            {
                int count = db.PersonalDetails.Where(c => c.ApplicantID == ApplicantID).Count();
                //HttpContext.Current.Session["AppCount"] = count;

                //var PD = (from pd in db.PersonalDetails
                //          where pd.ApplicantID == App.ApplicantID
                //          select pd).OrderBy(p => p.ApplicantID).First();
                int AppNo=0;
                if (HttpContext.Current.Session["ApplicantNo"] != null)
                {
                    AppNo = int.Parse(HttpContext.Current.Session["ApplicantNo"].ToString());
                }
                else { AppNo = 1; HttpContext.Current.Session["ApplicantNo"] = 1; }
                var PD = db.PersonalDetails.SingleOrDefault(pd => pd.ApplicantID == ApplicantID && pd.ApplicantNo == AppNo );

                app._PersonalDetailID = PD.PersonalDetailID;
                app._AppCount = count;
                app._Title = PD.Title;
                app._FullName = PD.FullName;
                app._MiddleName = PD.MiddleName;
                app._Surname = PD.Surname;
                app._BirthDate = Convert.ToDateTime(PD.BirthDate).ToString("dd/M/yyyy");
                app._BusinessNo = PD.BusinessNo;
                app._MobileNo = PD.MobileNo;
                app._AreaCodeB = PD.AreaCodeB;
                app._EmailAddress = PD.EmailAddress;
                app._LeadEmail = PD.LeadEmail;
                app._TaxFileNoExemptionCode = PD.TaxFileNoExemptionCode;
                app._IsAusResident = Convert.ToBoolean(PD.IsAusResident);
                if (PD.CountryTax != null)
                {
                    app._CountryTax = PD.CountryTax;
                }
                else { app._CountryTax = "Australia"; }

                var Addr = (from addr in db.Addresses
                            where addr.PersonalDetailID == PD.PersonalDetailID
                            select addr);

                foreach (var adr in Addr)
                {
                    if (adr.AddressType == "Residential")
                    {
                        app._RAddress = adr.Unit.ToString() + "  " + adr.StreetNo.ToString() + "  " + adr.StreetName + "  " + adr.StreetType;
                        if (adr.Unit == 0)
                        {
                            app._RUnit = null;
                        }
                        else
                        {
                            app._RUnit = adr.Unit.ToString();
                        } 
                        app._RStreetNo = adr.StreetNo.ToString();
                        app._RStreetName = adr.StreetName;
                        app._RStreetType = adr.StreetType;
                        app._RSuburb = adr.Suburb;
                        app._RState = adr.State;
                        app._RPostcode = adr.Postcode;
                        app._RCountry = adr.Country;
                    }
                }
            }
            else
            {
                var TC = db.TrustCompanies.SingleOrDefault(p => p.ApplicantID == App.ApplicantID);
                app._PersonalDetailID = TC.TrustCompanyID;
                app._FullName = TC.FullNameCP;
                app._Surname = TC.SurnameCP;
                app._BirthDate = "";
                app._BusinessNo = TC.BusinessNoCP;
                app._MobileNo = TC.MobileNoCP;
                app._AreaCodeB = TC.AreaCodeB;
                app._EmailAddress = TC.EmailAddressCP;
                app._TaxFileNoExemptionCode = TC.AusTFN;
                app._LeadEmail = TC.LeadEmail;
                app._IsAusResident = true;
                app._CountryTax = "Australia";

                var Addr = (from addr in db.TrustCompanyAddresses
                            where addr.TrustCompanyID == TC.TrustCompanyID
                            select addr);

                foreach (var adr in Addr)
                {
                    if (adr.AddressType == "Registered")
                    {
                        app._RAddressID = adr.TrustCompanyAddressID;
                        app._RAddress = adr.Address;
                        app._RSuburb = adr.Suburb;
                        app._RState = adr.State;
                        app._RPostcode = adr.Postcode;
                        app._RCountry = adr.Country;
                        app._IsSame = Convert.ToBoolean(adr.IsSame);
                    }
                }

            }

            return app;
        }

        public static List<ApplicantPersonalDetails> GetApplicantDetailsList(decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            List<ApplicantPersonalDetails> AAPDist = new List<ApplicantPersonalDetails>();
            ApplicantPersonalDetails APD = new ApplicantPersonalDetails();

            //Applicant
            var A = db.Applicants.SingleOrDefault(p => p.ApplicantID == ApplicantID);
            APD._ApplicantID = A.ApplicantID;
            APD._AccountType = A.AccountType;

            #region Individual-Joint-TrustIndividual
            if (A.AccountType != "TrustCompany")
            {
                //Personal Details
                var PD = from pd in db.PersonalDetails
                         where pd.ApplicantID == ApplicantID
                         select pd;

                decimal appcount = 1;
                decimal PersonalDetailID = 0;
                foreach (var pd in PD)
                {
                    ApplicantPersonalDetails apd = new ApplicantPersonalDetails();
                    PersonalDetailID = pd.PersonalDetailID;
                    apd._PersonalDetailID = pd.PersonalDetailID;
                    apd._ApplicantID = Convert.ToDecimal(pd.ApplicantID);
                    apd._Title = pd.Title;
                    apd._FullName = pd.FullName;
                    apd._MiddleName = pd.MiddleName;
                    apd._Surname = pd.Surname;
                    apd._BirthDate = Convert.ToDateTime(pd.BirthDate).ToString("dd/MM/yyyy");
                    apd._CountryOfBirth = pd.BirthCountry;
                    apd._Occupation = pd.Occupation;
                    apd._IsBeneficiary = Convert.ToBoolean(pd.IsBeneficiary);
                    apd._HomeNo = pd.HomeNo;
                    apd._BusinessNo = pd.BusinessNo;
                    apd._MobileNo = pd.MobileNo;
                    apd._AreaCodeH = pd.AreaCodeH;
                    apd._AreaCodeB = pd.AreaCodeB;
                    apd._EmailAddress = pd.EmailAddress;
                    apd._LeadEmail = pd.LeadEmail;
                    apd._TaxFileNoExemptionCode = pd.TaxFileNoExemptionCode;
                    apd._ExemptionReason = pd.ExemptionReason;
                    apd._CountryTax = pd.CountryTax;
                    apd._IsUSCitizen = Convert.ToBoolean(pd.IsUSCitizen);
                    apd._IsFATCA = Convert.ToBoolean(pd.IsFATCA);
                    apd._NameTrust = pd.NameTrust;
                    apd._ABN = pd.ABN;
                    apd._AusTFN = pd.AusTFN;
                    apd._JointInvestors = pd.JointInvestors;

                    //Address
                    var ADD = from ad in db.Addresses
                              where ad.PersonalDetailID == PersonalDetailID
                              select ad;
                    foreach (var ad in ADD)
                    {
                        if (ad.AddressType == "Residential")
                        {
                            apd._RAddressID = ad.AddressID;
                            //apd._RAddress = ad.Address1;
                            apd._RUnit = ad.Unit.ToString();
                            apd._RStreetNo = ad.StreetNo.ToString();
                            apd._RStreetName = ad.StreetName;
                            apd._RStreetType = ad.StreetType;
                            apd._RSuburb = ad.Suburb;
                            apd._RState = ad.State;
                            apd._RPostcode = ad.Postcode;
                            apd._RCountry = ad.Country;
                            apd._IsSame = Convert.ToBoolean(ad.IsSame);
                        }
                        else if (ad.AddressType == "Postal")
                        {
                            apd._PAddressID = ad.AddressID;
                            //apd._PAddress = ad.Address1;
                            apd._PUnit = ad.Unit.ToString();
                            apd._PStreetNo = ad.StreetNo.ToString();
                            apd._PStreetName = ad.StreetName;
                            apd._PStreetType = ad.StreetType;
                            apd._PSuburb = ad.Suburb;
                            apd._PState = ad.State;
                            apd._PCountry = ad.Country;
                            apd._PPostcode = ad.Postcode;
                        }
                    }

                    var V = db.ApplicantNoVerificationIds.SingleOrDefault(v => v.ApplicantID == ApplicantID && v.ApplicantNo == appcount);
                    if (V.VerificationResult == null || V.VerificationResult == "")
                    {
                        apd._VerificationResult = "IN_PROGRESS";
                    }
                    else
                    {
                        apd._VerificationResult = V.VerificationResult;
                    }

                    appcount++;
                    APD.PD.Add(apd);
                }
            }
            #endregion

            #region Trust Company
            //Trust Company
            if (A.AccountType == "TrustCompany")
            {
                var TC = db.TrustCompanies.SingleOrDefault(p => p.ApplicantID == ApplicantID);

                decimal TrustCompanyID = TC.TrustCompanyID;
                APD.cmp._TrustCompanyID = TC.TrustCompanyID;
                APD.cmp._FullNameCompany = TC.FullName;
                APD.cmp._ACN_ABN_Company = TC.ACN_ABN;
                APD.cmp._AusTFN_Company = TC.AusTFN;
                APD.cmp._TitleCP = TC.TitleCP;
                APD.cmp._FullNameCP = TC.FullNameCP;
                APD.cmp._SurnameCP = TC.SurnameCP;
                APD.cmp._AreaCodeB_CP = TC.AreaCodeB;
                APD.cmp._BusinessNoCP = TC.BusinessNoCP;
                APD.cmp._MobileNoCP = TC.MobileNoCP;
                APD.cmp._EmailAddrCP = TC.EmailAddressCP;
                APD.cmp._OccupationCP = TC.OccupationCP;
                //APD.cmp._FullNameFC = TC.FullNameFC;
                //APD.cmp._ARBN_FC = TC.ARBN_FC;
                //APD.cmp._IsRegByForeignRegBodyFC = Convert.ToBoolean(TC.IsRegByForeignRegBodyFC);
                //APD.cmp._ForeignRegBodyNameFC = TC.ForeignRegBodyNameFC;
                //APD.cmp._StatusFC = TC.StatusFC;
                //APD.cmp._NameLA = TC.NameLocaLAgent;
                //APD.cmp._FullNameFCNR = TC.FullNameFCNR;
                //APD.cmp._IsRegByForeignRegBodyFCNR = Convert.ToBoolean(TC.IsRegByForeignRegBodyFCNR);
                //APD.cmp._ForeignRegBodyNameFCNR = TC.ForeignRegBodyNameFCNR;
                //APD.cmp._ForeignRegBodyIdenNoFCNR = TC.ForeignRegBodyIdenNoFCNR;
                //APD.cmp._IsRegInUs = Convert.ToBoolean(TC.IsRegInUs);
                APD.cmp._NameTrust = TC.NameTrust;
                APD.cmp._ABN_Trust = TC.ABN_Trust;
                APD.cmp._AusTFN_Trust = TC.AusTFN_Trust;
                APD.cmp._LeadEmail = TC.LeadEmail;
                APD.cmp._IsPublicCompany = Convert.ToBoolean(TC.IspublicCompany);

                //TrustCompanyAddress
                var TCA = from tca in db.TrustCompanyAddresses
                          where tca.TrustCompanyID == TrustCompanyID
                          select tca;

                foreach (var ca in TCA)
                {
                    if (ca.AddressType == "Registered")
                    {
                        APD.cmp._RAddressCompanyID = ca.TrustCompanyAddressID;
                        APD.cmp._AddressCompany = ca.Address;
                        APD.cmp._SuburbCompany = ca.Suburb;
                        APD.cmp._StateCompany = ca.State;
                        APD.cmp._PostcodeCompany = ca.Postcode;
                        APD.cmp._CountryCompany = ca.Country;
                        APD.cmp._IsSameCompany = Convert.ToBoolean(ca.IsSame);
                    }
                    else if (ca.AddressType == "Postal")
                    {
                        APD.cmp._PAddressCompanyID = ca.TrustCompanyAddressID;
                        APD.cmp._PAddressCompany = ca.Address;
                        APD.cmp._PPOBoxCompany = ca.PPOBox;
                        APD.cmp._PSuburbCompany = ca.Suburb;
                        APD.cmp._PStateCompany = ca.State;
                        APD.cmp._PPostcodeCompany = ca.Postcode;
                        APD.cmp._PCountryCompany = ca.Country;
                    }
                    //else if (ca.AddressType == "RegisteredOfficeInAustralia")
                    //{
                    //    APD.cmp._AddressAusID = ca.TrustCompanyAddressID;
                    //    APD.cmp._AddressAus = ca.Address;
                    //    APD.cmp._SuburbAus = ca.Suburb;
                    //    APD.cmp._StateAus = ca.State;
                    //    APD.cmp._PostcodeAus = ca.Postcode;
                    //}
                    //else if (ca.AddressType == "LocalAgent")
                    //{
                    //    APD.cmp._AddressLAID = ca.TrustCompanyAddressID;
                    //    APD.cmp._AddressLA = ca.Address;
                    //    APD.cmp._SuburbLA = ca.Suburb;
                    //    APD.cmp._StateLA = ca.State;
                    //    APD.cmp._PostcodeLA = ca.Postcode;
                    //}
                    //else if (ca.AddressType == "FCNR")
                    //{
                    //    APD.cmp._AddressFCNRID = ca.TrustCompanyAddressID;
                    //    APD.cmp._AddressFCNR = ca.Address;
                    //    APD.cmp._SuburbFCNR = ca.Suburb;
                    //    APD.cmp._StateFCNR = ca.State;
                    //    APD.cmp._PostcodeFCNR = ca.Postcode;
                    //    APD.cmp._CountryFCNR = ca.Country;
                    //}
                    else if (ca.AddressType == "PPFCNR")
                    {
                        APD.cmp._AddressPPFCNRID = ca.TrustCompanyAddressID;
                        APD.cmp._AddressPPFCNR = ca.Address;
                        APD.cmp._SuburbPPFCNR = ca.Suburb;
                        APD.cmp._StatePPFCNR = ca.State;
                        APD.cmp._PostcodePPFCNR = ca.Postcode;
                        APD.cmp._CountryPPFCNR = ca.Country;
                    }
                }
                //Trust Company-Share holders
                var SH = from sh in db.Shareholders
                         where sh.TrustCompanyID == TrustCompanyID
                         select sh;
                foreach (var sh in SH)
                {
                    MISPortal.Models.Shareholder shd = new MISPortal.Models.Shareholder();
                    shd._ShareholderID = sh.ShareholderID;
                    shd._Title = sh.Title;
                    shd._FullName = sh.FullName;
                    shd._Surname = sh.Surname;
                    shd._Address = sh.Address;
                    shd._Suburb = sh.Suburb;
                    shd._State = sh.State;
                    shd._Postcode = sh.Postcode;
                    shd._Country = sh.Country;
                    APD.cmp.Shareholders.Add(shd);
                }
            }
            #endregion

            #region Investment Details
            //Investment Details
            var ID = from id in db.InvestmentDetails
                     where id.ApplicantID == ApplicantID
                     select id;
            foreach (var id in ID)
            {
                InvestmentDetails ind = new InvestmentDetails();
                ind._InvestmentDetailsID = id.InvestmentDetailsID;
                ind._FundName = id.FundName;
                ind._InvestmentAmount = Convert.ToDecimal(id.InvestmentAmount);
                ind._RegularSavingPlanPerMonth = Convert.ToDecimal(id.RegularSavingPlanPerMonth);
                ind._Reinvest = Convert.ToBoolean(id.Reinvest);
                ind._SourceofIncome = id.SourceofIncome;
                APD.ID.IDL.Add(ind);
            }
            #endregion

            #region Payment Method
            //Payment Method
            var PM = from pm in db.PaymentMethods
                     where pm.ApplicantID == ApplicantID
                     select pm;
            foreach (var pm in PM)
            {
                MISPortal.Models.PaymentMethod pmd = new MISPortal.Models.PaymentMethod();

                pmd._PaymentMethodID = pm.PaymentMethodID;
                pmd._PaymentType = pm.PaymentType;
                pmd._FinancialInstituteName = pm.FinancialInstituteName;
                pmd._BranchName = pm.BranchName;
                pmd._BSBNumber = pm.BSBNumber;
                pmd._AccountNumber = pm.AccountNumber;
                pmd._AccountName = pm.AccountName;
                APD.ID.PM.Add(pmd);
            }
            #endregion

            #region Authorised Representative Details
            //Authorised Representative Details
            var ARD = db.AuthorisedRepresentativeDetails.SingleOrDefault(p => p.ApplicantID == ApplicantID);
            if (ARD != null)
            {
                APD.ARD._AuthRepDetailID = ARD.AuthRepDetailID;
                APD.ARD._Title = ARD.Title;
                APD.ARD._FullName = ARD.FullName;
                APD.ARD._Surname = ARD.Surname;
                APD.ARD._TelephoneNo = ARD.TelephoneNo;
                APD.ARD._MobileNo = ARD.MobileNo;
                APD.ARD._EmailAddress = ARD.EmailAddress;
                APD.ARD._AFSL_LicenceNo = ARD.AFSL_LicenceNo;
            }
            #endregion

            #region Financial Adviser Details
            //Financial Adviser Details
            var FAD = db.FinancialAdviserDetails.SingleOrDefault(p => p.ApplicantID == ApplicantID);
            if (FAD != null)
            {
                decimal FinancialAdviserDetailID = FAD.FinancialAdviserDetailID;
                APD.FAD._FinancialAdviserDetailID = FAD.FinancialAdviserDetailID;
                APD.FAD._DealerGroupName = FAD.DealerGroupName;
                APD.FAD._AdviserName = FAD.AdviserName;
                APD.FAD._AFSL_Number = FAD.AFSL_Number;
                APD.FAD._ContactNo = FAD.ContactNo;
                APD.FAD._EmailAddress = FAD.EmailAddress;
                APD.FAD._BusinessNo = FAD.BusinessNo;
                APD.FAD._MobileNo = FAD.MobileNo;

                //Financial Adviser Address
                var FAA = from faa in db.FinancialAdviserAddresses
                          where faa.FinancialAdviserDetailID == FinancialAdviserDetailID
                          select faa;
                foreach (var faa in FAA)
                {
                    if (faa.AddressType == "Business")
                    {
                        APD.FAD._BFinancialAdviserAddressID = faa.FinancialAdviserAddressID;
                        APD.FAD._BAddress = faa.Address;
                        APD.FAD._BSuburb = faa.Suburb;
                        APD.FAD._BState = faa.State;
                        APD.FAD._BPostcode = faa.Postcode;
                        APD.FAD._BCountry = faa.Country;
                        APD.FAD._IsSame = Convert.ToBoolean(faa.IsSame);
                    }
                    else if (faa.AddressType == "Postal")
                    {
                        APD.FAD._PFinancialAdviserAddressID = faa.FinancialAdviserAddressID;
                        APD.FAD._PAddress = faa.Address;
                        APD.FAD._PPOBox = faa.POBox;
                        APD.FAD._PSuburb = faa.Suburb;
                        APD.FAD._PState = faa.State;
                        APD.FAD._PPostcode = faa.Postcode;
                        APD.FAD._PCountry = faa.Country;
                        APD.FAD._IsSame = Convert.ToBoolean(faa.IsSame);
                    }
                }
            }
            #endregion

            AAPDist.Add(APD);
            return AAPDist;
        }

        public static bool CreateApplicantPDF(decimal ApplicantID)
        {
            try
            {
                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/ApplicantDetails"), "ApplicantDetails.pdf");
                if (File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                
                FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);

                List<ApplicantPersonalDetails> APD = GetApplicantDetailsList(ApplicantID);

                int count=1;
                
                var htmlContent="<table bgcolor='#4184F3' width='100%' border='0' cellspacing='0' cellpadding='0' style='border:1px solid #e0e0e0;border-bottom:0;border-top-left-radius:3px;border-top-right-radius:3px' align='center'><tbody><tr><td height='72px' colspan='3'></td></tr><tr><td width='10%'></td><td style='font-family:Roboto-Regular,Helvetica,Arial,sans-serif;font-size:24px;color:#ffffff;line-height:1.25' align='center'>Applicant Details (reference "+ApplicantID+")</td><td width='10%'></td></tr><tr><td height='18px' colspan='3'></td></tr></tbody></table>";


                if (APD[0]._AccountType != "TrustCompany")
                {
                    //Individual-Joint-TrustIndividual
                    htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'><tr><td>Account Type</td><td>" + APD[0]._AccountType + "</td></tr>";

                    foreach (var ap in APD[0].PD)
                    {
                        if (APD[0]._AccountType != "Individual")
                        {
                            htmlContent = htmlContent + "<tr><td colspan='2' align='center'><b>'Applicant' # "+count+"</b></td></tr>";
                            count++;
                        }

                        htmlContent = htmlContent + "<tr>"
                        + "<td style='width:50%;'>Verification Result</td>"
                        + "<td style='width:50%;'>" + ap._VerificationResult + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Title</td>"
                        + "<td>" + ap._Title + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Full Name</td>"
                        + "<td>" + ap._FullName + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Middle Name</td>"
                        + "<td>" + ap._MiddleName + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Surname</td>"
                        + "<td>" + ap._Surname + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Birth Date</td>"
                        + "<td>" + ap._BirthDate + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Country of Birth</td>"
                        + "<td>" + ap._CountryOfBirth + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Occupation</td>"
                        + "<td>" + ap._Occupation + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Home Number</td>"
                        + "<td>" + ap._HomeNo + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Business Number</td>"
                        + "<td>" + ap._BusinessNo + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Mobile Number</td>"
                        + "<td>" + ap._MobileNo + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Email Address</td>"
                        + "<td>" + ap._EmailAddress + "</td>"
                        + "</tr>";
                        if (APD[0]._AccountType == "TrustIndividual")
                        {
                            htmlContent = htmlContent + "<tr>"
                            + "<td>Are you a Beneficiary</td>"
                            + "<td>" + ap._IsBeneficiary + "</td>"
                            + "</tr>";
                        }
                        if (APD[0]._AccountType != "TrustIndividual")
                        {
                            htmlContent = htmlContent + "<tr>"
                            + "<td>Tax File Number/Exemption Code</td>"
                            + "<td>" + ap._TaxFileNoExemptionCode + "</td>"
                            + "</tr>"
                            + "<tr>"
                            + "<td>Exemption Reason</td>"
                            + "<td>" + ap._ExemptionReason + "</td>"
                            + "</tr>"
                            + "<tr>"
                            + "<td>Country Tax</td>"
                            + "<td>" + ap._CountryTax + "</td>"
                            + "</tr>"
                            + "<tr>";
                        }
                        htmlContent = htmlContent + "<td>Residentail Address</td>"
                        + "<td>" + ap._RUnit + "  " + ap._RStreetNo + "  " + ap._RStreetName + "  " + ap._RStreetType + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Residential Suburb</td>"
                        + "<td>" + ap._RSuburb + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Residentail State</td>"
                        + "<td>" + ap._RState + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Residentail Postcode</td>"
                        + "<td>" + ap._RPostcode + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Residential Country</td>"
                        + "<td>" + ap._RCountry + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Is Same as residentail</td>"
                        + "<td>" + ap._IsSame + "</td>"
                        + "</tr>";
                        if (ap._IsSame == false)
                        {
                            htmlContent = htmlContent + "<tr>"
                            + "<td>Postal Address</td>"
                            + "<td>" + ap._PUnit + "  " + ap._PStreetNo + "  " + ap._PStreetName + "  " + ap._PStreetType + "</td>"
                            + "</tr>"
                            + "<tr>"
                            + "<td>Postal Suburb</td>"
                            + "<td>" + ap._PSuburb + "</td>"
                            + "</tr>"
                            + "<tr>"
                            + "<td>Postal State</td>"
                            + "<td>" + ap._PState + "</td>"
                            + "</tr>"
                            + "<tr>"
                            + "<td>Postal Postcode</td>"
                            + "<td>" + ap._PPostcode + "</td>"
                            + "</tr>"
                            + "<tr>"
                            + "<td>Postal Country</td>"
                            + "<td>" + ap._PCountry + "</td>"
                            + "</tr>";
                        }
                        if (APD[0]._AccountType != "TrustIndividual")
                        {
                            htmlContent = htmlContent + "<tr>"
                            + "<td>Is Us Citizen</td>"
                            + "<td>" + ap._IsUSCitizen + "</td>"
                            + "</tr>";
                            if (ap._IsUSCitizen == true)
                            {
                                htmlContent = htmlContent + "<tr>"
                                + "<td>Is FATCA Exempt</td>"
                                + "<td>" + ap._IsFATCA + "</td>"
                                + "</tr>";
                            }
                        }
                    }
                    if (APD[0]._AccountType == "TrustIndividual")
                    {
                        htmlContent = htmlContent + "<tr>"
                        + "<td>Trust name or superannutation fund</td>"
                        + "<td>" + APD[0].PD[0]._NameTrust + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>ABN</td>"
                        + "<td>" + APD[0].PD[0]._ABN + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Australian tax file number(TFN)</td>"
                        + "<td>" + APD[0].PD[0]._AusTFN + "</td>"
                        + "</tr>";
                    }
                    htmlContent = htmlContent + "<tr>"
                        + "<td>Lead Email</td>"
                        + "<td>" + APD[0].PD[0]._LeadEmail + "</td>"
                        + "</tr>";
                    if (APD[0]._AccountType == "Joint")
                    {
                        htmlContent = htmlContent + "<tr>"
                        + "<td>Joint Investors</td>"
                        + "<td>" + APD[0].PD[0]._JointInvestors + "</td>"
                        + "</tr>";
                    }
                    htmlContent = htmlContent + "</table>";
                }
                else
                {
                    //Trust Company
                    htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'><tr><td>Account Type</td><td>" + APD[0]._AccountType + "</td></tr>";
                    htmlContent = htmlContent + "<tr><td colspan='2' align='center'><b>Company</b></td></tr>"
                    + "<tr>"
                    + "<td style='width:50%;'>Verification Result</td>"
                    + "<td style='width:50%;'></td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Full Name</td>"
                    + "<td>" + APD[0].cmp._FullNameCompany + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>ACN ABN</td>"
                    + "<td>" + APD[0].cmp._ACN_ABN_Company + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Australian Tax file number</td>"
                    + "<td>" + APD[0].cmp._AusTFN_Company + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Registered office address</td>"
                    + "<td>" + APD[0].cmp._AddressCompany + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Suburb</td>"
                    + "<td>" + APD[0].cmp._SuburbCompany + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>State</td>"
                    + "<td>" + APD[0].cmp._StateCompany + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Postcode</td>"
                    + "<td>" + APD[0].cmp._PostcodeCompany + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Country</td>"
                    + "<td>" + APD[0].cmp._CountryCompany + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Is same as registered?</td>"
                    + "<td>" + APD[0].cmp._IsSameCompany + "</td>"
                    + "</tr>";

                    if (APD[0].cmp._IsSameCompany == false)
                    {
                        htmlContent = htmlContent + "<tr>"
                        + "<td>Postal address</td>"
                        + "<td>" + APD[0].cmp._PAddressCompany + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>PO Box</td>"
                        + "<td>" + APD[0].cmp._PPOBoxCompany + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Postal Suburb</td>"
                        + "<td>" + APD[0].cmp._PSuburbCompany + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Postal State</td>"
                        + "<td>" + APD[0].cmp._PStateCompany + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Postal Postcode</td>"
                        + "<td>" + APD[0].cmp._PPostcodeCompany + "</td>"
                        + "</tr>"
                        + "<tr>"
                        + "<td>Postal Country</td>"
                        + "<td>" + APD[0].cmp._PCountryCompany + "</td>"
                        + "</tr>";
                    }
                    htmlContent = htmlContent + "<tr>"
                    + "    <td colspan='2' align='center'><b>Contact Person at Company</b></td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Title</td>"
                    + "<td>" + APD[0].cmp._TitleCP + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Full Name</td>"
                    + "<td>" + APD[0].cmp._FullNameCP + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Surname</td>"
                    + "<td>" + APD[0].cmp._SurnameCP + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Business Number</td>"
                    + "<td>" + APD[0].cmp._BusinessNoCP + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Mobile Number</td>"
                    + "<td>" + APD[0].cmp._MobileNoCP + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Email Address</td>"
                    + "<td>" + APD[0].cmp._EmailAddrCP + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Occupation</td>"
                    + "<td>" + APD[0].cmp._OccupationCP + "</td>"
                    + "</tr>"
                    //+ "<tr>"
                    //+ "    <td colspan='2' align='center'><b>Foreign company registered by ASIC</b></td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Full Name</td>"
                    //+ "<td>" + APD[0].cmp._FullNameFC + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>ARBN</td>"
                    //+ "<td>" + APD[0].cmp._ARBN_FC + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Is registered by foreign registration body?</td>"
                    //+ "<td>" + APD[0].cmp._IsRegByForeignRegBodyFC + "</td>"
                    //+ "</tr>";

                    //if (APD[0].cmp._IsRegByForeignRegBodyFC == true)
                    //{
                    //    htmlContent = htmlContent + "<tr>"
                    //    + "<td>Foreign registration body name</td>"
                    //    + "<td>" + APD[0].cmp._ForeignRegBodyNameFC + "</td>"
                    //    + "</tr>";
                    //}

                    //htmlContent = htmlContent + "<tr>"
                    //+ "<td>Registered office address in Australia</td>"
                    //+ "<td>" + APD[0].cmp._AddressAus + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Suburb</td>"
                    //+ "<td>" + APD[0].cmp._SuburbAus + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>State</td>"
                    //+ "<td>" + APD[0].cmp._StateAus + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Postcode</td>"
                    //+ "<td>" + APD[0].cmp._PostcodeAus + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "    <td colspan='2' align='center'><b>Australian principal place of business or local agent address</b></td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Status</td>"
                    //+ "<td>" + APD[0].cmp._StatusFC + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Local Agent Name </td>"
                    //+ "<td>" + APD[0].cmp._NameLA + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Address</td>"
                    //+ "<td>" + APD[0].cmp._AddressLA + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Suburb</td>"
                    //+ "<td>" + APD[0].cmp._SuburbLA + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>State</td>"
                    //+ "<td>" + APD[0].cmp._StateLA + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Postcode</td>"
                    //+ "<td>" + APD[0].cmp._PostcodeLA + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "    <td colspan='2' align='center'><b>Foreign company not registered by ASIC</b></td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Full Name</td>"
                    //+ "<td>" + APD[0].cmp._FullNameFCNR + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Is your company registered by foregin registration body?</td>"
                    //+ "<td>" + APD[0].cmp._IsRegByForeignRegBodyFCNR + "</td>"
                    //+ "</tr>";

                    //if (APD[0].cmp._IsRegByForeignRegBodyFCNR == true)
                    //{
                    //    htmlContent = htmlContent + "<tr>"
                    //    + "<td>Foreign registration body name</td>"
                    //    + "<td>" + APD[0].cmp._ForeignRegBodyNameFCNR + "</td>"
                    //    + "</tr>"
                    //    + "<tr>"
                    //    + "<td>Identification number issued by foreign registration body</td>"
                    //    + "<td>" + APD[0].cmp._ForeignRegBodyIdenNoFCNR + "</td>"
                    //    + "</tr>";
                    //}

                    //htmlContent = htmlContent + "<tr>"
                    //+ "<td>Is your entity registered in US?</td>"
                    //+ "<td>" + APD[0].cmp._IsRegInUs + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Registered address as registered by foregin registration body</td>"
                    //+ "<td>" + APD[0].cmp._AddressFCNR + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Suburb</td>"
                    //+ "<td>" + APD[0].cmp._SuburbFCNR + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>State</td>"
                    //+ "<td>" + APD[0].cmp._StateFCNR + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Postcode</td>"
                    //+ "<td>" + APD[0].cmp._PostcodeFCNR + "</td>"
                    //+ "</tr>"
                    //+ "<tr>"
                    //+ "<td>Country</td>"
                    //+ "<td>" + APD[0].cmp._CountryFCNR + "</td>"
                    //+ "</tr>"
                    + "<tr>"
                    + "    <td colspan='2' align='center'><b>Principal place of business address if NOT registered by a foreign registered body</b></td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Registered address as registered by foregin registration body</td>"
                    + "<td>" + APD[0].cmp._AddressPPFCNR + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Suburb</td>"
                    + "<td>" + APD[0].cmp._SuburbPPFCNR + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>State</td>"
                    + "<td>" + APD[0].cmp._StatePPFCNR + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Postcode</td>"
                    + "<td>" + APD[0].cmp._PostcodePPFCNR + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Country</td>"
                    + "<td>" + APD[0].cmp._CountryPPFCNR + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "    <td colspan='2' align='center'><b>Trust</b></td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Name</td>"
                    + "<td>" + APD[0].cmp._NameTrust + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>ABN</td>"
                    + "<td>" + APD[0].cmp._ABN_Trust + "</td>"
                    + "</tr>"
                    + "<tr>"
                    + "<td>Australian Tax file number</td>"
                    + "<td>" + APD[0].cmp._AusTFN_Trust + "</td>"
                    + "</tr>";
                    htmlContent = htmlContent + "</table>";
                    
                    //Shareholders
                    htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'>"
                    + "<thead>"
                    + "<tr>"
                    + "    <td colspan='8' align='center'><b>Share holders</b></td>"
                    + "</tr>"
                    + "<tr>"
                    + "    <th>Title</th>"
                    + "    <th>Full given names</th>"
                    + "    <th>Surname</th>"
                    + "    <th>Address</th>"
                    + "    <th>Suburb</th>"
                    + "    <th>State</th>"
                    + "    <th>Postcode</th>"
                    + "    <th>Country</th>"
                    + "</tr>"
                    + "</thead>"
                    + "<tbody>";
                    foreach (var sh in APD[0].cmp.Shareholders)
                    {
                        htmlContent = htmlContent + "<tr>"
                        + "<td>"+sh._Title+"</td>"
                        + "<td>"+sh._FullName+"</td>"
                        + "<td>"+sh._Surname+"</td>"
                        + "<td>"+sh._Address+"</td>"
                        + "<td>"+sh._Suburb+"</td>"
                        + "<td>"+sh._State+"</td>"
                        + "<td>"+sh._Postcode+"</td>"
                        + "<td>"+sh._Country+"</td>"
                        + "</tr>";
                    }
                    htmlContent = htmlContent + "</tbody>"
                    + "</table>";
                }


                //Investment Details
                htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'>"
                    +"<thead>"
                    +"    <tr>"
                    +"        <td colspan='4' align='center'>"
                    +"        <h3>Investment Details</h3>"
                    +"        </td>"
                    +"    </tr>"
                    +"    <tr>"
                    +"        <th>"
                    +"            Fund Name"
                    +"        </th>"
                    +"        <th>"
                    +"            Investment Amount"
                    +"        </th>"
                    +"        <th>"
                    +"            Regular saving plan per month"
                    +"        </th>"
                    +"        <th>"
                    +"            Reinvest"
                    +"        </th>"
                    +"    </tr>"
                    +"</thead>"
                    +"<tbody>";
                    foreach (var id in APD[0].ID.IDL)
                    {
                      htmlContent = htmlContent + "<tr>"
                           +" <td>"+id._FundName+" "
                           +" </td>"
                           +" <td>"+id._InvestmentAmount+" "
                           +" </td>"
                           +" <td>"+id._RegularSavingPlanPerMonth+" "
                           +" </td>"
                           +" <td>"+id._Reinvest+" " 
                           +" </td>"
                        +"</tr>";
                    }
                  htmlContent = htmlContent + "</tbody>"
                + "<tfoot><tr><td colspan='2'>Source of Income</td><td colspan='2'>" + APD[0].ID.IDL[0]._SourceofIncome + "</td></tr></tfoot>"
                + "</table>";

                //Payment Method
                htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'>"
                    +"<thead>"
                    +"    <tr>";
                              if (APD[0].ID.PM[0]._PaymentType != "Cheque")
                              {
                                    htmlContent = htmlContent + "<td colspan='6' align='center'>"
                                    +" <h3>Payment Method</h3>"
                                    +" </td>";
                              }
                              else
                              {
                                  htmlContent = htmlContent + "<td colspan='2' align='center'>"
                                  +"  <h3>Payment Method</h3>"
                                  +"  </td>";
                                }
                    htmlContent = htmlContent + "</tr>"
                    +"    <tr>"
                    +"        <th>"
                    +"            Payment Type"
                    +"        </th>";
                            if (APD[0].ID.PM[0]._PaymentType != "Cheque")
                            {
                                htmlContent = htmlContent + "<th>"
                                +"    Financial Institute Name"
                                +"</th>"
                                +"<th>"
                                +"    Branch Name"
                                +"</th>"
                                +"<th>"
                                +"    BSB Name"
                                +"</th>"
                                +"<th>"
                                +"    Account Number"
                                +"</th>"
                                +"<th>"
                                +"    Account Name"
                                +"</th>";
                            }
                            else
                            { htmlContent = htmlContent + "<th>Cheque Address:</th>"; }
                    htmlContent = htmlContent + "</tr>"
                    +"</thead>"
                    +"<tbody>";
                    foreach (var pm in APD[0].ID.PM)
                    {
                        htmlContent = htmlContent + "<tr>"
                        +"    <td>"+@pm._PaymentType+" " 
                        +"    </td>";
                            if (APD[0].ID.PM[0]._PaymentType != "Cheque")
                            {
                                htmlContent = htmlContent + "<td>"+@pm._FinancialInstituteName+" " 
                                +"</td>"
                                +"<td>"+pm._BranchName+" " 
                                +"</td>"
                                +"<td>"+pm._BSBNumber+" "
                                +"</td>"
                                +"<td>"+pm._AccountNumber+" "
                                +"</td>"
                                +"<td>"+pm._AccountName+" "
                                +"</td>";
                            }
                            else
                            { 
                                htmlContent = htmlContent + "<td>"
                                +"    <p>"
                                +"    Crescent Wealth Managed Funds<br />"
                                +"    c/o FundBPO Pty Ltd<br />"
                                +"    GPO Box 4968<br />"
                                +"    Sydney NSW 2001<br />"
                                +"    </p>"
                                +"</td>";
                            }
                        htmlContent = htmlContent + "</tr>";
                    }
                    htmlContent = htmlContent + "</tbody>"
                +"</table>";

                //Authorised Representative Details
                htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'>"
                +"<tr>"
                +"    <td colspan='2' align='center'><h3>Authorised Representative Details</h3></td>"
                +"</tr>"
                +"<tr>"
                +"<td style='width:50%;'>Title</td>"
                +"<td style='width:50%;'>" + APD[0].ARD._Title + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Full Name</td>"
                +"<td>"+APD[0].ARD._FullName+"</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Surname</td>"
                + "<td>" + APD[0].ARD._Surname + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Telephone No</td>"
                + "<td>" + APD[0].ARD._TelephoneNo + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Mobile No</td>"
                + "<td>" + APD[0].ARD._MobileNo + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Email Address</td>"
                + "<td>" + APD[0].ARD._EmailAddress + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>AFSL Licence No</td>"
                + "<td>" + APD[0].ARD._AFSL_LicenceNo + "</td>"
                +"</tr>"
                +"</table>";

                //Financial Adviser Details
                htmlContent = htmlContent + "<table style='width:100%;' border='1px solid'>"
                +"<tr>"
                +"    <td colspan='2' align='center'><h3>Financial Adviser Details</h3></td>"
                +"</tr>"
                +"<tr>"
                +"<td style='width:50%;'>Dealer Group Name</td>"
                +"<td style='width:50%;'>"+APD[0].FAD._DealerGroupName+"</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Adviser Name</td>"
                + "<td>" + APD[0].FAD._AdviserName + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>AFSL Number</td>"
                + "<td>" + APD[0].FAD._AFSL_Number + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Contact No</td>"
                + "<td>" + APD[0].FAD._ContactNo + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Email Address</td>"
                + "<td>" + APD[0].FAD._EmailAddress + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Business No</td>"
                + "<td>" + APD[0].FAD._BusinessNo + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Mobile No</td>"
                + "<td>" + APD[0].FAD._MobileNo + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Business Address</td>"
                + "<td>" + APD[0].FAD._BAddress + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Business Suburb</td>"
                + "<td>" + APD[0].FAD._BSuburb + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Business State</td>"
                + "<td>" + APD[0].FAD._BState + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Business Postcode</td>"
                + "<td>" + APD[0].FAD._BPostcode + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Business Country</td>"
                + "<td>" + APD[0].FAD._BCountry + "</td>"
                +"</tr>"
                +"<tr>"
                +"<td>Is Same as Business</td>"
                + "<td>" + APD[0].FAD._IsSame + "</td>"
                +"</tr>";
                if (APD[0].FAD._IsSame == false)
                {
                    htmlContent = htmlContent + "<tr>"
                    +"<td>Postal Address</td>"
                    + "<td>" + APD[0].FAD._PAddress + "</td>"
                    +"</tr>"
                    +"<tr>"
                    +"<td>Postal Suburb</td>"
                    + "<td>" + APD[0].FAD._PSuburb + "</td>"
                    +"</tr>"
                    +"<tr>"
                    +"<td>Postal State</td>"
                    + "<td>" + APD[0].FAD._PState + "</td>"
                    +"</tr>"
                    +"<tr>"
                    +"<td>Postal Postcode</td>"
                    + "<td>" + APD[0].FAD._PPostcode + "</td>"
                    +"</tr>"
                    +"<tr>"
                    +"<td>Postal Country</td>"
                    + "<td>" + APD[0].FAD._PCountry + "</td>"
                    +"</tr>";
                }
                htmlContent = htmlContent + "</table>";


                var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
                var pdfBytes = htmlToPdf.GeneratePdf(htmlContent);

                fs.Write(pdfBytes, 0, pdfBytes.Length);
                fs.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }


        //greenid 
        public static bool GreenidVerificationId(decimal ApplicantID, string VerificationId, decimal GreenidVerificationID)
        {
            try
            {
                MISPortal_DBDataContext db = new MISPortal_DBDataContext();
                int AppNo = int.Parse(HttpContext.Current.Session["ApplicantNo"].ToString());
                var A = db.ApplicantNoVerificationIds.SingleOrDefault(a => a.ApplicantID == ApplicantID && a.ApplicantNo == AppNo);
                A.VerificationId = VerificationId;
                db.SubmitChanges();

                var G = db.GreenidVerifications.SingleOrDefault(g => g.GreenidVerificationID == GreenidVerificationID);
                G.GreenidVerificationID = GreenidVerificationID;
                G.VerificationId = VerificationId;
                db.SubmitChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool GreenidVerificationStatus(decimal ApplicantID, string VerificationStatus)
        {
            try
            {
                MISPortal_DBDataContext db = new MISPortal_DBDataContext();
                int AppNo = int.Parse(HttpContext.Current.Session["ApplicantNo"].ToString());
                var A = db.ApplicantNoVerificationIds.SingleOrDefault(a => a.ApplicantID == ApplicantID && a.ApplicantNo == AppNo);
                A.VerificationResult = VerificationStatus;
                string verificationId = A.VerificationId;
                db.SubmitChanges();

                var G = db.GreenidVerifications.SingleOrDefault(g => g.VerificationId == verificationId);
                G.VerificationResult = VerificationStatus;
                db.SubmitChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<ApplicantVerificationStatus> CheckGreenidVerificationStatus(decimal ApplicantID)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            List<ApplicantVerificationStatus> vslist = new List<ApplicantVerificationStatus>();
            var G = db.ApplicantNoVerificationIds.Where(g => g.ApplicantID == ApplicantID).OrderBy(x => x.ApplicantNo);

            string Status = "";

            foreach (var g in G)
            {
                ApplicantVerificationStatus vs = new ApplicantVerificationStatus();
                //if (g.VerificationResult == "VERIFIED" || g.VerificationResult == "VERIFIED_ADMIN" || g.VerificationResult == "VERIFIED_WITH_CHANGES")
                if (g.VerificationResult == "" || g.VerificationResult == null)
                {
                    vs._ApplicantStatus = "IN_PROGRESS";
                    if (Status != "IN_PROGRESS")
                    {
                        Status = "IN_PROGRESS";
                    }

                    vs._overallStatus = Status;
                    var name = db.PersonalDetails.SingleOrDefault(a => a.ApplicantID == g.ApplicantID && a.ApplicantNo == g.ApplicantNo);
                    vs._ApplicantName = name.FullName + " " + name.MiddleName + " " + name.Surname;
                    vslist.Add(vs);
                }
                else
                {
                    vs._ApplicantStatus = g.VerificationResult;
                    if (Status != "IN_PROGRESS")
                    {
                        Status = g.VerificationResult;
                    }

                    vs._overallStatus = Status; 
                    var name = db.PersonalDetails.SingleOrDefault(a => a.ApplicantID == g.ApplicantID && a.ApplicantNo == g.ApplicantNo);
                    vs._ApplicantName = name.FullName + " " + name.MiddleName + " " + name.Surname;
                    vslist.Add(vs);
                }
            }

            var A = db.Applicants.SingleOrDefault(a => a.ApplicantID == ApplicantID);
            A.VerificationResult = Status;
            db.SubmitChanges();


            return vslist;
        }

        public static greenidVerification GreenidRegisterVerification(ApplicantPersonalDetails APD)
        {
            greenidVerification ret = new greenidVerification();
            decimal GreenidVerificationID = 0;
            string VerificationResult = "";
            string verificationId = "";

            try
            {
                MISPortal_DBDataContext db = new MISPortal_DBDataContext();

                var G = (from g in db.GreenidVerifications
                         select g).OrderByDescending(x => x.GreenidVerificationID);

                bool issameperson = false;
                int count = 11;
                foreach (var item in G)
                {
                    count = 11;
                    if (item.GivenName.ToUpper() == APD._FullName.ToUpper()) { count--; }
                    if (item.MiddleName == "" || item.MiddleName == null || APD._MiddleName == "" || APD._MiddleName == null)
                    {
                        count--; 
                    }
                    else { if (item.MiddleName.ToUpper() == APD._MiddleName.ToUpper()) { count--; } }
                    if (item.Surname.ToUpper() == APD._Surname.ToUpper()) { count--; }
                    //if (item.Email.ToUpper() == APD._EmailAddress.ToUpper()) { count--; }
                    if (item.Dateofbirth.ToUpper() == APD._BirthDate.ToUpper()) { count--; }
                    if (item.FlatNumber == "" || item.FlatNumber == null || APD._RUnit == "" || APD._RUnit == null || APD._RUnit == "0")
                    {
                        count--; 
                    }
                    else { if (item.FlatNumber.ToUpper() == APD._RUnit.ToUpper()) { count--; } }
                    if (item.StreetNumber.ToUpper() == APD._RStreetNo.ToUpper()) { count--; }
                    if (item.StreetName.ToUpper() == APD._RStreetName.ToUpper()) { count--; }
                    if (item.StreetType == "" || item.StreetType == null || APD._RStreetType == "" || APD._RStreetType == null)
                    {
                        count--; 
                    }
                    else { if (item.StreetType.ToUpper() == APD._RStreetType.ToUpper()) { count--; } }
                    if (item.Suburb.ToUpper() == APD._RSuburb.ToUpper()) { count--; }
                    if (item.State.ToUpper() == APD._RState.ToUpper()) { count--; }
                    if (item.Postcode.ToUpper() == APD._RPostcode.ToUpper()) { count--; }

                    if (count == 0) { issameperson = true; GreenidVerificationID = item.GreenidVerificationID; VerificationResult = item.VerificationResult; verificationId = item.VerificationId; break; }
                }

                if (issameperson == false)
                {
                    MISPortal.GreenidVerification gid = new MISPortal.GreenidVerification();
                    gid.GivenName = APD._FullName;
                    gid.MiddleName = APD._MiddleName;
                    gid.Surname = APD._Surname;
                    gid.Email = APD._EmailAddress;
                    gid.Dateofbirth = APD._BirthDate;
                    gid.FlatNumber = APD._RUnit;
                    gid.StreetNumber = APD._RStreetNo;
                    gid.StreetName = APD._RStreetName;
                    gid.StreetType = APD._RStreetType;
                    gid.Suburb = APD._RSuburb;
                    gid.State = APD._RState;
                    gid.Postcode = APD._RPostcode;
                    gid.Country = APD._RCountry;
                    db.GreenidVerifications.InsertOnSubmit(gid);
                    db.SubmitChanges();

                    GreenidVerificationID = db.GreenidVerifications.Max(g => g.GreenidVerificationID);
                    
                }

                ret._GreenidVerificationID = GreenidVerificationID;
                ret._isRegistrationExists = issameperson;
                ret._VerificationId = verificationId;
                ret._VerificationResult = VerificationResult;

                return ret;
            }
            catch
            {
                return ret;
            }
        }


        //edit-Submit&complete-step5

        public static bool EditIndividualJointTrustIndividual(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                foreach (var app in APDList[0].PD)
                {
                    var APDMaster1 = db.PersonalDetails.SingleOrDefault(p => p.PersonalDetailID == app._PersonalDetailID);
                    APDMaster1.PersonalDetailID = app._PersonalDetailID;
                    APDMaster1.Title = app._Title;
                    APDMaster1.FullName = app._FullName;
                    APDMaster1.Surname = app._Surname;
                    APDMaster1.BirthDate = DateTime.ParseExact(app._BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    APDMaster1.BirthCountry = app._CountryOfBirth;
                    APDMaster1.Occupation = app._Occupation;
                    APDMaster1.IsBeneficiary = Convert.ToBoolean(app._IsBeneficiary);
                    
                    APDMaster1.HomeNo = app._HomeNo;
                    APDMaster1.BusinessNo = app._BusinessNo;
                    APDMaster1.MobileNo = app._MobileNo;
                    APDMaster1.AreaCodeH = app._AreaCodeH;
                    APDMaster1.AreaCodeB = app._AreaCodeB;
                    APDMaster1.EmailAddress = app._EmailAddress;
                    if (app._CountryTax != null)
                    {
                        APDMaster1.IsAusResident = false;
                        APDMaster1.CountryTax = app._CountryTax;
                    }
                    else
                    {
                        APDMaster1.IsAusResident = true;
                        APDMaster1.TaxFileNoExemptionCode = app._TaxFileNoExemptionCode;
                        APDMaster1.ExemptionReason = app._ExemptionReason;
                    }
                    APDMaster1.IsUSCitizen = app._IsUSCitizen;
                    if (app._IsUSCitizen == true)
                    {
                        APDMaster1.IsFATCA = app._IsFATCA;
                    }
                    APDMaster1.NameTrust = app._NameTrust;
                    APDMaster1.ABN = app._ABN;
                    APDMaster1.AusTFN = app._AusTFN;
                    db.SubmitChanges();

                    var APDAddr = db.Addresses.Where(a => a.PersonalDetailID == app._PersonalDetailID);

                    foreach (var APDMaster21 in APDAddr)
                    {
                        if (APDMaster21.AddressType == "Residential")
                        {
                            APDMaster21.AddressID = app._RAddressID;
                            APDMaster21.AddressType = "Residential";
                            APDMaster21.Address1 = app._RAddress;
                            APDMaster21.Suburb = app._RSuburb;
                            APDMaster21.State = app._RState;
                            APDMaster21.Postcode = app._RPostcode;
                            APDMaster21.Country = app._RCountry;
                            APDMaster21.IsSame = app._IsSame;
                            db.SubmitChanges();
                        }
                        if (APDMaster21.AddressType == "Postal")
                        {
                            APDMaster21.AddressID = app._PAddressID; ;
                            APDMaster21.AddressType = "Postal";
                            APDMaster21.Address1 = app._PAddress;
                            APDMaster21.Suburb = app._PSuburb;
                            APDMaster21.State = app._PState;
                            APDMaster21.Postcode = app._PPostcode;
                            APDMaster21.Country = app._PCountry;
                            APDMaster21.IsSame = app._IsSame;
                            db.SubmitChanges();
                        }
                    }
                }
                EditCommonDetials(APDList);

                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool EditTrustCompany(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                var APDMaster2 = db.TrustCompanies.SingleOrDefault(t => t.ApplicantID == APDList[0]._ApplicantID);

                APDMaster2.TrustCompanyID = APDList[0].cmp._TrustCompanyID;
                APDMaster2.FullName = APDList[0].cmp._FullNameCompany;
                APDMaster2.ACN_ABN = APDList[0].cmp._ACN_ABN_Company;
                APDMaster2.AusTFN = APDList[0].cmp._AusTFN_Company;
                APDMaster2.TitleCP = APDList[0].cmp._TitleCP;
                APDMaster2.FullNameCP = APDList[0].cmp._FullNameCP;
                APDMaster2.SurnameCP = APDList[0].cmp._SurnameCP;
                APDMaster2.BusinessNoCP = APDList[0].cmp._BusinessNoCP;
                APDMaster2.MobileNoCP = APDList[0].cmp._MobileNoCP;
                APDMaster2.AreaCodeB = APDList[0].cmp._AreaCodeB_CP;
                APDMaster2.EmailAddressCP = APDList[0].cmp._EmailAddrCP;
                APDMaster2.OccupationCP = APDList[0].cmp._OccupationCP;
                //APDMaster2.FullNameFC = APDList[0].cmp._FullNameFC;
                //APDMaster2.ARBN_FC = APDList[0].cmp._ARBN_FC;
                //APDMaster2.IsRegByForeignRegBodyFC = APDList[0].cmp._IsRegByForeignRegBodyFC;
                //APDMaster2.ForeignRegBodyNameFC = APDList[0].cmp._ForeignRegBodyNameFC;
                //APDMaster2.StatusFC = APDList[0].cmp._StatusFC;
                //APDMaster2.NameLocaLAgent = APDList[0].cmp._NameLA;
                //APDMaster2.FullNameFCNR = APDList[0].cmp._FullNameFCNR;
                //APDMaster2.IsRegByForeignRegBodyFCNR = APDList[0].cmp._IsRegByForeignRegBodyFCNR;
                //APDMaster2.ForeignRegBodyNameFCNR = APDList[0].cmp._ForeignRegBodyNameFCNR;
                //APDMaster2.ForeignRegBodyIdenNoFCNR = APDList[0].cmp._ForeignRegBodyIdenNoFCNR;
                //APDMaster2.IsRegInUs = APDList[0].cmp._IsRegInUs;
                APDMaster2.NameTrust = APDList[0].cmp._NameTrust;
                APDMaster2.ABN_Trust = APDList[0].cmp._ABN_Trust;
                APDMaster2.AusTFN_Trust = APDList[0].cmp._AusTFN_Trust;
                APDMaster2.IspublicCompany = APDList[0].cmp._IsPublicCompany;
                db.SubmitChanges();


                var TCA = db.TrustCompanyAddresses.Where(tca => tca.TrustCompanyID == APDList[0].cmp._TrustCompanyID);
                foreach (var APDMaster3 in TCA)
                {
                    if (APDMaster3.AddressType == "Registered")
                    {
                        APDMaster3.TrustCompanyAddressID = APDList[0].cmp._RAddressCompanyID;
                        APDMaster3.AddressType = "Registered";
                        APDMaster3.Address = APDList[0].cmp._AddressCompany;
                        APDMaster3.Suburb = APDList[0].cmp._SuburbCompany;
                        APDMaster3.State = APDList[0].cmp._StateCompany;
                        APDMaster3.Postcode = APDList[0].cmp._PostcodeCompany;
                        APDMaster3.Country = APDList[0].cmp._CountryCompany;
                        APDMaster3.IsSame = APDList[0].cmp._IsSameCompany;
                        db.SubmitChanges();
                    }
                    if (APDMaster3.AddressType == "Postal")
                    {
                        APDMaster3.TrustCompanyAddressID = APDList[0].cmp._PAddressCompanyID;
                        APDMaster3.AddressType = "Postal";
                        APDMaster3.Address = APDList[0].cmp._PAddressCompany;
                        APDMaster3.PPOBox = APDList[0].cmp._PPOBoxCompany;
                        APDMaster3.Suburb = APDList[0].cmp._PSuburbCompany;
                        APDMaster3.State = APDList[0].cmp._PStateCompany;
                        APDMaster3.Postcode = APDList[0].cmp._PPostcodeCompany;
                        APDMaster3.Country = APDList[0].cmp._PCountryCompany;
                        APDMaster3.IsSame = APDList[0].cmp._IsSameCompany;
                        db.SubmitChanges();
                    }
                    //if (APDMaster3.AddressType == "RegisteredOfficeInAustralia")
                    //{
                    //    APDMaster3.TrustCompanyAddressID = APDList[0].cmp._AddressAusID;
                    //    APDMaster3.AddressType = "RegisteredOfficeInAustralia";
                    //    APDMaster3.Address = APDList[0].cmp._AddressAus;
                    //    APDMaster3.Suburb = APDList[0].cmp._SuburbAus;
                    //    APDMaster3.State = APDList[0].cmp._StateAus;
                    //    APDMaster3.Postcode = APDList[0].cmp._PostcodeAus;
                    //    db.SubmitChanges();
                    //}
                    //if (APDMaster3.AddressType == "LocalAgent")
                    //{
                    //    APDMaster3.TrustCompanyAddressID = APDList[0].cmp._AddressLAID;
                    //    APDMaster3.AddressType = "LocalAgent";
                    //    APDMaster3.Address = APDList[0].cmp._AddressLA;
                    //    APDMaster3.Suburb = APDList[0].cmp._SuburbLA;
                    //    APDMaster3.State = APDList[0].cmp._StateLA;
                    //    APDMaster3.Postcode = APDList[0].cmp._PostcodeLA;
                    //    db.SubmitChanges();
                    //}
                    //if (APDMaster3.AddressType == "FCNR")
                    //{
                    //    APDMaster3.TrustCompanyAddressID = APDList[0].cmp._AddressFCNRID;
                    //    APDMaster3.AddressType = "FCNR";
                    //    APDMaster3.Address = APDList[0].cmp._AddressFCNR;
                    //    APDMaster3.Suburb = APDList[0].cmp._SuburbFCNR;
                    //    APDMaster3.State = APDList[0].cmp._StateFCNR;
                    //    APDMaster3.Postcode = APDList[0].cmp._PostcodeFCNR;
                    //    APDMaster3.Country = APDList[0].cmp._CountryFCNR;
                    //    db.SubmitChanges();
                    //}
                    if (APDMaster3.AddressType == "PPFCNR")
                    {
                        APDMaster3.TrustCompanyAddressID = APDList[0].cmp._AddressPPFCNRID;
                        APDMaster3.AddressType = "PPFCNR";
                        APDMaster3.Address = APDList[0].cmp._AddressPPFCNR;
                        APDMaster3.Suburb = APDList[0].cmp._SuburbPPFCNR;
                        APDMaster3.State = APDList[0].cmp._StatePPFCNR;
                        APDMaster3.Postcode = APDList[0].cmp._PostcodePPFCNR;
                        APDMaster3.Country = APDList[0].cmp._CountryPPFCNR;
                        db.SubmitChanges();
                    }
                }

                if (APDList[0].cmp._IsPublicCompany == false)
                {
                    foreach (var sh in APDList[0].cmp.Shareholders)
                    {
                        if (sh._FullName != null)
                        {
                            var APDMaster4 = db.Shareholders.SingleOrDefault(s => s.ShareholderID == sh._ShareholderID);
                            APDMaster4.ShareholderID = sh._ShareholderID;
                            APDMaster4.Title = sh._Title;
                            APDMaster4.FullName = sh._FullName;
                            APDMaster4.Surname = sh._Surname;
                            APDMaster4.Address = sh._Address;
                            APDMaster4.Suburb = sh._Suburb;
                            APDMaster4.State = sh._State;
                            APDMaster4.Postcode = sh._Postcode;
                            APDMaster4.Country = sh._Country;
                            db.SubmitChanges();
                        }
                    }
                }

                EditCommonDetials(APDList);

                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }

        public static bool EditCommonDetials(List<MISPortal.Models.ApplicantPersonalDetails> APDList)
        {
            MISPortal_DBDataContext db = new MISPortal_DBDataContext();
            System.Data.Common.DbTransaction transaction;
            db.Connection.Open();
            transaction = db.Connection.BeginTransaction();
            db.Transaction = transaction;

            try
            {
                foreach (var i in APDList[0].ID.IDL)
                {
                    if (i._InvestmentAmount != 0)
                    {
                        var IDMaster1 = db.InvestmentDetails.SingleOrDefault(id => id.InvestmentDetailsID == i._InvestmentDetailsID);
                        IDMaster1.InvestmentDetailsID = i._InvestmentDetailsID;
                        IDMaster1.FundName = i._FundName;
                        IDMaster1.InvestmentAmount = i._InvestmentAmount;
                        IDMaster1.RegularSavingPlanPerMonth = i._RegularSavingPlanPerMonth;
                        IDMaster1.Reinvest = i._Reinvest;
                        db.SubmitChanges();
                    }
                }
                foreach (var pm in APDList[0].ID.PM)
                {
                    if (pm._PaymentType != "Cheque")
                    {
                        var PMMaster1 = db.PaymentMethods.SingleOrDefault(p => p.PaymentMethodID == pm._PaymentMethodID);
                        PMMaster1.PaymentMethodID = pm._PaymentMethodID;
                        PMMaster1.PaymentType = pm._PaymentType;
                        PMMaster1.FinancialInstituteName = pm._FinancialInstituteName;
                        PMMaster1.BranchName = pm._BranchName;
                        PMMaster1.BSBNumber = pm._BSBNumber;
                        PMMaster1.AccountNumber = pm._BSBNumber;
                        PMMaster1.AccountName = pm._AccountName;
                        db.SubmitChanges();
                    }
                }
                if (APDList[0].ARD._AuthRepDetailID != 0)
                {
                    var ARDMaster1 = db.AuthorisedRepresentativeDetails.SingleOrDefault(a => a.AuthRepDetailID == APDList[0].ARD._AuthRepDetailID);
                    ARDMaster1.AuthRepDetailID = APDList[0].ARD._AuthRepDetailID;
                    ARDMaster1.Title = APDList[0].ARD._Title;
                    ARDMaster1.FullName = APDList[0].ARD._FullName;
                    ARDMaster1.Surname = APDList[0].ARD._Surname;
                    ARDMaster1.TelephoneNo = APDList[0].ARD._TelephoneNo;
                    ARDMaster1.MobileNo = APDList[0].ARD._MobileNo;
                    ARDMaster1.EmailAddress = APDList[0].ARD._EmailAddress;
                    ARDMaster1.AFSL_LicenceNo = APDList[0].ARD._AFSL_LicenceNo;
                    db.SubmitChanges();
                }

                if (APDList[0].FAD._FinancialAdviserDetailID != 0)
                {
                    var FADMaster1 = db.FinancialAdviserDetails.SingleOrDefault(f => f.FinancialAdviserDetailID == APDList[0].FAD._FinancialAdviserDetailID);
                    FADMaster1.FinancialAdviserDetailID = APDList[0].FAD._FinancialAdviserDetailID;
                    FADMaster1.DealerGroupName = APDList[0].FAD._DealerGroupName;
                    FADMaster1.AdviserName = APDList[0].FAD._AdviserName;
                    FADMaster1.AFSL_Number = APDList[0].FAD._AFSL_Number;
                    FADMaster1.ContactNo = APDList[0].FAD._ContactNo;
                    FADMaster1.EmailAddress = APDList[0].FAD._EmailAddress;
                    FADMaster1.BusinessNo = APDList[0].FAD._BusinessNo;
                    FADMaster1.MobileNo = APDList[0].FAD._MobileNo;
                    db.SubmitChanges();


                    var FAA = db.FinancialAdviserAddresses.Where(fa => fa.FinancialAdviserDetailID == APDList[0].FAD._FinancialAdviserDetailID);
                    foreach (var FAAMaster1 in FAA)
                    {
                        if (FAAMaster1.AddressType == "Business")
                        {
                            FAAMaster1.FinancialAdviserAddressID = APDList[0].FAD._BFinancialAdviserAddressID;
                            FAAMaster1.FinancialAdviserDetailID = APDList[0].FAD._FinancialAdviserDetailID;
                            FAAMaster1.AddressType = "Business";
                            FAAMaster1.Address = APDList[0].FAD._BAddress;
                            FAAMaster1.Suburb = APDList[0].FAD._BSuburb;
                            FAAMaster1.State = APDList[0].FAD._BState;
                            FAAMaster1.Postcode = APDList[0].FAD._BPostcode;
                            FAAMaster1.Country = APDList[0].FAD._BCountry;
                            FAAMaster1.IsSame = APDList[0].FAD._IsSame;
                            db.SubmitChanges();
                        }
                        if (FAAMaster1.AddressType == "Postal")
                        {
                            FAAMaster1.FinancialAdviserAddressID = APDList[0].FAD._PFinancialAdviserAddressID;
                            FAAMaster1.FinancialAdviserDetailID = APDList[0].FAD._FinancialAdviserDetailID;
                            FAAMaster1.AddressType = "Postal";
                            FAAMaster1.Address = APDList[0].FAD._PAddress;
                            FAAMaster1.POBox = APDList[0].FAD._PPOBox;
                            FAAMaster1.Suburb = APDList[0].FAD._PSuburb;
                            FAAMaster1.State = APDList[0].FAD._PState;
                            FAAMaster1.Postcode = APDList[0].FAD._PPostcode;
                            FAAMaster1.Country = APDList[0].FAD._PCountry;
                            db.SubmitChanges();
                        }
                    }
                }


                transaction.Commit();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return true;
            }
            catch
            {
                transaction.Rollback();
                if (db.Connection != null)
                {
                    db.Connection.Close();
                }
                return false;
            }
        }


    }
} 