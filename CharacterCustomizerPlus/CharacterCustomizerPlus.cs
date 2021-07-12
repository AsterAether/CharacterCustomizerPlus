using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using CharacterCustomizerPlus.CustomPlusSurvivors;
using CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors;
using EntityStates.Bandit2.Weapon;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using RoR2.UI;
using UnityEngine;

namespace CharacterCustomizerPlus
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("AsterAether.CharacterCustomizer")]
    [BepInPlugin("AsterAether.CharacterCustomizerPlus", "CharacterCustomizerPlus", "0.3.1")]
    [R2APISubmoduleDependency(nameof(SurvivorAPI))]
    public class CharacterCustomizerPlus : BaseUnityPlugin
    {
        private List<CustomPlusSurvivor> _plusSurvivors;

        private ConfigEntry<bool> CreateReadme { get; set; }
        private ConfigEntry<KeyCode> ReloadConfigButton { get; set; }

        private void Awake()
        {
            Config.SaveOnConfigSet = false;
            
            CreateReadme = Config.Bind(
                "General",
                "PrintReadme",
                false,
                "Outputs a file called \"config_values.md\" to the working directory, containing all config values formatted as Markdown. (Only used for development purposes)");

            ReloadConfigButton = Config.Bind(
                    "General",
                    "ReloadConfigButton",
                    KeyCode.F8,
                    "Loads the config from disk and applies all changes.");
            
            
            _plusSurvivors = new List<CustomPlusSurvivor>
            {
                new CustomPlusCommando(Config, Logger),
                new CustomPlusEngineer(Config, Logger),
                new CustomPlusHuntress(Config, Logger),
                new CustomPlusMercenary(Config, Logger)
            };

            On.RoR2.RoR2Application.OnLoad += AfterLoad;
        }

        private void Update()
        {
            if (Input.GetKeyDown(ReloadConfigButton.Value))
            {
                Config.Reload();
            }
        }

        private IEnumerator AfterLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            yield return orig(self);

            ApplyGeneralSettings();

            foreach (var survivorDef in ContentManager.survivorDefs)
            {
                var plusSurvivor = _plusSurvivors
                    .FirstOrDefault(survivor => survivor.CachedName.Equals(survivorDef.cachedName));
                if (plusSurvivor == null)
                {
                    Logger.LogInfo(survivorDef.cachedName + " is not supported by CharacterCustomizerPlus!");
                    continue;
                }
                plusSurvivor.InitContent(survivorDef);
                Logger.LogInfo("Loaded values for " + plusSurvivor.CommonName);
            }

            if (!CreateReadme.Value) yield break;
            var markdown = new StringBuilder("# Config Values\n");

            markdown.AppendLine("## General");
            markdown.AppendLine(CreateReadme.ToMarkdownString());

            foreach (var customSurvivor in _plusSurvivors)
            {
                markdown.AppendLine("# " + customSurvivor.CommonName);
                var markdownLines = customSurvivor.MarkdownConfigEntries
                    .Select(markdownDef => markdownDef.ToMarkdownString()).ToList();

                markdownLines.Sort();

                foreach (var markdownLine in markdownLines)
                {
                    markdown.AppendLine(markdownLine);
                }
            }

            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\config_values.md",
                markdown.ToString());
        }

        private void ApplyGeneralSettings()
        {
            
        }

        private void OnDestroy()
        {
            _plusSurvivors.ForEach(survivor => survivor.OnStop());
            Config.Save();
        }

        private void OnApplicationQuit()
        {
            _plusSurvivors.ForEach(survivor => survivor.OnStop());
            Config.Save();
        }
    }
}