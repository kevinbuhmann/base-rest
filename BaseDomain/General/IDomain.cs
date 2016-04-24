using System;

namespace BaseDomain.General
{
    public interface IDomain
    {
        int Id { get; }

        DateTime UtcDateCreated { get; }

        DateTime UtcDateModified { get; }

        DateTime? UtcDateDeleted { get; }

        void InitializeId(int id);

        void Create();

        void Modify();

        void Delete();
    }
}