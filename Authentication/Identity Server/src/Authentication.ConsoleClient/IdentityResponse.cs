using System.Collections.Generic;

namespace Authentication.ConsoleClient
{
    public class IdentityResponse
    {
        /// <summary>
        /// Flag indicating whether if the operation succeeded or not.
        /// </summary>
        /// <value>True if the operation succeeded, otherwise false.</value>
        public bool Succeeded { get; set; }

        /// <summary>
        /// An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />s containing an errors
        /// that occurred during the identity operation.
        /// </summary>
        /// <value>An <see cref="T:System.Collections.Generic.IEnumerable`1" /> of <see cref="T:Microsoft.AspNetCore.Identity.IdentityError" />s.</value>
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}