﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class Stat
    {
        public virtual string RawInput { get; private set; }
        public virtual string ScrubbedInput { get; private set; }
        public virtual int? Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string Defense { get; set; }
        public virtual decimal? DefenseValue { get; set; }

        /// <summary>
        /// Any text that is found AFTER the Defense value.
        /// </summary>
        public virtual string AdditionalInfo { get; set; }

        public override bool Equals(object obj)
        {
            var that = obj as Stat;
            if (that == null) return false;
            if (that == this) return true;

            return Nullable<int>.Equals(that.Level, this.Level)
                && string.Equals(that.Name, this.Name)
                && string.Equals(that.Defense, this.Defense)
                && Nullable<decimal>.Equals(that.DefenseValue, this.DefenseValue)
                && string.Equals(that.AdditionalInfo, this.AdditionalInfo);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 37;
                hash = hash * 23 + Level.GetHashCode();
                hash = hash * 23 + (Name ?? string.Empty).GetHashCode();
                hash = hash * 23 + (Defense ?? string.Empty).GetHashCode();
                hash = hash * 23 + DefenseValue.GetHashCode();
                hash = hash * 23 + (AdditionalInfo ?? string.Empty).GetHashCode();
                
                return hash;
            }
        }

        public override string ToString()
        {
            return string.Format("L:{0}, N:{1}, D:{2}, DV:{3}, AI:{4}", Level, Name, Defense, DefenseValue, AdditionalInfo);
        }

        /// <summary>
        /// Output as a single Stat Line in `Lvl Name Def` format
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public virtual string ToLine(string separator = " ")
        {
            return string.Join(separator, new string[] { Level.ToString(), Name, Defense, AdditionalInfo }).Trim();
        }

        /// <summary>
        /// Parse the level, name, and defense from a single stat line.
        /// Level MUST come before the defense!
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Stat Parse(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            var stat = new Stat { RawInput = s };

            //trim and remove any internal multi whitespace
            s = Regex.Replace(s.Trim(), @"\s+", " ");

            //trim any quotation marks from erronious copy/paste
            s = s.Trim(new char[] { '"', '\'' });

            //fix a defense number using a comma as decimal separator for value like 1,23m
            s = Regex.Replace(s, @"((?<!\d)\d),(\d{1,2}(?!\d)[\smM]?)", "$1.$2");

            //now remove any comma thousandths separators so we do not split on them next
            s = Regex.Replace(s, @"((?<!\d)\d),(\d{3}(?!\d))", "$1$2");

            //help catch parsing differences in the scrubbing vs. token capturing
            stat.ScrubbedInput = s;

            //split on some common separators and remove common lvl and def value indicator noise
            var tokens = s.Split(new char[] { ' ', ',', '/', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => new string[] { "l", "lv", "lvl", "d", "def"}.Contains(x.ToLower()) == false)
                .Select(x => x.Trim().Trim('.'))
                .ToList();

            int levelIndex = -1;
            int defenseIndex = -1;

            for (int i = 0; i < tokens.Count(); ++i)
            {
                //levels are any number from 1 to infinity with an optional "Lvl" prefix
                var lvlMatch = Regex.Match(tokens[i], "^(?:l|L|lvl|Lvl|lv|Lv)?([1-9][0-9]*)$");
                if (lvlMatch.Success)
                {
                    stat.Level = int.Parse(lvlMatch.Groups[1].Value);
                    levelIndex = i;

                    //def is always AFTER the level
                    if (MatchDefense(stat, tokens, i + 1, out defenseIndex))
                    {
                        break; //success!
                    }
                }
            }

            if (levelIndex < 0)
            {
                //we never found a level, which we never looked for a defense
                MatchDefense(stat, tokens, 0, out defenseIndex);
            }

            SetDefenseValue(stat);
            SetNameAndAdditonalInfo(stat, tokens, defenseIndex, levelIndex);

            return stat;
        }

        private static bool MatchDefense(Stat stat, IList<string> tokens, int startingIndex, out int defenseIndex)
        {
            defenseIndex = -1;

            var matches = new List<Tuple<int, string>>();

            for (int j = startingIndex; j < tokens.Count(); ++j)
            {
                var m = Regex.Match(tokens[j], @"^(?:D|d|Def|def)?([1-9][0-9]*\.?[0-9]*[a-zA-Z]*)$");
                if (m.Success)
                {
                    matches.Add(new Tuple<int, string>(j, m.Groups[1].Value));
                }
            }

            if (matches.Count == 0) return false;
            if (matches.Count == 1)
            {
                defenseIndex = matches[0].Item1;
                stat.Defense = matches[0].Item2;
            }

            //give precedence to number with a m(million) or k(thousandths) indicator
            for (int i = 0; i < matches.Count(); ++i)
            {
                if (Regex.IsMatch(matches[i].Item2, @"[mMkK]$"))
                {
                    defenseIndex = matches[i].Item1;
                    stat.Defense = matches[i].Item2;
                    return true;
                }
            }

            //or give it to the number with a decimal separator
            for (int i = 0; i < matches.Count(); ++i)
            {
                if (Regex.IsMatch(matches[i].Item2, @"\d\.\d"))
                {
                    defenseIndex = matches[i].Item1;
                    stat.Defense = matches[i].Item2;
                    return true;
                }
            }

            //couldn't find a winner, just use the first match
            defenseIndex = matches[0].Item1;
            stat.Defense = matches[0].Item2;
            return true;
        }

        private static void SetDefenseValue(Stat stat)
        {
            stat.DefenseValue = null;
            if (string.IsNullOrEmpty(stat.Defense)) return;

            var m = Regex.Match(stat.Defense, @"(\d+\.?\d*)([mMkK])?");
            if (!m.Groups[1].Success) return;

            decimal d;
            if (!decimal.TryParse(m.Groups[1].Value, out d)) return;

            decimal multiplier = 1;
            if (m.Groups[2].Success)
            {
                string s = m.Groups[2].Value.ToLower();
                if (s == "m")
                {
                    multiplier = 1000000;
                }
                else if (s == "k")
                {
                    multiplier = 1000;
                }
            }
            else
            {
                //assume m or k for certain value ranges
                if (d < 40)
                {
                    multiplier = 1000000;
                }
                else if(d < 10000)
                {
                    multiplier = 1000;
                }

            }
            
            stat.DefenseValue = d * multiplier;
        }

        private static void SetNameAndAdditonalInfo(Stat stat, IList<string> tokens, int defenseIndex, int levelIndex)
        {
            //the name depends on the position of the levelIndex and defenseIndex
            if (levelIndex >= 0 && defenseIndex >= 0)
            {
                if (levelIndex + 1 == defenseIndex)
                {
                    //lvl-def format, look for leading or trailing name
                    if (levelIndex == 0)
                    {
                        //trailing name, cannot distinguish additional info
                        stat.Name = string.Join(" ", tokens.Skip(defenseIndex + 1));
                    }
                    else
                    {
                        //leading name
                        stat.Name = string.Join(" ", tokens.Take(levelIndex).ToArray());

                        //and rest is additional info
                        stat.AdditionalInfo = string.Join(" ", tokens.Skip(defenseIndex + 1));
                    }
                }
                else
                {
                    //preName-lvl-name-def-additionalInfo format

                    //all tokens before the defense are merged to make the name
                    stat.Name = string.Join(" ", tokens.Where((x, i) => i != levelIndex && i < defenseIndex));

                    //tokens after the defense go to additional info
                    stat.AdditionalInfo = string.Join(" ", tokens.Skip(defenseIndex + 1));
                }
            }
            else if (levelIndex >= 0)
            {
                //level with no defense, use all tokens except the level
                stat.Name = string.Join(" ", tokens.Where((x, i) => i != levelIndex));
            }
            else if (defenseIndex >= 0)
            {
                //defense with no level
                stat.Name = string.Join(" ", tokens.Take(defenseIndex));
                stat.AdditionalInfo = string.Join(" ", tokens.Skip(defenseIndex+1));
            }
            else
            {
                //no level or defense, use the whole string :(
                stat.Name = string.Join(" ", tokens);
            }

            if (string.IsNullOrWhiteSpace(stat.Name)) stat.Name = null;
            if (string.IsNullOrWhiteSpace(stat.AdditionalInfo)) stat.AdditionalInfo = null;
        }
    }
}
