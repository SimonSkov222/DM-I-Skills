using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DM_Skills.Models;

namespace DM_Skills.Scripts
{
    class JsonObjectConverter : IConverterJsonTCP
    {
        private Dictionary<string, string> objectKeys = new Dictionary<string, string>();
        private Dictionary<string, Func<object, string>> convertTo = new Dictionary<string, Func<object, string>>();
        private Dictionary<string, Func<Dictionary<string, object>, object>> convertFrom = new Dictionary<string, Func<Dictionary<string, object>, object>>();


        public JsonObjectConverter()
        {
            objectKeys.Add(typeof(LocationModel).FullName, "LM");
            objectKeys.Add(typeof(SchoolModel).FullName, "SM");
            objectKeys.Add(typeof(PersonModel).FullName, "PM");
            objectKeys.Add(typeof(TeamModel).FullName, "TM");
            objectKeys.Add(typeof(TableModelN).FullName, "TAM");
            objectKeys.Add(typeof(string).FullName, "STR");
            objectKeys.Add(typeof(double).FullName, "DOU");
            objectKeys.Add(typeof(List<List<object>>).FullName, "LIST");
            
            convertTo.Add(objectKeys[typeof(LocationModel).FullName], To_LocationModel);
            convertTo.Add(objectKeys[typeof(SchoolModel).FullName], To_SchoolModel);
            convertTo.Add(objectKeys[typeof(PersonModel).FullName], To_PersonModel);
            convertTo.Add(objectKeys[typeof(TeamModel).FullName], To_TeamModel);
            convertTo.Add(objectKeys[typeof(TableModelN).FullName], To_TableModel);
            convertTo.Add(objectKeys[typeof(string).FullName], To_String);
            convertTo.Add(objectKeys[typeof(double).FullName], To_Double);
            convertTo.Add(objectKeys[typeof(List<List<object>>).FullName], To_QueryRead);

            convertFrom.Add(objectKeys[typeof(LocationModel).FullName], From_LocationModel);
            convertFrom.Add(objectKeys[typeof(SchoolModel).FullName], From_SchoolModel);
            convertFrom.Add(objectKeys[typeof(PersonModel).FullName], From_PersonModel);
            convertFrom.Add(objectKeys[typeof(TeamModel).FullName], From_TeamModel);
            convertFrom.Add(objectKeys[typeof(TableModelN).FullName], From_TableModel);
            convertFrom.Add(objectKeys[typeof(string).FullName], From_String);
            convertFrom.Add(objectKeys[typeof(double).FullName], From_Double);
            convertFrom.Add(objectKeys[typeof(List<List<object>>).FullName], From_QueryRead);
        }

        public object From(string key, Dictionary<string, object> data)
        {
            //Database.GetDB().ExecuteQuery()
            

            if (convertFrom.ContainsKey(key))
                return convertFrom[key](data);

            return "Failed : " + key;
        }

        public string To(out string key, object data)
        {
            key = objectKeys[data.GetType().FullName];

            if (convertTo.ContainsKey(key))
                return convertTo[key](data);

            return "{ \"Failed\" : \""+key+"\" }";
        }



        ////////////////////////////
        //      Object To Json
        ////////////////////////////

        #region ObjectToJson
        private string To_LocationModel(object data)
        {
            return "{ \"ID\" : " + ((LocationModel)data).ID + ", \"Name\" : \""+((LocationModel)data).Name + "\" }";
        }

        private string To_SchoolModel(object data)
        {
            return "{ \"ID\" : " + ((SchoolModel)data).ID + ", \"Name\" : \"" + ((SchoolModel)data).Name + "\" }";
        }

        private string To_PersonModel(object data)
        {
            return "{ \"ID\" : " + ((PersonModel)data).ID + ", \"Name\" : \"" + ((PersonModel)data).Name + "\", \"TeamID\" : "+ ((PersonModel)data).TeamID+ " }";
        }

