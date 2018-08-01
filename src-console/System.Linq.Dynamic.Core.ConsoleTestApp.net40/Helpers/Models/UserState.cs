namespace System.Linq.Dynamic.Core.Tests.Helpers.Models
{
    public class UserState
    {
        public Guid StatusCode { get; set; }
        public string Description { get; set; }

        public static implicit operator Guid(UserState state) => state?.StatusCode ?? Guid.Empty;
    }
}
