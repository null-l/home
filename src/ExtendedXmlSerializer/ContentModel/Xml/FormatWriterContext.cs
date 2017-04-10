// MIT License
//
// Copyright (c) 2016 Wojciech Nag�rski
//                    Michael DeMond
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using ExtendedXmlSerializer.ContentModel.Conversion.Formatting;
using ExtendedXmlSerializer.ContentModel.Xml.Namespacing;

namespace ExtendedXmlSerializer.ContentModel.Xml
{
	sealed class FormatWriterContext : IFormatWriters<System.Xml.XmlWriter>
	{
		readonly static Prefixes Prefixes = Prefixes.Default;

		readonly IPrefixes _table;
		readonly IIdentifierFormatter _formatter;
		readonly IIdentityStore _store;
		readonly ITypePartResolver _parts;

		public FormatWriterContext(IIdentifierFormatter formatter, IIdentityStore store, ITypePartResolver parts)
			: this(Prefixes, formatter, store, parts) {}

		public FormatWriterContext(IPrefixes table, IIdentifierFormatter formatter, IIdentityStore store,
		                           ITypePartResolver parts)
		{
			_table = table;
			_formatter = formatter;
			_store = store;
			_parts = parts;
		}

		public IFormatWriter Get(Writing<System.Xml.XmlWriter> parameter)
			=> new XmlWriter(_table, _formatter, _store, _parts, parameter);
	}
}