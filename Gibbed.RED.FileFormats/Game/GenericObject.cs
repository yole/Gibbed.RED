using System;
using System.Collections.Generic;

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

        public GenericObject(string type)
        {
            _type = type;
        }

        public string Type
        {
            get { return _type; }
        }

        public void Serialize(IFileStream stream)
        {
            if (stream.Mode == SerializeMode.Reading)
            {
                PropertySerializer.Serialize(this, stream);
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
    }
}