/* Copyright (c) 2011 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using Gibbed.RED.FileFormats.Game;
using Gibbed.RED.FileFormats.Serializers;

namespace Gibbed.RED.FileFormats
{
    public static class PropertySerializer
    {
        public static void Serialize(object target, IFileStream stream)
        {
            if (stream.Mode == SerializeMode.Reading)
            {
                while (true)
                {
                    string name = null;
                    stream.SerializeName(ref name);

                    if (String.IsNullOrEmpty(name) == true ||
                        name == "None")
                    {
                        break;
                    }

                    string type = null;
                    stream.SerializeName(ref type);

                    short unk2 = -1;
                    stream.SerializeValue(ref unk2);

                    if (unk2 != -1)
                    {
                        throw new FormatException();
                    }

                    var start = stream.Position;

                    uint size = 0;
                    stream.SerializeValue(ref size);

                    if (size < 4)
                    {
                        throw new FormatException();
                    }

                    var end = start + size;

                    if (target is GenericObject)
                    {
                        ReadGenericObjectProperty((GenericObject) target, stream, type, size, name);
                    }
                    else
                    {
                        ReadPropertyValue(stream, target, type, name);
                    }

                    if (stream.Position != end)
                    {
                        throw new FormatException();
                    }
                }
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        private static void ReadGenericObjectProperty(GenericObject target, IFileStream stream, string type, uint size, string name)
        {
            IPropertySerializer serializer = GetSerializer(type);
            if (serializer != null)
            {
                target.SetProperty(name, type, serializer.Deserialize(stream));
            }
            else
            {
                uint valueSize = size - 4;
                byte[] value = new byte[valueSize];
                stream.SerializeValue(ref value, valueSize);
                target.SetProperty(name, type, value);
            }
        }

        public static IPropertySerializer GetSerializer(string type)
        {
            if (type.StartsWith("@"))
            {
                string contentType = type.Substring(1);
                IPropertySerializer elementSerializer = GetSerializer(contentType);
                if (elementSerializer != null)
                {
                    return new ArraySerializer(elementSerializer);
                }
                 return new ArraySerializer(new StructureSerializer<GenericObject>());
            }
            if (type.StartsWith("*"))
            {
                return new PointerSerializer();
            }
            if (type.StartsWith("E") && Char.IsUpper(type[1]))
            {
                // it's actually an enum but we simply deserialize the enum member name
                return new CNameSerializer();
            }
            switch (type)
            {
                case "String": return new StringSerializer();
                case "Int": return new IntSerializer();
                case "Uint": return new UintSerializer();
                case "Bool": return new BoolSerializer();
                case "Float": return new FloatSerializer();
                case "CName": return new CNameSerializer();
                case "LocalizedString": return new LocalizedStringSerializer();
                case "TagList": return new TagListSerializer();
                case "CVariant": return new CVariantSerializer();
                case "CGUID": return new GuidSerializer();
                case "EngineTransform": return new EngineTransformSerializer();
                default: return null;
            }
        }

        private static Dictionary<Type, SerializableObjectInfo> TypeInfoCache
            = new Dictionary<Type, SerializableObjectInfo>();
        private static Dictionary<Type, IPropertySerializer> SerializerCache
            = new Dictionary<Type, IPropertySerializer>();

        private static SerializableObjectInfo GetTypeInfo(Type type)
        {
            if (TypeInfoCache.ContainsKey(type) == true)
            {
                return TypeInfoCache[type];
            }

            return TypeInfoCache[type] = new SerializableObjectInfo(type);
        }

        private static void ReadPropertyValue(
            IFileStream stream,
            object target,
            string typeName,
            string propertyName)
        {
            if (target == null)
            {
                throw new ArgumentNullException("obj");
            }
            else if (propertyName == null)
            {
                throw new ArgumentNullException("name");
            }
            else if (typeName == null)
            {
                throw new ArgumentNullException("type");
            }

            var type = target.GetType();
            var info = GetTypeInfo(type);
            if (info == null)
            {
                throw new InvalidOperationException();
            }

            if (info.Properties.ContainsKey(propertyName) == false)
            {
                throw new FormatException(String.Format(
                    "{0} does not contain a property '{1}' ({2})",
                    type, propertyName, typeName));
            }

            var prop = info.Properties[propertyName];
            IPropertySerializer serializer;

            if (SerializerCache.ContainsKey(prop.Serializer) == false)
            {
                serializer = (IPropertySerializer)Activator.CreateInstance(prop.Serializer);
                SerializerCache[prop.Serializer] = serializer;
            }
            else
            {
                serializer = SerializerCache[prop.Serializer];
            }

            var value = serializer.Deserialize(stream);
            prop.PropertyInfo.SetValue(target, value, null);
        }

        private class SerializableObjectInfo
        {
            public Dictionary<string, SerializablePropertyInfo> Properties
                = new Dictionary<string, SerializablePropertyInfo>();

            public SerializableObjectInfo(Type type)
            {
                foreach (var propInfo in type.GetProperties())
                {
                    var serializerAttributes = propInfo.GetCustomAttributes(typeof(PropertySerializerAttribute), false);
                    var nameAttributes = propInfo.GetCustomAttributes(typeof(PropertyNameAttribute), false);
                    var descAttributes = propInfo.GetCustomAttributes(typeof(PropertyDescriptionAttribute), false);

                    if (serializerAttributes.Length > 0 &&
                        nameAttributes.Length > 0)
                    {
                        var info = new SerializablePropertyInfo();
                        info.PropertyInfo = propInfo;
                        info.Serializer = ((PropertySerializerAttribute)serializerAttributes[0]).Serializer;
                        info.Name = ((PropertyNameAttribute)nameAttributes[0]).Name;

                        if (descAttributes.Length > 0)
                        {
                            info.Description = ((PropertyDescriptionAttribute)descAttributes[0]).Description;
                        }

                        if (String.IsNullOrEmpty(info.Name) == true ||
                            info.Serializer == null)
                        {
                            throw new InvalidOperationException();
                        }

                        this.Properties.Add(info.Name, info);
                    }
                }
            }
        }

        private struct SerializablePropertyInfo
        {
            public string Name;
            public string Description;
            public Type Serializer;
            public PropertyInfo PropertyInfo;
        }
    }
}
