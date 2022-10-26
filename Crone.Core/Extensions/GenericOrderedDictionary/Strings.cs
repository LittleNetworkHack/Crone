// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Specialized.Internal;

internal static class Strings
{
    public static readonly string Argument_InvalidOffLen = "Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.";
    public static readonly string Argument_AddingDuplicateWithKey = "An item with the same key has already been added. Key: {0}";
    public static readonly string ArgumentOutOfRange_Count = "Count must be positive and count must refer to a location within the string/array/collection.";
    public static readonly string ArgumentOutOfRange_Index = "Index was out of range. Must be non-negative and less than the size of the collection.";
    public static readonly string ArgumentOutOfRange_ListInsert = "Index must be within the bounds of the List.";
    public static readonly string ArgumentOutOfRange_NeedNonNegNum = "Non-negative number required.";
    public static readonly string Arg_ArrayPlusOffTooSmall = "Destination array is not long enough to copy all the items in the collection. Check array index and length.";
    public static readonly string Arg_HTCapacityOverflow = "Capacity has overflowed.";
    public static readonly string Arg_KeyNotFoundWithKey = "The given key '{0}' was not present in the dictionary.";
    public static readonly string CopyTo_ArgumentsTooSmall = "Destination array is not long enough to copy all the items in the collection. Check array index and length.";
    public static readonly string Create_TValueCollectionReadOnly = "The specified TValueCollection creates collections that have IsReadOnly set to true by default. TValueCollection must be a mutable ICollection.";
    public static readonly string InvalidOperation_ConcurrentOperationsNotSupported = "Operations that change non-concurrent collections must have exclusive access. A concurrent update was performed on this collection and corrupted its state. The collection's state is no longer correct.";
    public static readonly string InvalidOperation_EnumEnded = "Enumeration already finished.";
    public static readonly string InvalidOperation_EnumFailedVersion = "Collection was modified; enumeration operation may not execute.";
    public static readonly string InvalidOperation_EnumNotStarted = "Enumeration has not started. Call MoveNext.";
    public static readonly string InvalidOperation_EnumOpCantHappen = "Enumeration has either not started or has already finished.";
    public static readonly string NotSupported_KeyCollectionSet = "Mutating a key collection derived from a dictionary is not allowed.";
    public static readonly string NotSupported_ValueCollectionSet = "Mutating a value collection derived from a dictionary is not allowed.";
    public static readonly string ReadOnly_Modification = "The collection is read-only.";
}
