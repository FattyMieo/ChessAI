using System.Collections.Generic;
using Chess;

/// <summary>
/// A dictionary that stores chess piece profiles
/// </summary>
public class ChessPieceProfileDictionary
{
    protected IDictionary<ChessPieceType, ChessPieceProfile> wrappedDictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChessPieceProfileDictionary"/> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
    /// </summary>
    public ChessPieceProfileDictionary()
    {
        wrappedDictionary = new Dictionary<ChessPieceType, ChessPieceProfile>();
    }

    /// <summary>
    /// Reinitializes the <see cref="ChessPieceProfileDictionary"/> class that contains elements copied from given values.
    /// </summary>
    /// <param name="profiles">The elements which are copied to the new <see cref="VirtualDictionary{TKey,TValue}"/>.</param>
    public void Init(params ChessPieceProfile[] profiles)
    {
        wrappedDictionary = new Dictionary<ChessPieceType, ChessPieceProfile>();

        for (int i = 0; i < profiles.Length; i++)
        {
            Add(profiles[i].type, profiles[i]);
        }
    }

    /// <summary>
    /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    public virtual void Add(ChessPieceType key, ChessPieceProfile value)
    {
        wrappedDictionary.Add(key, value);
    }

    /// <summary>
    /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
    /// </summary>
    /// <returns>
    /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
    /// </returns>
    /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
    public virtual bool ContainsKey(ChessPieceType key)
    {
        return wrappedDictionary.ContainsKey(key);
    }

    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </returns>
    public virtual ICollection<ChessPieceType> Keys
    {
        get
        {
            return wrappedDictionary.Keys;
        }
    }

    /// <summary>
    /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    /// <returns>
    /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </returns>
    /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    public virtual bool Remove(ChessPieceType key)
    {
        return wrappedDictionary.Remove(key);
    }

    /// <summary>
    /// Gets the value associated with the specified key.
    /// </summary>
    /// <returns>
    /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
    /// </returns>
    /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
    public virtual bool TryGetValue(ChessPieceType key, out ChessPieceProfile value)
    {
        return wrappedDictionary.TryGetValue(key, out value);
    }

    /// <summary>
    /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </summary>
    /// <returns>
    /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
    /// </returns>
    public virtual ICollection<ChessPieceProfile> Values
    {
        get
        {
            return wrappedDictionary.Values;
        }
    }

    /// <summary>
    /// Gets or sets the element with the specified key.
    /// </summary>
    /// <returns>
    /// The element with the specified key.
    /// </returns>
    /// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
    public virtual ChessPieceProfile this[ChessPieceType key]
    {
        get
        {
            return wrappedDictionary[key];
        }
        set
        {
            wrappedDictionary[key] = value;
        }
    }

    /// <summary>
    /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
    /// </summary>
    /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
    public virtual void Clear()
    {
        wrappedDictionary.Clear();
    }

    /// <summary>
    /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
    /// </summary>
    /// <returns>
    /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
    /// </returns>
    public virtual int Count
    {
        get
        {
            return wrappedDictionary.Count;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
    /// </returns>
    /// <filterpriority>1</filterpriority>
    public virtual IEnumerator<KeyValuePair<ChessPieceType, ChessPieceProfile>> GetEnumerator()
    {
        return wrappedDictionary.GetEnumerator();
    }
}
