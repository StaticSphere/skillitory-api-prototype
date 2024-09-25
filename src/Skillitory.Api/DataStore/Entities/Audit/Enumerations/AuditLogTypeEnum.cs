namespace Skillitory.Api.DataStore.Entities.Audit.Enumerations;

public enum AuditLogTypeEnum
{
    SignIn = 1,
    SignOut,
    ForgotPassword,
    ResetPassword,
    NewUserRegistered,
    NewUserEmailVerified
}
