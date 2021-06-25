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
                filter.RecvFilter = new List<string> { "09037", "09037", "09037" };
                filter.SendFilter = new List<string> { "09037", "09037", "09037" };

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

        public void SaveConfig(Dictionary<string, Filters> configProfileToAdd)
        {
            var configProfiles = LoadConfigProfiles();

            List<string> profileList = new List<string>(configProfiles.configProfiles.Keys);

            string[] profileName = new string[1];
            configProfileToAdd.Keys.CopyTo(profileName, 0);
            string profileToAddOrChange = profileName[0];

            if (profileList.Contains(profileToAddOrChange))
            {
                configProfiles.configProfiles[profileToAddOrChange].RecvFilter = configProfileToAdd[profileToAddOrChange].RecvFilter;
                configProfiles.configProfiles[profileToAddOrChange].SendFilter = configProfileToAdd[profileToAddOrChange].SendFilter;

                string configs_text = JsonConvert.SerializeObject(configProfiles);
                File.WriteAllText("profiles.cfg", configs_text);
            }
            else
            {
                Filters newFilters = new Filters();
                newFilters.RecvFilter = configProfileToAdd[profileName[0]].RecvFilter;
                newFilters.SendFilter = configProfileToAdd[profileName[0]].SendFilter;

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
