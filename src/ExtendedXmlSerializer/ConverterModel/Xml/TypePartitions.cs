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

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using ExtendedXmlSerialization.Core;
using ExtendedXmlSerialization.Core.Sources;
using ExtendedXmlSerialization.TypeModel;

namespace ExtendedXmlSerialization.ConverterModel.Xml
{
	class TypePartitions : CacheBase<string, Func<string, TypeInfo>>, ITypePartitions
	{
		public static TypePartitions Default { get; } = new TypePartitions();
		TypePartitions() : this(DefaultParsingDelimiters.Default, AssemblyLoader.Default, TypeNameAlteration.Default) {}

		readonly IParsingDelimiters _delimiters;
		readonly IAssemblyLoader _loader;
		readonly IAlteration<string> _alteration;

		public TypePartitions(IParsingDelimiters delimiters, IAssemblyLoader loader, IAlteration<string> alteration)
		{
			_delimiters = delimiters;
			_loader = loader;
			_alteration = alteration;
		}

		protected override Func<string, TypeInfo> Create(string parameter)
		{
			var parts = parameter.ToStringArray(_delimiters.Part);
			var namespacePath = parts[0].Split(_delimiters.Namespace)[1];
			var assemblyPath = parts[1].Split(_delimiters.Assembly)[1];
			var assembly = _loader.Get(assemblyPath);
			var result = new Types(assembly, namespacePath, _alteration).ToDelegate();
			return result;
		}

		sealed class Types : IParameterizedSource<string, TypeInfo>
		{
			readonly Assembly _assembly;
			readonly string _ns;
			readonly IAlteration<string> _alteration;
			readonly Func<ImmutableArray<TypeInfo>> _types;

			public Types(Assembly assembly, string @namespace, IAlteration<string> alteration)
				: this(assembly, @namespace, alteration, new Namespaces(assembly).Build(@namespace)) {}

			public Types(Assembly assembly, string @namespace, IAlteration<string> alteration,
			                         Func<ImmutableArray<TypeInfo>> types)
			{
				_assembly = assembly;
				_ns = @namespace;
				_alteration = alteration;
				_types = types;
			}

			public TypeInfo Get(string parameter) => Locate($"{_ns}.{_alteration.Get(parameter)}");

			TypeInfo Locate(string parameter) => _assembly.GetType(parameter, false, false)?.GetTypeInfo() ?? Search(parameter);

			TypeInfo Search(string parameter)
			{
				foreach (var typeInfo in _types())
				{
					if (typeInfo.FullName.StartsWith(parameter))
					{
						return typeInfo;
					}
				}
				return null;
			}
		}

		sealed class Namespaces : IParameterizedSource<string, ImmutableArray<TypeInfo>>
		{
			readonly ImmutableArray<TypeInfo> _types;

			public Namespaces(Assembly assembly) : this(assembly.DefinedTypes.ToImmutableArray()) {}

			public Namespaces(ImmutableArray<TypeInfo> types)
			{
				_types = types;
			}

			public ImmutableArray<TypeInfo> Get(string parameter) => Yield(parameter).ToImmutableArray();

			IEnumerable<TypeInfo> Yield(string parameter)
			{
				foreach (var typeInfo in _types)
				{
					if (typeInfo.Namespace == parameter)
					{
						yield return typeInfo;
					}
				}
			}
		}
	}
}