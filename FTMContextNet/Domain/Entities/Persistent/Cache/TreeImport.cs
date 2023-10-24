using System;
namespace FTMContextNet.Domain.Entities.Persistent.Cache;

public partial class TreeImport: IEquatable<TreeImport>
{
    public int Id { get; set; }
    public string DateImported { get; set; }

    public string FileSize { get; set; }

    public string FileName { get; set; }

    public bool Selected { get; set; }

    public int UserId { get; set; }

    //Function to implement getHashCode
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + DateImported.GetHashCode();
            hash = hash * 23 + FileSize.GetHashCode();
            hash = hash * 23 + FileName.GetHashCode();
            hash = hash * 23 + Selected.GetHashCode();
            hash = hash * 23 + UserId.GetHashCode();
            return hash;
        } 
    }

    //Function to implement Equals
    public bool Equals(TreeImport other)
    {
        if (this.Id != other.Id) return false;
        if (this.DateImported != other.DateImported) return false;
        if (this.FileSize != other.FileSize) return false;
        if (this.FileName != other.FileName) return false;
        if (this.Selected != other.Selected) return false;
        if (this.UserId != other.UserId) return false;

        return true;
    }

    //Function to implement Equals
    public override bool Equals(object obj)
    {
        return this.Equals(obj as TreeImport);
    }
}