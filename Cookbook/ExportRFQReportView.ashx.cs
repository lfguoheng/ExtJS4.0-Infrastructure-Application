﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using ExcelLibrary.SpreadSheet;

namespace Cookbook
{

    public class ExportRFQReportView : DatabaseHandler
    {
        public override PagedData ProcessRequest(HttpContext context, CookDBDataContext db)
        {
            string user_name = context.Request.Params.Get("user_name");
            string export_location = context.Request.Params.Get("export_loc");
            System.IO.StreamReader reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
            var jsonSerializer = new JsonSerializer();
            using (new ImpersonateUser("cookbook", "USANAD", "987-oiu8"))
            {
                JArray blob = (JArray)jsonSerializer.Deserialize(new JsonTextReader(new StringReader(reader.ReadToEnd())));

                Workbook workbook = new Workbook();
                Worksheet projectReportViewTab = new Worksheet("RFQ Report View");

                /*
                 * fill first 200 cells with null data - this is a workaround using the ExcelLibrary so that a warning 
                 * prompt will not show when opening the file. File needs to be >6000 bytes for the warning not to show up...don't ask me...
                 */
                for (var k = 0; k < 200; k++)
                    projectReportViewTab.Cells[k, 0] = new Cell(null);


                //set up the column headers (titles)
                projectReportViewTab.Cells[0, 0] = new Cell("Exp?");
                projectReportViewTab.Cells[0, 1] = new Cell("Project Number");
                projectReportViewTab.Cells[0, 2] = new Cell("Project Name");
                projectReportViewTab.Cells[0, 3] = new Cell("Status");
                projectReportViewTab.Cells[0, 4] = new Cell("Bus Unit");
                projectReportViewTab.Cells[0, 5] = new Cell("TC");
                projectReportViewTab.Cells[0, 6] = new Cell("PM");
                projectReportViewTab.Cells[0, 7] = new Cell("Flow?");
                projectReportViewTab.Cells[0, 8] = new Cell("RFQ Rec\'d");
                projectReportViewTab.Cells[0, 9] = new Cell("Quote Due");
                projectReportViewTab.Cells[0, 10] = new Cell("Req UAT");
                projectReportViewTab.Cells[0, 11] = new Cell("Req PROD");
                int currentRow = 1;
                foreach (JObject currentRecord in blob)
                {

                    var currentProject = db.ProjectInformations.Single(a => a.project_id.Equals((Int32)currentRecord["project_id"]));

                    projectReportViewTab.Cells[currentRow, 0] = new Cell((String)currentRecord["exp"]);
                    projectReportViewTab.Cells[currentRow, 1] = new Cell((String)currentRecord["project_number"]);
                    projectReportViewTab.Cells[currentRow, 2] = new Cell((String)currentRecord["project_name"]);
                    projectReportViewTab.Cells[currentRow, 3] = new Cell((String)currentRecord["project_status"]);
                    projectReportViewTab.Cells[currentRow, 4] = new Cell((String)currentRecord["business_unit"]);
                    projectReportViewTab.Cells[currentRow, 5] = new Cell((String)currentRecord["tc"]);
                    projectReportViewTab.Cells[currentRow, 6] = new Cell((String)currentRecord["pm"]);
                    projectReportViewTab.Cells[currentRow, 7] = new Cell((String)currentRecord["flow"]);
                    projectReportViewTab.Cells[currentRow, 8] = new Cell((String)currentRecord["rfq_recd"]);
                    projectReportViewTab.Cells[currentRow, 9] = new Cell((String)currentRecord["quote_due"]);
                    projectReportViewTab.Cells[currentRow, 10] = new Cell((String)currentRecord["req_uat"]);
                    projectReportViewTab.Cells[currentRow, 11] = new Cell((String)currentRecord["req_prod"]);
                    currentRow++;
                }

                currentRow += 2;
                projectReportViewTab.Cells[currentRow, 0] = new Cell("Generated by " + user_name.Replace('.', ' ') + " on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString() + ".");
                projectReportViewTab.Cells.ColumnWidth[0, 14] = 8000;

                workbook.Worksheets.Add(projectReportViewTab);

                string folder, filename = "rfqReportViewExport" + DateTime.Today.Year + DateTime.Today.Month + DateTime.Today.Day +
                        "_" + DateTime.Now.Hour + DateTime.Now.Minute + ".xls";

                if (export_location == "" || export_location == null)
                {
                    folder = "\\\\nor2k3ops1\\e_drive\\Project Management\\Projects\\INT\\INT-551 Web Cookbook\\RFQ Exports\\";
                }
                else
                {
                    if ((export_location.Reverse()).ToString().Substring(0, 1) != "\\") //add a slash to the network path if it doesn't already exist or it will be put in the parent dir
                    {
                        folder = export_location + "\\";
                    }
                    else
                    {
                        folder = export_location;
                    }
                }

                try
                {
                    folder = Path.GetFullPath(folder);
                    string file = folder + filename;
                    workbook.Save(file);
                    return new PagedData("Project Report View Exported to " + folder + " with the filename: " + filename, true);
                }
                catch (Exception e)
                {
                    return new PagedData("Project Report View Unable To Be Exported; Contact Cookbook Admin - Error: " + e.Message, false);
                }
            }
        }
    }
}