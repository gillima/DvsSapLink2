using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DvsSapLink2.Settings
{
    public class SettingsDictionary : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new dictionary element
        /// </summary>
        protected sealed override ConfigurationElement CreateNewElement()
        {
            return new DictionaryElement();
        }

        /// <summary>
        /// Gets the key for the given dictionary element
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DictionaryElement)element).Name;
        }

        /// <summary>
        /// Converts the dictionary setting into the dictionary of the given type
        /// </summary>
        /// <typeparam name="TKey">Type of the dictionary key</typeparam>
        /// <typeparam name="TValue">Type of the dictionary value</typeparam>
        public IDictionary<TKey, TValue> As<TKey, TValue>()
        {
            return this.OfType<DictionaryElement>().ToDictionary(
                item => (TKey) Convert.ChangeType(item.Name, typeof(TKey)),
                item => (TValue) Convert.ChangeType(item.Value, typeof(TValue)));
        }
    }
}