﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VS_QuickNavigation.Utils
{
	class Description : Attribute
	{
		public string Text;

		public Description(string text)
		{
			Text = text;
		}
	}

	class CommonUtils
	{
		public static T[] ToArray<T>(params T[] objs)
		{
			return objs;
		}

		public static string GetDescription(Enum en)
		{
			Type type = en.GetType();
			System.Reflection.MemberInfo[] memInfo = type.GetMember(en.ToString());

			if (memInfo != null && memInfo.Length > 0)
			{
				object[] attrs = memInfo[0].GetCustomAttributes(typeof(Description), false);

				if (attrs != null && attrs.Length > 0)
					return ((Description)attrs[0]).Text;
			}

			return en.ToString();

		}

		public static bool IsWordCharacter(char c)
		{
			return (c == '_') | char.IsLetterOrDigit(c);
		}

		public static string GetWord(string sLine, int iPos)
		{
			int iMaxENd = sLine.Length - 1;
			int iStart = iPos;
			int iEnd = iPos;

			while (iStart > 1 && IsWordCharacter(sLine[iStart - 1]))
				iStart--;
			while (iEnd < iMaxENd && IsWordCharacter(sLine[iEnd]))
				iEnd++;

			return sLine.Substring(iStart, iEnd - iStart);
		}

		public static int GetCurrentLine()
		{
			EnvDTE.Document activeDoc = Common.Instance.DTE2.ActiveDocument;
			if (activeDoc != null)
			{
				EnvDTE.TextDocument textDoc = (EnvDTE.TextDocument)Common.Instance.DTE2.ActiveDocument.Object("TextDocument");
				if (textDoc != null)
				{
					return textDoc.Selection.ActivePoint.Line;
				}
			}
			return 0;
		}

		public static string GetCurrentWord()
		{
			EnvDTE.Document activeDoc = Common.Instance.DTE2.ActiveDocument;
			if (activeDoc != null)
			{
				EnvDTE.TextDocument textDoc = (EnvDTE.TextDocument)Common.Instance.DTE2.ActiveDocument.Object("TextDocument");
				if (textDoc != null)
				{
					if (textDoc.Selection.ActivePoint.Line > 0)
					{
						string sLine = textDoc.StartPoint.CreateEditPoint().GetLines(textDoc.Selection.ActivePoint.Line, textDoc.Selection.ActivePoint.Line + 1);
						return GetWord(sLine, textDoc.Selection.ActivePoint.LineCharOffset - 1);
					}
				}
			}
			return null;
		}

		public static bool GotoSymbol(Data.SymbolData symbol)
		{
			if (symbol != null)
			{
				EnvDTE.Window window = Common.Instance.DTE2.ItemOperations.OpenFile(symbol.AssociatedFile.Path, EnvDTE.Constants.vsViewKindTextView);
				if (null != window)
				{
					window.Activate();
					if (window.Document != null)
					{
						((EnvDTE.TextSelection)window.Document.Selection).GotoLine(symbol.StartLine);
						return true;
					}
				}
			}
			return false;
		}
	}
}
