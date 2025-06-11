namespace UserManagement.Shared.Enums
{
    [Flags]  
    public enum Status
    {
        None = 0,
        Active = 1,
        Deactive = 2,
        Invited = 3
    }
}
