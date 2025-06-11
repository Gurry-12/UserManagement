namespace UserManagement.Shared.Enums
{
    [Flags]
   
    public enum Actions
    {
        None = 0,
        Reactivate = 1,
        Deactivate = 2,
        ResendInvite = 3
    }
}
