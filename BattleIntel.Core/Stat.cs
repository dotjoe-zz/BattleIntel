using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class Stat
    {
        public virtual string RawInput { get; set; }
        public virtual int? Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string Defense { get; set; }
        //public virtual decimal DefenseValue { get; set; }

        /// <summary>
        /// When the stat is parsed from lvl-name-def format, this will contain any text that is found AFTER the def value.
        /// </summary>
        public virtual string AdditionalInfo { get; set; }

        public override bool Equals(object obj)
        {
            var that = obj as Stat;
            if (that == null) return false;
            if (that == this) return true;

            return Nullable<int>.Equals(that.Level, this.Level)
                && string.Equals(that.Name, this.Name, StringComparison.OrdinalIgnoreCase)
                && string.Equals(that.Defense, this.Defense, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 37;
                hash = hash * 23 + Level.GetHashCode();
                hash = hash * 23 + (Name != null ? Name.ToLower() : string.Empty).GetHashCode();
                hash = hash * 23 + (Defense != null ? Defense.ToLower() : string.Empty).GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return ToString(" ");
        }

        public virtual string ToString(string separator)
        {
            return string.Join(separator, new string[] { Level.ToString(), Name, Defense, AdditionalInfo }).Trim();
        }

        /// <summary>
        /// Parse the level, name, and defense from a single stat line
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

            //fix a defense number using a comma as decimal separator for value like 1,23m or 1,230k
            s = Regex.Replace(s, @"(\s\d),(\d+\s?[MmKk]?)", "$1.$2");

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
                    for (int j = i + 1; j < tokens.Count(); ++j)
                    {
                        var defMatch = Regex.Match(tokens[j], @"^(?:D|d|Def|def)?[1-9][0-9]*\.?[0-9]*\w*$");
                        if (defMatch.Captures.Count > 0)
                        {
                            stat.Defense = defMatch.Captures[0].Value;
                            defenseIndex = j;
                            break; //Success!!
                        }
                    }
                    break; //No Defense found
                }
            }

            //the name depends on the position of the levelIndex and defenseIndex
            if (levelIndex >= 0 && defenseIndex >= 0)
            {
                if (levelIndex + 1 == defenseIndex) 
                {
                    //lvl-def format, look for leading or trailing name
                    if (levelIndex == 0)
                    {
                        //trailing name, cannot distinguish additional info
                        stat.Name = string.Join(" ", tokens.Skip(defenseIndex+1).ToArray());
                    }
                    else 
                    {
                        //leading name
                        stat.Name = string.Join(" ", tokens.Take(levelIndex).ToArray());

                        //and rest is additional info
                        stat.AdditionalInfo = string.Join(" ", tokens.Skip(defenseIndex + 1).ToArray());
                    }
                }
                else 
                {
                    //preName-lvl-name-def-additionalInfo format

                    //all tokens before the defense are merged to make the name
                    stat.Name = string.Join(" ", tokens.Where((x, i) => i != levelIndex && i < defenseIndex).ToArray());
                
                    //tokens after the defense go to additional info
                    stat.AdditionalInfo = string.Join(" ", tokens.Skip(defenseIndex + 1).ToArray());
                }
            }
            else if (levelIndex >= 0)
            {
                //no defense, use all tokens except the level
                stat.Name = string.Join(" ", tokens.Where((x, i) => i != levelIndex).ToArray());
            }
            else
            {
                //no level or defense, use the whole string :(
                stat.Name = string.Join(" ", tokens.ToArray());
            }

            return stat;
        }
    }
}
