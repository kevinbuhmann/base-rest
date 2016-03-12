using System;

namespace GiveLoveFirst.Boundary
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