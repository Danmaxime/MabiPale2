using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace MabiPale2
{
    public class ConfigProfiles
    {
        public Dictionary<string, Filters> configProfiles = new Dictionary<string, Filters>();

        public ConfigProfiles()
        {
            if (!File.Exists("profiles.cfg"))
            {
                //File.Create("profiles.cfg");

                Filters filter = new Filters();
                string profileName = "Default";
                filter.RecvFilter = new List<string> { "0FD13021", "0F44BBA3" };
                filter.SendFilter = new List<string> { "0F213303", "0FF23431" };

                ConfigProfiles profile = this;
                profile.configProfiles.Add(profileName, filter);

                string configs_text = JsonConvert.SerializeObject(profile);
                File.WriteAllText("profiles.cfg", configs_text);
            }
        }

        public ConfigProfiles LoadConfigProfiles()
        {
            ConfigProfiles configProfiles;
            string config_text = File.ReadAllText("profiles.cfg");
            configProfiles = JsonConvert.DeserializeObject<ConfigProfiles>(config_text);

            return configProfiles;
        }

        public List<string> GetProfileNames()
        {
            var configProfiles = LoadConfigProfiles();

            List<string> profileNames = new List<string>(configProfiles.configProfiles.Keys);

            return profileNames;
        }

        public void SaveConfig(Dictionary<string, Filters> configProfileToAdd)
        {
            var configProfiles = LoadConfigProfiles();

            List<string> profileList = new List<string>(configProfiles.configProfiles.Keys);

            string[] profileName = new string[1];
            configProfileToAdd.Keys.CopyTo(profileName, 0);
            string profileToAddOrChange = profileName[0];

            if (profileList.Contains(profileToAddOrChange))
            {
                HashSet<string> removeDupeRecv = new HashSet<string>(configProfiles.configProfiles[profileToAddOrChange].RecvFilter);
                HashSet<string> removeDupeSend = new HashSet<string>(configProfiles.configProfiles[profileToAddOrChange].SendFilter);

                configProfiles.configProfiles[profileToAddOrChange].RecvFilter = new List<string>(removeDupeRecv);
                configProfiles.configProfiles[profileToAddOrChange].SendFilter = new List<string>(removeDupeSend);

                string configs_text = JsonConvert.SerializeObject(configProfiles);
                File.WriteAllText("profiles.cfg", configs_text);
            }
            else
            {
                HashSet<string> removeDupeRecv = new HashSet<string>(configProfileToAdd[profileName[0]].RecvFilter);
                HashSet<string> removeDupeSend = new HashSet<string>(configProfileToAdd[profileName[0]].SendFilter);

                Filters newFilters = new Filters();
                newFilters.RecvFilter = new List<string>(removeDupeRecv);
                newFilters.SendFilter = new List<string>(removeDupeSend);

                configProfiles.configProfiles.Add(profileName[0], newFilters);

                string configs_text = JsonConvert.SerializeObject(configProfiles);
                File.WriteAllText("profiles.cfg", configs_text);
            }
        }

        public void RemoveProfile(string profileName)
        {
            var configProfiles = LoadConfigProfiles();

            configProfiles.configProfiles.Remove(profileName);

            string configs_text = JsonConvert.SerializeObject(configProfiles);
            File.WriteAllText("profiles.cfg", configs_text);
        }
    }
}
