using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LandParserGenerator
{
	/// <summary>
	/// Возможные категории опций
	/// </summary>
	public enum OptionCategory { PARSING, NODES }

	/// <summary>
	/// Опции, касающиеся построения дерева
	/// </summary>
	public enum NodeOption { GHOST, LIST, LEAF }

	/// <summary>
	/// Опции, касающиеся процесса разбора
	/// </summary>
	public enum ParsingOption { START, SKIP, IGNORECASE, FRAGMENT }

	public class OptionsManager
	{
		public const string GLOBAL_PARAMETERS_SYMBOL = "";

		private Dictionary<NodeOption, Dictionary<string, List<dynamic>>> NodeOptions { get; set; } = 
			new Dictionary<NodeOption, Dictionary<string, List<dynamic>>>();
		private Dictionary<ParsingOption, Dictionary<string, List<dynamic>>> ParsingOptions { get; set; } = 
			new Dictionary<ParsingOption, Dictionary<string, List<dynamic>>>();

		public void Set(NodeOption opt, params string[] symbols)
		{
			if (!NodeOptions.ContainsKey(opt))
				NodeOptions[opt] = new Dictionary<string, List<dynamic>>();
			foreach (var smb in symbols)
			{
				if (!NodeOptions[opt].ContainsKey(smb))
					NodeOptions[opt].Add(smb, null);
			}
		}

		public void Set(ParsingOption opt, params string[] symbols)
		{
			if (!ParsingOptions.ContainsKey(opt))
				ParsingOptions[opt] = new Dictionary<string, List<dynamic>>();
			foreach (var smb in symbols)
			{
				if (!ParsingOptions[opt].ContainsKey(smb))
					ParsingOptions[opt].Add(smb, null);
			}
		}

		public bool IsSet(NodeOption opt, string symbol = null)
		{
			return NodeOptions.ContainsKey(opt) && (symbol == null || NodeOptions[opt].ContainsKey(symbol));
		}

		public bool IsSet(ParsingOption opt, string symbol = null)
		{
			return ParsingOptions.ContainsKey(opt) && (symbol == null || ParsingOptions[opt].ContainsKey(symbol));
		}

		public HashSet<string> GetSymbols(NodeOption opt)
		{
			return IsSet(opt) ? new HashSet<string>(NodeOptions[opt].Keys) : new HashSet<string>();
		}

		public HashSet<string> GetSymbols(ParsingOption opt)
		{
			return IsSet(opt) ? new HashSet<string>(ParsingOptions[opt].Keys) : new HashSet<string>();
		}

		public void Clear(NodeOption opt)
		{
			NodeOptions.Remove(opt);
		}

		public void Clear(ParsingOption opt)
		{
			ParsingOptions.Remove(opt);
		}
	}

	public class LocalOptions
	{
		public NodeOption? NodeOption { get; set; } = null;
		public double? Priority { get; set; } = null;
		public bool IsLand { get; set; } = false;
		public HashSet<string> AnySyncTokens { get; set; } = new HashSet<string>();

		public void Set(NodeOption opt)
		{
			NodeOption = opt;
		}
	}
}
