namespace NazarMahal.Application.RequestDto.UserRequestDto
{
    /// <summary>
    /// DTO for updating user status properties
    /// </summary>
    public class UpdateUserStatusRequestDto
    {
        /// <summary>
        /// Whether the user is disabled
        /// </summary>
        public bool IsDisabled { get; set; }
    }
}
