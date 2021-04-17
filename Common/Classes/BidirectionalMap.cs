using System;
using System.Collections;
using System.Collections.Generic;

namespace TheLion.Common
{
	/// <summary>Represents a collection of forward/reverse key pairs with bidirectional mapping.</summary>
	/// <typeparam name="TForwardKey">Forward mapping key.</typeparam>
	/// <typeparam name="TReverseKey">Reverse mapping key</typeparam>
	public class BiMap<TForwardKey, TReverseKey> : IEnumerable<KeyValuePair<TForwardKey, TReverseKey>>
	{
		public Indexer<TForwardKey, TReverseKey> Forward { get; } = new();
		public Indexer<TReverseKey, TForwardKey> Reverse { get; } = new();

		/// <summary>Construct an instance.</summary>
		public BiMap() { }

		/// <summary>Construct an instance by copying <see cref="KeyValuePairs"/> from a one-way <see cref="IDictionary"/>.</summary>
		/// <param name="oneWayMap">A one-way <see cref="IDictionary"/>.</param>
		/// <remarks>Throws <see cref="ArgumentException"/> if <paramref name="oneWayMap"/> contains repeated values.</remarks>
		public BiMap(IDictionary<TForwardKey, TReverseKey> oneWayMap)
		{
			Forward = new Indexer<TForwardKey, TReverseKey>(oneWayMap);
			//var reversedOneWayMap = oneWayMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
			//Reverse = new Indexer<TReverseKey, TForwardKey>(reversedOneWayMap);

			foreach (var forwardKey in oneWayMap.Keys)
			{
				var reverseKey = Forward[forwardKey];
				if (Reverse.ContainsKey(reverseKey))
					throw new ArgumentException("Cannot construct bidirectional map from a dictionary with non-unique values.");

				Reverse.Add(reverseKey, forwardKey);
			}
		}

		/// <summary>Add a new <see cref="KeyValuePair{TForwardKey, TReverseKey}"/> entry to the instance.</summary>
		/// <param name="forwardKey">A <typeparamref name="TForwardKey"/> object.</param>
		/// <param name="reverseKey">A <typeparamref name="TReverseKey"/> object.</param>
		/// <remarks>Throws <see cref="ArgumentException"/> if either <paramref name="forwardKey"/> or <paramref name="reverseKey"/> already exist in the instance.</remarks>
		public void Add(TForwardKey forwardKey, TReverseKey reverseKey)
		{
			if (Forward.ContainsKey(forwardKey))
				throw new ArgumentException($"An entry with the same key {forwardKey?.ToString() ?? ""} already exists.");
			if (Reverse.ContainsKey(reverseKey))
				throw new ArgumentException($"An entry with the same key {reverseKey?.ToString() ?? ""} already exists.");

			Forward.Add(forwardKey, reverseKey);
			Reverse.Add(reverseKey, forwardKey);
		}

		/// <summary>Remove a <see cref="KeyValuePair{TForwardKey, TReverseKey}"/> entry from the instance.</summary>
		/// <param name="forwardKey">Forward key.</param>
		/// <returns>Returns true if the pair exists and was successfully removed, otherwise returns false.</returns>
		public bool Remove(TForwardKey forwardKey)
		{
			if (!Forward.ContainsKey(forwardKey) || !Reverse.ContainsKey(Forward[forwardKey]))
				return false;

			return Reverse.Remove(Forward[forwardKey]) && Forward.Remove(forwardKey);
		}

		/// <summary>Remove a <see cref="KeyValuePair{TForwardKey, TReverseKey}"/> entry from the instance.</summary>
		/// <param name="entry"><see cref="KeyValuePair{TForwardKey, TReserveKey}"/> entry to be removed.</param>
		/// <returns>Returns true if the pair exists and was successfully removed, otherwise returns false.</returns>
		public bool Remove(KeyValuePair<TForwardKey, TReverseKey> entry)
		{
			return Forward.Remove(entry.Key) && Reverse.Remove(entry.Value);
		}

		/// <summary>Check if a <typeparamref name="TForwardKey"/> is present in the instance.</summary>
		/// <param name="key">The <typeparamref name="TForwardKey"/> object to check.</param>
		public bool Contains(TForwardKey key)
		{
			return Forward.ContainsKey(key);
		}

		/// <summary>Check if a <typeparamref name="TReverseKey"/> is present in the instance.</summary>
		/// <param name="key">The <typeparamref name="TReverseKey"/> object to check.</param>
		public bool Contains(TReverseKey key)
		{
			return Reverse.ContainsKey(key);
		}

		/// <summary>Check if a <typeparamref name="TForwardKey"/> is present in the instance and return the corresponding <typeparamref name="TReverseKey"/>.</summary>
		/// <param name="forwardKey">The <typeparamref name="TForwardKey"/> object to check.</param>
		/// <param name="reverseKey">The corresponding <typeparamref name="TReverseKey"/> object, if any.</param>
		public bool TryGetForwardValue(TForwardKey forwardKey, out TReverseKey reverseKey)
		{
			return Forward.TryGetValue(forwardKey, out reverseKey);
		}

		/// <summary>Check if a <typeparamref name="TReverseKey"/> is present in the instance and return the corresponding <typeparamref name="TForwardKey"/>.</summary>
		/// <param name="reverseKey">The <typeparamref name="TReverseKey"/> object to check.</param>
		/// <param name="forwardKey">The corresponding <typeparamref name="TForwardKey"/> object, if any.</param>
		public bool TryGetReverseValue(TReverseKey reverseKey, out TForwardKey forwardKey)
		{
			return Reverse.TryGetValue(reverseKey, out forwardKey);
		}

		/// <summary>Clear the instance of all entries.</summary>
		public void Clear()
		{
			Forward.Clear();
			Reverse.Clear();
		}

		/// <summary>Get the number of entries in the instance.</summary>
		public int Count()
		{
			return Forward.Count();
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

			public bool ContainsKey(Key key)
			{
				return _dictionary.ContainsKey(key);
			}

			public bool TryGetValue(Key key, out Value value)
			{
				return _dictionary.TryGetValue(key, out value);
			}

			internal void Clear()
			{
				_dictionary.Clear();
			}

			internal int Count()
			{
				return _dictionary.Count;
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