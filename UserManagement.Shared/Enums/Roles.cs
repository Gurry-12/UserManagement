namespace UserManagement.Shared.Enums
{
   
    [Flags]
    public enum Roles
    {
        None = 0,
        Administration = 1,
        Clinician = 2,
        Staff = 4,
        Patient = 8
    }
}
