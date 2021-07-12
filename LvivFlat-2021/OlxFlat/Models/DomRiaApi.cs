using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OlxFlat.Models
{
    public class DomRiaApi
    {
        public static class Main
        {
            public static Dictionary<int, GroupItem> GetCharacteristics()
            {
                var content = File.ReadAllText(Settings.DomRiaApiCharacteristicsFile).Replace("\"children\":[],", "");
                var o = JsonConvert.DeserializeObject<Group[]>(content);
                foreach (var group in o)
                foreach (var item in group.items)
                    item.group_name = group.group_name;

                var data = new Dictionary<int, GroupItem>();
                foreach (var group in o)
                foreach (var item in group.items)
                        data.Add(item.characteristic_id, item);

                foreach(var o1 in data.OrderBy(kvp=>kvp.Key))
                    Debug.Print($"{o1.Key}: {o1.Value.label_uk}, {o1.Value.child_text}");

                return data;
            }
        }

        public class Group
        {
            public string group_name;
            public int group_prio;
            public GroupItem[] items;
        }

        public class GroupItem
        {
            public int characteristic_id;
            public string label_uk;
            public bool required;
            public int prio;
            public Dictionary<int, ItemChild> children;
            public int[] options;
            public string group_name;

            public string child_text
            {
                get
                {
                    if (children == null) return null;
                    return string.Join(", ", children.Values.Select(a => a.name_uk));
                }
            }
        }
        public class ItemChild
        {
            public int characteristic_id;
            public string name_uk;
            public bool required;
            public int prio;
        }
    }

}
