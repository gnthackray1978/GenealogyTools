using System;

namespace AzureContext.Models;

public partial class TreeRecordMapGroup : IEquatable<TreeRecordMapGroup>
{
    public int Id { get; set; }

    public string TreeName { get; set; }

    public string GroupName { get; set; }

    public int ImportId { get; set; }

    public int UserId { get; set; }
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + TreeName.GetHashCode();
            hash = hash * 23 + GroupName.GetHashCode();
            hash = hash * 23 + ImportId.GetHashCode();
            hash = hash * 23 + UserId.GetHashCode();
            return hash;
        }
    }
    public bool Equals(TreeRecordMapGroup other)
    {
        if (this.Id != other.Id) return false;
        if (this.TreeName != other.TreeName) return false;
        if (this.GroupName != other.GroupName) return false;
        if (this.ImportId != other.ImportId) return false;
        if (this.UserId != other.UserId) return false;

        return true;
    }

    public override bool Equals(object obj)
    {
        return this.Equals(obj as TreeRecordMapGroup);
    }


}