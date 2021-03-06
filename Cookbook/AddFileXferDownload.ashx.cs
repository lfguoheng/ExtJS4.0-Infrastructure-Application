﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cookbook
{
    /// <summary>
    /// Summary description for AddFileXferDownload
    /// </summary>
    public class AddFileXferDownload : DatabaseHandler
    {

        public override PagedData ProcessRequest(HttpContext context, CookDBDataContext db)
        {
            if (context.Request.Params.Count == 0)
                return new PagedData("Can't call AddFileXferDownload.ashx without parameters");

            if (context.Request.Params.Get("name") == null)
                return new PagedData("Name is null");

            FileXferDownload file = new FileXferDownload();
            file.name = context.Request.Params.Get("name");
            db.FileXferDownloads.InsertOnSubmit(file);


            db.SubmitChanges();

            return new PagedData("");
        }
    }
}