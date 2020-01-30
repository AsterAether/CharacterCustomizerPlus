using System.Collections.Generic;
using BepInEx.Configuration;
using BepInEx.Logging;
using CharacterCustomizer.CustomSurvivors;
using CharacterCustomizer.Util.Config;
using RoR2;

namespace CharacterCustomizerPlus.CustomPlusSurvivors
{
    public abstract class CustomPlusSurvivor
    {
        public readonly List<IMarkdownString> MarkdownConfigEntries = new List<IMarkdownString>();
        private ConfigFile Config { get; set; }
        public string CharacterName { get; }
        public SurvivorIndex SurvivorIndex { get; }

        protected CustomPlusSurvivor(SurvivorIndex index, string characterName, ConfigFile file)
        {
            Config = file;
            SurvivorIndex = index;

            CharacterName = characterName;
        }

        public void Patch()
        {
            InitConfigValues();

            OverrideGameValues();
            WriteNewHooks();
        }

        public abstract void InitConfigValues();

        public abstract void OverrideGameValues();

        public abstract void WriteNewHooks();

        public ConfigEntryDescriptionWrapper<T> BindConfig<T>(string key, T defaultVal,
            string description)
        {
            ConfigEntryDescriptionWrapper<T> entry =
                new ConfigEntryDescriptionWrapper<T>(Config.Bind(CharacterName, key, defaultVal, description));
            MarkdownConfigEntries.Add(entry);
            return entry;
        }

        public ConfigEntryDescriptionWrapper<bool> BindConfigBool(string key, string description, bool defVal = false)
        {
            return BindConfig(key, defVal, description);
        }

        public ConfigEntryDescriptionWrapper<float> BindConfigFloat(string key, string description, float defVal = 0f)
        {
            return BindConfig(key, defVal, description);
        }

        public ConfigEntryDescriptionWrapper<int> BindConfigInt(string key, string description, int defVal = 0)
        {
            return BindConfig(key, defVal, description);
        }
    }
}