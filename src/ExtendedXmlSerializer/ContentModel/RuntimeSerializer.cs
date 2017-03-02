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
using System.Reflection;
using ExtendedXmlSerialization.ContentModel.Content;
using ExtendedXmlSerialization.ContentModel.Xml;

namespace ExtendedXmlSerialization.ContentModel
{
	sealed class RuntimeSerializer : SerializerBase
	{
		readonly ISerialization _serialization;

		public RuntimeSerializer(ISerialization serialization)
		{
			_serialization = serialization;
		}

		public override void Write(IXmlWriter writer, object instance)
		{
			var typeInfo = instance.GetType().GetTypeInfo();
			var container = _serialization.Get(typeInfo);
			var serializer = container.Get();
			if (serializer is RuntimeSerializer)
			{
				throw new InvalidOperationException($"Could not locate the appropriate serializer for {typeInfo}.");
			}
			serializer.Write(writer, instance);
		}

		public override object Get(IXmlReader reader) => _serialization.Get(reader.Classification).Get().Get(reader);
	}
}