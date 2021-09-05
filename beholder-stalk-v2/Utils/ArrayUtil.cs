namespace beholder_stalk_v2.Utils
{
  using System;

  public class ArrayUtil
  {
    /// <summary>
    /// Return a new array with the specified size.
    /// If the new array is larger than the original array, then the new array is filled with repeated copies of a. Note that this behavior is different from Array.Resize() which fills with zeros instead of repeated copies of a.
    /// </summary>
    public static T[] Resize<T>(int size, T[] a)
    {
      var new_array = new T[size];

      for (int i = 0; i < size; i++)
      {
        int pos = (int)Math.Floor(i * ((double)a.Length / size));
        new_array[i] = a[pos];
      }

      // Always ensure that the first and last elements are the same as the origin
      if (size > 0)
      {
        new_array[0] = a[0];
        new_array[size - 1] = a[a.Length - 1];
      }
      return new_array;
    }
  }
}