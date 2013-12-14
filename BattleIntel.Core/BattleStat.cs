using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BattleIntel.Core
{
    public class BattleStat
    {
        public virtual int? Level { get; set; }
        public virtual string Name { get; set; }
        public virtual string Defense { get; set; }
        //public virtual decimal DefenseValue { get; set; }

        public override string ToString()
        {
            return ToString(" ");
        }

        public virtual string ToString(string separator)
        {
            return string.Format("{0}{3}{1}{3}{2}{3}", Level, Name, Defense, separator).Trim();
        }

        public static BattleStat Parse(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            //trim and remove any internal multi whitespace
            s = Regex.Replace(s.Trim(), @"\s+", " ");

            //fix a defense number using a comma as decimal separator for million value like 1,23m
            s = Regex.Replace(s, @"(\s\d),(\d+\s?[Mm]?)", "$1.$2");

            var tokens = s.Split(new char[] { ' ', ',', '/', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => new string[] { "l", "lvl", "d", "def" }.Contains(x.ToLower()) == false)
                .Select(x => x.Trim().Trim('.'))
                .ToList();

            var stat = new BattleStat();
            char tokenPlaceholder = '/'; //some char we've already removed (i.e. split on)

            for (int i = 0; i < tokens.Count(); ++i)
            {
                var lvlMatch = Regex.Match(tokens[i], "^(?:l|L|lvl|Lvl)?([1-9][0-9]{0,2})$");
                if (lvlMatch.Success)
                {
                    stat.Level = int.Parse(lvlMatch.Groups[1].Value);
                    tokens[i] = tokenPlaceholder.ToString();

                    //def is always AFTER the level
                    for (int j = i + 1; j < tokens.Count(); ++j)
                    {
                        var defMatch = Regex.Match(tokens[j], @"^(?:D|d|Def|def)?[1-9][0-9]*\.?[0-9]*\w*$");
                        if (defMatch.Captures.Count > 0)
                        {
                            stat.Defense = defMatch.Captures[0].Value;
                            tokens[j] = tokenPlaceholder.ToString();
                            break;
                        }
                    }
                    break;
                }
            }

            //the name is the largest remaining continuous string not separated by the lvl or def tokenPlaceholders
            var tokenMash = string.Join(" ", tokens.ToArray());
            stat.Name = tokenMash.Split(tokenPlaceholder)
                .OrderByDescending(x => x.Length)
                .FirstOrDefault()
                .Trim();

            return stat;
        }
    }
}
