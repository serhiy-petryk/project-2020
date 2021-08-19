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
        public class User
        {
            public string name;
        }

        public class Agency
        {
            public int? agency_id;
            public string name;
            public int? agency_type;
            public int? user_id;
            // public int state_id; region
        }

        public class NewBuild
        {
            public int? newbuildId;
            public string name;
            public int? developerId;
            public int? flatCount;
            public string priceUSD;
            public int? userId;
            public int? noLegalStatus;
            public string street;
            public bool? buildReady;
            public int? buildClass;
            public string buildClassText;
            public DateTime? constructionEnd;
            public string hostName;
        }

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

        public string realtorName;
        public int? agencyId;
        public string agencyName;
        public int? agencyType;
        public int? agencyUserId;

        public int? buildId;
        public string buildName;
        public int? developerId;
        public int? flatCount;
        public int? buildPrice;
        public int? buildUserId;
        public int? noLegalStatus;
        public string buildStreet;
        public bool? buildReady;
        public int? buildClass;
        public string buildClassText;
        public DateTime? buildEnd;
        public string buildHostName;

        public User user;
        public Agency agency;
        public NewBuild newbuild;

        public string Heating;
        public string Obmin;
        public bool? Torg;
        public bool? Rozstrochka;
        public string Building;
        public bool? Dogovirna;
        public string Propozycia;

        public int Amount => int.Parse(priceArr[1].Replace(" ", ""));
        public string Description => description_uk ?? description;
        public bool LastFloor => floor == floors_count;

        public string Address
        {
            get
            {
                if (!string.IsNullOrEmpty(buildStreet))
                    return buildStreet;
                if (!string.IsNullOrEmpty(street_name_uk) && !string.IsNullOrEmpty(building_number_str))
                    return $"{street_name_uk}, {building_number_str}";
                if (string.IsNullOrEmpty(street_name_uk) && !string.IsNullOrEmpty(building_number_str))
                    return building_number_str;
                return street_name_uk;
            }
        }

        public void ApplyCharacteristics()
        {
            if (characteristics_values.ContainsKey(118))
            {
                var value = int.Parse(characteristics_values[118]);
                var a1 = GetCharacteristics()[118].children[value].name_uk;
                if (wall_type_uk == null)
                    wall_type_uk = a1;
                else if (wall_type_uk.Replace(" ", "") != a1.Replace(" ", ""))
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

            if (user != null && !string.IsNullOrEmpty(user.name))
                realtorName = user.name;

            if (agency != null)
            {
                if (agency.agency_id.HasValue)
                    agencyId = agency.agency_id;
                if (!string.IsNullOrEmpty(agency.name))
                    agencyName = agency.name;
                if (agency.agency_type.HasValue)
                    agencyType = agency.agency_type;
                if (agency.user_id.HasValue)
                    agencyUserId = agency.user_id;
                //if (!Equals(agencyUserId, user_id))
                  //  throw new Exception("agencyUserId");
            }

            if (newbuild != null)
            {
                if (newbuild.newbuildId.HasValue)
                    buildId = newbuild.newbuildId;
                if (!string.IsNullOrEmpty(newbuild.name))
                    buildName = newbuild.name;
                if (newbuild.developerId.HasValue)
                    developerId = newbuild.developerId;
                if (newbuild.flatCount.HasValue)
                    flatCount = newbuild.flatCount;
                if (!string.IsNullOrEmpty(newbuild.priceUSD))
                    buildPrice = int.Parse(newbuild.priceUSD, NumberStyles.AllowThousands, new CultureInfo("uk"));
                if (newbuild.userId.HasValue)
                    buildUserId = newbuild.userId;
                if (newbuild.noLegalStatus.HasValue)
                    noLegalStatus = newbuild.noLegalStatus;
                if (!string.IsNullOrEmpty(newbuild.street))
                    buildStreet = newbuild.street;
                if (newbuild.buildReady.HasValue)
                    buildReady = newbuild.buildReady;
                if (newbuild.buildClass.HasValue)
                    buildClass = newbuild.buildClass;
                if (!string.IsNullOrEmpty(newbuild.buildClassText))
                    buildClassText = newbuild.buildClassText;
                if (newbuild.constructionEnd.HasValue)
                    buildEnd = newbuild.constructionEnd;
                if (!string.IsNullOrEmpty(newbuild.hostName))
                    buildHostName = newbuild.hostName;
            }

            if (Equals(Propozycia, "від власника"))
                Propozycia = "власник";
            else if (Equals(Propozycia, "від забудовника"))
                Propozycia = "забудовник";
            else if (Equals(Propozycia, "від посередника"))
                Propozycia = "посередник";
            else if (Equals(Propozycia, "від представника забудовника"))
                Propozycia = "предст.забуд";

            if (string.IsNullOrEmpty(description)) description = null;
            if (string.IsNullOrEmpty(description_uk)) description_uk = null;
            if (string.IsNullOrEmpty(street_name_uk)) street_name_uk = null;
            if (string.IsNullOrEmpty(building_number_str)) building_number_str = null;
        }
    }
}
