using System;
using System.Collections;
using System.Collections.Generic;

namespace TheLion.Common.Classes
{
	public class BiMap<TForwardKey, TReverseKey> : IEnumerable<KeyValuePair<TForwardKey, TReverseKey>>
	{
		public Indexer<TForwardKey, TReverseKey> Forward { get; private set; } = new Indexer<TForwardKey, TReverseKey>();
		public Indexer<TReverseKey, TForwardKey> Reverse { get; private set; } = new Indexer<TReverseKey, TForwardKey>();

		public BiMap() {}

		public BiMap(IDictionary<TForwardKey, TReverseKey> oneWayMap)
		{
			Forward = new Indexer<TForwardKey, TReverseKey>(oneWayMap);
			//var reversedOneWayMap = oneWayMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
			//Reverse = new Indexer<TReverseKey, TForwardKey>(reversedOneWayMap);
			
			foreach (TForwardKey forwardKey in oneWayMap.Keys)
			{
				TReverseKey reverseKey = Forward[forwardKey];
				if (Reverse.ContainsKey(reverseKey))
					throw new ArgumentException("Cannot construct bidirectional map from a dictionary with non-unique values.");
				
				Reverse.Add(reverseKey, forwardKey);
			}
		}

		public void Add(TForwardKey forwardKey, TReverseKey reverseKey)
		{
			if (Forward.ContainsKey(forwardKey))
				throw new ArgumentException($"An entry with the same key {forwardKey?.ToString() ?? ""} already exists.");
			if (Reverse.ContainsKey(reverseKey))
				throw new ArgumentException($"An entry with the same key {reverseKey?.ToString() ?? ""} already exists.");

			Forward.Add(forwardKey, reverseKey);
			Reverse.Add(reverseKey, forwardKey);
		}

		public bool Remove(TForwardKey forwardKey)
		{
			if (!Forward.ContainsKey(forwardKey) || !Reverse.ContainsKey(Forward[forwardKey]))
				return false;

			return Reverse.Remove(Forward[forwardKey]) && Forward.Remove(forwardKey);
		}

		public bool Remove(KeyValuePair<TForwardKey, TReverseKey> entry)
		{
			return Forward.Remove(entry.Key) && Reverse.Remove(entry.Value);
		}

		public void Clear()
		{
			Forward.Clear();
			Reverse.Clear();
		}

		public int Count()
		{
			return Forward.Count();
		}

		public bool Contains(TForwardKey key)
		{
			return Forward.ContainsKey(key);
		}

		public bool Contains(TReverseKey key)
		{
			return Reverse.ContainsKey(key);
		}

		IEnumerator<KeyValuePair<TForwardKey, TReverseKey>> IEnumerable<KeyValuePair<TForwardKey, TReverseKey>>.GetEnumerator()
		{
			return Forward.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return Forward.GetEnumerator();
		}

		/// <summary>Publically read-only lookup to prevent inconsistent state between forward and reverse map lookup.</summary>
		/// <typeparam name="Key"></typeparam>
		/// <typeparam name="Value"></typeparam>
		public class Indexer<Key, Value> : IEnumerable<KeyValuePair<Key, Value>>
		{
			private IDictionary<Key, Value> _dictionary;

			public Indexer()
			{
				_dictionary = new Dictionary<Key, Value>();
			}

			public Indexer(IDictionary<Key, Value> dictionary)
			{
				_dictionary = dictionary;
			}

			public Value this[Key index]
			{
				get { return _dictionary[index]; }
			}

			public static implicit operator Dictionary<Key, Value>(Indexer<Key, Value> indexer)
			{
				return new Dictionary<Key, Value>(indexer._dictionary);
			}

			internal void Add(Key key, Value value)
			{
				_dictionary.Add(key, value);
			}

			internal bool Remove(Key key)
			{
				return _dictionary.Remove(key);
			}

			internal void Clear()
			{
				_dictionary.Clear();
			}

			internal int Count()
			{
				return _dictionary.Count;
			}

			public bool ContainsKey(Key key)
			{
				return _dictionary.ContainsKey(key);
			}

			public IEnumerable<Key> Keys
			{
				get
				{
					return _dictionary.Keys;
				}
			}

			public IEnumerable<Value> Values
			{
				get
				{
					return _dictionary.Values;
				}
			}

			public Dictionary<Key, Value> ToDictionary()
			{
				return new Dictionary<Key, Value>(_dictionary);
			}

			public IEnumerator<KeyValuePair<Key, Value>> GetEnumerator()
			{
				return _dictionary.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _dictionary.GetEnumerator();
			}
		}
	}
}