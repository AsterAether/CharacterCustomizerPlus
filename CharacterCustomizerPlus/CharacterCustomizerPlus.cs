using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using CharacterCustomizer.Util.Config;
using CharacterCustomizerPlus.CustomPlusSurvivors;
using CharacterCustomizerPlus.CustomPlusSurvivors.PlusSurvivors;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace CharacterCustomizerPlus
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("at.aster.charactercustomizer")]
    [BepInPlugin("at.aster.charactercustomizerplus", "CharacterCustomizerPlus", "<version>")]
    public class CharacterCustomizerPlus : BaseUnityPlugin
    {
        public List<CustomPlusSurvivor> CustomPlusSurvivors;

        public ConfigEntry<bool> CreateReadme;

        private void Awake()
        {
            CreateReadme = Config.Bind(
                "General",
                "PrintReadme",
                false,
                "Outputs a file called \"config_values.md\" to the working directory, containing all config values formatted as Markdown. (Only used for development purposes)");

            CustomPlusSurvivors = new List<CustomPlusSurvivor>
            {
                new CustomPlusArtificer(Config),
                new CustomPlusCommando(Config),
                new CustomPlusCroco(Config),
                new CustomPlusEngineer(Config),
                new CustomPlusHuntress(Config),
                new CustomPlusLoader(Config),
                new CustomPlusMercenary(Config),
                new CustomPlusMulT(Config),
                new CustomPlusTreebot(Config)
            };


            StringBuilder markdown = new StringBuilder("# Config Values\n");

            markdown.AppendLine("## General");
            markdown.AppendLine(CreateReadme.ToMarkdownString());

            foreach (var customSurvivor in CustomPlusSurvivors)
            {
                customSurvivor.Patch();
                
                if (CreateReadme.Value)
                {
                    markdown.AppendLine("# " + customSurvivor.CharacterName);
                    List<string> markdownLines = new List<string>();

                    foreach (IMarkdownString markdownDef in customSurvivor.MarkdownConfigEntries)
                    {
                        markdownLines.Add(markdownDef.ToMarkdownString());
                    }

                    markdownLines.Sort();

                    foreach (var markdownLine in markdownLines)
                    {
                        markdown.AppendLine(markdownLine);
                    }
                }
            }

            if (CreateReadme.Value)
            {
                System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\config_values.md",
                    markdown.ToString());
            }
        }
    }
}