        private string To_TeamModel(object data)
        {
            return "{ \"ID\" : " + ((TeamModel)data).ID + ", \"SchoolID\" : " + ((TeamModel)data).SchoolID + 
                ", \"LocationID\" : " + ((TeamModel)data).LocationID + ", \"UniqueID\" : " + ((TeamModel)data).UniqueID +
                ", \"Date\" : " + ((TeamModel)data).Date + ", \"Time\" : " + ((TeamModel)data).Time +
                ", \"Class\" : \"" + ((TeamModel)data).Class + " }";
        }

        private string To_TableModel(object data)
        {
            string schoolModel      = To_SchoolModel(((TableModelN)data).School);
            string locationModel    = To_LocationModel(((TableModelN)data).Location);
            string teamModel        = To_TeamModel(((TableModelN)data).Team);

            string personsModel     = "[";

            for (int i = 0; i < ((TableModelN)data).Persons.Count; i++)
            {
                if (i > 0) personsModel += ",";
                personsModel += To_PersonModel(((TableModelN)data).Persons[i]);
            }

            personsModel += "]";

            return "{ \"School\" : " + schoolModel + ", \"Location\" : "+ locationModel + ", \"Team\" : "+ teamModel + ", \"Persons\" : "+ teamModel + "}";
        }

        private string To_String(object data)
        {
            return "{ \"Str\" : \"" + (string)data + "\"}";
        }

        private string To_Double(object data)
        {
            return "{ \"Val\" : \"" + (double)data + "\"}";
        }

        private string To_QueryRead(object data)
        {
            List<string> rows = new List<string>();

            foreach (var item in (List<List<object>>)data)
            {
                List<string> row = new List<string>();

                foreach (var i in item)
                {
                    if (i is string)
                    {
                        row.Add("\"" + i + "\"");
                    }
                    else if (i is DBNull)
                    {
                        row.Add("{}");
                    }
                    else
                    {
                        row.Add(i.ToString());
                    }
                }

                rows.Add("[ "+string.Join(", ", row) + "]");
            }

            return "{ \"Rows\" : [ "+ string.Join(", ", rows) + " ] }";
        }

        #endregion


        ////////////////////////////
        //      Object From Json
        ////////////////////////////

        #region ObjectFromJson
        private object From_LocationModel(Dictionary<string, object> data)
        {
            return new LocationModel() { ID = Convert.ToInt32(data["ID"]), Name = (string)data["Name"] };
        }

        private object From_SchoolModel(Dictionary<string, object> data)
        {
            return new SchoolModel() { ID = Convert.ToInt32(data["ID"]), Name = (string)data["Name"] };
        }

        private object From_PersonModel(Dictionary<string, object> data)
        {
            return new PersonModel() { ID = Convert.ToInt32(data["ID"]), Name = (string)data["Name"] };
        }

        private object From_TeamModel(Dictionary<string, object> data)
        {
            return new TeamModel()
            {
                ID = Convert.ToInt32(data["ID"]),
                SchoolID = Convert.ToInt32(data["SchoolID"]),
                LocationID = Convert.ToInt32(data["LocationID"]),
                UniqueID = (string)data["UniqueID"],
                Date = (string)data["Date"],
                Time = (string)data["Time"],
                Class = (string)data["Class"]
            };
        }

        private object From_TableModel(Dictionary<string, object> data)
        {
            var result = new TableModelN()
            {
                School = (SchoolModel)From_SchoolModel((Dictionary<string, object>)data["School"]),
                Location = (LocationModel)From_LocationModel((Dictionary<string, object>)data["Location"]),
                Team = (TeamModel)From_TeamModel((Dictionary<string, object>)data["Team"])
            };

            foreach (var item in (Dictionary<string, object>[])data["Persons"])
            {
                result.Persons.Add((PersonModel)From_PersonModel(item));
            }

            return result;
        }

        private object From_String(Dictionary<string, object> data)
        {
            return data["Str"];
        }
        private object From_Double(Dictionary<string, object> data)
        {
            return Convert.ToDouble(data["Val"]);
        }

        private object From_QueryRead(Dictionary<string, object> data)
        {

            var result = new List<List<object>>();

            foreach (var row in (List<object>)data["Rows"])
            {
                result.Add((List<object>)row);
            }
            return result;
        }
        #endregion
    }
}
