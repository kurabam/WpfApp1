using DevScribble.ZohoCRM.XmlObjects.Entities;
using IronPdf;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZCRMSDK.CRM.Library.Api.Response;
using ZCRMSDK.CRM.Library.Common;
using ZCRMSDK.CRM.Library.CRUD;
using ZCRMSDK.CRM.Library.Setup.RestClient;
using ZCRMSDK.OAuth.Client;
using ZCRMSDK.OAuth.Contract;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        List<string> selectedLeads = new List<string>();
        public MainWindow()
        {
            Dictionary<string, string> config = new Dictionary<string, string>()
            {
                {"client_id","1000.CA55W5WB17IEJHLXLMIPIL6T6ZC8QH"},
                {"client_secret","c81a851b825ca20e89e1868a4a15f3581dc06fb38e"},
                {"redirect_uri","https://www.google.com/"},
                {"access_type","offline"},
                {"iamUrl","https://accounts.zoho.com"},
                {"persistence_handler_class","ZCRMSDK.OAuth.ClientApp.ZohoOAuthFilePersistence,ZCRMSDK"},
                {"oauth_tokens_file_path","C:\\Users\\Admin\\source\\repos\\WpfApp1\\tokens.txt"},
                {"mysql_username","root"},
                {"mysql_password",""},
                {"mysql_database","zohooauth"},
                {"mysql_server","localhost"},
                {"mysql_port","3306"},
                {"apiBaseUrl","https://www.zohoapis.com"},
                {"photoUrl","{photo_url}"},
                {"apiVersion","v2"},
                {"logFilePath","C:\\Users\\source\\repos\\WpfApp1\\logFile.txt" },
                {"timeout",""},
                {"minLogLevel",""},
                {"domainSuffix","com"},
                {"currentUserEmail","development@salesbridge.be"}
            };
            InitializeComponent();
            ZCRMRestClient.Initialize(config);                  
            ZohoOAuthClient client = ZohoOAuthClient.GetInstance();
            List<string> fieldsLeads = new List<string> { "id", "First_Name", "Last_Name", "E_mail", "Print_Status" };         
            ZCRMModule moduleIns = ZCRMModule.GetInstance("Leads");
            string modifiedTime = DateTime.Today.ToString("yyyy-MM-dd'T'HH:mm:ss");
            BulkAPIResponse<ZCRMRecord> response = moduleIns.GetRecords(2187808000000087509, "id", CommonUtil.SortOrder.asc, 1, 200, modifiedTime, fieldsLeads);
            dynamic records = JObject.Parse((response.ResponseJSON).ToString());
            var data = records.data;
            if (data != null)
            {
                List<Lead> leadsList = new List<Lead>();
                
                foreach (var record in data)
                {
                    leadsList.Add(new Lead { isSelected = false, id = record.id, First_Name = record.First_Name, Last_Name = record.Last_Name, E_mail = record.E_mail, Print_Status = record.Print_Status });
                }
                leadsGrid.ItemsSource = leadsList;
            }
          
        }

        private void print_Click(object sender, RoutedEventArgs e)
        {
            foreach (var selectedLead in selectedLeads)
            {
                var htmlToPdf = new HtmlToPdf();
                var pdf = htmlToPdf.RenderHtmlAsPdf(selectedLead);
                pdf.SaveAs(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "template.Pdf"));
                PrintDocument D = new PrintDocument();
                D.DocumentName = @"C:\Users\Admin\source\repos\WpfApp1\WpfApp1\bin\Debug\template.Pdf";
                D.Print();
            }
            /*Lead cell = leadsGrid.SelectedItem as Lead;  
            var htmlToPdf = new HtmlToPdf();  // new instance of HtmlToPdf
            var html = @"<h1>" + cell.First_Name + " " + cell.Last_Name + "</h1><br><p>id: " + cell.id + "<br>Email: " + cell.E_mail +"</p>";
            var pdf = htmlToPdf.RenderHtmlAsPdf(html);
            pdf.SaveAs(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "template.Pdf"));          
            PrintDocument D = new PrintDocument();
            D.DocumentName = @"C:\Users\Admin\source\repos\WpfApp1\WpfApp1\bin\Debug\template.Pdf";
            D.Print();*/
        }
        internal class Lead
        {
            public bool isSelected { get;  set; }
            public string id { get; internal set; }
            public string First_Name { get; internal set; }
            public string Last_Name { get; internal set; }
            public string E_mail { get; internal set; }
            public string Print_Status { get; internal set; }
        }

        private void chooseAll_Checked(object sender, RoutedEventArgs e)
        {
            
        }



        void OnChecked228(object sender, RoutedEventArgs e)
        {
            Lead row = leadsGrid.SelectedItem as Lead;
            if (row.isSelected == true)
            {
                var html = @"<h1>" + row.First_Name + " " + row.Last_Name + "</h1><br><p>id: " + row.id + "<br>Email: " + row.E_mail + "</p>";
                selectedLeads.Add(html);
            }
        }
    }
}
