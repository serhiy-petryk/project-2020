using System;
using System.Collections.Generic;
using System.Globalization;

namespace OlxFlat.Models
{
    public class DomRiaList
    {
        public int Count;
        public int[] Items;
    }

    public class DomRiaDetails
    {
        public static int Max1;

        private static Dictionary<int, DomRiaApi.GroupItem> _characteristics;
        private static Dictionary<int, DomRiaApi.GroupItem> GetCharacteristics()
        {
            if (_characteristics == null)
                _characteristics = DomRiaApi.Main.GetCharacteristics();
            return _characteristics;
        }

        public int realty_id;
        public string district_name_uk;
        public int user_id; // realtor
        public bool? realtorVerified;
        public DateTime publishing_date;
        public int rooms_count;
        public DateTime? inspected_at; // перевірена
        public string description;
        public string description_uk;
        public int floor;
        public int floors_count;
        public string wall_type_uk;
        public string street_name_uk;
        public string building_number_str;
        public decimal total_square_meters;
        public decimal living_square_meters;
        public decimal kitchen_square_meters;
        public decimal longitude;
        public decimal latitude;
        public string beautiful_url; //href
        public decimal price;
        public Dictionary<int, string> characteristics_values; // 19048801 - many characteristics
        public int? delete_reason; // deleted flag

        public Dictionary<int, string> priceArr;
        // public string currency_type;

        public string Heating;
        public string Obmin;
        public bool? Torg;
        public bool? Rozstrochka;
        public string Building;
        public bool? Dogovirna;
        public string Propozycia;

        public int Price => int.Parse(priceArr[1].Replace(" ", ""));
        public string Description => description_uk ?? description;
        public bool LastFloor => floor == floors_count;

        public void ApplyCharacteristics()
        {

            if (Description != null && Description.Length > Max1) Max1 = Description.Length;

            if (characteristics_values.ContainsKey(118))
            {
                var value = int.Parse(characteristics_values[118]);
                var a1 = GetCharacteristics()[118].children[value].name_uk;
                if (wall_type_uk == null)
                    wall_type_uk = a1;
                else if (wall_type_uk != a1)
                    throw new Exception($"DomRiaDetails. Check wall type");
            }

            if (characteristics_values.ContainsKey(209) && int.Parse(characteristics_values[209]) != rooms_count)
                throw new Exception($"DomRiaDetails. Check room count");

            if (characteristics_values.ContainsKey(214) && decimal.Parse(characteristics_values[214], CultureInfo.InvariantCulture) != total_square_meters)
                throw new Exception($"DomRiaDetails. Check total_square_meters");

            if (characteristics_values.ContainsKey(216) && decimal.Parse(characteristics_values[216], CultureInfo.InvariantCulture) != living_square_meters)
                throw new Exception($"DomRiaDetails. Check living_square_meters");

            if (characteristics_values.ContainsKey(218) && decimal.Parse(characteristics_values[218], CultureInfo.InvariantCulture) != kitchen_square_meters)
                throw new Exception($"DomRiaDetails. Check kitchen_square_meters");

            if (characteristics_values.ContainsKey(227) && int.Parse(characteristics_values[227]) != floor)
                throw new Exception($"DomRiaDetails. Check floor");
            if (characteristics_values.ContainsKey(228) && int.Parse(characteristics_values[228]) != floors_count)
                throw new Exception($"DomRiaDetails. Check floor count");

            if (characteristics_values.ContainsKey(265))
            {
                var value = int.Parse(characteristics_values[265]);
                Obmin = GetCharacteristics()[265].children[value].name_uk;
            }
            if (characteristics_values.ContainsKey(273))
                Torg = true;
            if (characteristics_values.ContainsKey(274))
                Rozstrochka = true;

            if (characteristics_values.ContainsKey(443))
            {
                var value = int.Parse(characteristics_values[443]);
                Building = GetCharacteristics()[443].children[value].name_uk;
                if (Building == "не вказано")
                    Building = null;
            }

            if (characteristics_values.ContainsKey(1011))
                Dogovirna = true;

            if (characteristics_values.ContainsKey(1437))
            {
                var value = int.Parse(characteristics_values[1437]);
                Propozycia = GetCharacteristics()[1437].children[value].name_uk;
            }
            if (characteristics_values.ContainsKey(1650))
            {
                var value = int.Parse(characteristics_values[1650]);
                Heating = GetCharacteristics()[1650].children[value].name_uk;
                if (Heating == @"індивідуальне")
                    Heating = @"Індив.";
                else if (Heating == @"централізоване")
                    Heating = @"ЦО";
                else if (Heating == "без опалення")
                    Heating = "без";
            }
        }
    }
}
