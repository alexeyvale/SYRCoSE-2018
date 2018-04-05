using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;

using Microsoft.CSharp;

using SpecParsing = LandParserGenerator.Builder;
using LandParserGenerator.Parsing.LL;

namespace LandParserGenerator
{
	public static class BuilderLL
	{
		public static Parser BuildParser(string text, List<Message> errors)
		{
			return BuilderBase.BuildParser(GrammarType.LL, text, errors) as Parser;
		}

		public static Parser BuildYacc()
		{
			Grammar yaccGrammar = new Grammar(GrammarType.LL);

			/// Пропускаемые сущности
			yaccGrammar.DeclareTerminal(new TerminalSymbol("COMMENT_L", @"'//' ~[\n\r]*"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("COMMENT_ML", "'/*' .*? '*/'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("COMMENT", "COMMENT_L|COMMENT_ML"));

            yaccGrammar.DeclareTerminal(new TerminalSymbol("STRING_SKIP", "'\\\\\"' | '\\\\\\\\'"));
            yaccGrammar.DeclareTerminal(new TerminalSymbol("STRING_STD", "'\"' (STRING_SKIP|.)*? '\"'"));
            yaccGrammar.DeclareTerminal(new TerminalSymbol("STRING_ESC", "'@\"' ~[\"]* '\"'"));
            yaccGrammar.DeclareTerminal(new TerminalSymbol("STRING", "STRING_STD|STRING_ESC"));

			yaccGrammar.DeclareTerminal(new TerminalSymbol("DECLARATION_CODE", "'%{' (STRING|COMMENT|.)*? '%}'"));

			yaccGrammar.SetOption(ParsingOption.SKIP, "COMMENT", "STRING", "DECLARATION_CODE");


			yaccGrammar.DeclareTerminal(new TerminalSymbol("BORDER", "'%%'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("DECLARATION_NAME", "'%' ID"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("CORNER_LEFT", "'<'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("ID", "[_a-zA-Z][_0-9a-zA-Z]*"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("CORNER_RIGHT", "'>'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("COLON", "':'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("SEMICOLON", "';'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("LITERAL", @"'\''(.|'\\\'')*?'\''"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("LBRACE", "'{'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("RBRACE", "'}'"));
			yaccGrammar.DeclareTerminal(new TerminalSymbol("PIPE", "'|'"));


			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("grammar", new string[][]
			{
				new string[]{ "declarations", "BORDER", "rules", "grammar_ending" }
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("grammar_ending", new string[][]
			{
				new string[]{ "BORDER", "Any" },
				new string[]{ }
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("declarations", new string[][]
			{
				new string[]{ "declaration", "declarations" },
				new string[]{ },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("declaration", new string[][]
			{
				new string[]{ "DECLARATION_NAME", "optional_type", "declaration_body" }
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("optional_type", new string[][]
			{
				new string[]{ "CORNER_LEFT", "ID", "CORNER_RIGHT" },
				new string[]{ }
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("declaration_body", new string[][]
			{
				new string[]{ "identifiers" },
				new string[]{ "Any" }
			}));


			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("identifiers", new string[][]
			{
				new string[]{ "ID", "identifiers_list" },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("identifiers_list", new string[][]
			{
				new string[]{ "ID", "identifiers_list" },
				new string[]{ }
			}));


			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("rules", new string[][]
			{
				new string[]{ "rule", "rules_list" },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("rules_list", new string[][]
			{
				new string[]{ "rule", "rules_list" },
				new string[]{ },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("rule", new string[][]
			{
				new string[]{ "ID", "COLON", "alternatives", "SEMICOLON" },
			}));


			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("alternatives", new string[][]
			{
				new string[]{ "alternative", "alternatives_list" },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("alternatives_list", new string[][]
			{
				new string[]{ "PIPE", "alternative", "alternatives_list" },
				new string[]{ },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("alternative", new string[][]
			{
				new string[]{ "alternative_component", "alternative" },
				new string[]{ }
			}));


			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("alternative_component", new string[][]
			{
				new string[]{ "ID" },
				new string[]{ "LBRACE", "code_content", "RBRACE" },
				new string[] {"LITERAL" }
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("code_content_element", new string[][]
			{
				new string[]{ "Any" },
				new string[]{ "LBRACE", "code_content", "RBRACE" },
			}));

			yaccGrammar.DeclareNonterminal(new NonterminalSymbol("code_content", new string[][]
			{
				new string[]{ "code_content_element", "code_content" },
				new string[] { }
			}));

			yaccGrammar.SetOption(ParsingOption.START, "grammar");

			yaccGrammar.SetOption(NodeOption.LIST, "alternatives_list", "alternative", "rules_list", "declarations", "identifiers_list", "code_content");

			yaccGrammar.SetOption(NodeOption.GHOST, "alternatives_list", "rules_list", "code_content_element");


            var errors = yaccGrammar.CheckValidity();

            if (errors.Count() == 0)
            {
                TableLL1 table = new TableLL1(yaccGrammar);
                table.ExportToCsv("yacc_table.csv");

                var lexerType = BuilderBase.BuildLexer(yaccGrammar, "YaccGrammarLexer");

                /// Создаём парсер
                var parser = new Parser(yaccGrammar,
                    new AntlrLexerAdapter(
                        (Antlr4.Runtime.ICharStream stream) => (Antlr4.Runtime.Lexer)Activator.CreateInstance(lexerType, stream)
                    )
                );

                return parser;
            }
            else
            {
                foreach (var error in errors)
                    Console.WriteLine(error);
                return null;
            }
		}

        public static Parser BuildExpressionGrammar()
		{
			/// Формируем грамматику

			Grammar exprGrammar = new Grammar(GrammarType.LL);

			exprGrammar.DeclareTerminal(new TerminalSymbol("PLUS", "'+'"));
			exprGrammar.DeclareTerminal(new TerminalSymbol("MULT", "'*'"));
			exprGrammar.DeclareTerminal(new TerminalSymbol("LPAR", "'('"));
			exprGrammar.DeclareTerminal(new TerminalSymbol("RPAR", "')'"));
			exprGrammar.DeclareTerminal(new TerminalSymbol("ID", "[_a-zA-Z][_0-9a-zA-Z]*"));

			exprGrammar.DeclareNonterminal(new NonterminalSymbol("E", new string[][]
			{
				new string[]{ "T", "E'" }
			}));

			exprGrammar.DeclareNonterminal(new NonterminalSymbol("E'", new string[][]
			{
				new string[]{ "PLUS", "T","E'" },
				new string[]{ }
			}));

			exprGrammar.DeclareNonterminal(new NonterminalSymbol("T", new string[][]
			{
				new string[]{ "F", "T'" },
			}));

			exprGrammar.DeclareNonterminal(new NonterminalSymbol("T'", new string[][]
			{
				new string[]{ "MULT", "F","T'" },
				new string[]{ }
			}));

			exprGrammar.DeclareNonterminal(new NonterminalSymbol("F", new string[][]
			{
				new string[]{ "LPAR", "E","RPAR" },
				new string[]{ "ID" }
			}));

			exprGrammar.SetOption(ParsingOption.START, "E");

            var errors = exprGrammar.CheckValidity();

            if (errors.Count() == 0)
            {
                TableLL1 table = new TableLL1(exprGrammar);
                table.ExportToCsv("expr_table.csv");

                var lexerType = BuilderBase.BuildLexer(exprGrammar, "ExprGrammarLexer");

                /// Создаём парсер
                var parser = new Parser(exprGrammar,
                    new AntlrLexerAdapter(
                        (Antlr4.Runtime.ICharStream stream) => (Antlr4.Runtime.Lexer)Activator.CreateInstance(lexerType, stream)
                    )
                );

                return parser;
            }
            else
            {
                foreach (var error in errors)
                    Console.WriteLine(error);
                return null;
            }
        }
	}
}
