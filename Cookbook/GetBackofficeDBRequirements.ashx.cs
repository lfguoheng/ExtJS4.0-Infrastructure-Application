﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;


namespace Cookbook
{
    /// <summary>
    /// Summary description for GetBackofficeDBRequirements
    /// </summary>
    public class GetBackofficeDBRequirements : DatabaseHandler
    {
        public override PagedData ProcessRequest(HttpContext context, CookDBDataContext db)
        {
            IQueryable<BackofficeDBReq> q = db.BackofficeDBReqs;

            string filter = context.Request.Params.Get("project_id");
            if (!isNull(filter))
            {
                q = q.Where(a => a.project_id == int.Parse(filter));
            }
            else
            {
                return new PagedData("GetBackofficeDBRequirements expects a project_id");
            }

            string readOnly = context.Request.Params.Get("read_only");
            if (isNull(readOnly))
            {
                return new PagedData("read_only flag is expected");
            }
            if (readOnly == "true" && context.Request.RequestType != "GET")
            {
                return new PagedData("Read Only");
            }

            string username = context.Request.Params.Get("user_name");
            
            System.IO.StreamReader reader = new System.IO.StreamReader(context.Request.InputStream, context.Request.ContentEncoding);

            var jsonSerializer = new JsonSerializer();
            JObject blob = (JObject)jsonSerializer.Deserialize(new JsonTextReader(new StringReader(reader.ReadToEnd())));


            switch (context.Request.RequestType)
            {
                case "GET":
                    {
                        return new PagedData(q.Select(a => new { a.backofficedb_req_id, a.project_id, a.@new, a.name, a.sql_file, a.notes, a.instructions }));
                    }
                case "POST":
                    {
                        if (blob["rows"].GetType() == typeof(JObject))
                        {
                            JObject obj = (JObject)blob["rows"];
                            BackofficeDBReq record = new BackofficeDBReq();

                            record.project_id = int.Parse(filter);
                            record.name = (string)obj["name"];
                            record.@new = (bool)obj["new"];
                            record.sql_file = (string)obj["sql_file"];
                            record.notes = (string)obj["notes"];
                            record.instructions = (string)obj["instructions"];  //smm added

                            db.BackofficeDBReqs.InsertOnSubmit(record);
                            db.SubmitChanges();

                            ChangeLog newLog = new ChangeLog();
                            newLog.project_id = Convert.ToInt32(int.Parse(filter));
                            newLog.time = DateTime.Now.ToShortTimeString();
                            newLog.date = DateTime.Now.ToShortDateString();
                            newLog.tab = "Requirements";
                            newLog.user_name = username;
                            newLog.description = "New Backoffice DB requirement added";
                            if (!db.ChangeLogs.Contains(newLog))
                            {
                                db.ChangeLogs.InsertOnSubmit(newLog);
                                db.SubmitChanges();
                            }

                            return new PagedData(record);
                        }

                        JArray objs = (JArray)blob["rows"];
                        List<BackofficeDBReq> list = new List<BackofficeDBReq>();
                        for (int j = 0; j < objs.Count; j++)
                        {
                            BackofficeDBReq record = new BackofficeDBReq();
                            record.project_id = int.Parse(filter);
                            record.name = (string)objs[j]["name"];
                            record.@new = (bool)objs[j]["new"];
                            record.sql_file = (string)objs[j]["sql_file"];
                            record.notes = (string)objs[j]["notes"];
                            record.instructions = (string)objs[j]["instructions"];  //smm added

                            db.BackofficeDBReqs.InsertOnSubmit(record);
                            db.SubmitChanges();
                            list.Add(record);
                        }

                        
                        return new PagedData(list);
                    }
                case "PUT":
                    {
                        if (blob["rows"].GetType() == typeof(JObject))
                        {
                            JObject obj = (JObject)blob["rows"];

                            string logBuilder = "";
                            string intro = "Existing Backoffice DB record modified: ";

                            BackofficeDBReq record = db.BackofficeDBReqs.Single(a => a.backofficedb_req_id.Equals((int)obj["backofficedb_req_id"]));
                            if (record.name != (string)obj["name"])
                            {
                                logBuilder += "Name changed from \"" + record.name + "\" to \"" + (string)obj["name"] + "\".";
                            }
                            record.name = (string)obj["name"];
                            //record.project_id = int.Parse(filter);
                            if (record.@new != (bool)obj["new"])
                            {
                                logBuilder += "New changed from \"" + record.@new + "\" to \"" + (bool)obj["new"] + "\".";
                            }
                            record.@new = (bool)obj["new"];
                            if (record.sql_file != (string)obj["sql_file"])
                            {
                                logBuilder += "SQL File changed from \"" + record.sql_file + "\" to \"" + (string)obj["sql_file"] + "\".";
                            }
                            record.sql_file = (string)obj["sql_file"];
                            if (record.notes != (string)obj["notes"])
                            {
                                logBuilder += "Notes changed from \"" + record.notes + "\" to \"" + (string)obj["notes"] + "\".";
                            }
                            record.notes = (string)obj["notes"];
                            if (record.instructions != (string)obj["instructions"])
                            {
                                logBuilder += "Instructions changed from \"" + record.instructions + "\" to \"" + (string)obj["instructions"] + "\".";
                            }
                            record.instructions = (string)obj["instructions"];  //smm added

                            db.SubmitChanges();

                            if (logBuilder != "")
                            {
                                ChangeLog newLog = new ChangeLog();
                                newLog.project_id = Convert.ToInt32(int.Parse(filter));
                                newLog.time = DateTime.Now.ToShortTimeString();
                                newLog.date = DateTime.Now.ToShortDateString();
                                newLog.tab = "Requirements";
                                newLog.user_name = username;
                                newLog.description = intro + logBuilder;
                                if (!db.ChangeLogs.Contains(newLog))
                                {
                                    db.ChangeLogs.InsertOnSubmit(newLog);
                                    db.SubmitChanges();
                                }
                            }

                            return new PagedData(record);
                        }


                        JArray objs = (JArray)blob["rows"];
                        List<BackofficeDBReq> list = new List<BackofficeDBReq>();
                        for (int j = 0; j < objs.Count; j++)
                        {
                            BackofficeDBReq record = db.BackofficeDBReqs.Single(a => a.backofficedb_req_id.Equals((int)objs[j]["backofficedb_req_id"]));
                            record.name = (string)objs[j]["name"];
                            //record.project_id = int.Parse(filter);
                            record.@new = (bool)objs[j]["new"];
                            record.sql_file = (string)objs[j]["sql_file"];
                            record.notes = (string)objs[j]["notes"];
                            record.instructions = (string)objs[j]["instructions"];  //smm added

                            db.SubmitChanges();
                            list.Add(record);
                        }

                        return new PagedData(list);
                    }
                case "DELETE":
                    {
                        if (blob["rows"].GetType() == typeof(JObject))
                        {
                            JObject obj = (JObject)blob["rows"];

                            string logbuilder = "";

                            BackofficeDBReq record = db.BackofficeDBReqs.Single(a => a.backofficedb_req_id.Equals((int)obj["backofficedb_req_id"]));
                            logbuilder += "Name: \"" + record.name + "\"; New: \"" + record.@new + "\"; SQL File: \"" + record.sql_file + "\"; Instructions: \"" +
                                record.instructions + "\"; Notes: \"" + record.notes + "\".";
                            
                            db.BackofficeDBReqs.DeleteOnSubmit(record);

                            db.SubmitChanges();

                            ChangeLog newLog = new ChangeLog();
                            newLog.project_id = Convert.ToInt32(int.Parse(filter));
                            newLog.time = DateTime.Now.ToShortTimeString();
                            newLog.date = DateTime.Now.ToShortDateString();
                            newLog.tab = "Requirements";
                            newLog.user_name = username;
                            newLog.description = "Existing Backoffice DB Requirement deleted: " + logbuilder;
                            if (!db.ChangeLogs.Contains(newLog))
                            {
                                db.ChangeLogs.InsertOnSubmit(newLog);
                                db.SubmitChanges();
                            }

                            return new PagedData("good");
                        }


                        JArray objs = (JArray)blob["rows"];
                        for (int j = 0; j < objs.Count; j++)
                        {
                            BackofficeDBReq record = db.BackofficeDBReqs.Single(a => a.backofficedb_req_id.Equals((int)objs[j]["backofficedb_req_id"]));
                            db.BackofficeDBReqs.DeleteOnSubmit(record);
                        }

                        db.SubmitChanges();
                        return new PagedData("BackofficeDBReq deleted");
                    }
                default:
                    return new PagedData("Unsupported Http Request:  " + context.Request.RequestType + " not recognized");
            }

            
        }
    }
}