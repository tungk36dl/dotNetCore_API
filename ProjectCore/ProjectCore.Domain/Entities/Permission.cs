using ProjectCore.Domain.Entities;
using ProjectCore.Domain.ValueObjects.Permission;

namespace ProjectCore.Domain.Entities;
    public class Permission : DomainEntity<Guid>
    {
        public PermissionCode Code { get; private set; }   // Ví dụ: USER_CREATE
        public string Module { get; private set; } // User
        public string Action { get; private set; } // Create
        public string? Description { get; private set; }

        protected Permission() { }

        // Field Code được tạo tự động dựa trên Module và Action
        /// <summary>
        /// Truong Code được tạo tự động dựa trên Module và Action
        /// </summary>
        /// <param name="id"></param>
        /// <param name="module"></param>
        /// <param name="action"></param>
        /// <param name="createdBy"></param>
        public Permission(Guid id, string module, string action, Guid createdBy)
            : base(id, createdBy)
        {
            Module = module;
            Action = action;
            Code = new PermissionCode(module, action);
        }
    }


