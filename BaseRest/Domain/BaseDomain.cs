using BaseRest.Boundary;
using BaseRest.General;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace BaseRest.Domain
{
    public abstract class BaseDomain : IDomain
    {
        [Key]
        public int Id { get; private set; }

        public DateTime UtcDateCreated { get; private set; }

        public DateTime UtcDateModified { get; private set; }

        public DateTime? UtcDateDeleted { get; private set; }

        public void InitializeId(int id)
        {
            if (this.Id != 0)
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Id already initialized");
            }

            this.Id = id;
        }

        public void Create()
        {
            if (this.UtcDateCreated != default(DateTime))
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Domain already created.");
            }

            this.UtcDateCreated = DateTime.UtcNow;
            this.UtcDateModified = DateTime.UtcNow;
        }

        public void Modify()
        {
            this.UtcDateModified = DateTime.UtcNow;
        }

        public void Delete()
        {
            if (this.UtcDateDeleted.HasValue)
            {
                throw new RestfulException(HttpStatusCode.Conflict, "Domain already deleted.");
            }

            this.UtcDateDeleted = DateTime.UtcNow;
        }
    }
}
