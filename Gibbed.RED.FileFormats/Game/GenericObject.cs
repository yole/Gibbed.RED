using System;
using System.Collections.Generic;
using System.Linq;

namespace Gibbed.RED.FileFormats.Game
{
    public class GenericObject: IFileObject
    {
        private class PropertyValue
        {
            private readonly string _type;
            private readonly object _value;

            public PropertyValue(string type, object value)
            {
                _type = type;
                _value = value;
            }

            public override string ToString()
            {
                if (_value is byte[])
                {
                    return '<' + _type + '>';
                }
                if (_value is List<object>)
                {
                    var listValue = (List<object>) _value;
                    if (_type.StartsWith("@*"))  // array of pointers
                    {
                        return "[" +
                               string.Join(", ",
                                           listValue.Select(
                                               e => e is GenericObject ? ((GenericObject) e).Type : "IFileObject")) +
                               "]";
                    }
                    return "[" + string.Join(", ", listValue) + "]";
                }
                if (_value is List<string>)
                {
                    return string.Join(", ", (List<string>) _value);
                }
                if (_value is GenericObject)
                {
                    return "{" + ((GenericObject) _value).GetDisplayName(false) + "}";
                }
                return _value.ToString();
            }

            public string Type
            {
                get { return _type; }
            }

            public object Value
            {
                get { return _value; }
            }
        }

        private readonly string _type;
        private readonly Dictionary<string, PropertyValue> _propertyValues = new Dictionary<string, PropertyValue>();

        public GenericObject()
        {
        }

        public GenericObject(string type)
        {
            _type = type;
        }

        public string Type
        {
            get { return _type; }
        }

        private byte _objectUnknown;
        public byte[] UndecodedData { get; set; }

        public virtual void Serialize(IFileStream stream)
        {
            if (stream.Mode == SerializeMode.Reading)
            {
                PropertySerializer.Serialize(this, stream);
                stream.SerializeValue(ref _objectUnknown);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public void SetProperty(string name, string type, object value)
        {
            _propertyValues.Add(name, new PropertyValue(type, value));
        }

        public ICollection<string> PropertyValues
        {
            get { return _propertyValues.Keys; }
        }

        public string GetDisplayName(bool includeUndecoded)
        {
            string namePropertyValue = "";
            foreach (var val in _propertyValues)
            {
                if (val.Key.ToLowerInvariant().Contains("name") && val.Value.Value is string)
                {
                    namePropertyValue = (string) val.Value.Value;
                    break;
                }
            }
            if (includeUndecoded && UndecodedData != null)
            {
                if (namePropertyValue.Length > 0)
                {
                    namePropertyValue += " ";
                }
                namePropertyValue += "<" + UndecodedData.Length + " undecoded>";
            }
            return namePropertyValue.Length > 0 ? Type + ": " + namePropertyValue : Type;
        }

        public string GetPropertyValueAsString(string name, StringsFile stringsFile)
        {
            if (_propertyValues[name].Type == "LocalizedString" && stringsFile != null)
            {
                uint key = (uint) _propertyValues[name].Value;
                string value;
                if (!stringsFile.Texts.TryGetValue(key, out value))
                {
                    return "<string " + key + " not found>";
                }
                return "\"" + stringsFile.Texts[key] + "\"";
            }
            return _propertyValues[name].ToString();
        }

        public string GetDataType(string name)
        {
            return _propertyValues[name].Type;
        }

        public override string ToString()
        {
            return "{" + string.Join(", ", _propertyValues.Keys.Select(name => name + ": " + _propertyValues[name])) +
                   "}";
        }
    }
}