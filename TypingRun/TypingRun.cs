using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TypingRunner
{
	static class TypingRun
	{
		private static readonly Random rand = new();
		private static readonly RandomBoxMuller muller_rand = new();
		public static Dictionary<string, string> symbols = new()
		{
			{ "!", "1" },
			{ "\"", "2" },
			{ "#", "3" },
			{ "$", "4" },
			{ "%", "5" },
			{ "&", "6" },
			{ "'", "7" },
			{ "(", "8" },
			{ ")", "9" },
			{ "=", "-" },
			{ "~", "^" },
			{ "|", "\\" },
			{ "{", "[" },
			{ "}", "]" },
			{ "+", ";" },
			{ "*", ":" },
			{ "<", "," },
			{ ">", "." },
			{ "?", "/" },
		};
		public enum CharType
		{
			Lower,
			Upper,
			NumSymbol,
			Symbol,
			Number,
			Other,
		};
		/*static List<string> Symbol_list = new List<string> 
			{"!","\"","#","$","%","&","'","(",")","=","~","|","{","}","*","+","<",">","?","_",
			"@","[","]","-","^","\\","/",".",","};*/
		public static CharType GetCharType(this char ch)
		{
			if ('\u0041' <= ch && ch <= '\u005A') return CharType.Upper;
			else if ('\u0061' <= ch && ch <= '\u007A') return CharType.Lower;
			else if ('\u0030' <= ch && ch <= '\u0039') return CharType.Number;
			else if ('\u0021' <= ch && ch <= '\u0029') return CharType.NumSymbol;
			else if ('\u0021' <= ch && ch <= '\u007E') return CharType.Symbol;
			else return CharType.Other;
		}
		private static readonly List<List<char>> keybord = new()
		{
			new List<char> { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', },
			new List<char> { 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ';' },
			new List<char> { 'z', 'x', 'c', 'v', 'b', 'n', 'm', },
		};
		static char ToLower(this char ch)
		{
			if (GetCharType(ch) == CharType.Upper) return (char)(ch + 0x0020);
			throw new FormatException();
		}
		static char ToUpper(this char ch)
		{
			if (GetCharType(ch) == CharType.Lower) return (char)(ch - 0x0020);
			throw new FormatException();
		}
		public static char GetNearChar(char ch)
		{
			bool upper = false;
			if (GetCharType(ch) == CharType.Upper)
			{
				ch = ch.ToLower();
				upper = true;
			}
			int y = keybord.FindIndex((i) => i.Contains(ch));
			int x = keybord[y].IndexOf(ch);
			Func<int, char> func = (i) =>
			{
				if (i == y)
				{
					if (x == 0) return keybord[i][1];
					if (x == keybord[i].Count() - 1) return keybord[i][keybord[i].Count() - 2];
					if (rand.Next(0, 1) == 0)
					{
						return keybord[i][x + 1];
					}
					else return keybord[i][x - 1];
				}
				else
				{
					if (x == 0) return keybord[i][rand.Next(0, 1)];
					if (x > keybord[i].Count() - 1) return keybord[i][keybord[i].Count() - 1];
					if (x == keybord[i].Count() - 1) return keybord[i][keybord[i].Count() - 1 - rand.Next(0, 1)];
					if (i == y + 1)
					{
						return keybord[i][x + rand.Next(-1, 0)];
					}
					else
					{
						return keybord[i][x + rand.Next(0, 1)];

					}
				}
			};
			Func<char, char> Change_Prime = (ch) =>
			 {
				 if (upper) return ch.ToUpper();
				 return ch;
			 };
			return Change_Prime(func(rand.Next(y == 2 ? 1 : 0, y == 0 ? 1 : 2)));
		}
		public static char GetRandomChar(char ch)
		{
			CharType type = GetCharType(ch);
			if (type == CharType.Upper || type == CharType.Lower)
			{
				return GetNearChar(ch);
			}
			else if (type == CharType.Number)
			{
				if (ch == '0') return '9';
				if (ch == '9') return '0';
				if (ch == '1') return '2';
				if (rand.Next(0, 1) == 0)
				{
					return (char)(ch + 1);
				}
				else
				{
					return (char)(ch - 1);
				}

			}
			else if (type == CharType.NumSymbol)
			{
				if (ch == '!') return '"';
				if (ch == ')') return '(';
				if (rand.Next(0, 1) == 0)
				{
					return (char)(ch + 1);
				}
				else
				{
					return (char)(ch - 1);
				}
			}
			else return ch;
		}
		public static void SendChar(char ch)
		{
			if (ch == '\n')
			{
				SendKeys.Send("{enter}");
			}
			else if (TypingRun.symbols.ContainsKey(ch.ToString()))
			{
				SendKeys.Send("+" + TypingRun.symbols[ch.ToString()] + "");
			}
			else
			{
				SendKeys.Send(ch.ToString());
			}
		}
		public static bool IsAlphabet(this char ch)
		{
			var type = ch.GetCharType();
			if (type == CharType.Lower || type == CharType.Upper) return true;
			return false;
		}
		public static bool IsSymbol(this char ch)
		{
			var type = ch.GetCharType();
			if (type == CharType.Symbol || type == CharType.NumSymbol) return true;
			return false;
		}
		public static async void SendString(string str)
		{
			const int MINIMUM_INTERVAL = 30;
			const int TYPO_CHANCE = 25;
			const int TYPO_INTERVAL = 250;
			int interval_ave = rand.Next(120, 160);
			const int TYPECHANGED_EXTRA_INTERVAL_AVE = 100;
			str = str.Replace("\t", "");
			str = str.Replace("  ", "");
			CharType previous = CharType.Other;
			Microsoft.VisualBasic.Interaction.AppActivate("Google Chrome");
			for (int i = 0; i < str.Length; i++)
			{
				char temp_char = str[i];
				await Task.Delay((int)muller_rand.Next(interval_ave / 4, interval_ave, MINIMUM_INTERVAL));
				if (str.Length - i >= 2)
				{
					char next_char = str[i + 1];
					if (temp_char == '/' && next_char == '/')
					{
						for (; i < str.Length; i++)
						{
							if (str.Substring(i, 1) == "\n") break;
						}
						continue;
					}
					if (i >= 1 && temp_char == ' ' && !(str[i - 1].IsAlphabet() && next_char.IsAlphabet()))
					{
						continue;
					}
				}
				if (GetCharType(temp_char) != CharType.Symbol
					&& GetCharType(temp_char) != CharType.Other
					&& (i == 0 || str[i] != str[i - 1])
					&& rand.Next(0, TYPO_CHANCE) == 0)
				{
					while (true)
					{
						SendChar(GetRandomChar(temp_char));
						await Task.Delay((int)muller_rand.Next(TYPO_INTERVAL / 2, TYPO_INTERVAL, MINIMUM_INTERVAL));
						if (rand.Next(0, 4) != 0 || !temp_char.IsAlphabet()) break;
					}
				}
				if (previous != GetCharType(temp_char))
				{
					await Task.Delay((int)muller_rand.Next(TYPECHANGED_EXTRA_INTERVAL_AVE / 3, TYPECHANGED_EXTRA_INTERVAL_AVE, MINIMUM_INTERVAL));
					previous = GetCharType(temp_char);
				}
				SendChar(temp_char);
				if (Control.ModifierKeys == Keys.Alt) break;
			}
		}
	}
}
