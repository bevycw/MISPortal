using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MISPortal.Models
{

    public class ApplicantPersonalDetails
    {

        //Applicant
        public decimal _ApplicantID { get; set; }
        public DateTime _CreationDateTime { get; set; }
        public string _AccountType { get; set; }
        public string _Status { get; set; }
        public decimal _Percent { get; set; }
        public string _Adviser { get; set; }
        public string _VerificationResult { get; set; }

        //PersonalDetail
        public decimal _PersonalDetailID { get; set; }
        public string _Title { get; set; }
        public string _FullName { get; set; }
        public string _MiddleName { get; set; }
        public string _Surname { get; set; }
        public string _BirthDate { get; set; }
        public string _CountryOfBirth { get; set; }
        public string _Occupation { get; set; }
        public string _HomeNo { get; set; }
        public string _BusinessNo { get; set; }
        public string _MobileNo { get; set; }
        public string _EmailAddress { get; set; }
        public string _LeadEmail { get; set; }

        //PersonalDetail-AreaCode
        public string _AreaCodeH { get; set; }
        public string _AreaCodeB { get; set; }

        //PersonalDetail-TaxDetail
        public string _TaxFileNoExemptionCode { get; set; }
        public string _ExemptionReason { get; set; }
        public string _CountryTax { get; set; }
        public Boolean _IsAusResident { get; set; }

        public Boolean _IsUSCitizen { get; set; }
        public Boolean _IsFATCA { get; set; }
        public Boolean _IsBeneficiary { get; set; }
        public string _JointInvestors { get; set; }

        //PersonalDetail-Trust
        public string _NameTrust { get; set; }
        public string _ABN { get; set; }
        public string _AusTFN { get; set; }


        //Address
        public decimal _AddressID { get; set; }
        public Boolean _IsSame { get; set; }

        //Address-Residentail 
        public decimal _RAddressID { get; set; }
        public string _RAddress { get; set; }
        public string _RUnit { get; set; }
        public string _RStreetNo { get; set; }
        public string _RStreetName { get; set; }
        public string _RStreetType { get; set; }
        public string _RSuburb { get; set; }
        public string _RState { get; set; }
        public string _RPostcode { get; set; }
        public string _RCountry { get; set; }

        //Address-Postal
        public decimal _PAddressID { get; set; }
        public string _PAddress { get; set; }
        public string _PUnit { get; set; }
        public string _PStreetNo { get; set; }
        public string _PStreetName { get; set; }
        public string _PStreetType { get; set; }
        public string _PSuburb { get; set; }
        public string _PState { get; set; }
        public string _PPostcode { get; set; }
        public string _PCountry { get; set; }


        public int _AppCount { get; set; }

        public List<ApplicantPersonalDetails> PD { get; set; }
        public Company cmp { get; set; }
        public InvestmentDetails ID { get; set; }
        public Step4 S4 { get;set; }
        public AuthorisedRepresentativeDetails ARD { get; set; }
        public FinancialAdviserDetails FAD { get; set; }

        public ApplicantPersonalDetails()
        {
            PD = new List<ApplicantPersonalDetails>();
            cmp = new Company();
            ID = new InvestmentDetails();
            ARD = new AuthorisedRepresentativeDetails();
            FAD = new FinancialAdviserDetails();
        }
    }

    public class Company
    {
        //Applicant
        public decimal _ApplicantID { get; set; }
        
        //TrustCompany
        public decimal _TrustCompanyID { get; set; }

        //TrustCompanyAddress
        public string _TrustCompanyAddressID { get; set; }
        public string _AddressType { get; set; }


        //TrustCompany-Australian Company Details
        public string _FullNameCompany { get; set; }
        public string _ACN_ABN_Company { get; set; }
        public string _AusTFN_Company { get; set; }

        //TrustCompanyAddress-Registered office address
        public decimal _RAddressCompanyID { get; set; }
        public string _AddressCompany { get; set; }
        public string _SuburbCompany { get; set; }
        public string _StateCompany { get; set; }
        public string _PostcodeCompany { get; set; }
        public string _CountryCompany { get; set; }
        public Boolean _IsSameCompany { get; set; }

        //TrustCompanyAddress-Postal
        public decimal _PAddressCompanyID { get; set; }
        public string _PAddressCompany { get; set; }
        public string _PPOBoxCompany { get; set; }
        public string _PSuburbCompany { get; set; }
        public string _PStateCompany { get; set; }
        public string _PPostcodeCompany { get; set; }
        public string _PCountryCompany { get; set; }

        //TrustCompany-Contact person at company
        public string _TitleCP { get; set; }
        public string _FullNameCP { get; set; }
        public string _SurnameCP { get; set; }
        public string _AreaCodeB_CP { get; set; }
        public string _BusinessNoCP { get; set; }
        public string _MobileNoCP { get; set; }
        public string _EmailAddrCP { get; set; }
        public string _OccupationCP { get; set; }

        //TrustCompany-Foreign company registered by ASIC 
        //public string _FullNameFC { get; set; }
        //public string _ARBN_FC { get; set; }
        //public Boolean _IsRegByForeignRegBodyFC { get; set; }
        //public string _ForeignRegBodyNameFC { get; set; }

        //TrustCompanyAddress-Registered office address in Australia
        //public decimal _AddressAusID { get; set; }
        //public string _AddressAus { get; set; }
        //public string _SuburbAus { get; set; }
        //public string _StateAus { get; set; }
        //public string _PostcodeAus { get; set; }


        //public string _StatusFC { get; set; }

        //TrustCompanyAddress-Local agent
        //public string _NameLA { get; set; }
        //public decimal _AddressLAID { get; set; }
        //public string _AddressLA { get; set; }
        //public string _SuburbLA { get; set; }
        //public string _StateLA { get; set; }
        //public string _PostcodeLA { get; set; }

        //TrustCompany-Foreign company not registered by ASIC
        //public string _FullNameFCNR { get; set; }
        //public Boolean _IsRegByForeignRegBodyFCNR { get; set; }
        //public string _ForeignRegBodyNameFCNR { get; set; }
        //public string _ForeignRegBodyIdenNoFCNR { get; set; }
        //public Boolean _IsRegInUs { get; set; }

        //TrustCompanyAddress-Registered address as registered by the foreign registration body
        //public decimal _AddressFCNRID { get; set; }
        //public string _AddressFCNR { get; set; }
        //public string _SuburbFCNR { get; set; }
        //public string _StateFCNR { get; set; }
        //public string _PostcodeFCNR { get; set; }
        //public string _CountryFCNR { get; set; }

        //TrustCompanyAddress-Principal place of business address it NOT reg by foreign req body(must not be PO BOx)
        public decimal _AddressPPFCNRID { get; set; }
        public string _AddressPPFCNR { get; set; }
        public string _SuburbPPFCNR { get; set; }
        public string _StatePPFCNR { get; set; }
        public string _PostcodePPFCNR { get; set; }
        public string _CountryPPFCNR { get; set; }


        //Shareholder
        public bool _IsPublicCompany { get; set; }
        public List<Shareholder> Shareholders { get; set; }

        //TrustCompany-Trust
        public string _NameTrust { get; set; }
        public string _ABN_Trust { get; set; }
        public string _AusTFN_Trust { get; set; }

        public string _LeadEmail { get; set; }
        
        public Company()
        {
            Shareholders = new List<Shareholder>();
        }
    }

    public class Shareholder
    {
        public decimal _ShareholderID { get; set; }
        public string _Title { get; set; }
        public string _FullName { get; set; }
        public string _Surname { get; set; }
        public string _Address { get; set; }
        public string _Suburb { get; set; }
        public string _State { get; set; }
        public string _Postcode { get; set; }
        public string _Country { get; set; }
    }

    public class InvestmentDetails
    {
        public decimal _InvestmentDetailsID { get; set; }
        public decimal _ApplicantID { get; set; }
        public string _FundName { get; set; }
        public decimal _InvestmentAmount { get; set; }
        public decimal _RegularSavingPlanPerMonth { get; set; }
        public Boolean _Reinvest { get; set; }
        public string _SourceofIncome { get; set; }

        public List<InvestmentDetails> IDL { get; set; }
        public List<PaymentMethod> PM { get; set; }

        public InvestmentDetails()
        {
            IDL = new List<InvestmentDetails>();
            PM = new List<PaymentMethod>();
        }
    }
    
    public class PaymentMethod
    {
        public decimal _PaymentMethodID { get; set; }
        public decimal _ApplicantID { get; set; }
        public bool _PaymentMethod { get; set; }
        public string _PaymentType { get; set; }
        public string _FinancialInstituteName { get; set; }
        public string _BranchName { get; set; }
        public string _BSBNumber { get; set; }
        public string _AccountNumber { get; set; }
        public string _AccountName { get; set; }
    }

    public class Step4 
    {
        public decimal ApplicantID { get; set; }
        public AuthorisedRepresentativeDetails ARD { get; set; }
        public FinancialAdviserDetails FAD { get; set; }
    }


    public class AuthorisedRepresentativeDetails
    {
        public decimal _AuthRepDetailID { get; set; }
        public string _Title { get; set; }
        public string _FullName { get; set; }
        public string _Surname { get; set; }
        public string _TelephoneNo { get; set; }
        public string _MobileNo { get; set; }
        public string _EmailAddress { get; set; }
        public string _AFSL_LicenceNo { get; set; }

        public Boolean _IsAuthRep { get; set; }
    }

    public class FinancialAdviserDetails
    {
        public decimal _FinancialAdviserDetailID { get; set; }
        public string _DealerGroupName { get; set; }
        public string _AdviserName { get; set; }
        public string _AFSL_Number { get; set; }
        public string _ContactNo { get; set; }
        public string _EmailAddress { get; set; }

        public decimal _FinancialAdviserAddressID { get; set; }
        //Business Address
        public decimal _BFinancialAdviserAddressID { get; set; }
        public string _BAddress { get; set; }
        public string _BSuburb { get; set; }
        public string _BState { get; set; }
        public string _BPostcode { get; set; }
        public string _BCountry { get; set; }

        public Boolean _IsSame { get; set; }

        //Postal Address
        public decimal _PFinancialAdviserAddressID { get; set; }
        public string _PAddress { get; set; }
        public string _PPOBox { get; set; }
        public string _PSuburb { get; set; }
        public string _PState { get; set; }
        public string _PPostcode { get; set; }
        public string _PCountry { get; set; }

        //Contact details
        public string _BusinessNo { get; set; }
        public string _MobileNo { get; set; }

        public Boolean _IsFinAdv { get; set; }
    }

    public class SalesforceModel
    {
        public string _EmailAddress { get; set; }
        public string _Address { get; set; }
        public string _City { get; set; }
        public string _State { get; set; }
        public string _Postcode { get; set; }
        public string _Country { get; set; }

        public string _FirstName { get; set; }
        public string _LastName { get; set; }
        public string _Mobile { get; set; }
        public string _Company { get; set; }
    }

    public class greenidVerification
    {
        public decimal _GreenidVerificationID { get; set; }
        public bool _isRegistrationExists { get; set; }
        public string _VerificationId { get; set; }
        public string _VerificationResult { get; set; }
    }

    public class ApplicantVerificationStatus
    {
        public string _ApplicantName { get; set; }
        public string _ApplicantStatus { get; set; }
        public string _overallStatus { get; set; }
    }
}