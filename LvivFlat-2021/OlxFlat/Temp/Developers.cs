using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace OlxFlat.Temp
{
    public static class Developers
    {
        public static void ParseZnList()
        {
            var fn = @"E:\Temp\flat_test\zabudovnyk\zabudovnyk.vn.list.txt";

            var lines = File.ReadLines(fn, Encoding.Default);
            foreach (var line in lines)
            {
                var ss1 = line.Split(new[]{"м. Львів", "с. Лапа"}, StringSplitOptions.None);
                if (ss1.Length != 2)
                    throw new Exception("Check!!!");
                var goodPrice = false;
                var name = ss1[0].Trim();
                if (ss1[0].Trim().EndsWith("ВІДМІННА ЦІНА"))
                {
                    goodPrice = true;
                    name = ss1[0].Replace("ВІДМІННА ЦІНА", "").Trim();
                }

                ss1[1] = ss1[1].Trim();
                if (ss1[1].StartsWith(","))
                    ss1[1] = ss1[1].Substring(1).Trim();
                ss1[1] = ss1[1].Split('\t')[0];

                string state = null;
                var states = new[] {"Продаж не розпочато", "Новий проект", "Продаж призупинено", "Усі квартири продано", "Невідомо","від "};
                string address = null;
                int? price = null;
                for (var k = 0; k < states.Length; k++)
                {
                    var k1 = ss1[1].IndexOf(states[k], StringComparison.InvariantCultureIgnoreCase);
                    if (k1 > -1)
                    {
                        state = states[k];
                        address = ss1[1].Substring(0, k1);
                        if (state == "від ")
                        {
                            var k2 = ss1[1].IndexOf(")", k1 + 4, StringComparison.InvariantCulture);
                            var ss2 = ss1[1].Substring(k1, k2 - k1 + 1).Split(new[] {"(від", "грн/м2)"}, StringSplitOptions.None);
                            price = int.Parse(ss2[1].Replace(" ", ""));
                            state = ss2[0].Trim();
                            ss1[1] = ss1[1].Substring(k2+1).Trim();
                            if (k2 == -1)
                                throw new Exception("Check!!!");
                        }
                        else
                            ss1[1] = ss1[1].Substring(k1 + state.Length).Trim();
                        break;
                    }
                }

                if (address == null)
                    throw new Exception("Check!!!");

                var stages = new[] { "будується", "збудовано", "в проекті", "заморожено"};
                string stage = null;
                for (var k = 0; k < stages.Length; k++)
                {
                    if (ss1[1].StartsWith(stages[k], StringComparison.InvariantCultureIgnoreCase))
                    // if (k1 > -1)
                    {
                        stage = stages[k];
                        ss1[1] = ss1[1].Substring(stage.Length).Trim();
                        break;
                    }
                }

                if (stage == null && !string.IsNullOrEmpty(ss1[1]))
                    throw new Exception("Check!!!");

                var recommended = ss1[1].EndsWith("рекомендуємо");
                if (recommended)
                    ss1[1] = ss1[1].Substring(0, ss1[1].Length - 12).Trim();

                string kind = null;
                string wall = null;
                // if (!string.IsNullOrEmpty(ss1[1]))
                {
                    var kinds = new[] {"комфорт", "бізнес", "еліт", "економ" };
                    var walls = new[] {"моно-каркас", "цегляна"};
                    var ss2 = new List<string>(ss1[1].Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries));
                    foreach (var s2 in ss2)
                    {
                        if (Array.IndexOf(kinds, s2) != -1)
                        {
                            kind = s2;
                        }
                        else if (Array.IndexOf(walls, s2) != -1)
                        {
                            wall = s2;
                        }
                        else
                            throw new Exception($"Check!!! {s2}");
                    }
                }
                Debug.Print($"{name}\t{address}\t{goodPrice}\t{price}\t{state}\t{stage}\t{kind}\t{wall}\t{recommended}");
            }
        }
    }
}
