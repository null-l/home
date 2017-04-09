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

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Reflection;
using ExtendedXmlSerializer.ContentModel;
using ExtendedXmlSerializer.ContentModel.Collections;
using ExtendedXmlSerializer.ContentModel.Content;
using ExtendedXmlSerializer.Core.Specifications;
using ExtendedXmlSerializer.TypeModel;
using JetBrains.Annotations;
using Activator = System.Activator;

namespace ExtendedXmlSerializer.ExtensionModel.Types
{
	sealed class ImmutableArrayContentOption : CollectionContentOptionBase
	{
		readonly static ISpecification<TypeInfo> Specification = new IsAssignableGenericSpecification(typeof(ImmutableArray<>));

		readonly IContentsServices _contents;
		readonly IEnumerators _enumerators;

		public ImmutableArrayContentOption(IContentsServices contents, IEnumerators enumerators, ISerializers serializers)
			: base(Specification, serializers)
		{
			_contents = contents;
			_enumerators = enumerators;
		}

		protected override ISerializer Create(ISerializer item, TypeInfo classification, TypeInfo itemType)
			=> new Serializer(CreateReader(itemType, _contents, item), new EnumerableWriter(_enumerators, item));

		static IContentReader CreateReader(TypeInfo itemType, IContentsServices contents, IContentReader item)
			=> (IContentReader) Activator.CreateInstance(typeof(ContentReader<>).MakeGenericType(itemType.AsType()), contents, item);

		sealed class ContentReader<T> : IContentReader
		{
			readonly IContentReader<Collection<T>> _reader;

			[UsedImplicitly]
			public ContentReader(IContentsServices services, IContentReader item)
				: this(services.CreateContents<Collection<T>>(new ConditionalContentHandler(services, new CollectionContentHandler(item, services)))) {}

			ContentReader(IContentReader<Collection<T>> reader)
			{
				_reader = reader;
			}

			public object Get(IFormatReader parameter) => _reader.Get(parameter).ToImmutableArray();
		}
	}
}