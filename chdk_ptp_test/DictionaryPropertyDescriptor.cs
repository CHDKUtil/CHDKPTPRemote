﻿using System;
using System.Collections;
using System.ComponentModel;

namespace chdk_ptp_test
{
    class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        private readonly IDictionary _dictionary;
        private readonly object _key;

        public DictionaryPropertyDescriptor(IDictionary d, object key)
            : base(key.ToString(), null)
        {
            _dictionary = d;
            _key = key;
        }

        public override Type PropertyType => _dictionary[_key].GetType();

        public override void SetValue(object component, object value)
        {
            _dictionary[_key] = value;
        }

        public override object GetValue(object component)
        {
            return _dictionary[_key];
        }

        public override bool IsReadOnly => false;

        public override Type ComponentType => null;

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}