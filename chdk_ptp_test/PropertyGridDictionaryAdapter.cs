using System;
using System.Collections;
using System.ComponentModel;

namespace chdk_ptp_test
{
    class PropertyGridDictionaryAdapter : CustomTypeDescriptor
    {
        private readonly IDictionary _dictionary;

        public PropertyGridDictionaryAdapter(IDictionary d)
        {
            _dictionary = d;
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _dictionary;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (DictionaryEntry e in _dictionary)
            {
                properties.Add(new DictionaryPropertyDescriptor(_dictionary, e.Key));
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }
    }
}
