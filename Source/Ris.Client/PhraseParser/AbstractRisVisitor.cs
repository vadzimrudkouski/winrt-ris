﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Ris.Client.Messages.Request;

namespace Ris.Client.PhraseParser
{
	/// <summary>
	/// Description of AbstractRisVisitor.
	/// </summary>
	public abstract class AbstractRisVisitor<T, S>
	{
		public S Visit(SearchExpression expr, T data)
		{
			return Visit((dynamic)expr, data);
		}
		
		public abstract S Visit(TermSearchExpression expr, T data);
		public abstract S Visit(PhraseSearchExpression expr, T data);
		public abstract S Visit(NotSearchExpression expr, T data);
		public abstract S Visit(AndSearchExpression expr, T data);
		public abstract S Visit(OrSearchExpression expr, T data);
	}
}
