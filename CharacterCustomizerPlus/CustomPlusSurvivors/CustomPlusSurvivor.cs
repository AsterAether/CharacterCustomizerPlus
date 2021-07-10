using System.Collections.Generic;
using System.Text.RegularExpressions;
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
        public ConfigEntry<bool> Enabled { get; private set; }
        public ConfigEntry<bool> UpdateVanillaValues { get; private set; }
        protected ConfigFile Config { get; set; }
        protected ManualLogSource Logger { get; set; }

        protected SurvivorDef SurvivorDef { get; private set; }

        public string CommonName { get; private set; }
        public string CachedName { get; private set; }

        protected CustomPlusSurvivor(string cachedName, ConfigFile file, ManualLogSource logger)
        {
            Config = file;
            Logger = logger;
            CachedName = cachedName;
        }

        public void InitContent(SurvivorDef survivorDef)
        {
            SurvivorDef = survivorDef;

            CommonName = Regex.Replace(Language.english.GetLocalizedStringByToken(survivorDef.displayNameToken),
                @"[^A-Za-z]+", string.Empty);
            
            Enabled = Config.Bind(
                CommonName,
                CommonName + " Enabled",
                false,
                "If changes for this character are enabled. Set to true to generate options on next startup!");

            if (!Enabled.Value) return;

            UpdateVanillaValues = Config.Bind(
                CommonName,
                CommonName + " UpdateVanillaValues",
                true,
                "Write default values in descriptions of settings. Will flip to false after doing it once.");
            
            InitConfigValues();

            OverrideGameValues();
            WriteNewHooks();
        }

        protected abstract void InitConfigValues();

        protected abstract void OverrideGameValues();

        protected abstract void WriteNewHooks();

        public void OnStop()
        {
            if (UpdateVanillaValues != null)
                UpdateVanillaValues.Value = false;
        }

        protected ConfigEntryDescriptionWrapper<T> BindConfig<T>(string key, T defaultVal,
            string description)
        {
            var entry =
                new ConfigEntryDescriptionWrapper<T>(Config.Bind(CommonName, key, defaultVal, description), UpdateVanillaValues.Value);
            MarkdownConfigEntries.Add(entry);
            return entry;
        }

        protected ConfigEntryDescriptionWrapper<bool> BindConfigBool(string key, string description, bool defVal = false)
        {
            return BindConfig(key, defVal, description);
        }

        protected ConfigEntryDescriptionWrapper<float> BindConfigFloat(string key, string description, float defVal = 0f)
        {
            return BindConfig(key, defVal, description);
        }

        public ConfigEntryDescriptionWrapper<int> BindConfigInt(string key, string description, int defVal = 0)
        {
            return BindConfig(key, defVal, description);
        }
    }
}