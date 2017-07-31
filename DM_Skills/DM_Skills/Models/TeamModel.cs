﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Models
{
    class TeamModel : ModelSettings
    {
        const int ERRNO_TIME_NULL = 1;

        public const string ERROR_TIME_NULL = "";
        
        public int ID { get; set; }
        public int SchoolID { get; set; }
        public int LocationID { get; set; }
        public int TableID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Class { get; set; }



        public override bool CanUpload
        {
            get
            {
                if (Time == null || Time == "")
                {
                    ErrNo = ERRNO_TIME_NULL;
                    Error = ERROR_TIME_NULL;
                    return false;
                }

                return true;
            }
        }


        protected override bool OnUpload()
        {
            if (Date == null || Date == "")
            {
                Date = DateTime.Now.ToShortDateString();
            }

            return true;
        }
    }
}