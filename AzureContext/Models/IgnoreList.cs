using System;

namespace AzureContext.Models;

public partial class IgnoreList : IEquatable<IgnoreList>
{
    public int Id { get; set; }
    public string Person1 { get; set; }

    public string Person2 { get; set; }

    //Function to implement getHashCode
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + Person1.GetHashCode();
            hash = hash * 23 + Person2.GetHashCode();
            return hash;
        }
    }

    //Function to implement Equals
    public bool Equals(IgnoreList other)
    {
        if (this.Id != other.Id) return false;
        if (this.Person1 != other.Person1) return false;
        if (this.Person2 != other.Person2) return false;

        return true;
    }

    //Function to implement Equals
    public override bool Equals(object obj)
    {
        return this.Equals(obj as IgnoreList);
    }
}