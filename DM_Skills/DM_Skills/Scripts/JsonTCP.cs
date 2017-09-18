using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM_Skills.Scripts
{
    class JsonTCP
    {
        public const int COMMAND_DISCONNECT = 1;
        public const int COMMAND_PING = 2;

        protected IConverterJsonTCP JsonConverter;
        protected IDataReceive DataController;

        protected string PackJson(int command, int? packetID = null, object dataObj = null)
        {
            List<string> options = new List<string>();
            options.Add("\"Command\": " + command);

            if (packetID.HasValue)
                options.Add("\"PacketID\": " + packetID.Value);

            if (dataObj != null)
            {
                string jsonObj = JsonConverter.To(out string key, dataObj);

                options.Add("\"ObjectID\": \"" + key + "\"");
                options.Add("\"Object\": " + jsonObj);
            }

            return "{ " + string.Join(", ", options) + " }";
        }

        protected object[] UnpackJson(string json)
        {
            var jsonObj = Json.JsonParser.FromJson(json);

            int command = Convert.ToInt32(jsonObj["Command"]);
            int packetID = jsonObj.ContainsKey("PacketID") ? Convert.ToInt32(jsonObj["PacketID"]) : -1;
            string dataID = jsonObj.ContainsKey("ObjectID") ? (string)jsonObj["ObjectID"] : null;
            object data = jsonObj.ContainsKey("Object") ? JsonConverter.From(dataID, (Dictionary<string, object>)jsonObj["Object"]) : null;
            
            return new object[] { command, packetID, dataID, data };
        }
    }
}
