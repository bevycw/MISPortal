<%@ WebHandler Language="C#" Class="ExportHandler" %>

using System;
using System.Web;
using System.IO;
using System.Web.UI;
using System.Text;
using NReco.PdfGenerator;//added this reference to DLL which is copied and pasted in bin folder. 

public class ExportHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string InpuData = context.Request.Form["data"];       //Div Content will be fetched from form data.
        //If You find this error in above line 
        //Error: A potentially dangerous Request.Form value was detected from the client (data="<html><head><title><...").
        //then add this in your Web.config file.
        // <httpRuntime requestValidationMode="2.0" /> inside   <system.web>  </system.web>
        
        string PageType = context.Request.Form["PageType"];  //here we will recieve Page Type sent from front end.
        if (string.IsNullOrEmpty(InpuData))
            InpuData = "Some Error occured.Content not found.Please try again.";
        
        string appPath = HttpContext.Current.Request.PhysicalApplicationPath; 
     
        var htmlContent = InpuData.Replace("AppPath", appPath);
        var pdfDoc = new NReco.PdfGenerator.HtmlToPdfConverter(); //created an object of HtmlToPdfConverter class.

        if (string.IsNullOrEmpty(PageType))
        pdfDoc.Orientation = PageOrientation.Default;  //setting orientation.
        else
        {
            if(PageType=="Landscape")
                pdfDoc.Orientation = PageOrientation.Landscape;
            else
                pdfDoc.Orientation = PageOrientation.Portrait;  
        }
        
        pdfDoc.Size = PageSize.A4;   //8.27 in × 11.02 in //Page Size
        PageMargins pageMargins = new PageMargins();     //Margins in mm
        pageMargins.Bottom = 05;
        pageMargins.Left = 05;
        pageMargins.Right = 05;
        pageMargins.Top = 05;
        pdfDoc.Margins = pageMargins;                      //margins added to PDF.

        pdfDoc.PageHeaderHtml = "";
        //Why I am adding this to page footer?? So we can get paging in footer section of each PDF page.how its working?? A Javascript code is written inside the DLL which is handling  Div's class i.e. page and topage.
        pdfDoc.PageFooterHtml = "<div style='float:right;'>  Page <span class='page'></span> of  <span class='topage'></span></div>";
        var pdfBytes=pdfDoc.GeneratePdf(htmlContent);
        context.Response.ContentType = "application/pdf";
        context.Response.AddHeader("content-disposition", "attachment;filename=ApplicantDetails.pdf");//Use inline in place of attachment If You wish to open PDF on Browser.
        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.BinaryWrite(pdfBytes);
        HttpContext.Current.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}